using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using StudentCourseRegistrationSystem.Data;

namespace StudentCourseRegistrationSystem.Forms
{
    public partial class HomeForm : Form
    {
        private Label lblTotalStudents, lblTotalCourses, lblTotalEnrollments;

        public HomeForm()
        {
            InitializeComponent();
            LoadStats();
        }

        private void InitializeComponent()
        {
            this.Text = "Ana Sayfa";
            this.BackColor = ThemeManager.BackgroundColor;
            this.FormBorderStyle = FormBorderStyle.None;
            this.TopLevel = false;
            this.Dock = DockStyle.Fill;

            var pnlTop = new Panel { Dock = DockStyle.Top, Height = 120, BackColor = Color.Transparent };

            var lblWelcome = new Label 
            { 
                Text = "Yönetim Paneli İstatistikleri", 
                Font = ThemeManager.HeaderFont, 
                ForeColor = ThemeManager.TextPrimary,
                Location = new Point(30, 30), 
                AutoSize = true 
            };
            
            var lblSub = new Label 
            { 
                Text = "Sisteme genel bakış ve anlık durum.", 
                Font = ThemeManager.RegularFont, 
                ForeColor = ThemeManager.TextSecondary,
                Location = new Point(35, 75), 
                AutoSize = true 
            };

            pnlTop.Controls.Add(lblWelcome);
            pnlTop.Controls.Add(lblSub);

            var pnlStats = new FlowLayoutPanel
            {
                Location = new Point(35, 130),
                AutoSize = true,
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = true,
                Width = 900
            };

            var cardStudents = CreateStatCard("Toplam Öğrenci", out lblTotalStudents);
            var cardCourses = CreateStatCard("Toplam Ders", out lblTotalCourses);
            var cardEnrollments = CreateStatCard("Toplam Kayıt", out lblTotalEnrollments);

            pnlStats.Controls.Add(cardStudents);
            pnlStats.Controls.Add(cardCourses);
            pnlStats.Controls.Add(cardEnrollments);

            this.Controls.Add(pnlStats);
            this.Controls.Add(pnlTop);
        }

        private Panel CreateStatCard(string title, out Label lblValue)
        {
            var pnl = new Panel { Size = new Size(250, 120), BackColor = Color.White, Margin = new Padding(0, 0, 20, 20) };
            pnl.Paint += (s, e) => { ControlPaint.DrawBorder(e.Graphics, pnl.ClientRectangle, ThemeManager.BorderColor, ButtonBorderStyle.Solid); };

            var lblTitle = new Label { Text = title, Font = ThemeManager.RegularFont, ForeColor = ThemeManager.TextSecondary, Location = new Point(25, 25), AutoSize = true };
            lblValue = new Label { Text = "0", Font = new Font("Segoe UI", 28, FontStyle.Bold), ForeColor = ThemeManager.PrimaryButton, Location = new Point(20, 55), AutoSize = true };

            pnl.Controls.Add(lblTitle);
            pnl.Controls.Add(lblValue);
            return pnl;
        }

        private void LoadStats()
        {
            try
            {
                using (var context = new AppDbContext())
                {
                    lblTotalStudents.Text = context.Students.Count().ToString();
                    lblTotalCourses.Text = context.Courses.Count().ToString();
                    lblTotalEnrollments.Text = context.Enrollments.Count().ToString();
                }
            }
            catch { }
        }
    }
}
