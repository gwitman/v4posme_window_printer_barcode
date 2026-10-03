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
        public void Print(IEnumerable<Product> products)
        {
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

            Logger.Info($"Iniciando impresion de {labels.Count} etiqueta(s) en impresora '{_config.PrinterName}'.");

            var bc = _config.Barcode;
            int index = 0;

            using (var doc = new PrintDocument())
            {
                doc.DocumentName = "v4posme Codigos de Barra";

                if (!string.IsNullOrWhiteSpace(_config.PrinterName))
                {
                    doc.PrinterSettings.PrinterName = _config.PrinterName;
                    if (!doc.PrinterSettings.IsValid)
                        throw new InvalidOperationException(
                            $"La impresora '{_config.PrinterName}' no es valida o no esta instalada.");
                }

                // Tamano de etiqueta en centesimas de pulgada (unidad de PaperSize).
                int widthHund = MmToHundredthsInch(bc.WidthMm);
                int heightHund = MmToHundredthsInch(bc.HeightMm);
                doc.DefaultPageSettings.PaperSize = new PaperSize("Etiqueta", widthHund, heightHund);
                doc.DefaultPageSettings.Margins = new Margins(0, 0, 0, 0);

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

        /// <summary>Dibuja una etiqueta individual.</summary>
        private void DrawLabel(Graphics g, Rectangle bounds, Product product, BarcodeConfig bc)
        {
            g.Clear(Color.White);

            int margin = MmToPixels(g, bc.MarginMm);
            var area = new Rectangle(
                bounds.X + margin,
                bounds.Y + margin,
                Math.Max(1, bounds.Width - margin * 2),
                Math.Max(1, bounds.Height - margin * 2));

            // Los codigos de barra son largos y llevan caracteres adicionales (prefijo/sufijo).
            string raw = product.EffectiveBarcode ?? string.Empty;
            string encoded = (bc.Prefix ?? string.Empty) + raw + (bc.Suffix ?? string.Empty);

            var format = new StringFormat
            {
                Alignment = StringAlignment.Center,
                LineAlignment = StringAlignment.Center,
                Trimming = StringTrimming.None,
                FormatFlags = StringFormatFlags.NoWrap
            };

            float y = area.Top;

            // Nombre del producto (arriba).
            if (bc.ShowProductName && !string.IsNullOrWhiteSpace(product.Name))
            {
                using (var labelFont = new Font(bc.LabelFontName, bc.LabelFontSize, FontStyle.Bold))
                {
                    var h = labelFont.GetHeight(g);
                    var r = new RectangleF(area.Left, y, area.Width, h);
                    g.DrawString(product.Name, labelFont, Brushes.Black, r, format);
                    y += h;
                }
            }

            // Codigo de barra (centro). Fuente de barras ajustada para que quepa.
            using (var barcodeFont = BuildFittingBarcodeFont(g, encoded, bc, area.Width))
            {
                float barcodeHeight = area.Bottom - y;
                if (bc.ShowPrice) barcodeHeight -= bc.LabelFontSize * 2f;
                if (barcodeHeight < 10) barcodeHeight = 10;

                var r = new RectangleF(area.Left, y, area.Width, barcodeHeight);
                g.DrawString(encoded, barcodeFont, Brushes.Black, r, format);

                // Texto legible del codigo debajo del simbolo.
                using (var human = new Font(bc.LabelFontName, bc.LabelFontSize))
                {
                    var rr = new RectangleF(area.Left, y + barcodeHeight, area.Width, bc.LabelFontSize * 1.6f);
                    g.DrawString(raw, human, Brushes.Black, rr, format);
                }
                y += barcodeHeight + bc.LabelFontSize * 1.6f;
            }

            // Precio (abajo).
            if (bc.ShowPrice)
            {
                using (var priceFont = new Font(bc.LabelFontName, bc.LabelFontSize + 1, FontStyle.Bold))
                {
                    var r = new RectangleF(area.Left, y, area.Width, priceFont.GetHeight(g));
                    g.DrawString(product.Price.ToString("C"), priceFont, Brushes.Black, r, format);
                }
            }
        }

        /// <summary>
        /// Construye la fuente del codigo de barra reduciendo el tamano hasta que el
        /// simbolo (que puede ser largo) quepa en el ancho de la etiqueta.
        /// </summary>
        private Font BuildFittingBarcodeFont(Graphics g, string encoded, BarcodeConfig bc, float maxWidth)
        {
            float size = bc.FontSize;
            Font font = new Font(bc.FontName, size);
            var measured = g.MeasureString(encoded, font);

            while (measured.Width > maxWidth && size > 6)
            {
                font.Dispose();
                size -= 1f;
                font = new Font(bc.FontName, size);
                measured = g.MeasureString(encoded, font);
            }

            return font;
        }

        private static int MmToHundredthsInch(double mm) => (int)Math.Round(mm / 25.4 * 100.0);

        private static int MmToPixels(Graphics g, double mm) => (int)Math.Round(mm / 25.4 * g.DpiX);
    }
}
