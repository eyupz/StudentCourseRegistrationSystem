using System;
using System.Drawing;
using System.Windows.Forms;

namespace StudentCourseRegistrationSystem.Forms
{
    public partial class MainForm : Form
    {
        private Panel pnlContent;

        public MainForm()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.Text = "Student Course Registration System";
            this.Size = new Size(1000, 700);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.FromArgb(236, 240, 241);

            var panelMenu = new Panel
            {
                Dock = DockStyle.Left,
                Width = 220,
                BackColor = Color.FromArgb(44, 62, 80) // Dark Slate Gray
            };

            var panelLogo = new Panel
            {
                Dock = DockStyle.Top,
                Height = 100,
                BackColor = Color.FromArgb(34, 49, 63)
            };

            var lblLogo = new Label
            {
                Text = "EduSystem",
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 18, FontStyle.Bold),
                AutoSize = false,
                TextAlign = ContentAlignment.MiddleCenter,
                Dock = DockStyle.Fill
            };
            panelLogo.Controls.Add(lblLogo);
            panelMenu.Controls.Add(panelLogo);

            string[] buttons = { "Students", "Courses", "Enrollments", "Reports", "Logout" };
            int yPos = 120;

            foreach (var btnText in buttons)
            {
                var btn = new Button
                {
                    Text = "  " + btnText,
                    Location = new Point(0, yPos),
                    Width = 220,
                    Height = 50,
                    FlatStyle = FlatStyle.Flat,
                    ForeColor = Color.WhiteSmoke,
                    Font = new Font("Segoe UI", 11, FontStyle.Regular),
                    TextAlign = ContentAlignment.MiddleLeft,
                    Cursor = Cursors.Hand,
                    BackColor = Color.FromArgb(44, 62, 80)
                };
                btn.FlatAppearance.BorderSize = 0;
                btn.FlatAppearance.MouseOverBackColor = Color.FromArgb(26, 188, 156);
                btn.FlatAppearance.MouseDownBackColor = Color.FromArgb(22, 160, 133);
                btn.Click += MenuButton_Click;
                panelMenu.Controls.Add(btn);
                yPos += 55;
            }

            pnlContent = new Panel { Dock = DockStyle.Fill, BackColor = Color.FromArgb(236, 240, 241), Padding = new Padding(20) };
            
            var lblWelcome = new Label 
            { 
                Text = "Dashboard Overview", 
                Font = new Font("Segoe UI", 24, FontStyle.Bold), 
                ForeColor = Color.FromArgb(44, 62, 80),
                Location = new Point(30, 30), 
                AutoSize = true 
            };
            
            var lblSub = new Label 
            { 
                Text = "Welcome to the Student Course Registration System.", 
                Font = new Font("Segoe UI", 12), 
                ForeColor = Color.Gray,
                Location = new Point(35, 75), 
                AutoSize = true 
            };

            pnlContent.Controls.Add(lblWelcome);
            pnlContent.Controls.Add(lblSub);

            this.Controls.Add(pnlContent);
            this.Controls.Add(panelMenu);
        }

        private void MenuButton_Click(object sender, EventArgs e)
        {
            var btn = sender as Button;
            Form frm = null;

            switch (btn.Text.Trim())
            {
                case "Students": frm = new StudentForm(); break;
                case "Courses": frm = new CourseForm(); break;
                case "Enrollments": frm = new EnrollmentForm(); break;
                case "Reports": frm = new ReportForm(); break;
                case "Logout": this.Close(); return;
            }

            if (frm != null)
            {
                frm.ShowDialog();
            }
        }
    }
}
