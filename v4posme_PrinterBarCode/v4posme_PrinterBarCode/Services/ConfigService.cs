using System;
using System.IO;
using Newtonsoft.Json;
using v4posme_PrinterBarCode.Models;

namespace v4posme_PrinterBarCode.Services
{
    /// <summary>
    /// Carga la configuracion de la aplicacion desde el archivo config.json.
    /// </summary>
    public static class ConfigService
    {
        public const string ConfigFileName = "config.json";

        /// <summary>
        /// Lee y deserializa config.json ubicado junto al ejecutable.
        /// </summary>
        public static AppConfig Load()
        {
            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, ConfigFileName);

            if (!File.Exists(path))
                throw new FileNotFoundException(
                    $"No se encontro el archivo de configuracion '{ConfigFileName}'. " +
                    $"Debe estar junto al ejecutable: {path}");

            var json = File.ReadAllText(path);
            var config = JsonConvert.DeserializeObject<AppConfig>(json);

            if (config == null)
                throw new InvalidOperationException("El archivo config.json esta vacio o es invalido.");

            if (string.IsNullOrWhiteSpace(config.ProductsUrl))
                throw new InvalidOperationException("config.json no contiene 'productsUrl'.");

            if (config.Barcode == null)
                config.Barcode = new BarcodeConfig();

            return config;
        }
    }
}
