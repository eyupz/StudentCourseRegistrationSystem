using System;
using System.Drawing;
using System.Windows.Forms;

namespace StudentCourseRegistrationSystem.Forms
{
    public partial class CourseForm : Form
    {
        private DataGridView dgvCourses;
        private TextBox txtCourseName, txtCredits;
        private Button btnAdd, btnUpdate, btnDelete;

        public CourseForm()
        {
            InitializeComponent();
            LoadCourses();
        }

        private void InitializeComponent()
        {
            this.Text = "Manage Courses";
            this.Size = new Size(850, 600);
            this.StartPosition = FormStartPosition.CenterParent;
            this.BackColor = Color.FromArgb(236, 240, 241);

            var pnlInputs = new Panel { Dock = DockStyle.Top, Height = 180, BackColor = Color.White };
            
            var lblTitle = new Label { Text = "Course Information", Font = new Font("Segoe UI Semibold", 16), ForeColor = Color.FromArgb(44, 62, 80), Location = new Point(20, 20), AutoSize = true };
            pnlInputs.Controls.Add(lblTitle);

            Font labelFont = new Font("Segoe UI", 10);
            Font textFont = new Font("Segoe UI", 11);

            pnlInputs.Controls.Add(new Label { Text = "Course Name", Font = labelFont, ForeColor = Color.Gray, Location = new Point(25, 70), AutoSize = true });
            txtCourseName = new TextBox { Location = new Point(25, 95), Width = 300, Font = textFont };
            pnlInputs.Controls.Add(txtCourseName);

            pnlInputs.Controls.Add(new Label { Text = "Credits", Font = labelFont, ForeColor = Color.Gray, Location = new Point(345, 70), AutoSize = true });
            txtCredits = new TextBox { Location = new Point(345, 95), Width = 150, Font = textFont };
            pnlInputs.Controls.Add(txtCredits);

            btnAdd = CreateButton("Add", new Point(25, 140), Color.FromArgb(26, 188, 156));
            btnUpdate = CreateButton("Update", new Point(135, 140), Color.FromArgb(52, 152, 219));
            btnDelete = CreateButton("Delete", new Point(245, 140), Color.FromArgb(231, 76, 60));

            pnlInputs.Controls.Add(btnAdd);
            pnlInputs.Controls.Add(btnUpdate);
            pnlInputs.Controls.Add(btnDelete);

            var pnlGrid = new Panel { Dock = DockStyle.Fill, Padding = new Padding(20) };
            dgvCourses = new DataGridView
            {
                Dock = DockStyle.Fill,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                AllowUserToAddRows = false,
                ReadOnly = true
            };
            StyleDataGrid(dgvCourses);
            pnlGrid.Controls.Add(dgvCourses);

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

        private void LoadCourses()
        {
            // TODO: Use CourseService to load courses
            dgvCourses.ColumnCount = 3;
            dgvCourses.Columns[0].Name = "ID";
            dgvCourses.Columns[1].Name = "Course Name";
            dgvCourses.Columns[2].Name = "Credits";
            dgvCourses.Rows.Add("101", "C# Programming", "3");
        }
    }
}
