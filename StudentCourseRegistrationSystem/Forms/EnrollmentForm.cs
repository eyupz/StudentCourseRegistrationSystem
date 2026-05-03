using System;
using System.Drawing;
using System.Windows.Forms;

namespace StudentCourseRegistrationSystem.Forms
{
    public partial class EnrollmentForm : Form
    {
        private ComboBox cmbStudent, cmbCourse, cmbSemester;
        private Button btnEnroll, btnDrop;
        private DataGridView dgvEnrollments;

        public EnrollmentForm()
        {
            InitializeComponent();
            LoadData();
        }

        private void InitializeComponent()
        {
            this.Text = "Manage Enrollments";
            this.Size = new Size(850, 600);
            this.StartPosition = FormStartPosition.CenterParent;
            this.BackColor = Color.FromArgb(236, 240, 241);

            var pnlInputs = new Panel { Dock = DockStyle.Top, Height = 180, BackColor = Color.White };
            
            var lblTitle = new Label { Text = "Enrollment Details", Font = new Font("Segoe UI Semibold", 16), ForeColor = Color.FromArgb(44, 62, 80), Location = new Point(20, 20), AutoSize = true };
            pnlInputs.Controls.Add(lblTitle);

            Font labelFont = new Font("Segoe UI", 10);
            Font textFont = new Font("Segoe UI", 11);

            pnlInputs.Controls.Add(new Label { Text = "Student", Font = labelFont, ForeColor = Color.Gray, Location = new Point(25, 70), AutoSize = true });
            cmbStudent = new ComboBox { Location = new Point(25, 95), Width = 200, Font = textFont, DropDownStyle = ComboBoxStyle.DropDownList };
            pnlInputs.Controls.Add(cmbStudent);

            pnlInputs.Controls.Add(new Label { Text = "Course", Font = labelFont, ForeColor = Color.Gray, Location = new Point(245, 70), AutoSize = true });
            cmbCourse = new ComboBox { Location = new Point(245, 95), Width = 200, Font = textFont, DropDownStyle = ComboBoxStyle.DropDownList };
            pnlInputs.Controls.Add(cmbCourse);

            pnlInputs.Controls.Add(new Label { Text = "Semester", Font = labelFont, ForeColor = Color.Gray, Location = new Point(465, 70), AutoSize = true });
            cmbSemester = new ComboBox { Location = new Point(465, 95), Width = 150, Font = textFont, DropDownStyle = ComboBoxStyle.DropDownList };
            pnlInputs.Controls.Add(cmbSemester);

            btnEnroll = CreateButton("Enroll", new Point(25, 140), Color.FromArgb(26, 188, 156));
            btnDrop = CreateButton("Drop", new Point(135, 140), Color.FromArgb(231, 76, 60));

            pnlInputs.Controls.Add(btnEnroll);
            pnlInputs.Controls.Add(btnDrop);

            var pnlGrid = new Panel { Dock = DockStyle.Fill, Padding = new Padding(20) };
            dgvEnrollments = new DataGridView
            {
                Dock = DockStyle.Fill,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                AllowUserToAddRows = false,
                ReadOnly = true
            };
            StyleDataGrid(dgvEnrollments);
            pnlGrid.Controls.Add(dgvEnrollments);

            this.Controls.Add(pnlGrid);
            this.Controls.Add(pnlInputs);
        }

        private Button CreateButton(string text, Point location, Color backColor)
        {
            var btn = new Button
            {
                Text = text,
                Location = location,
                Width = 100,
                Height = 30,
                BackColor = backColor,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btn.FlatAppearance.BorderSize = 0;
            return btn;
        }

        private void StyleDataGrid(DataGridView dgv)
        {
            dgv.BackgroundColor = Color.White;
            dgv.BorderStyle = BorderStyle.None;
            dgv.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            dgv.DefaultCellStyle.SelectionBackColor = Color.FromArgb(26, 188, 156);
            dgv.DefaultCellStyle.SelectionForeColor = Color.White;
            dgv.DefaultCellStyle.BackColor = Color.White;
            dgv.DefaultCellStyle.Font = new Font("Segoe UI", 10);
            dgv.RowHeadersVisible = false;
            dgv.EnableHeadersVisualStyles = false;
            dgv.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
            dgv.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(44, 62, 80);
            dgv.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgv.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI Semibold", 10);
            dgv.ColumnHeadersHeight = 40;
            dgv.RowTemplate.Height = 35;
        }

        private void LoadData()
        {
            cmbStudent.Items.Add("John Doe");
            cmbCourse.Items.Add("C# Programming");
            cmbSemester.Items.Add("Fall 2026");

            dgvEnrollments.ColumnCount = 3;
            dgvEnrollments.Columns[0].Name = "Student";
            dgvEnrollments.Columns[1].Name = "Course";
            dgvEnrollments.Columns[2].Name = "Semester";
        }
    }
}
