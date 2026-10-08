using System;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using Microsoft.Win32;

namespace v4posme_PrinterBarCode.Services
{
    /// <summary>
    /// Envia datos CRUDOS (RAW) directamente al spooler de Windows, sin que el driver
    /// los interprete como graficos. Es la unica forma de hacer que una impresora
    /// termica de etiquetas (HION / TSPL / ESC-POS) reciba sus comandos nativos.
    /// </summary>
    internal static class RawPrinterHelper
    {
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        private class DOCINFOW
        {
            [MarshalAs(UnmanagedType.LPWStr)] public string pDocName;
            [MarshalAs(UnmanagedType.LPWStr)] public string pOutputFile;
            [MarshalAs(UnmanagedType.LPWStr)] public string pDataType;
        }

        [DllImport("winspool.Drv", EntryPoint = "OpenPrinterW", SetLastError = true,
            CharSet = CharSet.Unicode, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        private static extern bool OpenPrinter(string src, out IntPtr hPrinter, IntPtr pd);

        [DllImport("winspool.Drv", EntryPoint = "ClosePrinter", SetLastError = true,
            ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        private static extern bool ClosePrinter(IntPtr hPrinter);

        [DllImport("winspool.Drv", EntryPoint = "StartDocPrinterW", SetLastError = true,
            CharSet = CharSet.Unicode, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        private static extern bool StartDocPrinter(IntPtr hPrinter, int level, [In] DOCINFOW di);

        [DllImport("winspool.Drv", EntryPoint = "EndDocPrinter", SetLastError = true,
            ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        private static extern bool EndDocPrinter(IntPtr hPrinter);

        [DllImport("winspool.Drv", EntryPoint = "StartPagePrinter", SetLastError = true,
            ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        private static extern bool StartPagePrinter(IntPtr hPrinter);

        [DllImport("winspool.Drv", EntryPoint = "EndPagePrinter", SetLastError = true,
            ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        private static extern bool EndPagePrinter(IntPtr hPrinter);

        [DllImport("winspool.Drv", EntryPoint = "WritePrinter", SetLastError = true,
            ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        private static extern bool WritePrinter(IntPtr hPrinter, IntPtr pBytes, int dwCount, out int dwWritten);

        // ------------------------------------------------------------------
        // API para escribir DIRECTO al puerto (USB001, COM1, LPT1...), evitando
        // por completo el driver de Windows. Es como lo hace la herramienta del
        // fabricante: el driver de etiquetas rechaza los datos RAW ("no puede
        // imprimir"), pero el puerto acepta el TSPL crudo sin interpretarlo.
        // ------------------------------------------------------------------

        private const uint GENERIC_WRITE = 0x40000000;
        private const uint OPEN_EXISTING = 3;
        private const uint FILE_ATTRIBUTE_NORMAL = 0x80;
        private static readonly IntPtr INVALID_HANDLE_VALUE = new IntPtr(-1);

        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        private static extern IntPtr CreateFile(string lpFileName, uint dwDesiredAccess,
            uint dwShareMode, IntPtr lpSecurityAttributes, uint dwCreationDisposition,
            uint dwFlagsAndAttributes, IntPtr hTemplateFile);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool WriteFile(IntPtr hFile, byte[] lpBuffer, int nNumberOfBytesToWrite,
            out int lpNumberOfBytesWritten, IntPtr lpOverlapped);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool CloseHandle(IntPtr hObject);

        /// <summary>
        /// Envia una cadena de comandos (TSPL/ESC) a la impresora como datos RAW.
        /// La codificacion por defecto es la de 1 byte (Latin1/ASCII extendido),
        /// adecuada para comandos TSPL que no llevan caracteres multibyte.
        /// </summary>
        public static void SendStringToPrinter(string printerName, string commands, Encoding encoding = null)
        {
            if (string.IsNullOrWhiteSpace(printerName))
                throw new ArgumentException("El nombre de la impresora es obligatorio.", nameof(printerName));

            encoding = encoding ?? Encoding.GetEncoding(850); // CP850: soporta acentos espanoles.
            byte[] bytes = encoding.GetBytes(commands);
            SendBytesToPrinter(printerName, bytes);
        }

        /// <summary>
        /// Envia una cadena de comandos DIRECTO a un puerto fisico (USB001, COM1,
        /// LPT1...), sin pasar por el driver ni el spooler. Es la via correcta
        /// cuando el driver de la impresora rechaza los datos RAW.
        /// </summary>
        public static void SendStringToPort(string portName, string commands, Encoding encoding = null)
        {
            if (string.IsNullOrWhiteSpace(portName))
                throw new ArgumentException("El nombre del puerto es obligatorio.", nameof(portName));

            encoding = encoding ?? Encoding.GetEncoding(850);
            byte[] bytes = encoding.GetBytes(commands);
            SendBytesToPort(portName, bytes);
        }

        /// <summary>
        /// Escribe bytes crudos directamente al puerto fisico de la impresora.
        ///
        /// IMPORTANTE: solo los puertos SERIE (COMx) y PARALELO (LPTx) se pueden
        /// abrir con CreateFile. Los puertos USBxxx de impresora NO son dispositivos
        /// abribles por nombre ("\\.\USB001" falla con codigo 2/3): ese nombre solo
        /// existe dentro del spooler. Para esos casos NO se puede escribir al puerto
        /// de forma directa; el metodo lanza una excepcion y el llamador debe caer
        /// al envio RAW por spooler (SendStringToPrinter), que si acepta los datos.
        /// </summary>
        public static void SendBytesToPort(string portName, byte[] bytes)
        {
            if (string.IsNullOrWhiteSpace(portName))
                throw new ArgumentException("El nombre del puerto es obligatorio.", nameof(portName));

            if (!IsDirectlyOpenablePort(portName))
                throw new NotSupportedException(
                    $"El puerto '{portName}' es un puerto USB de impresora y no se puede abrir " +
                    "directamente con CreateFile. Use el envio por spooler (RAW).");

            // COM/LPT: se abren con el prefijo "\\.\".
            string path = portName.StartsWith(@"\\.\") ? portName : @"\\.\" + portName.TrimEnd(':');

            IntPtr h = CreateFile(path, GENERIC_WRITE, 0, IntPtr.Zero, OPEN_EXISTING,
                FILE_ATTRIBUTE_NORMAL, IntPtr.Zero);

            if (h == INVALID_HANDLE_VALUE)
            {
                int code = Marshal.GetLastWin32Error();
                throw new InvalidOperationException(
                    $"No se pudo abrir el puerto '{portName}' directamente (codigo {code}). " +
                    "Verifique que el puerto exista y que la impresora no este en uso por otra " +
                    "aplicacion (como la herramienta del fabricante).");
            }

            try
            {
                int written;
                if (!WriteFile(h, bytes, bytes.Length, out written, IntPtr.Zero))
                    throw new InvalidOperationException(
                        $"WriteFile fallo al escribir en el puerto '{portName}' (codigo {Marshal.GetLastWin32Error()}).");

                Logger.Info($"Escritos {written} byte(s) directo al puerto '{portName}'.");
            }
            finally
            {
                CloseHandle(h);
            }
        }

        /// <summary>
        /// Indica si un puerto se puede abrir directamente con CreateFile.
        /// Solo COMx y LPTx son dispositivos reales abribles; USBxxx no lo es.
        /// </summary>
        public static bool IsDirectlyOpenablePort(string portName)
        {
            if (string.IsNullOrWhiteSpace(portName)) return false;
            string p = portName.Trim().TrimEnd(':').ToUpperInvariant();
            if (p.StartsWith(@"\\.\")) p = p.Substring(4);
            return p.StartsWith("COM") || p.StartsWith("LPT");
        }

        // ------------------------------------------------------------------
        // Escritura DIRECTA al dispositivo de impresora USB (clase usbprint),
        // igual que la herramienta del fabricante. Se abre la interfaz del
        // dispositivo por su VID/PID y se escriben los bytes TSPL con WriteFile,
        // SIN pasar por el spooler ni el driver (que atascan el trabajo en USB).
        // ------------------------------------------------------------------

        // GUID de la clase de interfaz de impresoras USB (GUID_DEVINTERFACE_USBPRINT).
        private const string PrinterInterfaceClassGuid = "{28d78fad-5a12-11d1-ae5b-0000f803a8c2}";

        /// <summary>
        /// Envia comandos a una impresora USB localizandola por VID y PID
        /// (ej. vid="0471", pid="0055"). Resuelve la ruta de interfaz del
        /// dispositivo y escribe los bytes directo, como la herramienta del fabricante.
        /// </summary>
        public static void SendStringToUsbDevice(string vid, string pid, string commands, Encoding encoding = null)
        {
            string devicePath = ResolveUsbPrinterPath(vid, pid);
            if (devicePath == null)
                throw new InvalidOperationException(
                    $"No se encontro un dispositivo de impresora USB con VID={vid} PID={pid}. " +
                    "Verifique que la impresora este encendida y conectada.");

            encoding = encoding ?? Encoding.GetEncoding(850);
            byte[] bytes = encoding.GetBytes(commands);
            SendBytesToDevicePath(devicePath, bytes);
        }

        /// <summary>
        /// Escribe bytes crudos a una ruta de dispositivo (ej.
        /// "\\?\USB#VID_0471&amp;PID_0055#...#{guid}") abierta con CreateFile.
        /// </summary>
        public static void SendBytesToDevicePath(string devicePath, byte[] bytes)
        {
            if (string.IsNullOrWhiteSpace(devicePath))
                throw new ArgumentException("La ruta del dispositivo es obligatoria.", nameof(devicePath));

            IntPtr h = CreateFile(devicePath, GENERIC_WRITE, 0, IntPtr.Zero, OPEN_EXISTING,
                FILE_ATTRIBUTE_NORMAL, IntPtr.Zero);

            if (h == INVALID_HANDLE_VALUE)
            {
                int code = Marshal.GetLastWin32Error();
                throw new InvalidOperationException(
                    $"No se pudo abrir el dispositivo USB (codigo {code}). " +
                    "Cierre la herramienta del fabricante (Printer Setting) si esta abierta, " +
                    "ya que mantiene el dispositivo ocupado.");
            }

            try
            {
                int written;
                if (!WriteFile(h, bytes, bytes.Length, out written, IntPtr.Zero))
                    throw new InvalidOperationException(
                        $"WriteFile fallo al escribir en el dispositivo USB (codigo {Marshal.GetLastWin32Error()}).");

                Logger.Info($"Escritos {written} byte(s) DIRECTO al dispositivo USB '{devicePath}'.");
            }
            finally
            {
                CloseHandle(h);
            }
        }

        /// <summary>
        /// Localiza la ruta de interfaz de un dispositivo de impresora USB por VID/PID
        /// leyendo el registro de clases de dispositivo de Windows. Devuelve una ruta
        /// lista para CreateFile ("\\?\USB#VID_xxxx&amp;PID_yyyy#...#{guid}") o null.
        /// </summary>
        public static string ResolveUsbPrinterPath(string vid, string pid)
        {
            if (string.IsNullOrWhiteSpace(vid) || string.IsNullOrWhiteSpace(pid))
                return null;

            // Normalizamos a mayusculas sin "0x".
            vid = vid.Trim().TrimStart('0', 'x', 'X').PadLeft(4, '0').ToUpperInvariant();
            pid = pid.Trim().TrimStart('0', 'x', 'X').PadLeft(4, '0').ToUpperInvariant();
            string match = $"VID_{vid}&PID_{pid}";

            string key = @"SYSTEM\CurrentControlSet\Control\DeviceClasses\" + PrinterInterfaceClassGuid;
            using (var baseKey = Registry.LocalMachine.OpenSubKey(key))
            {
                if (baseKey == null) return null;

                foreach (var sub in baseKey.GetSubKeyNames())
                {
                    // El nombre de subclave viene como "##?#USB#VID_0471&PID_0055#USB002#{guid}".
                    if (sub.IndexOf(match, StringComparison.OrdinalIgnoreCase) < 0)
                        continue;

                    // Convertimos el nombre simbolico a ruta de CreateFile:
                    //   "##?#USB#..." -> "\\?\USB#..."
                    string path = sub.Replace("##?#", @"\\?\").Replace("#", @"\");
                    // Ojo: el separador real entre VID/PID/serie y el GUID es '#',
                    // asi que reconstruimos correctamente respetando ese formato.
                    path = BuildDevicePath(sub);
                    Logger.Info($"Dispositivo USB impresora encontrado: {path}");
                    return path;
                }
            }

            return null;
        }

        /// <summary>
        /// Convierte el nombre simbolico del registro
        /// ("##?#USB#VID_0471&amp;PID_0055#USB002#{guid}") a la ruta valida para
        /// CreateFile ("\\?\USB#VID_0471&amp;PID_0055#USB002#{guid}").
        /// </summary>
        private static string BuildDevicePath(string symbolic)
        {
            // Solo cambia el prefijo "##?#" por "\\?\"; el resto de '#' se conservan.
            if (symbolic.StartsWith("##?#"))
                return @"\\?\" + symbolic.Substring(4);
            if (symbolic.StartsWith(@"\\?\"))
                return symbolic;
            return @"\\?\" + symbolic;
        }

        /// <summary>Envia un arreglo de bytes crudos a la impresora.</summary>
        public static void SendBytesToPrinter(string printerName, byte[] bytes)
        {
            IntPtr hPrinter;
            var di = new DOCINFOW
            {
                pDocName = "v4posme Etiquetas TSPL",
                pDataType = "RAW"
            };

            if (!OpenPrinter(printerName, out hPrinter, IntPtr.Zero))
            {
                int code = Marshal.GetLastWin32Error();
                // 1801 = ERROR_INVALID_PRINTER_NAME: el nombre no corresponde a
                // ninguna impresora instalada. Es el fallo mas comun en campo.
                string detalle = code == 1801
                    ? $"El nombre '{printerName}' no corresponde a ninguna impresora instalada. " +
                      "Revise 'printerName'/'printerNamePriority' en config.json y use el nombre exacto " +
                      "que aparece en Windows (Configuracion > Impresoras)."
                    : $"No se pudo abrir la impresora '{printerName}' (codigo {code}).";
                throw new InvalidOperationException(detalle);
            }

            IntPtr pUnmanagedBytes = IntPtr.Zero;
            try
            {
                if (!StartDocPrinter(hPrinter, 1, di))
                    throw new InvalidOperationException(
                        $"StartDocPrinter fallo (codigo {Marshal.GetLastWin32Error()}).");

                try
                {
                    if (!StartPagePrinter(hPrinter))
                        throw new InvalidOperationException(
                            $"StartPagePrinter fallo (codigo {Marshal.GetLastWin32Error()}).");

                    pUnmanagedBytes = Marshal.AllocCoTaskMem(bytes.Length);
                    Marshal.Copy(bytes, 0, pUnmanagedBytes, bytes.Length);

                    int written;
                    if (!WritePrinter(hPrinter, pUnmanagedBytes, bytes.Length, out written))
                        throw new InvalidOperationException(
                            $"WritePrinter fallo (codigo {Marshal.GetLastWin32Error()}).");

                    EndPagePrinter(hPrinter);
                }
                finally
                {
                    EndDocPrinter(hPrinter);
                }
            }
            finally
            {
                if (pUnmanagedBytes != IntPtr.Zero)
                    Marshal.FreeCoTaskMem(pUnmanagedBytes);
                ClosePrinter(hPrinter);
            }
        }
    }
}
