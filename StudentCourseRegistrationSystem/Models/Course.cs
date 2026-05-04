using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StudentCourseRegistrationSystem.Models
{
    public class Course
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(20)]
        public string CourseCode { get; set; }

        [Required]
        [MaxLength(100)]
        public string Title { get; set; }

        public int Credits { get; set; }
        
        public int Capacity { get; set; }

        [MaxLength(50)]
        public string Schedule { get; set; } // Örn: "Pzt 10:00-12:00"

        public int DepartmentId { get; set; }
        public int InstructorId { get; set; }

        public int? PrerequisiteCourseId { get; set; } // Nullable

        // Navigation properties
        public virtual Department Department { get; set; }
        public virtual Instructor Instructor { get; set; }
        
        [ForeignKey("PrerequisiteCourseId")]
        public virtual Course PrerequisiteCourse { get; set; }

        public virtual ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();
    }
}
