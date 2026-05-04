using System;
using System.Windows.Forms;

namespace StudentCourseRegistrationSystem.Helpers
{
    /// <summary>
    /// UI katmanında exception'ları yakalayıp kullanıcıya anlamlı şekilde gösterir.
    /// </summary>
    public static class ErrorHelper
    {
        /// <summary>
        /// Exception zincirindeki en derin ve anlamlı mesajı bulur.
        /// EF hataları için genellikle InnerException içindeki SQLite hatasını yakalar.
        /// </summary>
        public static string GetFullMessage(Exception ex)
        {
            if (ex == null) return "Bilinmeyen bir hata oluştu.";

            var current = ex;
            string lastMessage = ex.Message;

            // Zinciri tara ve en son anlamlı mesajı bul
            while (current.InnerException != null)
            {
                current = current.InnerException;
                
                // Eğer mesaj "An error occurred while saving the entity changes..." ise 
                // bu EF'nin genel mesajıdır, bir sonrakine geçelim.
                if (current.Message.Contains("An error occurred while saving the entity changes") ||
                    current.Message.Contains("See the inner exception for details"))
                {
                    continue;
                }
                
                lastMessage = current.Message;
            }

            // SQLite özel hataları için Türkçeleştirme veya temizlik
            if (lastMessage.Contains("UNIQUE constraint failed"))
            {
                if (lastMessage.Contains("Users.Username")) return "Bu kullanıcı adı zaten sistemde kayıtlı!";
                if (lastMessage.Contains("Students.StudentNumber")) return "Bu öğrenci numarası zaten sistemde kayıtlı!";
                return "Bu kayıt zaten mevcut (Benzersizlik hatası).";
            }

            return lastMessage;
        }

        /// <summary>
        /// Hata mesajını hem kullanıcıya gösterir hem loglar.
        /// </summary>
        public static void Show(Exception ex, string context = "")
        {
            string rootMsg = GetFullMessage(ex);
            Logger.Error(string.IsNullOrEmpty(context) ? "Hata" : context, ex);
            
            MessageBox.Show(
                $"❌ {rootMsg}",
                context ?? "Sistem Hatası",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error
            );
        }

        /// <summary>
        /// İş kuralı ihlallerini (InvalidOperationException) kullanıcıya gösterir.
        /// </summary>
        public static void ShowWarning(string message)
        {
            MessageBox.Show($"⚠️ {message}", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
    }
}
