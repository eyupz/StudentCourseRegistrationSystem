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
            this.Size = new Size(850, 600);
            this.StartPosition = FormStartPosition.CenterParent;
            this.BackColor = Color.FromArgb(236, 240, 241);

            var pnlInputs = new Panel { Dock = DockStyle.Top, Height = 180, BackColor = Color.White };
            
            var lblTitle = new Label { Text = "Student Information", Font = new Font("Segoe UI Semibold", 16), ForeColor = Color.FromArgb(44, 62, 80), Location = new Point(20, 20), AutoSize = true };
            pnlInputs.Controls.Add(lblTitle);

            Font labelFont = new Font("Segoe UI", 10);
            Font textFont = new Font("Segoe UI", 11);

            pnlInputs.Controls.Add(new Label { Text = "First Name", Font = labelFont, ForeColor = Color.Gray, Location = new Point(25, 70), AutoSize = true });
            txtFirstName = new TextBox { Location = new Point(25, 95), Width = 200, Font = textFont };
            pnlInputs.Controls.Add(txtFirstName);

            pnlInputs.Controls.Add(new Label { Text = "Last Name", Font = labelFont, ForeColor = Color.Gray, Location = new Point(245, 70), AutoSize = true });
            txtLastName = new TextBox { Location = new Point(245, 95), Width = 200, Font = textFont };
            pnlInputs.Controls.Add(txtLastName);

            pnlInputs.Controls.Add(new Label { Text = "Email", Font = labelFont, ForeColor = Color.Gray, Location = new Point(465, 70), AutoSize = true });
            txtEmail = new TextBox { Location = new Point(465, 95), Width = 250, Font = textFont };
            pnlInputs.Controls.Add(txtEmail);

            btnAdd = CreateButton("Add", new Point(25, 140), Color.FromArgb(26, 188, 156));
            btnUpdate = CreateButton("Update", new Point(135, 140), Color.FromArgb(52, 152, 219));
            btnDelete = CreateButton("Delete", new Point(245, 140), Color.FromArgb(231, 76, 60));

            btnAdd.Click += BtnAdd_Click;
            btnUpdate.Click += BtnUpdate_Click;
            btnDelete.Click += BtnDelete_Click;

            pnlInputs.Controls.Add(btnAdd);
            pnlInputs.Controls.Add(btnUpdate);
            pnlInputs.Controls.Add(btnDelete);

            var pnlGrid = new Panel { Dock = DockStyle.Fill, Padding = new Padding(20) };
            dgvStudents = new DataGridView
            {
                Dock = DockStyle.Fill,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                AllowUserToAddRows = false,
                ReadOnly = true
            };
            StyleDataGrid(dgvStudents);
            pnlGrid.Controls.Add(dgvStudents);

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
