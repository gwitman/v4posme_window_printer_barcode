using Newtonsoft.Json;

namespace v4posme_PrinterBarCode.Models
{
    /// <summary>
    /// Representa la configuracion de la aplicacion leida desde config.json.
    /// </summary>
    public class AppConfig
    {
        /// <summary>Version de la aplicacion, leida directamente del config.json.</summary>
        [JsonProperty("version")]
        public string Version { get; set; }

        /// <summary>
        /// Si tiene un valor distinto de vacio, SIEMPRE se imprime en esta impresora,
        /// sin importar cual seleccione el usuario en el dialogo.
        /// </summary>
        [JsonProperty("printerNamePriority")]
        public string PrinterNamePriority { get; set; }

        [JsonProperty("productsUrl")]
        public string ProductsUrl { get; set; }

        /// <summary>Metodo HTTP para la peticion de productos: GET o POST.</summary>
        [JsonProperty("httpMethod")]
        public string HttpMethod { get; set; } = "GET";

        /// <summary>Usuario enviado como parametro txtNickname en el POST.</summary>
        [JsonProperty("txtNickname")]
        public string TxtNickname { get; set; }

        /// <summary>Clave enviada como parametro txtPassword en el POST.</summary>
        [JsonProperty("txtPassword")]
        public string TxtPassword { get; set; }

        /// <summary>
        /// User-Agent de la peticion. El WAF del servidor bloquea User-Agents de
        /// navegador (Chrome/Mozilla); por eso el valor por defecto imita a PowerShell.
        /// </summary>
        [JsonProperty("userAgent")]
        public string UserAgent { get; set; } =
            "Mozilla/5.0 (Windows NT; Windows NT 10.0; es-NI) WindowsPowerShell/5.1";

        [JsonProperty("printerName")]
        public string PrinterName { get; set; }

        [JsonProperty("logFilePath")]
        public string LogFilePath { get; set; } = "logs/app_log.txt";

        [JsonProperty("requestTimeoutSeconds")]
        public int RequestTimeoutSeconds { get; set; } = 60;

        [JsonProperty("barcode")]
        public BarcodeConfig Barcode { get; set; } = new BarcodeConfig();
    }

    /// <summary>
    /// Parametros de impresion de los codigos de barra.
    /// </summary>
    public class BarcodeConfig
    {
        // Etiqueta por defecto: 2 x 1 pulgada (50.8 x 25.4 mm).
        [JsonProperty("widthMm")]
        public double WidthMm { get; set; } = 50.8;

        [JsonProperty("heightMm")]
        public double HeightMm { get; set; } = 25.4;

        [JsonProperty("fontName")]
        public string FontName { get; set; } = "IDAutomationHC39M";

        [JsonProperty("fontSize")]
        public float FontSize { get; set; } = 24;

        [JsonProperty("labelFontName")]
        public string LabelFontName { get; set; } = "Arial";

        [JsonProperty("labelFontSize")]
        public float LabelFontSize { get; set; } = 7;

        /// <summary>Caracter(es) adicionales al inicio del codigo (ej. "*" para Code39).</summary>
        [JsonProperty("prefix")]
        public string Prefix { get; set; } = "*";

        /// <summary>Caracter(es) adicionales al final del codigo.</summary>
        [JsonProperty("suffix")]
        public string Suffix { get; set; } = "*";

        [JsonProperty("showProductName")]
        public bool ShowProductName { get; set; } = true;

        [JsonProperty("showPrice")]
        public bool ShowPrice { get; set; } = true;

        [JsonProperty("marginMm")]
        public double MarginMm { get; set; } = 1.5;

        [JsonProperty("copiesPerRow")]
        public int CopiesPerRow { get; set; } = 1;
    }
}
