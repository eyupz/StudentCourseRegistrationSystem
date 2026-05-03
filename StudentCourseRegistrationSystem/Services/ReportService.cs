using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using StudentCourseRegistrationSystem.Data;
using StudentCourseRegistrationSystem.Models;

namespace StudentCourseRegistrationSystem.Services
{
    public class StudentReportDto
    {
        public string StudentNumber { get; set; }
        public string FullName { get; set; }
        public string DepartmentName { get; set; }
        public int TotalEnrolledCourses { get; set; }
    }

    public class CourseReportDto
    {
        public string CourseCode { get; set; }
        public string Title { get; set; }
        public string InstructorName { get; set; }
        public int Capacity { get; set; }
        public int CurrentEnrollment { get; set; }
        public int AvailableSpots { get; set; }
    }

    public class EnrollmentReportDto
    {
        public string SemesterName { get; set; }
        public string CourseCode { get; set; }
        public string CourseTitle { get; set; }
        public string StudentNumber { get; set; }
        public string StudentName { get; set; }
    }

    public class ReportService
    {
        private readonly AppDbContext _context;

        public ReportService(AppDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public List<StudentReportDto> GetStudentReport()
        {
            return _context.Students
                .Include(s => s.Department)
                .Include(s => s.Enrollments)
                .Select(s => new StudentReportDto
                {
                    StudentNumber = s.StudentNumber,
                    FullName = s.FirstName + " " + s.LastName,
                    DepartmentName = s.Department != null ? s.Department.Name : "N/A",
                    TotalEnrolledCourses = s.Enrollments.Count
                })
                .ToList();
        }

        public List<CourseReportDto> GetCourseReport(int? semesterId = null)
        {
            var courses = _context.Courses
                .Include(c => c.Instructor)
                .Include(c => c.Enrollments)
                .ToList();

            var report = new List<CourseReportDto>();

            foreach (var course in courses)
            {
                int enrollmentCount = semesterId.HasValue 
                    ? course.Enrollments.Count(e => e.SemesterId == semesterId.Value)
                    : course.Enrollments.Count;

                report.Add(new CourseReportDto
                {
                    CourseCode = course.CourseCode,
                    Title = course.Title,
                    InstructorName = course.Instructor != null ? course.Instructor.FirstName + " " + course.Instructor.LastName : "N/A",
                    Capacity = course.Capacity,
                    CurrentEnrollment = enrollmentCount,
                    AvailableSpots = Math.Max(0, course.Capacity - enrollmentCount)
                });
            }

            return report;
        }

        public List<EnrollmentReportDto> GetEnrollmentReport(int? semesterId = null)
        {
            var query = _context.Enrollments
                .Include(e => e.Student)
                .Include(e => e.Course)
                .Include(e => e.Semester)
                .AsQueryable();

            if (semesterId.HasValue)
            {
                query = query.Where(e => e.SemesterId == semesterId.Value);
            }

            return query.Select(e => new EnrollmentReportDto
            {
                SemesterName = e.Semester != null ? e.Semester.Name : "N/A",
                CourseCode = e.Course != null ? e.Course.CourseCode : "N/A",
                CourseTitle = e.Course != null ? e.Course.Title : "N/A",
                StudentNumber = e.Student != null ? e.Student.StudentNumber : "N/A",
                StudentName = e.Student != null ? e.Student.FirstName + " " + e.Student.LastName : "N/A"
            }).ToList();
        }
    }
}
