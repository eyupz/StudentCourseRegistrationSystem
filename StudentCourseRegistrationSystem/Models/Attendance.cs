using System;

namespace StudentCourseRegistrationSystem.Models
{
    public class Attendance
    {
        public int Id { get; set; }
        
        public int EnrollmentId { get; set; }
        public virtual Enrollment Enrollment { get; set; }
        
        public int Week { get; set; } // 1 to 15
        public int Hour { get; set; } // 1, 2, 3...
        public bool IsPresent { get; set; }
        public DateTime Date { get; set; } = DateTime.Now;
    }
}
