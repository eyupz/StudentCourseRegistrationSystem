using Microsoft.EntityFrameworkCore;
using StudentCourseRegistrationSystem.Models;
using System;
using System.IO;

namespace StudentCourseRegistrationSystem.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<Role> Roles { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<Student> Students { get; set; }
        public DbSet<Instructor> Instructors { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<Semester> Semesters { get; set; }
        public DbSet<Enrollment> Enrollments { get; set; }
        public DbSet<Grade> Grades { get; set; }

        public AppDbContext()
        {
        }

        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                string basePath = AppDomain.CurrentDomain.BaseDirectory;

                string dbFolder = Path.Combine(basePath, "Database");

                if (!Directory.Exists(dbFolder))
                {
                    Directory.CreateDirectory(dbFolder);
                }

                string dbPath = Path.Combine(dbFolder, "student_system.db");

                optionsBuilder.UseSqlite($"Data Source={dbPath}");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Unique constraints
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Username)
                .IsUnique();

            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

            modelBuilder.Entity<Student>()
                .HasIndex(s => s.StudentNumber)
                .IsUnique();

            modelBuilder.Entity<Course>()
                .HasIndex(c => c.CourseCode)
                .IsUnique();

            modelBuilder.Entity<Department>()
                .HasIndex(d => d.Code)
                .IsUnique();

            modelBuilder.Entity<Role>()
                .HasIndex(r => r.Name)
                .IsUnique();

            modelBuilder.Entity<Semester>()
                .HasIndex(s => s.Name)
                .IsUnique();

            modelBuilder.Entity<Enrollment>()
                .HasIndex(e => new { e.StudentId, e.CourseId, e.SemesterId })
                .IsUnique();

            // Role 1 - N Users
            modelBuilder.Entity<User>()
                .HasOne(u => u.Role)
                .WithMany(r => r.Users)
                .HasForeignKey(u => u.RoleId)
                .OnDelete(DeleteBehavior.Restrict);

            // Department 1 - N Students
            modelBuilder.Entity<Student>()
                .HasOne(s => s.Department)
                .WithMany(d => d.Students)
                .HasForeignKey(s => s.DepartmentId)
                .OnDelete(DeleteBehavior.Restrict);

            // Department 1 - N Courses
            modelBuilder.Entity<Course>()
                .HasOne(c => c.Department)
                .WithMany(d => d.Courses)
                .HasForeignKey(c => c.DepartmentId)
                .OnDelete(DeleteBehavior.Restrict);

            // Instructor 1 - N Courses (Dersç görevsiz de olabilir)
            modelBuilder.Entity<Course>()
                .HasOne(c => c.Instructor)
                .WithMany(i => i.Courses)
                .HasForeignKey(c => c.InstructorId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.SetNull);

            // Student 1 - N Enrollments
            modelBuilder.Entity<Enrollment>()
                .HasOne(e => e.Student)
                .WithMany(s => s.Enrollments)
                .HasForeignKey(e => e.StudentId)
                .OnDelete(DeleteBehavior.Cascade);

            // Course 1 - N Enrollments
            modelBuilder.Entity<Enrollment>()
                .HasOne(e => e.Course)
                .WithMany(c => c.Enrollments)
                .HasForeignKey(e => e.CourseId)
                .OnDelete(DeleteBehavior.Restrict);

            // Semester 1 - N Enrollments
            modelBuilder.Entity<Enrollment>()
                .HasOne(e => e.Semester)
                .WithMany(s => s.Enrollments)
                .HasForeignKey(e => e.SemesterId)
                .OnDelete(DeleteBehavior.Restrict);

            // Enrollment 1 - 1 Grade
            modelBuilder.Entity<Grade>()
                .HasOne(g => g.Enrollment)
                .WithOne(e => e.Grade)
                .HasForeignKey<Grade>(g => g.EnrollmentId)
                .OnDelete(DeleteBehavior.Cascade);

            // User 1 - 1 Student
            modelBuilder.Entity<User>()
                .HasOne(u => u.Student)
                .WithOne(s => s.User)
                .HasForeignKey<User>(u => u.StudentId)
                .OnDelete(DeleteBehavior.SetNull);

            // User 1 - 1 Instructor
            modelBuilder.Entity<User>()
                .HasOne(u => u.Instructor)
                .WithOne(i => i.User)
                .HasForeignKey<User>(u => u.InstructorId)
                .OnDelete(DeleteBehavior.SetNull);
        }
    }
}