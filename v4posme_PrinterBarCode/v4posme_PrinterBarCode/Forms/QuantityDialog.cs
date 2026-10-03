using System;
using System.Windows.Forms;

namespace v4posme_PrinterBarCode.Forms
{
    /// <summary>
    /// Dialogo que pregunta cuantas etiquetas imprimir por producto seleccionado.
    /// </summary>
    public partial class QuantityDialog : Form
    {
        /// <summary>Cantidad elegida por el usuario.</summary>
        public int Quantity { get; private set; } = 1;

        public QuantityDialog(int selectedCount)
        {
            InitializeComponent();
            lblInfo.Text = $"Se aplica a cada uno de los {selectedCount} producto(s) seleccionado(s).";
            numQuantity.Select(0, numQuantity.Text.Length);
        }

        private void btnAccept_Click(object sender, EventArgs e)
        {
            Quantity = (int)numQuantity.Value;
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
