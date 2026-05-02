using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

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

        // Navigation properties
        public virtual Department Department { get; set; }
        public virtual ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();
        public virtual User User { get; set; }
    }
}
