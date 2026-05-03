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
            this.Size = new Size(800, 500);
            this.StartPosition = FormStartPosition.CenterParent;

            var pnlTop = new Panel { Dock = DockStyle.Top, Height = 60, BackColor = Color.WhiteSmoke };
            
            pnlTop.Controls.Add(new Label { Text = "Report Type:", Location = new Point(20, 20), AutoSize = true });
            
            cmbReportType = new ComboBox { Location = new Point(100, 18), Width = 250, DropDownStyle = ComboBoxStyle.DropDownList };
            cmbReportType.Items.AddRange(new string[] { "All Students", "All Courses", "Enrollments by Course" });
            cmbReportType.SelectedIndex = 0;
            pnlTop.Controls.Add(cmbReportType);

            btnGenerate = new Button { Text = "Generate", Location = new Point(370, 17), Width = 100, BackColor = Color.LightSkyBlue, FlatStyle = FlatStyle.Flat };
            btnGenerate.Click += BtnGenerate_Click;
            pnlTop.Controls.Add(btnGenerate);

            this.Controls.Add(pnlTop);

            dgvReports = new DataGridView
            {
                Dock = DockStyle.Fill,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                AllowUserToAddRows = false,
                ReadOnly = true,
                BackgroundColor = Color.White
            };
            this.Controls.Add(dgvReports);
            dgvReports.BringToFront(); // Ensure it fills remaining space below top panel
        }

        private void BtnGenerate_Click(object sender, EventArgs e)
        {
            // TODO: Use ReportService to fetch data
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
