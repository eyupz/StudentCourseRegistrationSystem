using System;
using System.Security.Cryptography;
using System.Text;

namespace StudentCourseRegistrationSystem.Helpers
{
    public static class PasswordHelper
    {
        private const int SaltSize = 16;
        
        public static string HashPassword(string password)
        {
            if (string.IsNullOrEmpty(password))
                throw new ArgumentException("Password cannot be null or empty.");

            byte[] salt;
            using (var rng = RandomNumberGenerator.Create())
            {
                salt = new byte[SaltSize];
                rng.GetBytes(salt);
            }

            using (var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 100000, HashAlgorithmName.SHA256))
            {
                byte[] hash = pbkdf2.GetBytes(32);
                return $"{Convert.ToBase64String(salt)}:{Convert.ToBase64String(hash)}";
            }
        }

        public static bool VerifyPassword(string password, string hashedPassword)
        {
            if (string.IsNullOrEmpty(password) || string.IsNullOrEmpty(hashedPassword))
                return false;

            var parts = hashedPassword.Split(':');
            if (parts.Length != 2)
            {
                // Fallback for old un-salted SHA256 hashes if any existed
                using (var sha256 = SHA256.Create())
                {
                    var oldHash = Convert.ToBase64String(sha256.ComputeHash(Encoding.UTF8.GetBytes(password)));
                    return oldHash == hashedPassword;
                }
            }

            try
            {
                byte[] salt = Convert.FromBase64String(parts[0]);
                byte[] storedHash = Convert.FromBase64String(parts[1]);

                using (var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 100000, HashAlgorithmName.SHA256))
                {
                    byte[] hash = pbkdf2.GetBytes(32);
                    for (int i = 0; i < 32; i++)
                    {
                        if (hash[i] != storedHash[i]) return false;
                    }
                    return true;
                }
            }
            catch
            {
                return false;
            }
        }
    }
}
