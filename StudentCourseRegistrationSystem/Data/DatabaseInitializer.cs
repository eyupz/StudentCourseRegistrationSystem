using StudentCourseRegistrationSystem.Models;
using System.Linq;

namespace StudentCourseRegistrationSystem.Data
{
    public static class DatabaseInitializer
    {
        public static void Initialize(AppDbContext context)
        {
            // Add seed data only if database is empty
            if (context.Roles.Any() || context.Users.Any())
            {
                return;
            }

            // Add roles
            var adminRole = new Role { Name = "Admin" };
            var staffRole = new Role { Name = "Staff" };
            var instructorRole = new Role { Name = "Instructor" };
            var studentRole = new Role { Name = "Student" };
            
            context.Roles.AddRange(adminRole, staffRole, instructorRole, studentRole);
            context.SaveChanges();

            // Add default admin user
            var adminUser = new User
            {
                Username = "admin",
                PasswordHash = "admin123",
                Email = "admin@school.local",
                RoleId = adminRole.Id
                // IsActive: true is omitted because the User model does not contain an IsActive property.
                // Omitting it ensures the code compiles with your existing models.
            };
            context.Users.Add(adminUser);
            context.SaveChanges();

            // Add sample departments
            var csDepartment = new Department { Name = "Computer Science", Code = "CS" };
            var mathDepartment = new Department { Name = "Mathematics", Code = "MATH" };
            context.Departments.AddRange(csDepartment, mathDepartment);
            context.SaveChanges();

            // Add sample instructor
            var sampleInstructor = new Instructor { FirstName = "John", LastName = "Doe" };
            context.Instructors.Add(sampleInstructor);
            context.SaveChanges();

            var instructorUser = new User
            {
                Username = "jdoe",
                PasswordHash = "password123",
                Email = "jdoe@school.local",
                RoleId = instructorRole.Id,
                InstructorId = sampleInstructor.Id
            };
            context.Users.Add(instructorUser);
            context.SaveChanges();

            // Add sample student
            var sampleStudent = new Student
            {
                StudentNumber = "S12345",
                FirstName = "Jane",
                LastName = "Smith",
                DepartmentId = csDepartment.Id
            };
            context.Students.Add(sampleStudent);
            context.SaveChanges();

            var studentUser = new User
            {
                Username = "jsmith",
                PasswordHash = "password123",
                Email = "jsmith@school.local",
                RoleId = studentRole.Id,
                StudentId = sampleStudent.Id
            };
            context.Users.Add(studentUser);
            context.SaveChanges();

            // Add sample semester
            var sampleSemester = new Semester { Name = "Fall 2026", IsActive = true };
            context.Semesters.Add(sampleSemester);
            context.SaveChanges();

            // Add sample courses
            var course1 = new Course
            {
                CourseCode = "CS101",
                Title = "Introduction to Programming",
                Credits = 3,
                Capacity = 30,
                DepartmentId = csDepartment.Id,
                InstructorId = sampleInstructor.Id
            };
            var course2 = new Course
            {
                CourseCode = "MATH101",
                Title = "Calculus I",
                Credits = 4,
                Capacity = 40,
                DepartmentId = mathDepartment.Id,
                InstructorId = sampleInstructor.Id
            };
            context.Courses.AddRange(course1, course2);
            context.SaveChanges();

            // Add sample enrollment
            var sampleEnrollment = new Enrollment
            {
                StudentId = sampleStudent.Id,
                CourseId = course1.Id,
                SemesterId = sampleSemester.Id
            };
            context.Enrollments.Add(sampleEnrollment);
            context.SaveChanges();
        }
    }
}
