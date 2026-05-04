using System;
using System.Drawing;
using System.Windows.Forms;
using StudentCourseRegistrationSystem.Services;

namespace StudentCourseRegistrationSystem.Forms
{
    public class ReportForm : Form
    {
        private readonly ReportService _reportService;
        private DataGridView gridReport;
        private TextBox txtSearch;
        private string _currentReportType = "";
        private object _currentData;

        public ReportForm(ReportService reportService)
        {
            _reportService = reportService;
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.Text = "Sistem Raporları";
            this.Size = new Size(1000, 650);
            this.StartPosition = FormStartPosition.CenterParent;
            this.BackColor = Color.FromArgb(244, 246, 249);
            this.Font = new Font("Segoe UI", 10F);

            // Top Panel
            Panel pnlTop = new Panel() { Dock = DockStyle.Top, Height = 70, Padding = new Padding(15), BackColor = Color.White };
            
            Button btnStudents = CreateButton("👥 Öğrenci Raporu", 15, 15, 180);
            btnStudents.Click += (s, e) => LoadReport("Students");

            Button btnCourses = CreateButton("📚 Ders Raporu", 205, 15, 180);
            btnCourses.Click += (s, e) => LoadReport("Courses");

            Button btnEnrollments = CreateButton("🎓 Kayıt Raporu", 395, 15, 180);
            btnEnrollments.Click += (s, e) => LoadReport("Enrollments");

            txtSearch = new TextBox() { Location = new Point(600, 20), Width = 200, Font = new Font("Segoe UI", 11F), BorderStyle = BorderStyle.FixedSingle };
            Button btnSearch = CreateButton("🔍 Ara", 810, 15, 80);
            btnSearch.Click += BtnSearch_Click;

            pnlTop.Controls.Add(btnStudents);
            pnlTop.Controls.Add(btnCourses);
            pnlTop.Controls.Add(btnEnrollments);
            pnlTop.Controls.Add(txtSearch);
            pnlTop.Controls.Add(btnSearch);

            // Grid
            gridReport = new DataGridView() { 
                Dock = DockStyle.Fill, 
                ReadOnly = true, 
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.None,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                EnableHeadersVisualStyles = false,
                RowHeadersVisible = false,
                GridColor = Color.FromArgb(230, 230, 230)
            };
            gridReport.ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle() { BackColor = Color.FromArgb(67, 97, 238), ForeColor = Color.White, Font = new Font("Segoe UI", 10F, FontStyle.Bold), Padding = new Padding(5) };
            gridReport.DefaultCellStyle = new DataGridViewCellStyle() { SelectionBackColor = Color.FromArgb(200, 210, 255), SelectionForeColor = Color.Black, Padding = new Padding(5) };
            gridReport.AlternatingRowsDefaultCellStyle = new DataGridViewCellStyle() { BackColor = Color.FromArgb(245, 245, 245) };

            this.Controls.Add(gridReport);
            this.Controls.Add(pnlTop);
        }

        private Button CreateButton(string text, int x, int y, int width)
        {
            Button btn = new Button() { 
                Text = text, Location = new Point(x, y), Width = width, Height = 40,
                FlatStyle = FlatStyle.Flat, ForeColor = Color.White, BackColor = Color.FromArgb(67, 97, 238),
                Font = new Font("Segoe UI", 11F, FontStyle.Bold), Cursor = Cursors.Hand
            };
            btn.FlatAppearance.BorderSize = 0;
            return btn;
        }

        private void LoadReport(string type)
        {
            try
            {
                _currentReportType = type;
                if (type == "Students")
                {
                    _currentData = _reportService.GetStudentReport();
                }
                else if (type == "Courses")
                {
                    _currentData = _reportService.GetCourseReport();
                }
                else if (type == "Enrollments")
                {
                    _currentData = _reportService.GetEnrollmentReport();
                }

                gridReport.DataSource = _currentData;
                txtSearch.Text = "";
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnSearch_Click(object? sender, EventArgs e)
        {
            if (_currentData == null) return;
            string term = txtSearch.Text.ToLower();

            if (string.IsNullOrWhiteSpace(term))
            {
                gridReport.DataSource = _currentData;
                return;
            }

            try
            {
                // Basic string search across all columns using reflection on the anonymous type list
                var list = (System.Collections.IEnumerable)_currentData;
                var filtered = new System.Collections.ArrayList();

                foreach (var item in list)
                {
                    bool match = false;
                    var props = item.GetType().GetProperties();
                    foreach (var prop in props)
                    {
                        var val = prop.GetValue(item)?.ToString()?.ToLower();
                        if (val != null && val.Contains(term))
                        {
                            match = true;
                            break;
                        }
                    }
                    if (match) filtered.Add(item);
                }

                gridReport.DataSource = filtered;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Arama sırasında hata oluştu: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
