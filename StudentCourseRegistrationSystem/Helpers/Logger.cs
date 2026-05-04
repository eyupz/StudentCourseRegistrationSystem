using System;
using System.IO;

namespace StudentCourseRegistrationSystem.Helpers
{
    /// <summary>
    /// Basit dosya tabanlı loglama sistemi. SOLID - Single Responsibility.
    /// </summary>
    public static class Logger
    {
        private static readonly string LogFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Logs");
        private static readonly string LogFile = Path.Combine(LogFolder, $"obs_{DateTime.Now:yyyy-MM-dd}.log");

        static Logger()
        {
            if (!Directory.Exists(LogFolder))
                Directory.CreateDirectory(LogFolder);
        }

        public static void Info(string message) => Write("INFO", message);
        public static void Warning(string message) => Write("WARN", message);
        public static void Error(string message, Exception ex = null)
        {
            Write("ERROR", message);
            if (ex != null)
            {
                Write("ERROR", $"  → {ex.GetType().Name}: {ex.Message}");
                if (ex.InnerException != null)
                    Write("ERROR", $"  → Inner: {ex.InnerException.GetType().Name}: {ex.InnerException.Message}");
                if (ex.InnerException?.InnerException != null)
                    Write("ERROR", $"  → Inner2: {ex.InnerException.InnerException.Message}");
            }
        }

        private static void Write(string level, string message)
        {
            try
            {
                string line = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] [{level}] {message}";
                File.AppendAllText(LogFile, line + Environment.NewLine);
            }
            catch { /* Log hataları uygulamayı durdurmasın */ }
        }
    }
}
