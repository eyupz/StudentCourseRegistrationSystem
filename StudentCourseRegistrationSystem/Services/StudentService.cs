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

        // Türkiye üniversiteleri standart harf notu → katsayı tablosu
        private static readonly Dictionary<string, decimal> GradePoints = new()
        {
            { "AA", 4.0m }, { "BA", 3.5m }, { "BB", 3.0m },
            { "CB", 2.5m }, { "CC", 2.0m }, { "DC", 1.5m },
            { "DD", 1.0m }, { "FD", 0.5m }, { "FF", 0.0m }
        };

        public StudentService(AppDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public List<Student> GetAllStudents() =>
            _context.Students.Include(s => s.Department).Include(s => s.User).ToList();

        public Student GetStudentById(int id) =>
            _context.Students.Include(s => s.Department).Include(s => s.User)
                .SingleOrDefault(s => s.Id == id);

        public Student GetStudentByUserId(int userId)
        {
            // FK User.StudentId tarafında — bu yüzden User üzerinden gidiyoruz
            var user = _context.Users
                .Include(u => u.Student)
                    .ThenInclude(s => s.Department)
                .Include(u => u.Student)
                    .ThenInclude(s => s.Enrollments)
                        .ThenInclude(e => e.Course)
                            .ThenInclude(c => c.Instructor)
                .FirstOrDefault(u => u.Id == userId);

            return user?.Student;
        }

        public Student GetStudentByNumber(string studentNumber) =>
            string.IsNullOrWhiteSpace(studentNumber) ? null :
            _context.Students.Include(s => s.Department).Include(s => s.User)
                .SingleOrDefault(s => s.StudentNumber == studentNumber);

        public void AddStudent(Student student)
        {
            if (student == null) throw new ArgumentNullException(nameof(student));
            if (string.IsNullOrWhiteSpace(student.StudentNumber)) throw new ArgumentException("Öğrenci numarası zorunludur.");
            if (string.IsNullOrWhiteSpace(student.FirstName)) throw new ArgumentException("Ad zorunludur.");
            if (string.IsNullOrWhiteSpace(student.LastName)) throw new ArgumentException("Soyad zorunludur.");

            if (_context.Students.Any(s => s.StudentNumber == student.StudentNumber))
                throw new InvalidOperationException("Bu öğrenci numarası zaten kayıtlı.");

            _context.Students.Add(student);
            _context.SaveChanges();
            Logger.Info($"Öğrenci eklendi: {student.StudentNumber} – {student.FirstName} {student.LastName}");
        }

        public void UpdateStudent(Student student)
        {
            if (student == null) throw new ArgumentNullException(nameof(student));
            var existing = _context.Students.Find(student.Id)
                ?? throw new InvalidOperationException("Öğrenci bulunamadı.");

            existing.FirstName = student.FirstName;
            existing.LastName = student.LastName;
            existing.StudentNumber = student.StudentNumber;
            existing.DepartmentId = student.DepartmentId;
            _context.SaveChanges();
            Logger.Info($"Öğrenci güncellendi: ID={student.Id}");
        }

        public void DeleteStudent(int id)
        {
            var student = _context.Students.Find(id)
                ?? throw new InvalidOperationException("Öğrenci bulunamadı.");
            _context.Students.Remove(student);
            _context.SaveChanges();
            Logger.Info($"Öğrenci silindi: ID={id}");
        }

        /// <summary>
        /// Ağırlıklı Not Ortalaması hesaplar ve DB'ye kaydeder.
        /// GPA = Σ(kredi × not katsayısı) / Σ(kredi)
        /// </summary>
        public decimal CalculateAndSaveGPA(int studentId)
        {
            var completedEnrollments = _context.Enrollments
                .Include(e => e.Course)
                .Where(e => e.StudentId == studentId &&
                            e.Status == EnrollmentStatus.Completed &&
                            !string.IsNullOrEmpty(e.Grade))
                .ToList();

            if (!completedEnrollments.Any()) return 0m;

            decimal totalPoints = 0;
            int totalCredits = 0;

            foreach (var enrollment in completedEnrollments)
            {
                if (GradePoints.TryGetValue(enrollment.Grade, out decimal point))
                {
                    int credits = enrollment.Course?.Credits ?? 0;
                    totalPoints += point * credits;
                    totalCredits += credits;
                }
            }

            decimal gpa = totalCredits > 0 ? Math.Round(totalPoints / totalCredits, 2) : 0;

            var student = _context.Students.Find(studentId);
            if (student != null)
            {
                student.GPA = gpa;
                _context.SaveChanges();
            }

            Logger.Info($"GPA hesaplandı – Öğrenci:{studentId} → GPA:{gpa}");
            return gpa;
        }

        public List<Student> SearchStudents(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm)) return GetAllStudents();
            searchTerm = searchTerm.ToLower();
            return _context.Students
                .Include(s => s.Department).Include(s => s.User)
                .Where(s =>
                    s.StudentNumber.ToLower().Contains(searchTerm) ||
                    s.FirstName.ToLower().Contains(searchTerm) ||
                    s.LastName.ToLower().Contains(searchTerm) ||
                    s.Department.Name.ToLower().Contains(searchTerm))
                .ToList();
        }
    }
}
