using System;
using System.Linq;
using System.Windows.Forms;
using StudentCourseRegistrationSystem.Forms;
using StudentCourseRegistrationSystem.Data;
using StudentCourseRegistrationSystem.Models;
using StudentCourseRegistrationSystem.Helpers;

namespace StudentCourseRegistrationSystem
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            ApplicationConfiguration.Initialize();
            Logger.Info("=== Uygulama başlatıldı ===");

            try
            {
                using (var context = new AppDbContext())
                {
                    context.Database.EnsureCreated();
                    SeedDatabase(context);
                }
            }
            catch (Exception ex)
            {
                Logger.Error("Veritabanı başlatma hatası", ex);
                MessageBox.Show("Veritabanı başlatılırken hata oluştu:\n" + ex.Message, "Kritik Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            Application.Run(new LoginForm());
            Logger.Info("=== Uygulama kapatıldı ===");
        }

        private static void SeedDatabase(AppDbContext context)
        {
            // ─── Roller ───────────────────────────────────────────────────────────
            if (!context.Roles.Any())
            {
                context.Roles.AddRange(
                    new Role { Name = "Admin" },
                    new Role { Name = "Student" },
                    new Role { Name = "Instructor" }
                );
                context.SaveChanges();
            }

            // ─── Bölümler ─────────────────────────────────────────────────────────
            if (!context.Departments.Any())
            {
                context.Departments.AddRange(
                    new Department { Name = "Bilgisayar Mühendisliği", Code = "CENG", Faculty = "Mühendislik Fakültesi" },
                    new Department { Name = "Yazılım Mühendisliği",    Code = "SENG", Faculty = "Mühendislik Fakültesi" },
                    new Department { Name = "Elektrik-Elektronik Mühendisliği", Code = "EENG", Faculty = "Mühendislik Fakültesi" },
                    new Department { Name = "Matematik",               Code = "MATH", Faculty = "Fen Fakültesi" }
                );
                context.SaveChanges();
            }

            // ─── Dönemler ─────────────────────────────────────────────────────────
            if (!context.Semesters.Any())
            {
                context.Semesters.AddRange(
                    new Semester { Name = "Güz 2025",   IsActive = false },
                    new Semester { Name = "Bahar 2026", IsActive = false },
                    new Semester { Name = "Güz 2026",   IsActive = true  }
                );
                context.SaveChanges();
            }

            var adminRole = context.Roles.First(r => r.Name == "Admin");
            var instrRole = context.Roles.First(r => r.Name == "Instructor");
            var studRole  = context.Roles.First(r => r.Name == "Student");
            var dept      = context.Departments.First(d => d.Code == "CENG");
            var mathDept  = context.Departments.First(d => d.Code == "MATH");

            // ─── Admin Kullanıcısı ────────────────────────────────────────────────
            if (!context.Users.Any(u => u.Username == "admin"))
            {
                context.Users.Add(new User
                {
                    Username = "admin",
                    PasswordHash = PasswordHelper.HashPassword("admin123"),
                    Email = "admin@obs.edu.tr",
                    RoleId = adminRole.Id
                });
                context.SaveChanges();
            }

            // ─── Eğitmen: Önce User, sonra Instructor, sonra geri link ───────────
            if (!context.Users.Any(u => u.Username == "fatihzahidgenç"))
            {
                // 1. User'ı InstructorId olmadan yaz
                var instrUser = new User
                {
                    Username     = "fatihzahidgenç",
                    PasswordHash = PasswordHelper.HashPassword("fzg123"),
                    Email        = "fatihzahid.genc@obs.edu.tr",
                    RoleId       = instrRole.Id
                };
                context.Users.Add(instrUser);
                context.SaveChanges();

                // 2. Instructor profilini UserId ile oluştur
                var instrProfile = new Instructor
                {
                    FirstName = "Fatih Zahid",
                    LastName  = "Genç",
                    UserId    = instrUser.Id
                };
                context.Instructors.Add(instrProfile);
                context.SaveChanges();

                // 3. User → Instructor geri-link
                instrUser.InstructorId = instrProfile.Id;
                context.SaveChanges();
            }

            // ─── Öğrenci: Önce User, sonra Student, sonra geri link ──────────────
            if (!context.Users.Any(u => u.Username == "zeynepdemir"))
            {
                // 1. User'ı StudentId olmadan yaz
                var studUser = new User
                {
                    Username     = "zeynepdemir",
                    PasswordHash = PasswordHelper.HashPassword("zd123"),
                    Email        = "zeynepdemir@obs.edu.tr",
                    RoleId       = studRole.Id
                };
                context.Users.Add(studUser);
                context.SaveChanges();

                // 2. Student profilini UserId ile oluştur
                var studProfile = new Student
                {
                    StudentNumber = "2024001",
                    FirstName     = "Zeynep",
                    LastName      = "Demir",
                    DepartmentId  = dept.Id,
                    UserId        = studUser.Id,
                    GPA           = 0
                };
                context.Students.Add(studProfile);
                context.SaveChanges();

                // 3. User → Student geri-link
                studUser.StudentId = studProfile.Id;
                context.SaveChanges();
            }

            // ─── Örnek Dersler ────────────────────────────────────────────────────
            if (!context.Courses.Any())
            {
                var instructor = context.Instructors.First();

                var ceng101 = new Course { CourseCode = "CENG101", Title = "Programlamaya Giriş",   Credits = 4, Capacity = 30, Schedule = "Pzt 09:00-11:00", DepartmentId = dept.Id,     InstructorId = instructor.Id };
                var ceng102 = new Course { CourseCode = "CENG102", Title = "Veri Yapıları",         Credits = 4, Capacity = 25, Schedule = "Sal 13:00-15:00", DepartmentId = dept.Id,     InstructorId = instructor.Id };
                var ceng201 = new Course { CourseCode = "CENG201", Title = "Algoritma Analizi",     Credits = 3, Capacity = 20, Schedule = "Çar 10:00-12:00", DepartmentId = dept.Id,     InstructorId = instructor.Id };
                var ceng301 = new Course { CourseCode = "CENG301", Title = "Veritabanı Sistemleri", Credits = 3, Capacity = 30, Schedule = "Per 14:00-16:00", DepartmentId = dept.Id,     InstructorId = instructor.Id };
                var math101 = new Course { CourseCode = "MATH101", Title = "Matematik I",           Credits = 4, Capacity = 40, Schedule = "Cum 09:00-11:00", DepartmentId = mathDept.Id, InstructorId = instructor.Id };

                context.Courses.AddRange(ceng101, ceng102, ceng201, ceng301, math101);
                context.SaveChanges();

                // Ön Koşul: CENG201 → CENG102 gerektirir
                ceng201.PrerequisiteCourseId = ceng102.Id;
                context.SaveChanges();
            }

            Logger.Info("Veritabanı seed işlemi tamamlandı.");
        }
    }
}
