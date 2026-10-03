using Newtonsoft.Json;

namespace v4posme_PrinterBarCode.Models
{
    /// <summary>
    /// Representa la configuracion de la aplicacion leida desde config.json.
    /// </summary>
    public class AppConfig
    {
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
        [JsonProperty("widthMm")]
        public double WidthMm { get; set; } = 50;

        [JsonProperty("heightMm")]
        public double HeightMm { get; set; } = 30;

        [JsonProperty("fontName")]
        public string FontName { get; set; } = "IDAutomationHC39M";

        [JsonProperty("fontSize")]
        public float FontSize { get; set; } = 24;

        [JsonProperty("labelFontName")]
        public string LabelFontName { get; set; } = "Arial";

        [JsonProperty("labelFontSize")]
        public float LabelFontSize { get; set; } = 8;

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
        public double MarginMm { get; set; } = 2;

        [JsonProperty("copiesPerRow")]
        public int CopiesPerRow { get; set; } = 1;

        /// <summary>Dibuja un borde (perimetro) alrededor de cada etiqueta.</summary>
        [JsonProperty("showBorder")]
        public bool ShowBorder { get; set; } = true;

        /// <summary>Grosor del borde de la etiqueta en milimetros.</summary>
        [JsonProperty("borderThicknessMm")]
        public double BorderThicknessMm { get; set; } = 0.3;
    }
}
