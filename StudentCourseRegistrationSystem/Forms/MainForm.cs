using System;
using System.Drawing;
using System.Windows.Forms;

namespace StudentCourseRegistrationSystem.Forms
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.Text = "Student Course Registration System - Main Dashboard";
            this.Size = new Size(800, 600);
            this.StartPosition = FormStartPosition.CenterScreen;

            var panelMenu = new Panel
            {
                Dock = DockStyle.Left,
                Width = 200,
                BackColor = Color.FromArgb(45, 45, 48)
            };

            var lblMenu = new Label
            {
                Text = "Menu",
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                Location = new Point(20, 20),
                AutoSize = true
            };
            panelMenu.Controls.Add(lblMenu);

            string[] buttons = { "Students", "Courses", "Enrollments", "Reports", "Logout" };
            int yPos = 80;

            foreach (var btnText in buttons)
            {
                var btn = new Button
                {
                    Text = btnText,
                    Location = new Point(10, yPos),
                    Width = 180,
                    Height = 40,
                    FlatStyle = FlatStyle.Flat,
                    ForeColor = Color.White,
                    Font = new Font("Segoe UI", 10)
                };
                btn.FlatAppearance.BorderSize = 0;
                btn.Click += MenuButton_Click;
                panelMenu.Controls.Add(btn);
                yPos += 50;
            }

            this.Controls.Add(panelMenu);
            
            var pnlMain = new Panel { Dock = DockStyle.Fill, BackColor = Color.WhiteSmoke };
            pnlMain.Controls.Add(new Label { Text = "Welcome to the Student Course Registration System", Font = new Font("Segoe UI", 16), Location = new Point(50, 50), AutoSize = true });
            this.Controls.Add(pnlMain);
        }

        private void MenuButton_Click(object sender, EventArgs e)
        {
            var btn = sender as Button;
            switch (btn.Text)
            {
                case "Students":
                    new StudentForm().ShowDialog();
                    break;
                case "Courses":
                    new CourseForm().ShowDialog();
                    break;
                case "Enrollments":
                    new EnrollmentForm().ShowDialog();
                    break;
                case "Reports":
                    new ReportForm().ShowDialog();
                    break;
                case "Logout":
                    this.Close();
                    break;
            }
        }
    }
}
