using System;
using System.Windows.Forms;
using v4posme_PrinterBarCode.Models;
using v4posme_PrinterBarCode.Services;

namespace v4posme_PrinterBarCode.Forms
{
    /// <summary>
    /// Formulario de configuracion: carga los valores de config.json en controles
    /// editables y, al guardar, los escribe de vuelta al archivo en disco.
    /// </summary>
    public partial class ConfigForm : Form
    {
        /// <summary>Configuracion resultante tras guardar (null si el usuario cancela).</summary>
        public AppConfig ResultConfig { get; private set; }

        private readonly AppConfig _config;

        public ConfigForm(AppConfig config)
        {
            InitializeComponent();
            _config = config;
            CargarValores();
        }

        /// <summary>Vuelca los valores del AppConfig en los controles del formulario.</summary>
        private void CargarValores()
        {
            txtVersion.Text = _config.Version ?? "";
            txtProductsUrl.Text = _config.ProductsUrl ?? "";
            txtHttpMethod.Text = _config.HttpMethod ?? "POST";
            txtNickname.Text = _config.TxtNickname ?? "";
            txtPassword.Text = _config.TxtPassword ?? "";
            txtUserAgent.Text = _config.UserAgent ?? "";
            txtPrinterName.Text = _config.PrinterName ?? "";
            txtPrinterPort.Text = _config.PrinterPort ?? "";
            txtUsbVid.Text = _config.UsbVid ?? "";
            txtUsbPid.Text = _config.UsbPid ?? "";
            txtPrinterNamePriority.Text = _config.PrinterNamePriority ?? "";
            txtTypePrinter.Text = _config.TypePrinter ?? "";
            txtLogFilePath.Text = _config.LogFilePath ?? "";
            numTimeout.Value = Clamp(_config.RequestTimeoutSeconds, (int)numTimeout.Minimum, (int)numTimeout.Maximum);
        }

        /// <summary>Toma los valores de los controles y los escribe en el AppConfig.</summary>
        private void VolcarValores()
        {
            _config.Version = txtVersion.Text.Trim();
            _config.ProductsUrl = txtProductsUrl.Text.Trim();
            _config.HttpMethod = txtHttpMethod.Text.Trim();
            _config.TxtNickname = txtNickname.Text.Trim();
            _config.TxtPassword = txtPassword.Text;
            _config.UserAgent = txtUserAgent.Text.Trim();
            _config.PrinterName = txtPrinterName.Text.Trim();
            _config.PrinterPort = txtPrinterPort.Text.Trim();
            _config.UsbVid = txtUsbVid.Text.Trim();
            _config.UsbPid = txtUsbPid.Text.Trim();
            _config.PrinterNamePriority = txtPrinterNamePriority.Text.Trim();
            _config.TypePrinter = txtTypePrinter.Text.Trim();
            _config.LogFilePath = txtLogFilePath.Text.Trim();
            _config.RequestTimeoutSeconds = (int)numTimeout.Value;
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtProductsUrl.Text))
            {
                MessageBox.Show(this, "La URL de productos es obligatoria.", "Configuracion",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                VolcarValores();
                ConfigService.Save(_config);
                ResultConfig = _config;

                MessageBox.Show(this,
                    "Configuracion guardada correctamente.\n\n" +
                    "Algunos cambios (como la impresora o el servidor) se aplican al reiniciar la aplicacion.",
                    "Configuracion", MessageBoxButtons.OK, MessageBoxIcon.Information);

                DialogResult = DialogResult.OK;
                Close();
            }
            catch (Exception ex)
            {
                Logger.Error("No se pudo guardar la configuracion.", ex);
                MessageBox.Show(this,
                    "No se pudo guardar la configuracion.\n\n" + ex.Message,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private static int Clamp(int v, int min, int max) =>
            v < min ? min : (v > max ? max : v);
    }
}
