using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Printing;
using System.Linq;
using v4posme_PrinterBarCode.Models;

namespace v4posme_PrinterBarCode.Services
{
    /// <summary>
    /// Genera e imprime etiquetas de codigo de barra en la impresora configurada.
    /// Cada producto se imprime segun su cantidad (PrintQuantity).
    /// </summary>
    public class BarcodePrinter
    {
        private readonly AppConfig _config;

        public BarcodePrinter(AppConfig config)
        {
            _config = config;
        }

        /// <summary>
        /// Imprime las etiquetas de la lista de productos (una pagina por etiqueta).
        /// Es sincrono y debe invocarse en un hilo de fondo.
        /// </summary>
        public void Print(IEnumerable<Product> products, string printerName = null,
            double widthMm = 0, double heightMm = 0)
        {
            // La impresora seleccionada en la UI tiene prioridad sobre el config.
            if (string.IsNullOrWhiteSpace(printerName))
                printerName = _config.PrinterName;

            // El tamano elegido en la UI tiene prioridad sobre el config.
            if (widthMm <= 0) widthMm = _config.Barcode.WidthMm;
            if (heightMm <= 0) heightMm = _config.Barcode.HeightMm;

            // Expandimos la lista segun la cantidad de copias de cada producto.
            var labels = new List<Product>();
            foreach (var p in products)
            {
                int qty = p.PrintQuantity < 1 ? 1 : p.PrintQuantity;
                for (int i = 0; i < qty; i++)
                    labels.Add(p);
            }

            if (labels.Count == 0)
            {
                Logger.Warn("Impresion solicitada sin etiquetas que imprimir.");
                return;
            }

            Logger.Info($"Iniciando impresion de {labels.Count} etiqueta(s) en impresora '{printerName}'.");

            var bc = _config.Barcode;
            int index = 0;

            using (var doc = new PrintDocument())
            {
                doc.DocumentName = "v4posme Codigos de Barra";

                if (!string.IsNullOrWhiteSpace(printerName))
                {
                    doc.PrinterSettings.PrinterName = printerName;
                    if (!doc.PrinterSettings.IsValid)
                        throw new InvalidOperationException(
                            $"La impresora '{printerName}' no es valida o no esta instalada.");
                }

                // Tamano de etiqueta en centesimas de pulgada (unidad de PaperSize).
                int widthHund   = MmToHundredthsInch(widthMm);
                int heightHund  = MmToHundredthsInch(heightMm);
                doc.DefaultPageSettings.PaperSize = new PaperSize(
                    $"Etiqueta {widthMm:0.#}x{heightMm:0.#}mm", widthHund, heightHund);
                doc.DefaultPageSettings.Margins = new Margins(0, 0, 0, 0);
                Logger.Info($"Tamano de pagina aplicado: {widthMm:0.#}x{heightMm:0.#} mm " +
                    $"({widthHund}x{heightHund} centesimas de pulgada).");

                doc.PrintPage += (sender, e) =>
                {
                    var product = labels[index];
                    DrawLabel(e.Graphics, e.MarginBounds.IsEmpty ? e.PageBounds : e.PageBounds, product, bc);
                    index++;
                    e.HasMorePages = index < labels.Count;
                };

                doc.Print();
            }

            Logger.Info("Impresion finalizada correctamente.");
        }

