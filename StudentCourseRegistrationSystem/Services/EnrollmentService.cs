using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using StudentCourseRegistrationSystem.Data;
using StudentCourseRegistrationSystem.Helpers;
using StudentCourseRegistrationSystem.Models;

namespace StudentCourseRegistrationSystem.Services
{
    public class EnrollmentService
    {
        private readonly AppDbContext _context;
        private readonly CourseService _courseService;

        public EnrollmentService(AppDbContext context, CourseService courseService)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _courseService = courseService ?? throw new ArgumentNullException(nameof(courseService));
        }

        public ServiceResult Enroll(int studentId, int courseId, int semesterId)
        {
            try
            {
                if (!_context.Students.Any(s => s.Id == studentId))
                    return ServiceResult.Failure("Öğrenci bulunamadı.");
                if (!_context.Courses.Any(c => c.Id == courseId))
                    return ServiceResult.Failure("Ders bulunamadı.");
                if (!_context.Semesters.Any(s => s.Id == semesterId))
                    return ServiceResult.Failure("Dönem bulunamadı.");
                if (IsAlreadyEnrolled(studentId, courseId, semesterId))
                    return ServiceResult.Failure("Öğrenci bu derse seçili dönem için zaten kayıtlı.");
                if (!_courseService.HasAvailableCapacity(courseId, semesterId))
                    return ServiceResult.Failure("Ders bu dönem için maksimum kapasiteye ulaştı.");

                _context.Enrollments.Add(new Enrollment
                {
                    StudentId = studentId,
                    CourseId = courseId,
                    SemesterId = semesterId
                });
                _context.SaveChanges();
                return ServiceResult.SuccessResult("Kayıt başarıyla tamamlandı.");
            }
            catch (Exception ex)
            {
                return ServiceResult.Failure(ErrorHelper.GetDbUpdateErrorMessage(ex));
            }
        }

        public ServiceResult Drop(int enrollmentId)
        {
            try
            {
                var enrollment = _context.Enrollments.Find(enrollmentId);
                if (enrollment == null)
                    return ServiceResult.Failure("Kayıt bulunamadı.");

                _context.Enrollments.Remove(enrollment);
                _context.SaveChanges();
                return ServiceResult.SuccessResult("Kayıt başarıyla silindi.");
            }
            catch (Exception ex)
            {
                return ServiceResult.Failure(ErrorHelper.GetDbUpdateErrorMessage(ex));
            }
        }

        /// <summary>
        /// Öğretmen panelinden not kaydetme işlemi.
        /// Kendi AppDbContext'ini açar, değişiklik izleme çakışmalarını önler.
        /// </summary>
        public ServiceResult SaveGrade(int enrollmentId, double score)
        {
            if (score < 0 || score > 100)
                return ServiceResult.Failure("Not 0 ile 100 arasında olmalıdır.");

            string letterGrade = CalculateLetterGrade(score);

            try
            {
                using (var ctx = new AppDbContext())
                {
                    var enr = ctx.Enrollments
                        .Include(e => e.Grade)
                        .FirstOrDefault(e => e.Id == enrollmentId);

                    if (enr == null)
                        return ServiceResult.Failure("Kayıt bulunamadı. (ID: " + enrollmentId + ")");

                    if (enr.Grade == null)
                    {
                        ctx.Grades.Add(new Grade
                        {
                            EnrollmentId = enr.Id,
                            Score = score,
                            LetterGrade = letterGrade
                        });
                    }
                    else
                    {
                        enr.Grade.Score = score;
                        enr.Grade.LetterGrade = letterGrade;
                    }

                    ctx.SaveChanges();
                }

                return ServiceResult.SuccessResult($"Not kaydedildi: {score:F1} → {letterGrade}");
            }
            catch (Exception ex)
            {
                return ServiceResult.Failure(ErrorHelper.GetDbUpdateErrorMessage(ex));
            }
        }

        public bool IsAlreadyEnrolled(int studentId, int courseId, int semesterId)
        {
            return _context.Enrollments.Any(e =>
                e.StudentId == studentId &&
                e.CourseId == courseId &&
                e.SemesterId == semesterId);
        }

        public List<Enrollment> GetStudentEnrollments(int studentId)
        {
            return _context.Enrollments
                .Include(e => e.Course)
                    .ThenInclude(c => c.Department)
                .Include(e => e.Semester)
                .Include(e => e.Grade)
                .Where(e => e.StudentId == studentId)
                .ToList();
        }

        public List<Enrollment> GetCourseEnrollments(int courseId, int semesterId)
        {
            return _context.Enrollments
                .Include(e => e.Student)
                    .ThenInclude(s => s.Department)
                .Include(e => e.Semester)
                .Include(e => e.Grade)
                .Where(e => e.CourseId == courseId && e.SemesterId == semesterId)
                .ToList();
        }

        public List<Enrollment> GetAllEnrollments()
        {
            return _context.Enrollments
                .Include(e => e.Student)
                .Include(e => e.Course)
                .Include(e => e.Semester)
                .Include(e => e.Grade)
                .ToList();
        }

        public static string CalculateLetterGrade(double score)
        {
            if (score >= 90) return "AA";
            if (score >= 85) return "BA";
            if (score >= 80) return "BB";
            if (score >= 75) return "CB";
            if (score >= 70) return "CC";
            if (score >= 65) return "DC";
            if (score >= 60) return "DD";
            if (score >= 50) return "FD";
            return "FF";
        }

        public static double GetGradeWeight(string letterGrade)
        {
            return letterGrade switch
            {
                "AA" => 4.0,
                "BA" => 3.5,
                "BB" => 3.0,
                "CB" => 2.5,
                "CC" => 2.0,
                "DC" => 1.5,
                "DD" => 1.0,
                "FD" => 0.5,
                _ => 0.0
            };
        }

        public double CalculateGNO(int studentId, int semesterId)
        {
            var enrollments = _context.Enrollments
                .Include(e => e.Course)
                .Include(e => e.Grade)
                .Where(e => e.StudentId == studentId && e.SemesterId == semesterId && e.Grade != null)
                .ToList();

            double totalPoints = 0;
            int totalCredits = 0;
            foreach (var en in enrollments)
            {
                if (en.Grade != null && !string.IsNullOrEmpty(en.Grade.LetterGrade))
                {
                    totalPoints += GetGradeWeight(en.Grade.LetterGrade) * en.Course.Credits;
                    totalCredits += en.Course.Credits;
                }
            }
            return totalCredits == 0 ? 0 : Math.Round(totalPoints / totalCredits, 2);
        }

        public double CalculateAGNO(int studentId)
        {
            var enrollments = _context.Enrollments
                .Include(e => e.Course)
                .Include(e => e.Grade)
                .Where(e => e.StudentId == studentId && e.Grade != null)
                .ToList();

            double totalPoints = 0;
            int totalCredits = 0;
            foreach (var en in enrollments)
            {
                if (en.Grade != null && !string.IsNullOrEmpty(en.Grade.LetterGrade))
                {
                    totalPoints += GetGradeWeight(en.Grade.LetterGrade) * en.Course.Credits;
                    totalCredits += en.Course.Credits;
                }
            }
            return totalCredits == 0 ? 0 : Math.Round(totalPoints / totalCredits, 2);
        }
    }
}
