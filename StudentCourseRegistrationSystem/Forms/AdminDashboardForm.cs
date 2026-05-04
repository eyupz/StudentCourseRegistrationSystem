using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using StudentCourseRegistrationSystem.Data;

namespace StudentCourseRegistrationSystem.Forms
{
    public partial class AdminDashboardForm : Form
    {
        public AdminDashboardForm()
        {
            this.BackColor = ThemeManager.BackgroundColor;
            this.FormBorderStyle = FormBorderStyle.None;
            this.TopLevel = false;
            this.Dock = DockStyle.Fill;
            BuildUI();
        }

        private void BuildUI()
        {
            var pnlWrap = new Panel { Dock = DockStyle.Fill, Padding = new Padding(40) };

            var lblTitle = new Label { Text = "Yönetici Paneline Hoşgeldiniz", Font = ThemeManager.HeaderFont, ForeColor = ThemeManager.TextPrimary, Location = new Point(40, 30), AutoSize = true };
            var lblSub = new Label { Text = "Sistemin anlık durumu", Font = ThemeManager.RegularFont, ForeColor = ThemeManager.TextSecondary, Location = new Point(42, 72), AutoSize = true };

            var statPanel = new FlowLayoutPanel { Location = new Point(40, 110), AutoSize = true, FlowDirection = FlowDirection.LeftToRight, WrapContents = true, Width = 1100 };

            try
            {
                using var ctx = new AppDbContext();
                statPanel.Controls.Add(MakeCard("🎓 Toplam Öğrenci", ctx.Students.Count().ToString(), Color.FromArgb(67, 97, 238)));
                statPanel.Controls.Add(MakeCard("📚 Toplam Ders", ctx.Courses.Count().ToString(), Color.FromArgb(76, 201, 140)));
                statPanel.Controls.Add(MakeCard("📝 Aktif Kayıt", ctx.Enrollments.Count(e => e.Status == Models.EnrollmentStatus.Enrolled).ToString(), Color.FromArgb(248, 150, 30)));
                statPanel.Controls.Add(MakeCard("👨‍🏫 Eğitmen Sayısı", ctx.Instructors.Count().ToString(), Color.FromArgb(114, 9, 183)));
            }
            catch { }

            this.Controls.Add(lblSub);
            this.Controls.Add(lblTitle);
            this.Controls.Add(statPanel);
        }

        private Panel MakeCard(string title, string value, Color accent)
        {
            var card = new Panel { Size = new Size(260, 130), BackColor = Color.White, Margin = new Padding(0, 0, 25, 25) };
            var accentBar = new Panel { Dock = DockStyle.Top, Height = 5, BackColor = accent };
            var lblT = new Label { Text = title, Font = ThemeManager.LabelFont, ForeColor = ThemeManager.TextSecondary, Location = new Point(20, 25), AutoSize = true };
            var lblV = new Label { Text = value, Font = new Font("Segoe UI", 32, FontStyle.Bold), ForeColor = accent, Location = new Point(15, 55), AutoSize = true };
            card.Controls.Add(accentBar);
            card.Controls.Add(lblT);
            card.Controls.Add(lblV);
            card.Paint += (s, e) => ControlPaint.DrawBorder(e.Graphics, card.ClientRectangle, ThemeManager.BorderColor, ButtonBorderStyle.Solid);
            return card;
        }
    }
}
