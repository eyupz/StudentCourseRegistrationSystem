using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using StudentCourseRegistrationSystem.Data;
using StudentCourseRegistrationSystem.Models;

namespace StudentCourseRegistrationSystem.Services
{
    public class EnrollmentService
    {
        private readonly AppDbContext _context;
        private readonly CourseService _courseService;

        public EnrollmentService(AppDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _courseService = new CourseService(context);
        }

        public void Enroll(int studentId, int courseId, int semesterId)
        {
            if (IsAlreadyEnrolled(studentId, courseId, semesterId))
            {
                throw new InvalidOperationException("Student is already enrolled in this course for the selected semester.");
            }

            if (!_courseService.HasAvailableCapacity(courseId, semesterId))
            {
                throw new InvalidOperationException("The course has reached its maximum capacity for this semester.");
            }

            var enrollment = new Enrollment
            {
                StudentId = studentId,
                CourseId = courseId,
                SemesterId = semesterId
            };

            _context.Enrollments.Add(enrollment);
            _context.SaveChanges();
        }

        public void Drop(int enrollmentId)
        {
            var enrollment = _context.Enrollments.Find(enrollmentId);
            if (enrollment == null)
            {
                throw new InvalidOperationException("Enrollment record not found.");
            }

            _context.Enrollments.Remove(enrollment);
            _context.SaveChanges();
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
    }
}
