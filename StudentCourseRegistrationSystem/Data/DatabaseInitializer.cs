using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using StudentCourseRegistrationSystem.Data;
using StudentCourseRegistrationSystem.Helpers;
using StudentCourseRegistrationSystem.Models;

namespace StudentCourseRegistrationSystem.Data
{
    /// <summary>
    /// Veritabanını oluşturur ve ilk verileri idempotent (tekrarsız) biçimde ekler.
    /// Her uygulama başlangıcında güvenle çağrılabilir.
    /// </summary>
    public static class DatabaseInitializer
    {
        public static void Initialize(AppDbContext context)
        {
            context.Database.EnsureCreated();

            SeedRoles(context);
            SeedAdmin(context);
            SeedDepartments(context);
            SeedSemesters(context);
            SeedInstructors(context);
            SeedCourses(context);
            SeedStudents(context);
            SeedEnrollments(context);
        }

        private static void SeedRoles(AppDbContext ctx)
        {
            if (!ctx.Roles.Any(r => r.Name == "Admin"))
                ctx.Roles.Add(new Role { Name = "Admin" });
            if (!ctx.Roles.Any(r => r.Name == "Instructor"))
                ctx.Roles.Add(new Role { Name = "Instructor" });
            if (!ctx.Roles.Any(r => r.Name == "Student"))
                ctx.Roles.Add(new Role { Name = "Student" });
            ctx.SaveChanges();
        }

        private static void SeedAdmin(AppDbContext ctx)
        {
            if (ctx.Users.Any(u => u.Username == "admin")) return;
            var adminRole = ctx.Roles.First(r => r.Name == "Admin");
            ctx.Users.Add(new User
            {
                Username = "admin",
                PasswordHash = PasswordHelper.HashPassword("admin123"),
                Email = "admin@system.com",
                RoleId = adminRole.Id
            });
            ctx.SaveChanges();
        }

        private static void SeedDepartments(AppDbContext ctx)
        {
            if (!ctx.Departments.Any(d => d.Code == "13"))
                ctx.Departments.Add(new Department { Name = "Bilgisayar Mühendisliği", Code = "13" });
            if (!ctx.Departments.Any(d => d.Code == "14"))
                ctx.Departments.Add(new Department { Name = "Elektrik Elektronik Mühendisliği", Code = "14" });
            ctx.SaveChanges();
        }

        private static void SeedSemesters(AppDbContext ctx)
        {
            if (!ctx.Semesters.Any(s => s.Name == "2026 Güz"))
                ctx.Semesters.Add(new Semester { Name = "2026 Güz" });
            ctx.SaveChanges();
        }

        private static void SeedInstructors(AppDbContext ctx)
        {
            var instRole = ctx.Roles.First(r => r.Name == "Instructor");

            if (!ctx.Instructors.Any(i => i.FirstName == "Ahmet" && i.LastName == "Yilmaz"))
            {
                var inst1 = new Instructor { FirstName = "Ahmet", LastName = "Yilmaz" };
                ctx.Instructors.Add(inst1);
                ctx.SaveChanges();
                string uname1 = "ahmetyilmaz";
                if (!ctx.Users.Any(u => u.Username == uname1))
                {
                    ctx.Users.Add(new User
                    {
                        Username = uname1,
                        PasswordHash = PasswordHelper.HashPassword("123ay"),
                        Email = $"{uname1}@system.com",
                        RoleId = instRole.Id,
                        InstructorId = inst1.Id
                    });
                    ctx.SaveChanges();
                }
            }

            if (!ctx.Instructors.Any(i => i.FirstName == "Ayse" && i.LastName == "Kaya"))
            {
                var inst2 = new Instructor { FirstName = "Ayse", LastName = "Kaya" };
                ctx.Instructors.Add(inst2);
                ctx.SaveChanges();
                string uname2 = "aysekaya";
                if (!ctx.Users.Any(u => u.Username == uname2))
                {
                    ctx.Users.Add(new User
                    {
                        Username = uname2,
                        PasswordHash = PasswordHelper.HashPassword("123ak"),
                        Email = $"{uname2}@system.com",
                        RoleId = instRole.Id,
                        InstructorId = inst2.Id
                    });
                    ctx.SaveChanges();
                }
            }
        }

