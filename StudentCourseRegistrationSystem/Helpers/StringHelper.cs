using System;
using System.Text;

namespace StudentCourseRegistrationSystem.Helpers
{
    public static class StringHelper
    {
        public static string ConvertToEnglishLowercase(string text)
        {
            if (string.IsNullOrWhiteSpace(text)) return string.Empty;

            text = text.ToLower();
            text = text.Replace("ç", "c")
                       .Replace("ğ", "g")
                       .Replace("ı", "i")
                       .Replace("ö", "o")
                       .Replace("ş", "s")
                       .Replace("ü", "u");

            // Remove any remaining non-alphanumeric characters except letters
            StringBuilder sb = new StringBuilder();
            foreach (char c in text)
            {
                if ((c >= 'a' && c <= 'z') || (c >= '0' && c <= '9'))
                {
                    sb.Append(c);
                }
            }
            return sb.ToString();
        }
    }
}
