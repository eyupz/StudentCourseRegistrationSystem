using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace StudentCourseRegistrationSystem.Models
{
    public class Semester
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string Name { get; set; } // e.g., "Fall 2026"

        public bool IsActive { get; set; }

        // Navigation properties
        public virtual ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();
    }
}
