using System.ComponentModel.DataAnnotations;

namespace StudentCourseRegistrationSystem.Models
{
    public class User
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string Username { get; set; }

        [Required]
        public string PasswordHash { get; set; }

        [Required]
        [EmailAddress]
        [MaxLength(100)]
        public string Email { get; set; }

        public int RoleId { get; set; }
        
        // Navigation properties
        public virtual Role Role { get; set; }
        
        // Nullable reference to student or instructor depending on role
        public int? StudentId { get; set; }
        public virtual Student Student { get; set; }

        public int? InstructorId { get; set; }
        public virtual Instructor Instructor { get; set; }
    }
}
