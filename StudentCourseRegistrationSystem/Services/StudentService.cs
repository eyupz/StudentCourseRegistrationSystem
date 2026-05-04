using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using StudentCourseRegistrationSystem.Data;
using StudentCourseRegistrationSystem.Helpers;
using StudentCourseRegistrationSystem.Models;

namespace StudentCourseRegistrationSystem.Services
{
    public class StudentService
    {
        private readonly AppDbContext _context;

        public StudentService(AppDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public List<Student> GetAllStudents()
        {
            return _context.Students
                .Include(s => s.Department)
                .Include(s => s.User)
                .ToList();
        }

        public Student GetStudentById(int id)
        {
            return _context.Students
                .Include(s => s.Department)
                .Include(s => s.User)
                .SingleOrDefault(s => s.Id == id);
        }

        public Student GetStudentByNumber(string studentNumber)
        {
            if (string.IsNullOrWhiteSpace(studentNumber)) return null;
            return _context.Students
                .Include(s => s.Department)
                .Include(s => s.User)
                .SingleOrDefault(s => s.StudentNumber == studentNumber);
        }

        public ServiceResult AddStudent(Student student)
        {
            try
            {
                if (student == null)
                    return ServiceResult.Failure("Öğrenci nesnesi boş olamaz.");
                if (string.IsNullOrWhiteSpace(student.FirstName))
                    return ServiceResult.Failure("Ad alanı zorunludur.");
                if (string.IsNullOrWhiteSpace(student.LastName))
                    return ServiceResult.Failure("Soyad alanı zorunludur.");
                if (student.DepartmentId <= 0 || !_context.Departments.Any(d => d.Id == student.DepartmentId))
                    return ServiceResult.Failure("Geçerli bir bölüm seçilmelidir.");

                // Öğrenci numarası üret
                var dept = _context.Departments.Find(student.DepartmentId);
                string deptCode = string.IsNullOrWhiteSpace(dept.Code) ? "00" : dept.Code;
                string prefix = $"26{deptCode}";

                var lastStudent = _context.Students
                    .Where(s => s.StudentNumber != null && s.StudentNumber.StartsWith(prefix))
                    .OrderByDescending(s => s.StudentNumber)
                    .FirstOrDefault();

                int seq = 1001;
                if (lastStudent != null && lastStudent.StudentNumber.Length >= prefix.Length + 4)
                {
                    if (int.TryParse(lastStudent.StudentNumber.Substring(prefix.Length), out int lastSeq))
                        seq = lastSeq + 1;
                }
                student.StudentNumber = $"{prefix}{seq}";

                _context.Students.Add(student);
                _context.SaveChanges();

                // Kullanıcı hesabı oluştur
                var studentRole = _context.Roles.FirstOrDefault(r => r.Name == "Student");
                if (studentRole != null)
                {
                    string username = StringHelper.ConvertToEnglishLowercase(student.FirstName + student.LastName);
                    int count = 1;
                    string baseUsername = username;
                    while (_context.Users.Any(u => u.Username == username))
                    {
                        username = baseUsername + count;
                        count++;
                    }

                    string firstInitial = student.FirstName.Substring(0, 1).ToLower();
                    string lastInitial = student.LastName.Substring(0, 1).ToLower();
                    string rawPassword = $"{firstInitial}{lastInitial}123";

                    _context.Users.Add(new User
                    {
                        Username = username,
                        PasswordHash = PasswordHelper.HashPassword(rawPassword),
                        Email = $"{username}@student.edu.tr",
                        RoleId = studentRole.Id,
                        StudentId = student.Id
                    });
                    _context.SaveChanges();
                }

                return ServiceResult.SuccessResult($"Öğrenci başarıyla eklendi. Numara: {student.StudentNumber}");
            }
            catch (Exception ex)
            {
                return ServiceResult.Failure(ErrorHelper.GetDbUpdateErrorMessage(ex));
            }
        }

        public ServiceResult UpdateStudent(Student student)
        {
            try
            {
                if (student == null)
                    return ServiceResult.Failure("Öğrenci nesnesi boş olamaz.");
                if (string.IsNullOrWhiteSpace(student.StudentNumber))
                    return ServiceResult.Failure("Öğrenci numarası zorunludur.");
                if (string.IsNullOrWhiteSpace(student.FirstName))
                    return ServiceResult.Failure("Ad alanı zorunludur.");
                if (string.IsNullOrWhiteSpace(student.LastName))
                    return ServiceResult.Failure("Soyad alanı zorunludur.");
                if (student.DepartmentId <= 0 || !_context.Departments.Any(d => d.Id == student.DepartmentId))
                    return ServiceResult.Failure("Geçerli bir bölüm seçilmelidir.");

                var existing = _context.Students.Find(student.Id);
                if (existing == null)
                    return ServiceResult.Failure("Öğrenci bulunamadı.");

                if (existing.StudentNumber != student.StudentNumber &&
                    _context.Students.Any(s => s.StudentNumber == student.StudentNumber))
                    return ServiceResult.Failure("Bu öğrenci numarasıyla kayıtlı başka bir öğrenci var.");

                existing.StudentNumber = student.StudentNumber;
                existing.FirstName = student.FirstName;
                existing.LastName = student.LastName;
                existing.DepartmentId = student.DepartmentId;

                _context.SaveChanges();
                return ServiceResult.SuccessResult("Öğrenci başarıyla güncellendi.");
            }
            catch (Exception ex)
            {
                return ServiceResult.Failure(ErrorHelper.GetDbUpdateErrorMessage(ex));
            }
        }

        public ServiceResult DeleteStudent(int id)
        {
            try
            {
                var student = _context.Students.Find(id);
                if (student == null)
                    return ServiceResult.Failure("Öğrenci bulunamadı.");

                _context.Students.Remove(student);
                _context.SaveChanges();
                return ServiceResult.SuccessResult("Öğrenci başarıyla silindi.");
            }
            catch (Exception ex)
            {
                return ServiceResult.Failure(ErrorHelper.GetDbUpdateErrorMessage(ex));
            }
        }

        public List<Student> SearchStudents(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm)) return GetAllStudents();
            searchTerm = searchTerm.ToLower();
            return _context.Students
                .Include(s => s.Department)
                .Include(s => s.User)
                .Where(s =>
                    (s.StudentNumber != null && s.StudentNumber.ToLower().Contains(searchTerm)) ||
                    s.FirstName.ToLower().Contains(searchTerm) ||
                    s.LastName.ToLower().Contains(searchTerm) ||
                    (s.Department != null && s.Department.Name.ToLower().Contains(searchTerm)))
                .ToList();
        }
    }
}
