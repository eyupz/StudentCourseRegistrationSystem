using System;
using System.Drawing;
using System.Windows.Forms;

namespace StudentCourseRegistrationSystem.Forms
{
    public partial class StudentForm : Form
    {
        private DataGridView dgvStudents;
        private TextBox txtFirstName, txtLastName, txtEmail;
        private Button btnAdd, btnUpdate, btnDelete;

        public StudentForm()
        {
            InitializeComponent();
            LoadStudents();
        }

        private void InitializeComponent()
        {
            this.Text = "Manage Students";
            this.Size = new Size(800, 500);
            this.StartPosition = FormStartPosition.CenterParent;

            // Form Layout
            var splitContainer = new SplitContainer
            {
                Dock = DockStyle.Fill,
                Orientation = Orientation.Horizontal,
                SplitterDistance = 150
            };

            // Top Panel (Inputs)
            var pnlInputs = new Panel { Dock = DockStyle.Fill, BackColor = Color.WhiteSmoke };
            
            pnlInputs.Controls.Add(new Label { Text = "First Name:", Location = new Point(20, 20), AutoSize = true });
            txtFirstName = new TextBox { Location = new Point(100, 18), Width = 150 };
            pnlInputs.Controls.Add(txtFirstName);

            pnlInputs.Controls.Add(new Label { Text = "Last Name:", Location = new Point(270, 20), AutoSize = true });
            txtLastName = new TextBox { Location = new Point(350, 18), Width = 150 };
            pnlInputs.Controls.Add(txtLastName);

            pnlInputs.Controls.Add(new Label { Text = "Email:", Location = new Point(20, 60), AutoSize = true });
            txtEmail = new TextBox { Location = new Point(100, 58), Width = 400 };
            pnlInputs.Controls.Add(txtEmail);

            btnAdd = new Button { Text = "Add", Location = new Point(100, 100), Width = 80, BackColor = Color.LightGreen, FlatStyle = FlatStyle.Flat };
            btnUpdate = new Button { Text = "Update", Location = new Point(190, 100), Width = 80, BackColor = Color.LightSkyBlue, FlatStyle = FlatStyle.Flat };
            btnDelete = new Button { Text = "Delete", Location = new Point(280, 100), Width = 80, BackColor = Color.LightCoral, FlatStyle = FlatStyle.Flat };

            btnAdd.Click += BtnAdd_Click;
            btnUpdate.Click += BtnUpdate_Click;
            btnDelete.Click += BtnDelete_Click;

            pnlInputs.Controls.Add(btnAdd);
            pnlInputs.Controls.Add(btnUpdate);
            pnlInputs.Controls.Add(btnDelete);

            splitContainer.Panel1.Controls.Add(pnlInputs);

            // Bottom Panel (DataGrid)
            dgvStudents = new DataGridView
            {
                Dock = DockStyle.Fill,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                AllowUserToAddRows = false,
                ReadOnly = true,
                BackgroundColor = Color.White
            };
            splitContainer.Panel2.Controls.Add(dgvStudents);

            this.Controls.Add(splitContainer);
        }

        private void LoadStudents()
        {
            // TODO: Use StudentService to load students
            dgvStudents.ColumnCount = 3;
            dgvStudents.Columns[0].Name = "ID";
            dgvStudents.Columns[1].Name = "Name";
            dgvStudents.Columns[2].Name = "Email";
            dgvStudents.Rows.Add("1", "John Doe", "john@example.com");
        }

        private void BtnAdd_Click(object sender, EventArgs e) { /* TODO: Implement */ }
        private void BtnUpdate_Click(object sender, EventArgs e) { /* TODO: Implement */ }
        private void BtnDelete_Click(object sender, EventArgs e) { /* TODO: Implement */ }
    }
}
