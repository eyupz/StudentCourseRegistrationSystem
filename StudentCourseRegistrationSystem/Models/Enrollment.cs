using System.ComponentModel.DataAnnotations;

namespace StudentCourseRegistrationSystem.Models
{
    public class Enrollment
    {
        public int Id { get; set; }

        public int StudentId { get; set; }
        public int CourseId { get; set; }
        public int SemesterId { get; set; }

        // Navigation properties
        public virtual Student Student { get; set; }
        public virtual Course Course { get; set; }
        public virtual Semester Semester { get; set; }
        
        public virtual Grade Grade { get; set; }
    }
}
