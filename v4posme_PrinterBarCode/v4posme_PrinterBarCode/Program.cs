using System;
using System.Net;
using System.Windows.Forms;
using v4posme_PrinterBarCode.Forms;
using v4posme_PrinterBarCode.Models;
using v4posme_PrinterBarCode.Services;

namespace v4posme_PrinterBarCode
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            // Forzar TLS 1.2/1.3. .NET Framework 4.7.2 puede negociar protocolos
            // antiguos (TLS 1.0/1.1) que el servidor rechaza, provocando que la
            // peticion falle en la PC con "no se puede crear un canal seguro SSL/TLS".
            ConfigureSecurityProtocol();

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // Capturamos cualquier excepcion no controlada en el log.
            Application.ThreadException += (s, e) =>
            {
                Logger.Error("Excepcion no controlada (UI).", e.Exception);
                MessageBox.Show("Ocurrio un error inesperado. Revise el log.\n\n" + e.Exception.Message,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            };
            AppDomain.CurrentDomain.UnhandledException += (s, e) =>
            {
                Logger.Error("Excepcion no controlada (dominio).", e.ExceptionObject as Exception);
            };

            AppConfig config;
            try
            {
                config = ConfigService.Load();
                Logger.Initialize(config.LogFilePath);
                Logger.Info($"Configuracion cargada. URL productos: {config.ProductsUrl}, Impresora: '{config.PrinterName}'.");
            }
            catch (Exception ex)
            {
                // Logger puede no estar inicializado todavia; intentamos registrar igual.
                Logger.Initialize(null);
                Logger.Error("No se pudo cargar la configuracion inicial.", ex);
                MessageBox.Show(
                    "No se pudo cargar la configuracion (config.json).\n\n" + ex.Message,
                    "Error de configuracion", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            Application.Run(new MainForm(config));
            Logger.Info("====== Aplicacion finalizada ======");
        }

        /// <summary>
        /// Habilita TLS 1.2 y, si el sistema operativo lo soporta, TLS 1.3.
        /// Tambien ignora protocolos obsoletos para que el handshake no falle.
        /// </summary>
        private static void ConfigureSecurityProtocol()
        {
            // Usamos SOLO TLS 1.2. Al mezclar TLS 1.3 el handshake cambia la huella
            // (fingerprint) y el WAF del servidor puede responder 403. TLS 1.2 es el
            // protocolo que el servidor acepta de forma consistente.
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            // El WAF tambien rechaza "Expect: 100-continue".
            ServicePointManager.Expect100Continue = false;
        }
    }
}
