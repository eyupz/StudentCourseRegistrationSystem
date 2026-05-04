using System;
using System.Text;

namespace StudentCourseRegistrationSystem.Helpers
{
    public static class ErrorHelper
    {
        /// <summary>
        /// Tüm iç exception'ları yinelemeli olarak döndürür.
        /// </summary>
        public static string GetFullExceptionMessage(Exception ex)
        {
            if (ex == null) return "Bilinmeyen hata.";

            var sb = new StringBuilder();
            sb.AppendLine(ex.Message);

            var inner = ex.InnerException;
            int depth = 1;
            while (inner != null && depth <= 5)
            {
                sb.AppendLine($"[Detay {depth}]: {inner.Message}");
                inner = inner.InnerException;
                depth++;
            }

            return sb.ToString().Trim();
        }

        /// <summary>
        /// EF Core DbUpdateException için tüm detayları döndürür.
        /// </summary>
        public static string GetDbUpdateErrorMessage(Exception ex)
        {
            if (ex == null) return "Bilinmeyen veritabanı hatası.";

            var sb = new StringBuilder();
            sb.AppendLine("Veritabanı kayıt hatası:");
            sb.AppendLine(GetFullExceptionMessage(ex));

            // SQLite inner exception genellikle SqliteException'dır
            var inner = ex;
            while (inner != null)
            {
                if (inner.GetType().Name.Contains("SqliteException") ||
                    inner.GetType().Name.Contains("SQLiteException"))
                {
                    sb.AppendLine($"[SQLite]: {inner.Message}");
                    break;
                }
                inner = inner.InnerException;
            }

            return sb.ToString().Trim();
        }
    }
}
