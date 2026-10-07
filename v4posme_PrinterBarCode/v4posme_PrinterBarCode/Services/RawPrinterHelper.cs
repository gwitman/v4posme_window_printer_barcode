using System;
using System.Runtime.InteropServices;
using System.Text;

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