        private static void SeedCourses(AppDbContext ctx)
        {
            if (ctx.Courses.Any(c => c.CourseCode == "BLG101")) return;

            var deptCE = ctx.Departments.First(d => d.Code == "13");
            var deptEE = ctx.Departments.First(d => d.Code == "14");
            var inst1 = ctx.Instructors.FirstOrDefault(i => i.FirstName == "Ahmet");
            var inst2 = ctx.Instructors.FirstOrDefault(i => i.FirstName == "Ayse");

            ctx.Courses.Add(new Course
            {
                CourseCode = "BLG101",
                Title = "Programlamaya Giris",
                Capacity = 50,
                Credits = 4,
                DepartmentId = deptCE.Id,
                InstructorId = inst1 != null ? (int?)inst1.Id : null
            });
            ctx.Courses.Add(new Course
            {
                CourseCode = "ELK101",
                Title = "Devre Teorisi",
                Capacity = 40,
                Credits = 3,
                DepartmentId = deptEE.Id,
                InstructorId = inst2 != null ? (int?)inst2.Id : null
            });
            ctx.SaveChanges();
        }

        private static void SeedStudents(AppDbContext ctx)
        {
            var stdRole = ctx.Roles.First(r => r.Name == "Student");
            var deptCE = ctx.Departments.First(d => d.Code == "13");
            var deptEE = ctx.Departments.First(d => d.Code == "14");

            if (!ctx.Students.Any(s => s.StudentNumber == "26131001"))
            {
                var std1 = new Student
                {
                    FirstName = "Emre",
                    LastName = "Oz",
                    DepartmentId = deptCE.Id,
                    StudentNumber = "26131001"
                };
                ctx.Students.Add(std1);
                ctx.SaveChanges();
                string uname = "emreoz";
                if (!ctx.Users.Any(u => u.Username == uname))
                {
                    ctx.Users.Add(new User
                    {
                        Username = uname,
                        PasswordHash = PasswordHelper.HashPassword("eo123"),
                        Email = $"{uname}@student.edu.tr",
                        RoleId = stdRole.Id,
                        StudentId = std1.Id
                    });
                    ctx.SaveChanges();
                }
            }

            if (!ctx.Students.Any(s => s.StudentNumber == "26141001"))
            {
                var std2 = new Student
                {
                    FirstName = "Fatma",
                    LastName = "Demir",
                    DepartmentId = deptEE.Id,
                    StudentNumber = "26141001"
                };
                ctx.Students.Add(std2);
                ctx.SaveChanges();
                string uname = "fatmademir";
                if (!ctx.Users.Any(u => u.Username == uname))
                {
                    ctx.Users.Add(new User
                    {
                        Username = uname,
                        PasswordHash = PasswordHelper.HashPassword("fd123"),
                        Email = $"{uname}@student.edu.tr",
                        RoleId = stdRole.Id,
                        StudentId = std2.Id
                    });
                    ctx.SaveChanges();
                }
            }
        }

        private static void SeedEnrollments(AppDbContext ctx)
        {
            var semester = ctx.Semesters.FirstOrDefault(s => s.Name == "2026 Güz");
            if (semester == null) return;

            var std1 = ctx.Students.FirstOrDefault(s => s.StudentNumber == "26131001");
            var std2 = ctx.Students.FirstOrDefault(s => s.StudentNumber == "26141001");
            var course1 = ctx.Courses.FirstOrDefault(c => c.CourseCode == "BLG101");
            var course2 = ctx.Courses.FirstOrDefault(c => c.CourseCode == "ELK101");

            if (std1 != null && course1 != null &&
                !ctx.Enrollments.Any(e => e.StudentId == std1.Id && e.CourseId == course1.Id && e.SemesterId == semester.Id))
            {
                ctx.Enrollments.Add(new Enrollment { StudentId = std1.Id, CourseId = course1.Id, SemesterId = semester.Id });
            }

            if (std2 != null && course2 != null &&
                !ctx.Enrollments.Any(e => e.StudentId == std2.Id && e.CourseId == course2.Id && e.SemesterId == semester.Id))
            {
                ctx.Enrollments.Add(new Enrollment { StudentId = std2.Id, CourseId = course2.Id, SemesterId = semester.Id });
            }

            ctx.SaveChanges();
        }
    }
}
