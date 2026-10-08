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

        /// <summary>
        /// Puerto fisico de la impresora (ej. "COM1", "LPT1"). Si es un puerto COM/LPT
        /// real, los trabajos TSPL se escriben DIRECTO al puerto. Los puertos USBxxx
        /// NO sirven aqui (no se pueden abrir por nombre); para USB use usbVid/usbPid.
        /// </summary>
        [JsonProperty("printerPort")]
        public string PrinterPort { get; set; } = "";

        /// <summary>
        /// VID (Vendor ID) del dispositivo de impresora USB, en hexadecimal de 4
        /// digitos (ej. "0471"). Si usbVid y usbPid tienen valor, los trabajos TSPL
        /// se escriben DIRECTO al dispositivo USB (igual que la herramienta del
        /// fabricante), evitando el spooler/driver que atascan el trabajo. Es el
        /// metodo mas fiable para impresoras TSPL tipo HOIN conectadas por USB.
        /// </summary>
        [JsonProperty("usbVid")]
        public string UsbVid { get; set; } = "";

        /// <summary>PID (Product ID) del dispositivo de impresora USB, hex 4 digitos (ej. "0055").</summary>
        [JsonProperty("usbPid")]
        public string UsbPid { get; set; } = "";

        /// <summary>
        /// Tipo/lenguaje de la impresora. Determina COMO se envia el trabajo:
        ///   "HION" / "TSPL" / "TSC"  -> se envian comandos TSPL nativos en RAW
        ///                                (la impresora genera el codigo de barra por hardware).
        ///   cualquier otro valor o vacio -> se dibuja con GDI (System.Drawing.Printing).
        /// Las impresoras termicas de etiqueta HION requieren TSPL para que el codigo
        /// de barra salga nitido y escaneable; por eso es el modo recomendado.
        /// </summary>
        [JsonProperty("typePrinter")]
        public string TypePrinter { get; set; } = "";

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

        /// <summary>
        /// Si es true, se imprime el valor del codigo de barra como TEXTO legible
        /// (una linea con los digitos), ubicado entre el nombre y las barras. Si es
        /// false, no se muestra ese texto y solo salen las barras del codigo.
        /// Orden de la etiqueta: nombre, codigo de barra (texto), barras, precio.
        /// </summary>
        [JsonProperty("showBarcodeText")]
        public bool ShowBarcodeText { get; set; } = true;

        [JsonProperty("showPrice")]
        public bool ShowPrice { get; set; } = true;

        [JsonProperty("marginMm")]
        public double MarginMm { get; set; } = 1.5;

        [JsonProperty("copiesPerRow")]
        public int CopiesPerRow { get; set; } = 1;

        // ------------------------------------------------------------------
        // Parametros especificos de la impresion TSPL (impresoras HION/TSC).
        // Solo se usan cuando typePrinter es HION/TSPL/TSC.
        // ------------------------------------------------------------------

        /// <summary>
        /// Resolucion de la impresora en puntos por pulgada. La mayoria de las
        /// termicas de etiqueta son de 203 dpi (8 dots/mm); algunas son de 300 dpi
        /// (~11.8 dots/mm). TSPL trabaja en dots, por eso debe coincidir con tu modelo.
        /// </summary>
        [JsonProperty("printerDpi")]
        public int PrinterDpi { get; set; } = 203;

        /// <summary>Separacion vertical (GAP) entre etiquetas, en mm.</summary>
        [JsonProperty("gapMm")]
        public double GapMm { get; set; } = 2.0;

        /// <summary>Desplazamiento del GAP, en mm (normalmente 0).</summary>
        [JsonProperty("gapOffsetMm")]
        public double GapOffsetMm { get; set; } = 0.0;

        /// <summary>Oscuridad/densidad del cabezal termico TSPL (0-15).</summary>
        [JsonProperty("density")]
        public int Density { get; set; } = 10;

        /// <summary>
        /// Velocidad de impresion TSPL en pulgadas por segundo. 0 = no enviar el
        /// comando SPEED (usar el valor por defecto de la impresora).
        /// </summary>
        [JsonProperty("speed")]
        public int Speed { get; set; } = 0;

        /// <summary>
        /// Si es true, antes de imprimir se envia un pitido (comando SOUND) para
        /// confirmar fisicamente que la impresora recibe e interpreta el TSPL.
        /// Util para diagnosticar en campo cuando "parece que imprime pero no pasa nada".
        /// </summary>
        [JsonProperty("tsplBeepTest")]
        public bool TsplBeepTest { get; set; } = false;
    }
}
