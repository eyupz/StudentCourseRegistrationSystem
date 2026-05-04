using System;
using System.Collections.Generic;
using System.Linq;
using StudentCourseRegistrationSystem.Data;
using StudentCourseRegistrationSystem.Helpers;
using StudentCourseRegistrationSystem.Models;

namespace StudentCourseRegistrationSystem.Services
{
    public class InstructorService
    {
        private readonly AppDbContext _context;

        public InstructorService(AppDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public List<Instructor> GetAllInstructors()
        {
            return _context.Instructors.ToList();
        }

        public ServiceResult AddInstructor(Instructor instructor)
        {
            try
            {
                if (instructor == null)
                    return ServiceResult.Failure("Öğretmen nesnesi boş olamaz.");
                if (string.IsNullOrWhiteSpace(instructor.FirstName))
                    return ServiceResult.Failure("Ad alanı zorunludur.");
                if (string.IsNullOrWhiteSpace(instructor.LastName))
                    return ServiceResult.Failure("Soyad alanı zorunludur.");

                _context.Instructors.Add(instructor);
                _context.SaveChanges();

                var role = _context.Roles.FirstOrDefault(r => r.Name == "Instructor");
                if (role != null)
                {
                    string username = StringHelper.ConvertToEnglishLowercase(instructor.FirstName + instructor.LastName);
                    int count = 1;
                    string baseUsername = username;
                    while (_context.Users.Any(u => u.Username == username))
                    {
                        username = baseUsername + count;
                        count++;
                    }

                    string firstInitial = instructor.FirstName.Substring(0, 1).ToLower();
                    string lastInitial = instructor.LastName.Substring(0, 1).ToLower();

                    _context.Users.Add(new User
                    {
                        Username = username,
                        PasswordHash = PasswordHelper.HashPassword($"123{firstInitial}{lastInitial}"),
                        Email = $"{username}@system.com",
                        RoleId = role.Id,
                        InstructorId = instructor.Id
                    });
                    _context.SaveChanges();
                }

                return ServiceResult.SuccessResult("Öğretmen başarıyla eklendi.");
            }
            catch (Exception ex)
            {
                return ServiceResult.Failure(ErrorHelper.GetDbUpdateErrorMessage(ex));
            }
        }

        public ServiceResult UpdateInstructor(Instructor instructor)
        {
            try
            {
                if (instructor == null)
                    return ServiceResult.Failure("Öğretmen nesnesi boş olamaz.");

                var existing = _context.Instructors.Find(instructor.Id);
                if (existing == null)
                    return ServiceResult.Failure("Öğretmen bulunamadı.");

                existing.FirstName = instructor.FirstName;
                existing.LastName = instructor.LastName;

                _context.SaveChanges();
                return ServiceResult.SuccessResult("Öğretmen başarıyla güncellendi.");
            }
            catch (Exception ex)
            {
                return ServiceResult.Failure(ErrorHelper.GetDbUpdateErrorMessage(ex));
            }
        }

        public ServiceResult DeleteInstructor(int id)
        {
            try
            {
                var instructor = _context.Instructors.Find(id);
                if (instructor == null)
                    return ServiceResult.Failure("Öğretmen bulunamadı.");

                _context.Instructors.Remove(instructor);
                _context.SaveChanges();
                return ServiceResult.SuccessResult("Öğretmen başarıyla silindi.");
            }
            catch (Exception ex)
            {
                return ServiceResult.Failure(ErrorHelper.GetDbUpdateErrorMessage(ex));
            }
        }

        public List<Instructor> SearchInstructors(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm)) return GetAllInstructors();
            searchTerm = searchTerm.ToLower();
            return _context.Instructors
                .Where(i => i.FirstName.ToLower().Contains(searchTerm) ||
                            i.LastName.ToLower().Contains(searchTerm))
                .ToList();
        }
    }
}
