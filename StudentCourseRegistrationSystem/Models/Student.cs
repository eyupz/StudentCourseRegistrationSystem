using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace StudentCourseRegistrationSystem.Models
{
    public class Student
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(20)]
        public string StudentNumber { get; set; }

        [Required]
        [MaxLength(50)]
        public string FirstName { get; set; }

        [Required]
        [MaxLength(50)]
        public string LastName { get; set; }

        public int DepartmentId { get; set; }
        public int UserId { get; set; }

        public decimal GPA { get; set; }

        // Navigation properties
        public virtual Department Department { get; set; }
        public virtual User User { get; set; }
        public virtual ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();

        [NotMapped]
        public List<Course> CompletedCourses => Enrollments?
            .Where(e => e.Status == EnrollmentStatus.Completed && (e.Grade != "FF" && e.Grade != "FD" && !string.IsNullOrEmpty(e.Grade)))
            .Select(e => e.Course)
            .ToList() ?? new List<Course>();
    }
}
