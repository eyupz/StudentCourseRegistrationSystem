using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using StudentCourseRegistrationSystem.Data;
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

        public void AddStudent(Student student)
        {
            if (student == null) throw new ArgumentNullException(nameof(student));
            if (string.IsNullOrWhiteSpace(student.StudentNumber)) throw new ArgumentException("Student number is required.");
            if (string.IsNullOrWhiteSpace(student.FirstName)) throw new ArgumentException("First name is required.");
            if (string.IsNullOrWhiteSpace(student.LastName)) throw new ArgumentException("Last name is required.");
            
            if (_context.Students.Any(s => s.StudentNumber == student.StudentNumber))
                throw new InvalidOperationException("A student with this student number already exists.");

            _context.Students.Add(student);
            _context.SaveChanges();
        }

        public void UpdateStudent(Student student)
        {
            if (student == null) throw new ArgumentNullException(nameof(student));
            
            var existingStudent = _context.Students.Find(student.Id);
            if (existingStudent == null) throw new InvalidOperationException("Student not found.");

            existingStudent.FirstName = student.FirstName;
            existingStudent.LastName = student.LastName;
            existingStudent.DepartmentId = student.DepartmentId;
            // StudentNumber typically shouldn't be changed, but if needed, we'd verify uniqueness here

            _context.Students.Update(existingStudent);
            _context.SaveChanges();
        }

        public void DeleteStudent(int id)
        {
            var student = _context.Students.Find(id);
            if (student == null) throw new InvalidOperationException("Student not found.");

            _context.Students.Remove(student);
            _context.SaveChanges();
        }

        public List<Student> SearchStudents(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm)) return GetAllStudents();

            searchTerm = searchTerm.ToLower();

            return _context.Students
                .Include(s => s.Department)
                .Include(s => s.User)
                .Where(s => 
                    s.StudentNumber.ToLower().Contains(searchTerm) ||
                    s.FirstName.ToLower().Contains(searchTerm) ||
                    s.LastName.ToLower().Contains(searchTerm) ||
                    s.Department.Name.ToLower().Contains(searchTerm))
                .ToList();
        }
    }
}
