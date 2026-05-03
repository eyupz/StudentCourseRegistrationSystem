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
            this.Text = "Enrollments";
            this.Size = new Size(800, 500);
            this.StartPosition = FormStartPosition.CenterParent;

            var splitContainer = new SplitContainer
            {
                Dock = DockStyle.Fill,
                Orientation = Orientation.Horizontal,
                SplitterDistance = 150
            };

            var pnlInputs = new Panel { Dock = DockStyle.Fill, BackColor = Color.WhiteSmoke };

            pnlInputs.Controls.Add(new Label { Text = "Student:", Location = new Point(20, 20), AutoSize = true });
            cmbStudent = new ComboBox { Location = new Point(100, 18), Width = 150, DropDownStyle = ComboBoxStyle.DropDownList };
            pnlInputs.Controls.Add(cmbStudent);

            pnlInputs.Controls.Add(new Label { Text = "Course:", Location = new Point(270, 20), AutoSize = true });
            cmbCourse = new ComboBox { Location = new Point(330, 18), Width = 150, DropDownStyle = ComboBoxStyle.DropDownList };
            pnlInputs.Controls.Add(cmbCourse);

            pnlInputs.Controls.Add(new Label { Text = "Semester:", Location = new Point(500, 20), AutoSize = true });
            cmbSemester = new ComboBox { Location = new Point(570, 18), Width = 120, DropDownStyle = ComboBoxStyle.DropDownList };
            pnlInputs.Controls.Add(cmbSemester);

            btnEnroll = new Button { Text = "Enroll", Location = new Point(100, 70), Width = 100, BackColor = Color.LightGreen, FlatStyle = FlatStyle.Flat };
            btnDrop = new Button { Text = "Drop", Location = new Point(220, 70), Width = 100, BackColor = Color.LightCoral, FlatStyle = FlatStyle.Flat };

            pnlInputs.Controls.Add(btnEnroll);
            pnlInputs.Controls.Add(btnDrop);

            splitContainer.Panel1.Controls.Add(pnlInputs);

            dgvEnrollments = new DataGridView
            {
                Dock = DockStyle.Fill,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                AllowUserToAddRows = false,
                ReadOnly = true,
                BackgroundColor = Color.White
            };
            splitContainer.Panel2.Controls.Add(dgvEnrollments);

            this.Controls.Add(splitContainer);
        }

        private void LoadData()
        {
            // TODO: Load Comboboxes and Grid from EnrollmentService
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
