using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

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

        public int DepartmentId { get; set; }
        public int InstructorId { get; set; }

        // Navigation properties
        public virtual Department Department { get; set; }
        public virtual Instructor Instructor { get; set; }
        public virtual ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();
    }
}
