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

            var commands = BuildTspl(labels, bc);
            RawPrinterHelper.SendStringToPrinter(printerName, commands);

            Logger.Info("Trabajo TSPL enviado correctamente a la impresora.");
        }

        /// <summary>Genera el bloque de comandos TSPL para todas las etiquetas.</summary>
        private string BuildTspl(List<Product> labels, BarcodeConfig bc)
        {
            // TSPL trabaja en puntos (dots). El factor dots/mm depende del dpi de la
            // impresora (203 dpi => 8 dots/mm; 300 dpi => ~11.81 dots/mm).
            int dpi = bc.PrinterDpi > 0 ? bc.PrinterDpi : 203;
            double dotsPerMm = dpi / 25.4;

            int marginDots = (int)Math.Round(Math.Max(0, bc.MarginMm) * dotsPerMm);

            // Altura de las barras: dejamos espacio para nombre (arriba) y codigo/precio (abajo).
            int lineHeightDots = (int)Math.Round(Math.Max(8, bc.LabelFontSize) * dotsPerMm / 2.2);

            Logger.Info($"TSPL: dpi={dpi} ({dotsPerMm:0.##} dots/mm), gap={bc.GapMm}mm, " +
                $"density={bc.Density}, speed={(bc.Speed > 0 ? bc.Speed.ToString() : "default")}.");

            var sb = new StringBuilder();

            foreach (var product in labels)
            {
                string raw = Sanitize(product.EffectiveBarcode);

                // --- Configuracion de la etiqueta ---
                sb.Append("SIZE ")
                  .Append(Fmt(bc.WidthMm)).Append(" mm,")
                  .Append(Fmt(bc.HeightMm)).Append(" mm\r\n");
                sb.Append("GAP ")
                  .Append(Fmt(bc.GapMm)).Append(" mm,")
                  .Append(Fmt(bc.GapOffsetMm)).Append(" mm\r\n");
                sb.Append("DIRECTION 1\r\n");
                if (bc.Speed > 0)
                    sb.Append("SPEED ").Append(bc.Speed).Append("\r\n");
                sb.Append("DENSITY ").Append(Clamp(bc.Density, 0, 15)).Append("\r\n");
                sb.Append("CLS\r\n");

                int y = marginDots;

                // --- Nombre del producto (texto arriba) ---
                if (bc.ShowProductName && !string.IsNullOrWhiteSpace(product.Name))
                {
                    // Fuente "2" = fuente interna mediana. x,y en dots.
                    sb.Append("TEXT ").Append(marginDots).Append(',').Append(y)
                      .Append(",\"2\",0,1,1,\"").Append(EscapeText(Truncate(product.Name, 32))).Append("\"\r\n");
                    y += lineHeightDots + 6;
                }

                // --- Codigo de barra (generado por la impresora: Code 128) ---
                // BARCODE x,y,"code type",height,human_readable,rotation,narrow,wide,"content"
                int barHeight = (int)Math.Round(bc.HeightMm * dotsPerMm * 0.45);
                if (barHeight < 24) barHeight = 24;

                sb.Append("BARCODE ").Append(marginDots).Append(',').Append(y)
                  .Append(",\"128\",").Append(barHeight)
                  .Append(",1,0,2,4,\"").Append(EscapeText(raw)).Append("\"\r\n");

                // Avanzamos: altura de barras + texto legible que TSPL dibuja debajo.
                y += barHeight + lineHeightDots + 8;

                // --- Precio (texto abajo) ---
                if (bc.ShowPrice)
                {
                    string price = product.Price.ToString("C", CultureInfo.CurrentCulture);
                    sb.Append("TEXT ").Append(marginDots).Append(',').Append(y)
                      .Append(",\"3\",0,1,1,\"").Append(EscapeText(price)).Append("\"\r\n");
                }

                // --- Imprimir 1 copia de esta etiqueta ---
                sb.Append("PRINT 1,1\r\n");
            }

            return sb.ToString();
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
