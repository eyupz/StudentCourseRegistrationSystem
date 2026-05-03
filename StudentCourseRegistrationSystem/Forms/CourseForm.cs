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
            this.Size = new Size(800, 500);
            this.StartPosition = FormStartPosition.CenterParent;

            var splitContainer = new SplitContainer
            {
                Dock = DockStyle.Fill,
                Orientation = Orientation.Horizontal,
                SplitterDistance = 150
            };

            var pnlInputs = new Panel { Dock = DockStyle.Fill, BackColor = Color.WhiteSmoke };

            pnlInputs.Controls.Add(new Label { Text = "Course Name:", Location = new Point(20, 20), AutoSize = true });
            txtCourseName = new TextBox { Location = new Point(120, 18), Width = 200 };
            pnlInputs.Controls.Add(txtCourseName);

            pnlInputs.Controls.Add(new Label { Text = "Credits:", Location = new Point(340, 20), AutoSize = true });
            txtCredits = new TextBox { Location = new Point(400, 18), Width = 100 };
            pnlInputs.Controls.Add(txtCredits);

            btnAdd = new Button { Text = "Add", Location = new Point(120, 70), Width = 80, BackColor = Color.LightGreen, FlatStyle = FlatStyle.Flat };
            btnUpdate = new Button { Text = "Update", Location = new Point(210, 70), Width = 80, BackColor = Color.LightSkyBlue, FlatStyle = FlatStyle.Flat };
            btnDelete = new Button { Text = "Delete", Location = new Point(300, 70), Width = 80, BackColor = Color.LightCoral, FlatStyle = FlatStyle.Flat };

            pnlInputs.Controls.Add(btnAdd);
            pnlInputs.Controls.Add(btnUpdate);
            pnlInputs.Controls.Add(btnDelete);

            splitContainer.Panel1.Controls.Add(pnlInputs);

            dgvCourses = new DataGridView
            {
                Dock = DockStyle.Fill,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                AllowUserToAddRows = false,
                ReadOnly = true,
                BackgroundColor = Color.White
            };
            splitContainer.Panel2.Controls.Add(dgvCourses);

            this.Controls.Add(splitContainer);
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
