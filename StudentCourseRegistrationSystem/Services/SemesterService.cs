using System;
using System.Collections.Generic;
using System.Linq;
using StudentCourseRegistrationSystem.Data;
using StudentCourseRegistrationSystem.Models;

namespace StudentCourseRegistrationSystem.Services
{
    public class SemesterService
    {
        private readonly AppDbContext _context;

        public SemesterService(AppDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public List<Semester> GetAllSemesters()
        {
            return _context.Semesters.ToList();
        }
    }
}
