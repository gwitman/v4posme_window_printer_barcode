using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using v4posme_PrinterBarCode.Models;

namespace v4posme_PrinterBarCode.Services
{
    /// <summary>
    /// Imprime etiquetas en impresoras termicas tipo HION / TSC usando el lenguaje
    /// nativo TSPL. A diferencia del dibujo GDI, aqui la impresora genera el codigo
    /// de barra por hardware (comando BARCODE), lo que garantiza barras nitidas y
    /// escaneables. Los comandos se envian en RAW al spooler.
    /// </summary>
    public class TsplBarcodePrinter
    {
        private readonly AppConfig _config;

        public TsplBarcodePrinter(AppConfig config)
        {
            _config = config;
        }

        /// <summary>
        /// Construye y envia un trabajo TSPL con una etiqueta por cada copia solicitada.
        /// </summary>
        public void Print(IEnumerable<Product> products, string printerName, BarcodeConfig barcode)
        {
            var bc = barcode ?? _config.Barcode;

            // Expandimos segun la cantidad de copias de cada producto.
            var labels = new List<Product>();
            foreach (var p in products)
            {
                int qty = p.PrintQuantity < 1 ? 1 : p.PrintQuantity;
                for (int i = 0; i < qty; i++)
                    labels.Add(p);
            }

            if (labels.Count == 0)
            {
                Logger.Warn("Impresion TSPL solicitada sin etiquetas que imprimir.");
                return;
            }

            Logger.Info($"Generando trabajo TSPL para {labels.Count} etiqueta(s) en '{printerName}'.");

            // El trabajo se construye como BYTES: el nombre y el precio se envian
            // como imagen (comando BITMAP con datos binarios), porque el firmware
            // de estos clones HOIN NO implementa el comando TEXT. Los bytes binarios
            // del BITMAP no sobreviven una conversion a texto, por eso trabajamos a
            // nivel de byte[] y no de string.
            byte[] commands = BuildTsplBytes(labels, bc);

            // Registramos solo la parte legible (los comandos ASCII) para diagnostico;
            // los datos binarios del BITMAP se omiten del log para no ensuciarlo.
            Logger.Info($"Trabajo TSPL generado: {commands.Length} byte(s) " +
                $"para {labels.Count} etiqueta(s).");

            // Decidimos el canal de envio, en orden de fiabilidad:
            //  1) usbVid+usbPid  -> escritura DIRECTA al dispositivo USB (como la
            //     herramienta del fabricante). Evita el spooler/driver que atascan
            //     el trabajo en el puerto USB. Es el metodo recomendado para HOIN.
            //  2) puerto COM/LPT -> escritura DIRECTA al puerto serie/paralelo.
            //  3) resto          -> envio RAW por el spooler usando printerName.
            bool useUsb = !string.IsNullOrWhiteSpace(_config.UsbVid)
                          && !string.IsNullOrWhiteSpace(_config.UsbPid);
            bool usePort = !string.IsNullOrWhiteSpace(_config.PrinterPort)
                           && RawPrinterHelper.IsDirectlyOpenablePort(_config.PrinterPort);

            if (useUsb)
            {
                Logger.Info($"Enviando TSPL DIRECTO al dispositivo USB (VID={_config.UsbVid}, " +
                    $"PID={_config.UsbPid}), sin spooler ni driver.");
                RawPrinterHelper.SendBytesToUsbDevice(_config.UsbVid, _config.UsbPid, commands);
            }
            else if (usePort)
            {
                Logger.Info($"Enviando TSPL DIRECTO al puerto '{_config.PrinterPort}' (sin driver).");
                RawPrinterHelper.SendBytesToPort(_config.PrinterPort, commands);
            }
            else
            {
                Logger.Info($"Enviando TSPL por el spooler (RAW) a la impresora '{printerName}'.");
                RawPrinterHelper.SendBytesToPrinter(printerName, commands);
            }

            Logger.Info("Trabajo TSPL enviado correctamente a la impresora.");
        }

