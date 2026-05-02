using System.ComponentModel.DataAnnotations;

namespace StudentCourseRegistrationSystem.Models
{
    public class Grade
    {
        public int Id { get; set; }

        public int EnrollmentId { get; set; }

        [Range(0, 100)]
        public double? Score { get; set; }
        
        [MaxLength(2)]
        public string LetterGrade { get; set; } // e.g., AA, BA, BB, etc.

        // Navigation properties
        public virtual Enrollment Enrollment { get; set; }
    }
}
