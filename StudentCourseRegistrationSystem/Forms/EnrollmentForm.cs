using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Microsoft.EntityFrameworkCore;
using StudentCourseRegistrationSystem.Services;

namespace StudentCourseRegistrationSystem.Forms
{
    public class EnrollmentForm : Form
    {
        private readonly EnrollmentService _enrollmentService;
        private readonly StudentService _studentService;
        private readonly CourseService _courseService;
        private readonly SemesterService _semesterService;

        private DataGridView gridEnrollments;
        private ComboBox cmbStudent, cmbCourse, cmbSemester;
        private TextBox txtEnrollmentId, txtSearch;
        private Button btnEnroll, btnDrop;

        public EnrollmentForm(EnrollmentService enrollmentService, StudentService studentService, CourseService courseService, SemesterService semesterService)
        {
            _enrollmentService = enrollmentService;
            _studentService = studentService;
            _courseService = courseService;
            _semesterService = semesterService;
            
            InitializeComponent();
            LoadDropdowns();
            LoadData();
        }

        private void InitializeComponent()
        {
            this.Text = "Kayıt Yönetimi";
            this.Size = new Size(1000, 650);
            this.StartPosition = FormStartPosition.CenterParent;
            this.BackColor = Color.FromArgb(244, 246, 249);
            this.Font = new Font("Segoe UI", 10F);

            // Search Panel
            Panel pnlSearch = new Panel() { Dock = DockStyle.Top, Height = 60, Padding = new Padding(15), BackColor = Color.White };
            txtSearch = new TextBox() { Width = 250, Location = new Point(15, 18), Font = new Font("Segoe UI", 11F), BorderStyle = BorderStyle.FixedSingle };
            
            Button btnSearch = CreateButton("🔍 Ara", 280, 15, 100);
            btnSearch.Click += (s, e) => LoadData(txtSearch.Text);
            
            Button btnReset = CreateButton("🔄 Temizle", 390, 15, 100);
            btnReset.Click += (s, e) => { txtSearch.Text = ""; LoadData(); };
            
            pnlSearch.Controls.Add(txtSearch);
            pnlSearch.Controls.Add(btnSearch);
            pnlSearch.Controls.Add(btnReset);

            // Grid
            gridEnrollments = new DataGridView() { 
                Dock = DockStyle.Fill, 
                SelectionMode = DataGridViewSelectionMode.FullRowSelect, 
                MultiSelect = false,
                ReadOnly = true,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.None,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                EnableHeadersVisualStyles = false,
                RowHeadersVisible = false,
                GridColor = Color.FromArgb(230, 230, 230)
            };
            gridEnrollments.ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle() { BackColor = Color.FromArgb(67, 97, 238), ForeColor = Color.White, Font = new Font("Segoe UI", 10F, FontStyle.Bold), Padding = new Padding(5) };
            gridEnrollments.DefaultCellStyle = new DataGridViewCellStyle() { SelectionBackColor = Color.FromArgb(200, 210, 255), SelectionForeColor = Color.Black, Padding = new Padding(5) };
            gridEnrollments.AlternatingRowsDefaultCellStyle = new DataGridViewCellStyle() { BackColor = Color.FromArgb(245, 245, 245) };
            gridEnrollments.SelectionChanged += GridEnrollments_SelectionChanged;

            // Input Panel
            Panel pnlInput = new Panel() { Dock = DockStyle.Bottom, Height = 130, Padding = new Padding(15), BackColor = Color.White };
            
            txtEnrollmentId = new TextBox() { Visible = false };

            Label lblStudent = new Label() { Text = "Öğrenci:", Location = new Point(15, 20), AutoSize = true, ForeColor = Color.Gray };
            cmbStudent = new ComboBox() { Location = new Point(80, 18), Width = 220, DisplayMember = "Name", ValueMember = "Id", DropDownStyle = ComboBoxStyle.DropDownList, Font = new Font("Segoe UI", 11F) };

            Label lblCourse = new Label() { Text = "Ders:", Location = new Point(320, 20), AutoSize = true, ForeColor = Color.Gray };
            cmbCourse = new ComboBox() { Location = new Point(380, 18), Width = 220, DisplayMember = "Name", ValueMember = "Id", DropDownStyle = ComboBoxStyle.DropDownList, Font = new Font("Segoe UI", 11F) };

            Label lblSemester = new Label() { Text = "Dönem:", Location = new Point(620, 20), AutoSize = true, ForeColor = Color.Gray };
            cmbSemester = new ComboBox() { Location = new Point(690, 18), Width = 150, DisplayMember = "Name", ValueMember = "Id", DropDownStyle = ComboBoxStyle.DropDownList, Font = new Font("Segoe UI", 11F) };

            btnEnroll = CreateButton("✅ Kayıt Ekle", 15, 70, 150);
            btnEnroll.BackColor = Color.FromArgb(46, 204, 113);
            btnEnroll.Click += BtnEnroll_Click;

            btnDrop = CreateButton("❌ Kayıt Sil", 180, 70, 150);
            btnDrop.BackColor = Color.FromArgb(231, 76, 60);
            btnDrop.Click += BtnDrop_Click;

            pnlInput.Controls.Add(txtEnrollmentId);
            pnlInput.Controls.Add(lblStudent); pnlInput.Controls.Add(cmbStudent);
            pnlInput.Controls.Add(lblCourse); pnlInput.Controls.Add(cmbCourse);
            pnlInput.Controls.Add(lblSemester); pnlInput.Controls.Add(cmbSemester);
            pnlInput.Controls.Add(btnEnroll); pnlInput.Controls.Add(btnDrop);

            this.Controls.Add(gridEnrollments);
            this.Controls.Add(pnlInput);
            this.Controls.Add(pnlSearch);
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

        private void LoadDropdowns()
        {
            var students = _studentService.GetAllStudents()
                .Select(s => new { Id = s.Id, Name = $"{s.StudentNumber} - {s.FirstName} {s.LastName}" })
                .ToList();
            cmbStudent.DataSource = students;

            var courses = _courseService.GetAllCourses()
                .Select(c => new { Id = c.Id, Name = $"{c.CourseCode} - {c.Title}" })
                .ToList();
            cmbCourse.DataSource = courses;

            var semesters = _semesterService.GetAllSemesters()
                .Select(s => new { Id = s.Id, Name = s.Name })
                .ToList();
            cmbSemester.DataSource = semesters;
        }

        private void LoadData(string searchTerm = "")
        {
            var data = _enrollmentService.GetAllEnrollments();

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                var lowerTerm = searchTerm.ToLower();
                data = data.Where(e => 
                    (e.Student?.FirstName?.ToLower().Contains(lowerTerm) ?? false) ||
                    (e.Student?.LastName?.ToLower().Contains(lowerTerm) ?? false) ||
                    (e.Course?.Title?.ToLower().Contains(lowerTerm) ?? false) ||
                    (e.Course?.CourseCode?.ToLower().Contains(lowerTerm) ?? false)
                ).ToList();
            }

            gridEnrollments.DataSource = data.Select(e => new {
                e.Id,
                Ogrenci = e.Student?.StudentNumber + " " + e.Student?.FirstName + " " + e.Student?.LastName,
                Ders = e.Course?.CourseCode + " - " + e.Course?.Title,
                Donem = e.Semester?.Name
            }).ToList();
        }