        /// <summary>
        /// Genera el trabajo TSPL completo como BYTES. El codigo de barra lo genera
        /// la impresora (comando BARCODE), pero el nombre y el precio se envian como
        /// IMAGEN (comando BITMAP) porque el firmware de estos clones HOIN no
        /// implementa el comando TEXT. Al mezclar texto ASCII con datos binarios del
        /// BITMAP, el trabajo debe construirse a nivel de byte[].
        /// </summary>
        private byte[] BuildTsplBytes(List<Product> labels, BarcodeConfig bc)
        {
            // TSPL trabaja en puntos (dots). El factor dots/mm depende del dpi de la
            // impresora (203 dpi => 8 dots/mm; 300 dpi => ~11.81 dots/mm).
            int dpi = bc.PrinterDpi > 0 ? bc.PrinterDpi : 203;
            double dotsPerMm = dpi / 25.4;

            int marginDots = (int)Math.Round(Math.Max(0, bc.MarginMm) * dotsPerMm);
            int labelWidthDots = (int)Math.Round(bc.WidthMm * dotsPerMm);
            int labelHeightDots = (int)Math.Round(bc.HeightMm * dotsPerMm);

            Logger.Info($"TSPL: dpi={dpi} ({dotsPerMm:0.##} dots/mm), gap={bc.GapMm}mm, " +
                $"density={bc.Density}, speed={(bc.Speed > 0 ? bc.Speed.ToString() : "default")}.");

            // CP850 para los comandos ASCII; los datos del BITMAP se anexan como bytes crudos.
            Encoding enc = Encoding.GetEncoding(850);
            using (var ms = new System.IO.MemoryStream())
            {
                void Ascii(string s)
                {
                    byte[] b = enc.GetBytes(s);
                    ms.Write(b, 0, b.Length);
                }

                // CRLF inicial: limpia cualquier byte residual del buffer de comandos.
                Ascii("\r\n");

                foreach (var product in labels)
                {
                    string raw = Sanitize(product.EffectiveBarcode);
                    if (string.IsNullOrEmpty(raw))
                    {
                        Logger.Warn($"Producto '{product.Name}' sin codigo de barra; se omite su etiqueta TSPL.");
                        continue;
                    }

                    // --- Configuracion de la etiqueta ---
                    Ascii($"SIZE {Fmt(bc.WidthMm)} mm,{Fmt(bc.HeightMm)} mm\r\n");
                    Ascii($"GAP {Fmt(bc.GapMm)} mm,{Fmt(bc.GapOffsetMm)} mm\r\n");
                    Ascii("CODEPAGE 850\r\n");
                    Ascii("DIRECTION 1\r\n");
                    if (bc.Speed > 0) Ascii($"SPEED {bc.Speed}\r\n");
                    Ascii($"DENSITY {Clamp(bc.Density, 0, 15)}\r\n");
                    Ascii("CLS\r\n");
                    if (bc.TsplBeepTest) Ascii("SOUND 2,100\r\n");

                    // Margenes: usamos marginDots para los cuatro lados. El contenido
                    // (texto y barras) arranca en marginX/marginTop y nunca invade el
                    // margen derecho/inferior, de modo que no quede pegado al borde.
                    int marginX = marginDots;
                    int marginTop = marginDots;
                    int marginBottom = marginDots;

                    // Ancho util para texto y barras: ancho total menos margenes.
                    int contentWidth = Math.Max(32, labelWidthDots - marginX * 2);

                    // Alto de una linea de texto del nombre y del precio.
                    int lineH = Math.Max(16, (int)Math.Round(bc.LabelFontSize * dotsPerMm / 1.6));
                    int priceH = Math.Max(18, (int)Math.Round((bc.LabelFontSize + 2) * dotsPerMm / 1.6));

                    bool drawName = bc.ShowProductName && !string.IsNullOrWhiteSpace(product.Name);
                    bool drawBarcodeText = bc.ShowBarcodeText;

                    // El nombre puede ocupar hasta 2 lineas si no cabe en una. Medimos
                    // cuantas lineas necesita para reservar su altura exacta.
                    int nameLines = 0;
                    int nameH = 0;
                    if (drawName)
                    {
                        nameLines = MeasureTextLines(product.Name, bc.LabelFontName, bc.LabelFontSize,
                            contentWidth, lineH, maxLines: 2);
                        nameH = nameLines * lineH;
                    }

                    // Orden de la etiqueta: nombre, codigo de barra (texto), barras, precio.
                    int nameY = marginTop;

                    // El texto del codigo de barra (los digitos) va DEBAJO del nombre y
                    // ENCIMA de las barras. Lo dibujamos nosotros como imagen y
                    // deshabilitamos el texto legible interno de la impresora.
                    int barcodeTextH = drawBarcodeText ? lineH : 0;
                    int barcodeTextY = marginTop + (drawName ? nameH + 4 : 0);

                    int barcodeY = barcodeTextY + (drawBarcodeText ? barcodeTextH + 2 : 0);

                    // Como ahora dibujamos el texto del codigo nosotros (arriba de las
                    // barras), la impresora NO dibuja su texto legible interno.
                    const int humanReadableH = 0;

                    // Reservamos abajo solo lo del precio (si aplica). El codigo de
                    // barra se estira para ocupar casi todo el espacio disponible.
                    int reservedBottom = (bc.ShowPrice ? priceH + 2 : 0) + marginBottom;
                    int available = labelHeightDots - barcodeY - reservedBottom;
                    int barHeight = available - humanReadableH;
                    if (barHeight < 30) barHeight = 30;

                    // Precio pegado justo debajo del texto legible del codigo (sin
                    // hueco): barcodeY + barras + texto legible.
                    int priceY = barcodeY + barHeight + humanReadableH;
                    int maxPriceY = labelHeightDots - priceH - marginBottom;
                    if (priceY > maxPriceY) priceY = maxPriceY;
                    if (priceY < 0) priceY = 0;

                    // --- 1) Nombre del producto como IMAGEN (BITMAP), con wrap a 2 lineas ---
                    if (drawName)
                        AppendTextBitmap(ms, enc, marginX, nameY, contentWidth, nameH,
                            product.Name, bc.LabelFontName, bc.LabelFontSize, bold: false, maxLines: 2);

                    // --- 2) Codigo de barra como TEXTO (BITMAP), encima de las barras ---
                    if (drawBarcodeText)
                        AppendTextBitmap(ms, enc, marginX, barcodeTextY, contentWidth, barcodeTextH,
                            raw, bc.LabelFontName, bc.LabelFontSize, bold: false, maxLines: 1);

                    // --- 3) Precio como IMAGEN (BITMAP) ---
                    if (bc.ShowPrice)
                    {
                        string price = product.Price.ToString("C", CultureInfo.CurrentCulture);
                        AppendTextBitmap(ms, enc, marginX, priceY, contentWidth, priceH,
                            price, bc.LabelFontName, bc.LabelFontSize + 2, bold: true, maxLines: 1);
                    }

                    // --- 4) Barras del codigo (lo genera la impresora: Code 128) ---
                    // El 5o parametro (texto legible) va en 0: el texto ya lo dibujamos
                    // nosotros arriba de las barras cuando showBarcodeText esta activo.
                    Ascii($"BARCODE {marginX},{barcodeY},\"128\",{barHeight},0,0,2,4,\"{EscapeText(raw)}\"\r\n");

                    // --- Imprimir 1 copia de esta etiqueta ---
                    Ascii("PRINT 1,1\r\n");
                }

                // CRLF final: asegura que el ultimo comando se ejecute.
                Ascii("\r\n");
                return ms.ToArray();
            }
        }

