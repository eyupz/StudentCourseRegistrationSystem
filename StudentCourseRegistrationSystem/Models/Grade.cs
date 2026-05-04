using System.ComponentModel.DataAnnotations;

namespace StudentCourseRegistrationSystem.Models
{
    public class Grade
    {
        public int Id { get; set; }

        public int EnrollmentId { get; set; }

        [Range(0, 100)]
        public double? Score { get; set; }
        
        [MaxLength(10)]
        public string LetterGrade { get; set; } // Örn: AA, BA, BB, CB, CC, DC, DD, FD, FF

        // Navigation properties
        public virtual Enrollment Enrollment { get; set; }
    }
}
