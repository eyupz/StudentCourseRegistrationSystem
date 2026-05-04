using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using StudentCourseRegistrationSystem.Data;
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

        public void AddCourse(Course course)
        {
            if (course == null) throw new ArgumentNullException(nameof(course));
            if (string.IsNullOrWhiteSpace(course.CourseCode)) throw new ArgumentException("Course code is required.");
            if (string.IsNullOrWhiteSpace(course.Title)) throw new ArgumentException("Course title is required.");
            if (course.Capacity <= 0) throw new ArgumentException("Capacity must be greater than zero.");
            if (course.Credits <= 0) throw new ArgumentException("Credits must be greater than zero.");

            if (_context.Courses.Any(c => c.CourseCode == course.CourseCode))
                throw new InvalidOperationException("A course with this code already exists.");

            _context.Courses.Add(course);
            _context.SaveChanges();
        }

        public void UpdateCourse(Course course)
        {
            if (course == null) throw new ArgumentNullException(nameof(course));
            
            var existingCourse = _context.Courses.Find(course.Id);
            if (existingCourse == null) throw new InvalidOperationException("Course not found.");

            existingCourse.Title = course.Title;
            existingCourse.Credits = course.Credits;
            existingCourse.Capacity = course.Capacity;
            existingCourse.Schedule = course.Schedule;
            existingCourse.PrerequisiteCourseId = course.PrerequisiteCourseId;
            existingCourse.DepartmentId = course.DepartmentId;
            existingCourse.InstructorId = course.InstructorId;

            _context.SaveChanges();
        }

        public void DeleteCourse(int id)
        {
            var course = _context.Courses.Find(id);
            if (course == null) throw new InvalidOperationException("Course not found.");

            if (_context.Enrollments.Any(e => e.CourseId == id))
                throw new InvalidOperationException("Cannot delete course because there are students enrolled in it.");

            _context.Courses.Remove(course);
            _context.SaveChanges();
        }

        public bool HasAvailableCapacity(int courseId, int semesterId)
        {
            var course = _context.Courses.Find(courseId);
            if (course == null) throw new InvalidOperationException("Course not found.");

            var currentEnrollmentCount = _context.Enrollments
                .Count(e => e.CourseId == courseId && e.SemesterId == semesterId);

            return currentEnrollmentCount < course.Capacity;
        }
        
        public int GetAvailableSpots(int courseId, int semesterId)
        {
            var course = _context.Courses.Find(courseId);
            if (course == null) throw new InvalidOperationException("Course not found.");

            var currentEnrollmentCount = _context.Enrollments
                .Count(e => e.CourseId == courseId && e.SemesterId == semesterId);

            return Math.Max(0, course.Capacity - currentEnrollmentCount);
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
                    c.Department.Name.ToLower().Contains(searchTerm))
                .ToList();
        }
    }
}
