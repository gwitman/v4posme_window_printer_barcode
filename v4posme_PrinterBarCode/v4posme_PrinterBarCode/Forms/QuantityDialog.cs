using System;
using System.Collections.Generic;
using System.Windows.Forms;
using v4posme_PrinterBarCode.Models;

namespace v4posme_PrinterBarCode.Forms
{
    /// <summary>
    /// Dialogo de impresion: cantidad por producto, impresora y TODA la
    /// configuracion de la etiqueta (tamano, margenes, fuentes, prefijo/sufijo,
    /// bordes y que elementos mostrar). Devuelve un BarcodeConfig editado.
    /// </summary>
    public partial class QuantityDialog : Form
    {
        private readonly int _selectedCount;

        /// <summary>Cantidad elegida (por producto).</summary>
        public int Quantity { get; private set; } = 1;

        /// <summary>Impresora seleccionada.</summary>
        public string SelectedPrinter { get; private set; }

        /// <summary>Configuracion de etiqueta resultante (editada por el usuario).</summary>
        public BarcodeConfig ResultBarcode { get; private set; }

        public QuantityDialog(int selectedCount, IEnumerable<string> printers,
            string preselectedPrinter, BarcodeConfig barcode)
        {
            InitializeComponent();
            _selectedCount = selectedCount;

            lblInfo.Text = $"Se aplica a cada uno de los {selectedCount} producto(s) seleccionado(s).";

            cboPrinter.Items.Clear();
            foreach (var p in printers)
                cboPrinter.Items.Add(p);

            if (!string.IsNullOrWhiteSpace(preselectedPrinter) && cboPrinter.Items.Contains(preselectedPrinter))
                cboPrinter.SelectedItem = preselectedPrinter;
            else if (cboPrinter.Items.Count > 0)
                cboPrinter.SelectedIndex = 0;

            // Precargamos TODOS los valores desde la configuracion recibida.
            var bc = barcode ?? new BarcodeConfig();
            numWidth.Value = Clamp((decimal)bc.WidthMm, numWidth.Minimum, numWidth.Maximum);
            numHeight.Value = Clamp((decimal)bc.HeightMm, numHeight.Minimum, numHeight.Maximum);
            numMargin.Value = Clamp((decimal)bc.MarginMm, numMargin.Minimum, numMargin.Maximum);
            txtLabelFont.Text = bc.LabelFontName;
            numLabelFontSize.Value = Clamp((decimal)bc.LabelFontSize, numLabelFontSize.Minimum, numLabelFontSize.Maximum);
            txtPrefix.Text = bc.Prefix;
            txtSuffix.Text = bc.Suffix;
            chkShowProductName.Checked = bc.ShowProductName;
            chkShowPrice.Checked = bc.ShowPrice;

            // Guardamos los campos del config que no se editan en la UI para no
            // perderlos. Incluye la fuente de barras heredada, copias por fila y
            // TODOS los parametros TSPL (dpi, gap, densidad, velocidad); si se
            // perdieran, la impresion en impresoras HION/TSC saldria mal o vacia.
            ResultBarcode = new BarcodeConfig
            {
                FontName = bc.FontName,
                FontSize = bc.FontSize,
                CopiesPerRow = bc.CopiesPerRow,
                PrinterDpi = bc.PrinterDpi,
                GapMm = bc.GapMm,
                GapOffsetMm = bc.GapOffsetMm,
                Density = bc.Density,
                Speed = bc.Speed
            };

            UpdateSummary();
            numQuantity.Select(0, numQuantity.Text.Length);
        }

        private static decimal Clamp(decimal value, decimal min, decimal max)
            => value < min ? min : (value > max ? max : value);

        private void OnSelectionChanged(object sender, EventArgs e) => UpdateSummary();

        private void UpdateSummary()
        {
            int perProduct = (int)numQuantity.Value;
            int total = perProduct * _selectedCount;
            string printer = cboPrinter.SelectedItem as string ?? "(ninguna)";
            lblSummary.Text =
                $"Se imprimiran {total} etiqueta(s) ({perProduct} x {_selectedCount}) en: {printer}" +
                Environment.NewLine +
                $"Tamano: {numWidth.Value:0.#} x {numHeight.Value:0.#} mm";
        }

        private void btnAccept_Click(object sender, EventArgs e)
        {
            if (cboPrinter.SelectedItem == null)
            {
                MessageBox.Show(this, "Seleccione una impresora.", "Imprimir",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            Quantity = (int)numQuantity.Value;
            SelectedPrinter = cboPrinter.SelectedItem as string;

            // Volcamos los valores editados al BarcodeConfig resultante.
            ResultBarcode.WidthMm = (double)numWidth.Value;
            ResultBarcode.HeightMm = (double)numHeight.Value;
            ResultBarcode.MarginMm = (double)numMargin.Value;
            ResultBarcode.LabelFontName = string.IsNullOrWhiteSpace(txtLabelFont.Text) ? "Arial" : txtLabelFont.Text.Trim();
            ResultBarcode.LabelFontSize = (float)numLabelFontSize.Value;
            ResultBarcode.Prefix = txtPrefix.Text;
            ResultBarcode.Suffix = txtSuffix.Text;
            ResultBarcode.ShowProductName = chkShowProductName.Checked;
            ResultBarcode.ShowPrice = chkShowPrice.Checked;

            DialogResult = DialogResult.OK;
            Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }
    }
}
