using System;
using System.Windows.Forms;
using StudentCourseRegistrationSystem.Data;
using StudentCourseRegistrationSystem.Forms;
using StudentCourseRegistrationSystem.Services;

namespace StudentCourseRegistrationSystem
{
    internal static class Program
    {
        /// <summary>
        /// Uygulamanın ana giriş noktası.
        /// </summary>
        [STAThread]
        static void Main()
        {
            ApplicationConfiguration.Initialize();

            try
            {
                // Veritabanını oluştur ve seed verisini idempotent şekilde ekle
                using (var initCtx = new AppDbContext())
                {
                    DatabaseInitializer.Initialize(initCtx);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Veritabanı başlatılamadı:\n" + Helpers.ErrorHelper.GetFullExceptionMessage(ex),
                    "Başlatma Hatası", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Tüm servisler için paylaşılan tek AppDbContext
            var dbContext = new AppDbContext();

            var authService       = new AuthService(dbContext);
            var studentService    = new StudentService(dbContext);
            var courseService     = new CourseService(dbContext);
            var enrollmentService = new EnrollmentService(dbContext, courseService);
            var reportService     = new ReportService(dbContext);
            var instructorService = new InstructorService(dbContext);
            var semesterService   = new SemesterService(dbContext);

            Application.Run(new LoginForm(
                authService, studentService, courseService,
                enrollmentService, reportService, instructorService, semesterService));
        }
    }
}
