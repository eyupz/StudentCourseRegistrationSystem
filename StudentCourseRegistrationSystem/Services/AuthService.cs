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
            {
                throw new ArgumentException("Username and password are required.");
            }

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
            // Since there is no 'IsActive' flag in User model, we'll assume they are active if they exist in DB
            if (user == null) return false;

            return _context.Users.Any(u => u.Id == user.Id);
        }
        
        public void Logout()
        {
            SessionManager.Logout();
        }
    }
}
