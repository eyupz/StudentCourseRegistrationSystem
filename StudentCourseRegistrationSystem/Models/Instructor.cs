using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StudentCourseRegistrationSystem.Models
{
    public class Instructor
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string FirstName { get; set; }

        [Required]
        [MaxLength(50)]
        public string LastName { get; set; }

        /// <summary>Seed sırasında kullanıcı ile ilişkilendirmek için. EF FK olarak yönetmiyor.</summary>
        [NotMapped]
        public int? UserId { get; set; }

        // Navigation properties
        public virtual ICollection<Course> Courses { get; set; } = new List<Course>();
        public virtual User User { get; set; }
    }
}
