using System;
using System.IO;
using System.Text;

namespace v4posme_PrinterBarCode.Services
{
    /// <summary>
    /// Registra el comportamiento de la aplicacion y los errores en un archivo de texto.
    /// Thread-safe mediante un lock sencillo.
    /// </summary>
    public static class Logger
    {
        private static readonly object _lock = new object();
        private static string _logFilePath;

        /// <summary>
        /// Inicializa el logger con la ruta destino. Si la ruta es relativa se resuelve
        /// contra el directorio de la aplicacion.
        /// </summary>
        public static void Initialize(string logFilePath)
        {
            if (string.IsNullOrWhiteSpace(logFilePath))
                logFilePath = "logs/app_log.txt";

            if (!Path.IsPathRooted(logFilePath))
                logFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, logFilePath);

            _logFilePath = logFilePath;

            try
            {
                var dir = Path.GetDirectoryName(_logFilePath);
                if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
                    Directory.CreateDirectory(dir);
            }
            catch
            {
                // Si no se puede crear el directorio, caemos a la carpeta base.
                _logFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "app_log.txt");
            }

            Info("====== Aplicacion iniciada ======");
        }

        public static void Info(string message) => Write("INFO", message);

        public static void Warn(string message) => Write("WARN", message);

        public static void Error(string message, Exception ex = null)
        {
            var sb = new StringBuilder(message);
            if (ex != null)
            {
                sb.AppendLine();
                sb.AppendLine("  Exception: " + ex.GetType().FullName);
                sb.AppendLine("  Message: " + ex.Message);
                sb.AppendLine("  StackTrace: " + ex.StackTrace);
                if (ex.InnerException != null)
                    sb.AppendLine("  Inner: " + ex.InnerException.Message);
            }
            Write("ERROR", sb.ToString());
        }

        private static void Write(string level, string message)
        {
            var line = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}] [{level}] {message}";
            lock (_lock)
            {
                try
                {
                    var path = _logFilePath ?? Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "app_log.txt");
                    File.AppendAllText(path, line + Environment.NewLine, Encoding.UTF8);
                }
                catch
                {
                    // Nunca propagar errores del logger.
                }
            }
        }
    }
}
