using System.ComponentModel.DataAnnotations;

namespace StudentCourseRegistrationSystem.Models
{
    public enum EnrollmentStatus
    {
        Enrolled,
        Completed,
        Dropped
    }

    public class Enrollment
    {
        public int Id { get; set; }

        public int StudentId  { get; set; }
        public int CourseId   { get; set; }
        public int SemesterId { get; set; }

        // ─── Sayısal Notlar (0-100) ─────────────────────────────────────────────
        public decimal? Vize  { get; set; }   // %35
        public decimal? Final { get; set; }   // %50
        public decimal? Odev  { get; set; }   // %15

        // ─── Hesaplanan Harf Notu (otomatik doldurulur) ─────────────────────────
        [MaxLength(4)]
        public string? Grade { get; set; }

        public decimal? Average 
        { 
            get 
            {
                if (Vize.HasValue || Final.HasValue || Odev.HasValue)
                {
                    decimal v = Vize ?? 0;
                    decimal f = Final ?? 0;
                    decimal o = Odev ?? 0;
                    return (v * 0.35m) + (f * 0.50m) + (o * 0.15m);
                }
                return null;
            }
        }

        public EnrollmentStatus Status { get; set; } = EnrollmentStatus.Enrolled;

        // ─── Navigation ─────────────────────────────────────────────────────────
        public virtual Student  Student  { get; set; }
        public virtual Course   Course   { get; set; }
        public virtual Semester Semester { get; set; }

        public virtual System.Collections.Generic.ICollection<Attendance> Attendances { get; set; } = new System.Collections.Generic.List<Attendance>();
    }
}