        private void GridEnrollments_SelectionChanged(object? sender, EventArgs e)
        {
            if (gridEnrollments.SelectedRows.Count > 0)
            {
                var row = gridEnrollments.SelectedRows[0];
                txtEnrollmentId.Text = row.Cells["Id"].Value?.ToString();
            }
        }

        private void BtnEnroll_Click(object? sender, EventArgs e)
        {
            if (cmbStudent.SelectedValue == null || cmbCourse.SelectedValue == null || cmbSemester.SelectedValue == null)
            {
                MessageBox.Show("Lütfen öğrenci, ders ve dönem seçiniz.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int studentId = (int)cmbStudent.SelectedValue;
            int courseId = (int)cmbCourse.SelectedValue;
            int semesterId = (int)cmbSemester.SelectedValue;

            var result = _enrollmentService.Enroll(studentId, courseId, semesterId);
            if (result.Success) { LoadData(); MessageBox.Show(result.Message, "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information); }
            else MessageBox.Show(result.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void BtnDrop_Click(object? sender, EventArgs e)
        {
            if (!int.TryParse(txtEnrollmentId.Text, out int id))
            {
                MessageBox.Show("Lütfen silinecek kaydı seçiniz.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (MessageBox.Show("Bu kaydı silmek istediğinize emin misiniz?", "Onay",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;

            var result = _enrollmentService.Drop(id);
            if (result.Success) { LoadData(); MessageBox.Show(result.Message, "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information); }
            else MessageBox.Show(result.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}
