using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using StudentCourseRegistrationSystem.Data;
using StudentCourseRegistrationSystem.Helpers;
using StudentCourseRegistrationSystem.Models;

namespace StudentCourseRegistrationSystem.Services
{
    public class AuthService
    {
        private readonly AppDbContext _context;

        public AuthService(AppDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public User Login(string username, string password)
        {
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
                throw new ArgumentException("Kullanıcı adı ve şifre zorunludur.");

            var user = _context.Users
                .Include(u => u.Role)
                .Include(u => u.Student)
                .Include(u => u.Instructor)
                .SingleOrDefault(u => u.Username == username);

            if (user != null && PasswordHelper.VerifyPassword(password, user.PasswordHash))
            {
                SessionManager.Login(user);
                return user;
            }

            return null;
        }

        public string GetUserRole(User user)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            return user.Role?.Name ?? "Unknown";
        }

        public bool IsUserActive(User user)
        {
            if (user == null) return false;
            // Kullanıcı DB'de varsa aktif kabul edilir
            return _context.Users.Any(u => u.Id == user.Id);
        }
        
        public void Logout()
        {
            SessionManager.Logout();
        }

        public void ChangePassword(string oldPassword, string newPassword)
        {
            var user = SessionManager.CurrentUser;
            if (user == null) throw new InvalidOperationException("Aktif oturum bulunamadı.");
            if (string.IsNullOrWhiteSpace(oldPassword) || string.IsNullOrWhiteSpace(newPassword)) 
                throw new ArgumentException("Şifre boş olamaz.");

            var dbUser = _context.Users.Find(user.Id);
            if (dbUser == null) throw new InvalidOperationException("Kullanıcı bulunamadı.");

            if (!PasswordHelper.VerifyPassword(oldPassword, dbUser.PasswordHash))
                throw new InvalidOperationException("Eski şifre yanlış.");

            dbUser.PasswordHash = PasswordHelper.HashPassword(newPassword);
            _context.Users.Update(dbUser);
            _context.SaveChanges();
            
            // Oturum kullanıcısını güncelle
            SessionManager.CurrentUser.PasswordHash = dbUser.PasswordHash;
        }

        public void ResetUserPassword(int userId, string newPassword)
        {
            var dbUser = _context.Users.Find(userId);
            if (dbUser == null) throw new InvalidOperationException("Kullanıcı bulunamadı.");
            
            dbUser.PasswordHash = PasswordHelper.HashPassword(newPassword);
            _context.Users.Update(dbUser);
            _context.SaveChanges();
        }
    }
}