        /// <summary>Dibuja una etiqueta individual con borde, nombre, barras reales y precio.</summary>
        private void DrawLabel(Graphics g, Rectangle bounds, Product product, BarcodeConfig bc)
        {
            g.Clear(Color.White);
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.None;
            g.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.Half;

            int margin = MmToPixels(g, bc.MarginMm);
            var area = new Rectangle(
                bounds.X + margin,
                bounds.Y + margin,
                Math.Max(1, bounds.Width - margin * 2),
                Math.Max(1, bounds.Height - margin * 2));

            // Perimetro (borde) de la etiqueta para delimitar cada pagina.
            if (bc.ShowBorder)
            {
                using (var pen = new Pen(Color.Black, Math.Max(1f, (float)MmToPixels(g, bc.BorderThicknessMm))))
                {
                    pen.Alignment = System.Drawing.Drawing2D.PenAlignment.Inset;
                    g.DrawRectangle(pen, area.X, area.Y, area.Width - 1, area.Height - 1);
                }
            }

            // Area util interna (dejamos un pequeno respiro desde el borde).
            int pad = Math.Max(2, MmToPixels(g, 1));
            var inner = new Rectangle(area.X + pad, area.Y + pad,
                Math.Max(1, area.Width - pad * 2), Math.Max(1, area.Height - pad * 2));

            // El valor a codificar es el codigo de barra del producto.
            string raw = product.EffectiveBarcode ?? string.Empty;

            var centerFormat = new StringFormat
            {
                Alignment = StringAlignment.Center,
                LineAlignment = StringAlignment.Center,
                Trimming = StringTrimming.EllipsisCharacter,
                FormatFlags = StringFormatFlags.NoWrap
            };

            float top = inner.Top;
            float bottom = inner.Bottom;

            // Nombre del producto (arriba).
            if (bc.ShowProductName && !string.IsNullOrWhiteSpace(product.Name))
            {
                using (var labelFont = new Font(bc.LabelFontName, bc.LabelFontSize, FontStyle.Bold))
                {
                    float h = labelFont.GetHeight(g);
                    g.DrawString(product.Name, labelFont, Brushes.Black,
                        new RectangleF(inner.Left, top, inner.Width, h), centerFormat);
                    top += h;
                }
            }

            // Reservamos espacio para el texto legible del codigo y el precio (abajo).
            float humanTextHeight = bc.LabelFontSize * 1.6f;
            float priceHeight = bc.ShowPrice ? (bc.LabelFontSize + 1) * 1.5f : 0f;
            float reservedBottom = humanTextHeight + priceHeight;

            // Zona para las barras verticales.
            var barcodeRect = new RectangleF(
                inner.Left, top, inner.Width, Math.Max(10f, bottom - top - reservedBottom));

            DrawCode128(g, raw, barcodeRect);

            // Texto legible del codigo (debajo de las barras).
            using (var human = new Font(bc.LabelFontName, bc.LabelFontSize))
            {
                g.DrawString(raw, human, Brushes.Black,
                    new RectangleF(inner.Left, barcodeRect.Bottom, inner.Width, humanTextHeight), centerFormat);
            }

            // Precio (abajo del todo).
            if (bc.ShowPrice)
            {
                using (var priceFont = new Font(bc.LabelFontName, bc.LabelFontSize + 1, FontStyle.Bold))
                {
                    g.DrawString(product.Price.ToString("C"), priceFont, Brushes.Black,
                        new RectangleF(inner.Left, barcodeRect.Bottom + humanTextHeight, inner.Width, priceHeight),
                        centerFormat);
                }
            }
        }

        /// <summary>
        /// Dibuja un codigo de barras Code128 real (barras verticales negras) dentro
        /// del rectangulo dado, escalando el ancho de modulo para que ocupe el area.
        /// </summary>
        private void DrawCode128(Graphics g, string data, RectangleF rect)
        {
            var widths = Code128Encoder.Encode(data);

            // Total de modulos (suma de anchos) para escalar al ancho disponible.
            int totalModules = 0;
            foreach (var w in widths) totalModules += w;
            if (totalModules <= 0) return;

            // Quiet zone de 10 modulos a cada lado (recomendado por el estandar).
            float moduleWidth = rect.Width / (totalModules + 20f);
            if (moduleWidth <= 0) moduleWidth = 0.1f;

            float x = rect.Left + moduleWidth * 10f; // quiet zone izquierda
            bool bar = true; // el patron empieza con barra

            foreach (var w in widths)
            {
                float segmentWidth = w * moduleWidth;
                if (bar)
                {
                    g.FillRectangle(Brushes.Black, x, rect.Top, segmentWidth, rect.Height);
                }
                x += segmentWidth;
                bar = !bar;
            }
        }

        private static int MmToHundredthsInch(double mm) => (int)Math.Round(mm / 25.4 * 100.0);

        private static int MmToPixels(Graphics g, double mm) => (int)Math.Round(mm / 25.4 * g.DpiX);
    }
}
