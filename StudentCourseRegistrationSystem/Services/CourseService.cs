using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using StudentCourseRegistrationSystem.Data;
using StudentCourseRegistrationSystem.Helpers;
using StudentCourseRegistrationSystem.Models;

namespace StudentCourseRegistrationSystem.Services
{
    public class CourseService
    {
        private readonly AppDbContext _context;

        public CourseService(AppDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public List<Course> GetAllCourses()
        {
            return _context.Courses
                .Include(c => c.Department)
                .Include(c => c.Instructor)
                .Include(c => c.Enrollments)
                .ToList();
        }

        public Course GetCourseById(int id)
        {
            return _context.Courses
                .Include(c => c.Department)
                .Include(c => c.Instructor)
                .Include(c => c.Enrollments)
                .SingleOrDefault(c => c.Id == id);
        }

        public Course GetCourseByCode(string courseCode)
        {
            if (string.IsNullOrWhiteSpace(courseCode)) return null;
            return _context.Courses
                .Include(c => c.Department)
                .Include(c => c.Instructor)
                .Include(c => c.Enrollments)
                .SingleOrDefault(c => c.CourseCode == courseCode);
        }

        public ServiceResult AddCourse(Course course)
        {
            try
            {
                if (course == null)
                    return ServiceResult.Failure("Ders nesnesi boş olamaz.");
                if (string.IsNullOrWhiteSpace(course.CourseCode))
                    return ServiceResult.Failure("Ders kodu zorunludur.");
                if (string.IsNullOrWhiteSpace(course.Title))
                    return ServiceResult.Failure("Ders adı zorunludur.");
                if (course.Capacity <= 0)
                    return ServiceResult.Failure("Kapasite sıfırdan büyük olmalıdır.");
                if (course.Credits <= 0)
                    return ServiceResult.Failure("Kredi sayısı sıfırdan büyük olmalıdır.");
                if (course.DepartmentId <= 0 || !_context.Departments.Any(d => d.Id == course.DepartmentId))
                    return ServiceResult.Failure("Geçerli bir bölüm seçilmelidir.");
                if (course.InstructorId.HasValue && course.InstructorId > 0 &&
                    !_context.Instructors.Any(i => i.Id == course.InstructorId))
                    return ServiceResult.Failure("Seçilen öğretmen bulunamadı.");
                if (_context.Courses.Any(c => c.CourseCode == course.CourseCode))
                    return ServiceResult.Failure($"'{course.CourseCode}' kodu zaten kullanılıyor.");

                _context.Courses.Add(course);
                _context.SaveChanges();
                return ServiceResult.SuccessResult("Ders başarıyla eklendi.");
            }
            catch (Exception ex)
            {
                return ServiceResult.Failure(ErrorHelper.GetDbUpdateErrorMessage(ex));
            }
        }

        public ServiceResult UpdateCourse(Course course)
        {
            try
            {
                if (course == null)
                    return ServiceResult.Failure("Ders nesnesi boş olamaz.");
                if (string.IsNullOrWhiteSpace(course.CourseCode))
                    return ServiceResult.Failure("Ders kodu zorunludur.");
                if (string.IsNullOrWhiteSpace(course.Title))
                    return ServiceResult.Failure("Ders adı zorunludur.");
                if (course.Capacity <= 0)
                    return ServiceResult.Failure("Kapasite sıfırdan büyük olmalıdır.");
                if (course.Credits <= 0)
                    return ServiceResult.Failure("Kredi sayısı sıfırdan büyük olmalıdır.");
                if (course.DepartmentId <= 0 || !_context.Departments.Any(d => d.Id == course.DepartmentId))
                    return ServiceResult.Failure("Geçerli bir bölüm seçilmelidir.");

                var existing = _context.Courses.Find(course.Id);
                if (existing == null)
                    return ServiceResult.Failure("Ders bulunamadı.");

                if (existing.CourseCode != course.CourseCode &&
                    _context.Courses.Any(c => c.CourseCode == course.CourseCode))
                    return ServiceResult.Failure($"'{course.CourseCode}' kodu zaten kullanılıyor.");

                existing.CourseCode = course.CourseCode;
                existing.Title = course.Title;
                existing.Credits = course.Credits;
                existing.Capacity = course.Capacity;
                existing.DepartmentId = course.DepartmentId;
                existing.InstructorId = (course.InstructorId.HasValue && course.InstructorId > 0)
                    ? course.InstructorId : null;

                _context.SaveChanges();
                return ServiceResult.SuccessResult("Ders başarıyla güncellendi.");
            }
            catch (Exception ex)
            {
                return ServiceResult.Failure(ErrorHelper.GetDbUpdateErrorMessage(ex));
            }
        }

        public ServiceResult DeleteCourse(int id)
        {
            try
            {
                var course = _context.Courses.Find(id);
                if (course == null)
                    return ServiceResult.Failure("Ders bulunamadı.");
                if (_context.Enrollments.Any(e => e.CourseId == id))
                    return ServiceResult.Failure("Bu derse kayıtlı öğrenciler olduğu için silinemez.");

                _context.Courses.Remove(course);
                _context.SaveChanges();
                return ServiceResult.SuccessResult("Ders başarıyla silindi.");
            }
            catch (Exception ex)
            {
                return ServiceResult.Failure(ErrorHelper.GetDbUpdateErrorMessage(ex));
            }
        }

        public bool HasAvailableCapacity(int courseId, int semesterId)
        {
            var course = _context.Courses.Find(courseId);
            if (course == null) return false;
            var count = _context.Enrollments.Count(e => e.CourseId == courseId && e.SemesterId == semesterId);
            return count < course.Capacity;
        }

        public int GetAvailableSpots(int courseId, int semesterId)
        {
            var course = _context.Courses.Find(courseId);
            if (course == null) return 0;
            var count = _context.Enrollments.Count(e => e.CourseId == courseId && e.SemesterId == semesterId);
            return Math.Max(0, course.Capacity - count);
        }

        public List<Course> SearchCourses(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm)) return GetAllCourses();
            searchTerm = searchTerm.ToLower();
            return _context.Courses
                .Include(c => c.Department)
                .Include(c => c.Instructor)
                .Include(c => c.Enrollments)
                .Where(c =>
                    c.CourseCode.ToLower().Contains(searchTerm) ||
                    c.Title.ToLower().Contains(searchTerm) ||
                    (c.Department != null && c.Department.Name.ToLower().Contains(searchTerm)))
                .ToList();
        }
    }
}
