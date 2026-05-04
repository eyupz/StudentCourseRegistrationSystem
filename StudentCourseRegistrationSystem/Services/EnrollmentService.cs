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
        private const int MaxCreditPerSemester = 30;

        private readonly AppDbContext _context;

        public EnrollmentService(AppDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        // ─── Ana Kayıt Metodu ────────────────────────────────────────────────────

        public void Enroll(int studentId, int courseId, int semesterId)
        {
            var course = _context.Courses
                .Include(c => c.Enrollments)
                .FirstOrDefault(c => c.Id == courseId)
                ?? throw new InvalidOperationException("Ders bulunamadı.");

            if (IsAlreadyEnrolled(studentId, courseId, semesterId))
                throw new InvalidOperationException("Bu derse zaten kayıtlısınız.");

            CheckCapacity(course, semesterId);
            CheckPrerequisite(studentId, course);
            CheckScheduleConflict(studentId, semesterId, course);
            CheckCreditLimit(studentId, semesterId, course.Credits);

            var enrollment = new Enrollment
            {
                StudentId = studentId,
                CourseId = courseId,
                SemesterId = semesterId,
                Status = EnrollmentStatus.Enrolled
            };

            _context.Enrollments.Add(enrollment);
            _context.SaveChanges();

            Logger.Info($"Kayıt yapıldı – Öğrenci:{studentId}, Ders:{course.CourseCode}, Dönem:{semesterId}");
        }

        public void Drop(int enrollmentId)
        {
            var enrollment = _context.Enrollments.Find(enrollmentId)
                ?? throw new InvalidOperationException("Kayıt bulunamadı.");

            enrollment.Status = EnrollmentStatus.Dropped;
            _context.SaveChanges();
            Logger.Info($"Ders bırakıldı – Kayıt ID:{enrollmentId}");
        }

        /// <summary>
        /// Üç bileşenli sayısal not girişi: Final %50, Vize %35, Ödev %15
        /// Ağırlıklı ortalama → harf notuna dönüştürür → GPA günceller.
        /// </summary>
        public void SetNumericGrades(int enrollmentId, decimal vize, decimal final, decimal odev)
        {
            if (vize  < 0 || vize  > 100) throw new InvalidOperationException("Vize notu 0-100 arasında olmalıdır.");
            if (final < 0 || final > 100) throw new InvalidOperationException("Final notu 0-100 arasında olmalıdır.");
            if (odev  < 0 || odev  > 100) throw new InvalidOperationException("Ödev notu 0-100 arasında olmalıdır.");

            var enrollment = _context.Enrollments.Find(enrollmentId)
                ?? throw new InvalidOperationException("Kayıt bulunamadı.");

            decimal agirlikli = final * 0.50m + vize * 0.35m + odev * 0.15m;
            string harf = HarfNotu(agirlikli);

            enrollment.Vize  = vize;
            enrollment.Final = final;
            enrollment.Odev  = odev;
            enrollment.Grade  = harf;
            enrollment.Status = (harf == "FF" || harf == "FD")
                ? EnrollmentStatus.Dropped
                : EnrollmentStatus.Completed;

            _context.SaveChanges();

            new StudentService(_context).CalculateAndSaveGPA(enrollment.StudentId);
            Logger.Info($"Not girildi – Kayıt:{enrollmentId} | Vize:{vize} Final:{final} Ödev:{odev} → Ağırlıklı:{agirlikli:F1} → {harf}");
        }

        /// <summary>Ağırlıklı ortalamayı harf notuna çevirir.</summary>
        public static string HarfNotu(decimal puan) => puan switch
        {
            >= 90 => "AA",
            >= 80 => "BA",
            >= 70 => "BB",
            >= 60 => "CB",
            >= 55 => "CC",
            >= 50 => "DC",
            >= 45 => "DD",
            >= 30 => "FD",
            _     => "FF"
        };

        /// <summary>Eski uyumluluk — doğrudan harf notu kayıt (admin override).</summary>
        public void SetGrade(int enrollmentId, string grade)
        {
            var enrollment = _context.Enrollments.Find(enrollmentId)
                ?? throw new InvalidOperationException("Kayıt bulunamadı.");
            enrollment.Grade  = grade;
            enrollment.Status = (grade == "FF" || grade == "FD") ? EnrollmentStatus.Dropped : EnrollmentStatus.Completed;
            _context.SaveChanges();
            new StudentService(_context).CalculateAndSaveGPA(enrollment.StudentId);
            Logger.Info($"Not girildi – Kayıt:{enrollmentId}, Not:{grade}");
        }

        // ─── İş Kuralları ────────────────────────────────────────────────────────

        /// <summary>Dersin kapasitesi dolmuş mu?</summary>
        public void CheckCapacity(Course course, int semesterId)
        {
            int enrolled = _context.Enrollments.Count(e =>
                e.CourseId == course.Id &&
                e.SemesterId == semesterId &&
                e.Status == EnrollmentStatus.Enrolled);

            if (enrolled >= course.Capacity)
                throw new InvalidOperationException($"'{course.Title}' dersinin kontenjanı dolmuştur. (Kapasite: {course.Capacity})");
        }

        /// <summary>Öğrenci ön koşul dersi geçmiş mi?</summary>
        public void CheckPrerequisite(int studentId, Course course)
        {
            if (course.PrerequisiteCourseId == null) return;

            bool passed = _context.Enrollments.Any(e =>
                e.StudentId == studentId &&
                e.CourseId == course.PrerequisiteCourseId &&
                e.Status == EnrollmentStatus.Completed &&
                e.Grade != "FF" && e.Grade != "FD" && !string.IsNullOrEmpty(e.Grade));

            if (!passed)
            {
                var prereq = _context.Courses.Find(course.PrerequisiteCourseId);
                throw new InvalidOperationException(
                    $"Ön koşul dersi tamamlanmadı: '{prereq?.Title ?? "Bilinmiyor"}'. Bu dersi başarıyla geçmelisiniz.");
            }
        }

        /// <summary>Aynı dönemde program (Schedule) çakışması var mı?</summary>
        public void CheckScheduleConflict(int studentId, int semesterId, Course newCourse)
        {
            if (string.IsNullOrWhiteSpace(newCourse.Schedule)) return;

            var currentSchedules = _context.Enrollments
                .Include(e => e.Course)
                .Where(e => e.StudentId == studentId && e.SemesterId == semesterId && e.Status == EnrollmentStatus.Enrolled)
                .Select(e => e.Course.Schedule)
                .ToList();

            foreach (var schedule in currentSchedules)
            {
                if (!string.IsNullOrWhiteSpace(schedule) && schedule == newCourse.Schedule)
                    throw new InvalidOperationException(
                        $"Program çakışması! '{newCourse.Schedule}' saatinde zaten başka bir dersiniz var.");
            }
        }

        /// <summary>Dönemlik maksimum kredi limitini aşıyor mu?</summary>
        public void CheckCreditLimit(int studentId, int semesterId, int newCourseCredits)
        {
            int currentCredits = _context.Enrollments
                .Include(e => e.Course)
                .Where(e => e.StudentId == studentId && e.SemesterId == semesterId && e.Status == EnrollmentStatus.Enrolled)
                .Sum(e => (int?)e.Course.Credits) ?? 0;

            if (currentCredits + newCourseCredits > MaxCreditPerSemester)
                throw new InvalidOperationException(
                    $"Maksimum kredi limitini ({MaxCreditPerSemester} AKTS) aşıyorsunuz. Mevcut: {currentCredits}, Eklenecek: {newCourseCredits}");
        }

        // ─── Sorgular ─────────────────────────────────────────────────────────

        public bool IsAlreadyEnrolled(int studentId, int courseId, int semesterId) =>
            _context.Enrollments.Any(e =>
                e.StudentId == studentId &&
                e.CourseId == courseId &&
                e.SemesterId == semesterId &&
                e.Status == EnrollmentStatus.Enrolled);

        public List<Enrollment> GetStudentEnrollments(int studentId) =>
            _context.Enrollments
                .Include(e => e.Course).ThenInclude(c => c.Department)
                .Include(e => e.Course).ThenInclude(c => c.Instructor)
                .Include(e => e.Semester)
                .Where(e => e.StudentId == studentId)
                .OrderByDescending(e => e.SemesterId)
                .ToList();

        public List<Enrollment> GetCourseEnrollments(int courseId, int semesterId) =>
            _context.Enrollments
                .Include(e => e.Student).ThenInclude(s => s.Department)
                .Include(e => e.Semester)
                .Where(e => e.CourseId == courseId && e.SemesterId == semesterId)
                .ToList();

        public List<Enrollment> GetAllEnrollments() =>
            _context.Enrollments
                .Include(e => e.Student)
                .Include(e => e.Course)
                .Include(e => e.Semester)
                .ToList();
    }
}
