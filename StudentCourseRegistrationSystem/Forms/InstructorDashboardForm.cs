using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Microsoft.EntityFrameworkCore;
using StudentCourseRegistrationSystem.Data;
using StudentCourseRegistrationSystem.Helpers;
using StudentCourseRegistrationSystem.Services;

namespace StudentCourseRegistrationSystem.Forms
{
    public partial class InstructorDashboardForm : Form
    {
        public InstructorDashboardForm()
        {
            this.BackColor = ThemeManager.BackgroundColor;
            this.FormBorderStyle = FormBorderStyle.None;
            this.TopLevel = false;
            this.Dock = DockStyle.Fill;
            BuildUI();
        }

        private void BuildUI()
        {
            try
            {
                using var ctx = new AppDbContext();
                var instrUser = ctx.Users.Include(u => u.Instructor).ThenInclude(i => i.Courses).FirstOrDefault(u => u.Id == SessionManager.CurrentUser.Id);
                var instructor = instrUser?.Instructor;
                if (instructor == null) { AddError("Eğitmen profili bulunamadı."); return; }

                var pnlCard = new Panel { Location = new Point(30, 30), Size = new Size(600, 90), BackColor = Color.White };
                pnlCard.Paint += (s, e) => ControlPaint.DrawBorder(e.Graphics, pnlCard.ClientRectangle, ThemeManager.BorderColor, ButtonBorderStyle.Solid);
                pnlCard.Controls.Add(new Panel { Dock = DockStyle.Top, Height = 6, BackColor = ThemeManager.PrimaryButton });
                pnlCard.Controls.Add(new Label { Text = $"👨‍🏫  {instructor.FirstName} {instructor.LastName}", Font = ThemeManager.HeaderFont, ForeColor = ThemeManager.TextPrimary, Location = new Point(20, 20), AutoSize = true });

                var myCourses = ctx.Courses.Where(c => c.InstructorId == instructor.Id).ToList();

                var lblTitle = new Label { Text = "Verdiğim Dersler", Font = ThemeManager.TitleFont, ForeColor = ThemeManager.TextPrimary, Location = new Point(30, 140), AutoSize = true };
                var statsFlow = new FlowLayoutPanel { Location = new Point(30, 175), AutoSize = true, FlowDirection = FlowDirection.LeftToRight };
                statsFlow.Controls.Add(MiniCard("Toplam Ders", myCourses.Count.ToString(), Color.FromArgb(67, 97, 238)));

                int totalStudents = ctx.Enrollments.Count(e => myCourses.Select(c => c.Id).Contains(e.CourseId) && e.Status == Models.EnrollmentStatus.Enrolled);
                statsFlow.Controls.Add(MiniCard("Toplam Öğrenci", totalStudents.ToString(), Color.FromArgb(76, 201, 140)));

                var dgv = new DataGridView();
                ThemeManager.StyleDataGrid(dgv);
                dgv.Location = new Point(30, 295);
                dgv.Size = new Size(1100, 350);
                dgv.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
                dgv.DataSource = myCourses.Select(c => new
                {
                    Kod       = c.CourseCode,
                    DersAdı   = c.Title,
                    Program   = c.Schedule,
                    Kredi     = c.Credits,
                    Kontenjan = c.Capacity,
                    Kayıtlı   = ctx.Enrollments.Count(e => e.CourseId == c.Id && e.Status == Models.EnrollmentStatus.Enrolled)
                }).ToList();

                this.Controls.AddRange(new Control[] { pnlCard, lblTitle, statsFlow, dgv });
            }
            catch (Exception ex) { AddError(ex.Message); }
        }

        private Panel MiniCard(string title, string value, Color color)
        {
            var p = new Panel { Size = new Size(180, 90), BackColor = Color.White, Margin = new Padding(0, 0, 20, 0) };
            p.Paint += (s, e) => ControlPaint.DrawBorder(e.Graphics, p.ClientRectangle, ThemeManager.BorderColor, ButtonBorderStyle.Solid);
            p.Controls.Add(new Panel { Dock = DockStyle.Top, Height = 5, BackColor = color });
            p.Controls.Add(new Label { Text = title, Font = ThemeManager.LabelFont, ForeColor = ThemeManager.TextSecondary, Location = new Point(15, 20), AutoSize = true });
            p.Controls.Add(new Label { Text = value, Font = new Font("Segoe UI", 24, FontStyle.Bold), ForeColor = color, Location = new Point(12, 45), AutoSize = true });
            return p;
        }

        private void AddError(string msg) =>
            this.Controls.Add(new Label { Text = "⚠️ " + msg, Font = ThemeManager.RegularFont, ForeColor = ThemeManager.DangerButton, Location = new Point(40, 40), AutoSize = true });
    }
}
