using System;
using System.Drawing;
using System.Windows.Forms;

namespace StudentCourseRegistrationSystem.Forms
{
    public partial class ReportForm : Form
    {
        private DataGridView dgvReports;
        private ComboBox cmbReportType;
        private Button btnGenerate;

        public ReportForm()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.Text = "System Reports";
            this.Size = new Size(850, 600);
            this.StartPosition = FormStartPosition.CenterParent;
            this.BackColor = Color.FromArgb(236, 240, 241);

            var pnlTop = new Panel { Dock = DockStyle.Top, Height = 100, BackColor = Color.White };
            
            var lblTitle = new Label { Text = "Generate Reports", Font = new Font("Segoe UI Semibold", 16), ForeColor = Color.FromArgb(44, 62, 80), Location = new Point(20, 20), AutoSize = true };
            pnlTop.Controls.Add(lblTitle);

            Font labelFont = new Font("Segoe UI", 10);
            Font textFont = new Font("Segoe UI", 11);

            pnlTop.Controls.Add(new Label { Text = "Report Type", Font = labelFont, ForeColor = Color.Gray, Location = new Point(25, 60), AutoSize = true });
            
            cmbReportType = new ComboBox { Location = new Point(120, 58), Width = 250, Font = textFont, DropDownStyle = ComboBoxStyle.DropDownList };
            cmbReportType.Items.AddRange(new string[] { "All Students", "All Courses", "Enrollments by Course" });
            cmbReportType.SelectedIndex = 0;
            pnlTop.Controls.Add(cmbReportType);

            btnGenerate = new Button
            {
                Text = "Generate",
                Location = new Point(390, 57),
                Width = 100,
                Height = 30,
                BackColor = Color.FromArgb(52, 152, 219),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnGenerate.FlatAppearance.BorderSize = 0;
            btnGenerate.Click += BtnGenerate_Click;
            pnlTop.Controls.Add(btnGenerate);

            var pnlGrid = new Panel { Dock = DockStyle.Fill, Padding = new Padding(20) };
            dgvReports = new DataGridView
            {
                Dock = DockStyle.Fill,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                AllowUserToAddRows = false,
                ReadOnly = true
            };
            StyleDataGrid(dgvReports);
            pnlGrid.Controls.Add(dgvReports);

            this.Controls.Add(pnlGrid);
            this.Controls.Add(pnlTop);
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

        private void BtnGenerate_Click(object sender, EventArgs e)
        {
            dgvReports.Rows.Clear();
            dgvReports.Columns.Clear();
            
            if (cmbReportType.Text == "All Students")
            {
                dgvReports.Columns.Add("ID", "ID");
                dgvReports.Columns.Add("Name", "Name");
                dgvReports.Rows.Add("1", "John Doe");
            }
            else if (cmbReportType.Text == "All Courses")
            {
                dgvReports.Columns.Add("ID", "ID");
                dgvReports.Columns.Add("CourseName", "Course Name");
                dgvReports.Rows.Add("101", "C# Programming");
            }
            else if (cmbReportType.Text == "Enrollments by Course")
            {
                dgvReports.Columns.Add("StudentName", "Student Name");
                dgvReports.Columns.Add("CourseName", "Course Name");
                dgvReports.Rows.Add("John Doe", "C# Programming");
            }
        }
    }
}
