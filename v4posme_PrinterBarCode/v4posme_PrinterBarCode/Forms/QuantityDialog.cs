using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace v4posme_PrinterBarCode.Forms
{
    /// <summary>
    /// Dialogo que pregunta cuantas etiquetas imprimir por producto, en que
    /// impresora y con que tamano de pagina (ancho/alto en mm). Muestra un
    /// resumen en vivo de copias totales, impresora y tamano.
    /// </summary>
    public partial class QuantityDialog : Form
    {
        private readonly int _selectedCount;

        /// <summary>Cantidad elegida por el usuario (por producto).</summary>
        public int Quantity { get; private set; } = 1;

        /// <summary>Impresora seleccionada por el usuario.</summary>
        public string SelectedPrinter { get; private set; }

        /// <summary>Ancho de pagina elegido (mm).</summary>
        public double PageWidthMm { get; private set; }

        /// <summary>Alto de pagina elegido (mm).</summary>
        public double PageHeightMm { get; private set; }

        public QuantityDialog(int selectedCount, IEnumerable<string> printers,
            string preselectedPrinter, double widthMm, double heightMm)
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

            // Cargamos el tamano de pagina desde la configuracion, respetando limites.
            numWidth.Value = Clamp((decimal)widthMm, numWidth.Minimum, numWidth.Maximum);
            numHeight.Value = Clamp((decimal)heightMm, numHeight.Minimum, numHeight.Maximum);

            UpdateSummary();
            numQuantity.Select(0, numQuantity.Text.Length);
        }

        private static decimal Clamp(decimal value, decimal min, decimal max)
            => value < min ? min : (value > max ? max : value);

        private void OnSelectionChanged(object sender, EventArgs e)
        {
            UpdateSummary();
        }

        /// <summary>Actualiza el texto con copias totales, impresora y tamano de pagina.</summary>
        private void UpdateSummary()
        {
            int perProduct = (int)numQuantity.Value;
            int total = perProduct * _selectedCount;
            string printer = cboPrinter.SelectedItem as string ?? "(ninguna)";
            lblSummary.Text =
                $"Se imprimiran {total} etiqueta(s) ({perProduct} x {_selectedCount}) en: {printer}" +
                Environment.NewLine +
                $"Tamano de pagina: {numWidth.Value:0.#} x {numHeight.Value:0.#} mm";
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
            PageWidthMm = (double)numWidth.Value;
            PageHeightMm = (double)numHeight.Value;
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