        /// <summary>
        /// Mide cuantas lineas (1..maxLines) necesita un texto para caber en el ancho
        /// dado con la fuente indicada. Sirve para reservar la altura exacta del
        /// bloque del nombre antes de dibujarlo.
        /// </summary>
        private static int MeasureTextLines(string text, string fontName, float fontSize,
            int widthDots, int lineH, int maxLines)
        {
            if (string.IsNullOrWhiteSpace(text)) return 1;

            using (var bmp = new System.Drawing.Bitmap(1, 1))
            using (var g = System.Drawing.Graphics.FromImage(bmp))
            {
                float emPx = Math.Max(6f, lineH * 0.72f);
                using (var font = new System.Drawing.Font(
                           string.IsNullOrWhiteSpace(fontName) ? "Arial" : fontName,
                           emPx, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel))
                {
                    // Medimos el texto permitiendo ajuste al ancho disponible.
                    var size = g.MeasureString(text, font, widthDots);
                    int lines = (int)Math.Ceiling(size.Height / lineH);
                    if (lines < 1) lines = 1;
                    if (lines > maxLines) lines = maxLines;
                    return lines;
                }
            }
        }

        /// <summary>
        /// Dibuja un texto en un bitmap monocromatico y lo anexa al stream como un
        /// comando TSPL BITMAP. Es la via fiable para imprimir texto en clones HOIN
        /// que no soportan el comando TEXT: la impresora solo tiene que pintar pixeles.
        ///
        /// Formato TSPL: BITMAP x,y,width_bytes,height,mode,&lt;datos&gt;
        ///   - width_bytes = ancho en bytes (cada byte son 8 pixeles horizontales).
        ///   - mode 0 = OVERWRITE.
        ///   - En TSPL el bit 1 = pixel BLANCO y el bit 0 = pixel NEGRO (invertido).
        /// </summary>
        private static void AppendTextBitmap(System.IO.MemoryStream ms, Encoding enc,
            int x, int y, int widthDots, int heightDots, string text,
            string fontName, float fontSize, bool bold, int maxLines = 1)
        {
            // El ancho debe ser multiplo de 8 (cada byte = 8 pixeles).
            int widthBytes = (widthDots + 7) / 8;
            int width = widthBytes * 8;
            int height = Math.Max(8, heightDots);

            using (var bmp = new System.Drawing.Bitmap(width, height,
                       System.Drawing.Imaging.PixelFormat.Format32bppArgb))
            {
                using (var g = System.Drawing.Graphics.FromImage(bmp))
                {
                    g.Clear(System.Drawing.Color.White);
                    g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.SingleBitPerPixelGridFit;

                    var style = bold ? System.Drawing.FontStyle.Bold : System.Drawing.FontStyle.Regular;
                    // Tamano de fuente en pixeles basado en el alto de UNA linea, para
                    // que si el bloque es de 2 lineas la fuente no se agigante.
                    float lineHeightPx = (float)height / Math.Max(1, maxLines);
                    float emPx = Math.Max(6f, lineHeightPx * 0.72f);

                    // Con maxLines > 1 permitimos ajuste de linea (word wrap); con 1
                    // linea evitamos el wrap y recortamos con puntos suspensivos.
                    bool wrap = maxLines > 1;
                    using (var font = new System.Drawing.Font(
                               string.IsNullOrWhiteSpace(fontName) ? "Arial" : fontName,
                               emPx, style, System.Drawing.GraphicsUnit.Pixel))
                    using (var fmt = new System.Drawing.StringFormat
                    {
                        Alignment = System.Drawing.StringAlignment.Near,
                        LineAlignment = System.Drawing.StringAlignment.Center,
                        Trimming = System.Drawing.StringTrimming.EllipsisCharacter,
                        FormatFlags = wrap
                            ? (System.Drawing.StringFormatFlags)0
                            : System.Drawing.StringFormatFlags.NoWrap
                    })
                    {
                        g.DrawString(text ?? string.Empty, font, System.Drawing.Brushes.Black,
                            new System.Drawing.RectangleF(0, 0, width, height), fmt);
                    }
                }

                // Convertimos el bitmap a los bytes 1bpp que espera TSPL.
                byte[] data = new byte[widthBytes * height];
                for (int row = 0; row < height; row++)
                {
                    for (int col = 0; col < width; col++)
                    {
                        var px = bmp.GetPixel(col, row);
                        // Pixel oscuro => negro (bit 0); claro => blanco (bit 1).
                        bool dark = (px.R + px.G + px.B) / 3 < 128;
                        if (!dark)
                            data[row * widthBytes + col / 8] |= (byte)(0x80 >> (col % 8));
                    }
                }

                byte[] header = enc.GetBytes($"BITMAP {x},{y},{widthBytes},{height},0,");
                ms.Write(header, 0, header.Length);
                ms.Write(data, 0, data.Length);
                byte[] crlf = enc.GetBytes("\r\n");
                ms.Write(crlf, 0, crlf.Length);
            }
        }

        /// <summary>Formatea un numero con punto decimal para TSPL (independiente de la cultura).</summary>
        private static string Fmt(double v) => v.ToString("0.##", CultureInfo.InvariantCulture);

        /// <summary>Quita saltos de linea que romperian el comando TSPL.</summary>
        private static string Sanitize(string s) =>
            (s ?? string.Empty).Replace("\r", "").Replace("\n", "").Trim();

        /// <summary>Escapa comillas dobles dentro del contenido de los comandos TSPL.</summary>
        private static string EscapeText(string s) =>
            Sanitize(s).Replace("\"", "'");

        private static string Truncate(string s, int max) =>
            s.Length <= max ? s : s.Substring(0, max);

        private static int Clamp(int v, int min, int max) =>
            v < min ? min : (v > max ? max : v);
    }
}
