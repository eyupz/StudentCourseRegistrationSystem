using StudentCourseRegistrationSystem.Models;
using System.Linq;

namespace StudentCourseRegistrationSystem.Data
{
    public static class DatabaseInitializer
    {
        public static void Initialize(AppDbContext context)
        {
            // Ensure the database is created
            context.Database.EnsureCreated();

            // Look for any roles.
            if (context.Roles.Any())
            {
                return;   // DB has been seeded
            }

            // 1. Seed Roles
            var roles = new Role[]
            {
                new Role { Name = "Admin" },
                new Role { Name = "Staff" },
                new Role { Name = "Instructor" },
                new Role { Name = "Student" }
            };
            context.Roles.AddRange(roles);
            context.SaveChanges();

            // 2. Seed Default Admin User
            var adminUser = new User
            {
                Username = "admin",
                PasswordHash = "admin123", // In a real app, hash this!
                Email = "admin@university.edu",
                RoleId = roles.Single(r => r.Name == "Admin").Id
            };
            context.Users.Add(adminUser);
            context.SaveChanges();

            // 3. Seed Sample Departments
            var departments = new Department[]
            {
                new Department { Name = "Computer Engineering", Code = "CENG" },
                new Department { Name = "Software Engineering", Code = "SENG" },
                new Department { Name = "Mathematics", Code = "MATH" }
            };
            context.Departments.AddRange(departments);
            context.SaveChanges();

            // 4. Seed Sample Semester
            var semester = new Semester { Name = "Fall 2026", IsActive = true };
            context.Semesters.Add(semester);
            context.SaveChanges();

            // 5. Seed Sample Instructor
            var instructor = new Instructor { FirstName = "Alan", LastName = "Turing" };
            context.Instructors.Add(instructor);
            context.SaveChanges();
            
            // Add Instructor User
            var instructorUser = new User
            {
                Username = "aturing",
                PasswordHash = "password123",
                Email = "aturing@university.edu",
                RoleId = roles.Single(r => r.Name == "Instructor").Id,
                InstructorId = instructor.Id
            };
            context.Users.Add(instructorUser);
            context.SaveChanges();

            // 6. Seed Sample Courses
            var courses = new Course[]
            {
                new Course 
                { 
                    CourseCode = "CENG101", 
                    Title = "Introduction to Programming", 
                    Credits = 4, 
                    Capacity = 50,
                    DepartmentId = departments.Single(d => d.Code == "CENG").Id,
                    InstructorId = instructor.Id
                },
                new Course 
                { 
                    CourseCode = "CENG201", 
                    Title = "Data Structures", 
                    Credits = 4, 
                    Capacity = 40,
                    DepartmentId = departments.Single(d => d.Code == "CENG").Id,
                    InstructorId = instructor.Id
                }
            };
            context.Courses.AddRange(courses);
            context.SaveChanges();

            // 7. Seed Sample Student
            var student = new Student
            {
                StudentNumber = "20260001",
                FirstName = "Grace",
                LastName = "Hopper",
                DepartmentId = departments.Single(d => d.Code == "CENG").Id
            };
            context.Students.Add(student);
            context.SaveChanges();
            
            // Add Student User
            var studentUser = new User
            {
                Username = "20260001",
                PasswordHash = "password123",
                Email = "ghopper@student.university.edu",
                RoleId = roles.Single(r => r.Name == "Student").Id,
                StudentId = student.Id
            };
            context.Users.Add(studentUser);
            context.SaveChanges();
        }
    }
}
