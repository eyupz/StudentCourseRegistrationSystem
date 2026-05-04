using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using StudentCourseRegistrationSystem.Services;

namespace StudentCourseRegistrationSystem.Forms
{
    public class InstructorPanelForm : Form
    {
        private readonly EnrollmentService _enrollmentService;
        private readonly CourseService _courseService;
        private readonly SemesterService _semesterService;

        private TabControl tabControl;

        // Sekme 1: Verdiğim Dersler
        private DataGridView gridMyCourses;
        private ComboBox cmbSemester;

        // Sekme 2: Öğrenci Notlandırma
        private ComboBox cmbGradeCourse;
        private DataGridView gridStudents;
        private TextBox txtEnrollmentId, txtScore;
        private Button btnSaveGrade;

        private int _instructorId;

        public InstructorPanelForm(EnrollmentService enrollmentService, CourseService courseService, SemesterService semesterService)
        {
            _enrollmentService = enrollmentService;
            _courseService = courseService;
            _semesterService = semesterService;

            _instructorId = Helpers.SessionManager.CurrentUser?.InstructorId ?? 0;

            InitializeComponent();
            LoadDropdowns();
            LoadMyCourses();
        }

        private void InitializeComponent()
        {
            this.Text = "Öğretmen İşlemleri";
            this.Size = new Size(1000, 700);
            this.StartPosition = FormStartPosition.CenterParent;
            this.BackColor = Color.FromArgb(244, 246, 249);
            this.Font = new Font("Segoe UI", 10F);

            tabControl = new TabControl() { Dock = DockStyle.Fill, ItemSize = new Size(150, 40) };

            // Sekme 1: Verdiğim Dersler
            TabPage tabCourses = new TabPage("Verdiğim Dersler") { BackColor = Color.White, Padding = new Padding(15) };

            Panel pnlCoursesTop = new Panel() { Dock = DockStyle.Top, Height = 60 };
            Label lblSem = new Label() { Text = "Dönem:", Location = new Point(0, 15), AutoSize = true, ForeColor = Color.Gray };
            cmbSemester = new ComboBox() { Location = new Point(60, 12), Width = 150, DisplayMember = "Name", ValueMember = "Id", DropDownStyle = ComboBoxStyle.DropDownList };
            cmbSemester.SelectedIndexChanged += (s, e) => { LoadMyCourses(); };

            pnlCoursesTop.Controls.Add(lblSem);
            pnlCoursesTop.Controls.Add(cmbSemester);

            gridMyCourses = CreateGrid();
            tabCourses.Controls.Add(gridMyCourses);
            tabCourses.Controls.Add(pnlCoursesTop);

            // Sekme 2: Öğrenci Notlandırma
            TabPage tabGrading = new TabPage("Öğrenci Notlandırma") { BackColor = Color.White, Padding = new Padding(15) };

            Panel pnlGradingTop = new Panel() { Dock = DockStyle.Top, Height = 60 };
            Label lblGCourse = new Label() { Text = "Ders Seç:", Location = new Point(0, 15), AutoSize = true, ForeColor = Color.Gray };
            cmbGradeCourse = new ComboBox() { Location = new Point(80, 12), Width = 250, DisplayMember = "Name", ValueMember = "Id", DropDownStyle = ComboBoxStyle.DropDownList };
            cmbGradeCourse.SelectedIndexChanged += (s, e) => LoadStudentsForGrading();
            pnlGradingTop.Controls.Add(lblGCourse);
            pnlGradingTop.Controls.Add(cmbGradeCourse);

            gridStudents = CreateGrid();
            gridStudents.SelectionChanged += GridStudents_SelectionChanged;

            Panel pnlGradingBottom = new Panel() { Dock = DockStyle.Bottom, Height = 80, Padding = new Padding(10), BackColor = Color.FromArgb(240, 242, 245) };
            txtEnrollmentId = new TextBox() { Visible = false };

            Label lblScore = new Label() { Text = "Sayısal Not (0-100):", Location = new Point(20, 25), AutoSize = true, Font = new Font("Segoe UI", 11F, FontStyle.Bold) };
            txtScore = new TextBox() { Location = new Point(200, 23), Width = 80, Font = new Font("Segoe UI", 12F) };

            btnSaveGrade = new Button()
            {
                Text = "Notu Kaydet",
                Location = new Point(300, 20),
                Width = 150,
                Height = 35,
                BackColor = Color.FromArgb(46, 204, 113),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10F, FontStyle.Bold)
            };
            btnSaveGrade.FlatAppearance.BorderSize = 0;
            btnSaveGrade.Click += BtnSaveGrade_Click;

            pnlGradingBottom.Controls.Add(txtEnrollmentId);
            pnlGradingBottom.Controls.Add(lblScore);
            pnlGradingBottom.Controls.Add(txtScore);
            pnlGradingBottom.Controls.Add(btnSaveGrade);

            tabGrading.Controls.Add(gridStudents);
            tabGrading.Controls.Add(pnlGradingTop);
            tabGrading.Controls.Add(pnlGradingBottom);

            tabControl.TabPages.Add(tabCourses);
            tabControl.TabPages.Add(tabGrading);

            this.Controls.Add(tabControl);
        }

        private DataGridView CreateGrid()
        {
            var grid = new DataGridView()
            {
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
            grid.ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle() { BackColor = Color.FromArgb(67, 97, 238), ForeColor = Color.White, Font = new Font("Segoe UI", 10F, FontStyle.Bold), Padding = new Padding(5) };
            grid.DefaultCellStyle = new DataGridViewCellStyle() { SelectionBackColor = Color.FromArgb(200, 210, 255), SelectionForeColor = Color.Black, Padding = new Padding(5) };
            grid.AlternatingRowsDefaultCellStyle = new DataGridViewCellStyle() { BackColor = Color.FromArgb(245, 245, 245) };
            return grid;
        }

        private void LoadDropdowns()
        {
            var semesters = _semesterService.GetAllSemesters()
                .Select(s => new { Id = s.Id, Name = s.Name })
                .ToList();
            cmbSemester.DataSource = semesters;

            // Öğretmene atanmış dersleri yükle
            var courses = _courseService.GetAllCourses()
                .Where(c => c.InstructorId == _instructorId)
                .Select(c => new { Id = c.Id, Name = $"{c.CourseCode} - {c.Title}" })
                .ToList();
            cmbGradeCourse.DataSource = courses;
        }

        private void LoadMyCourses()
        {
            if (cmbSemester.SelectedValue == null) return;
            int semId = (int)cmbSemester.SelectedValue;

            var courses = _courseService.GetAllCourses()
                .Where(c => c.InstructorId == _instructorId).ToList();

            var courseStats = courses.Select(c => new {
                Kod = c.CourseCode,
                Ders = c.Title,
                Kredi = c.Credits,
                KayıtlıÖğrenci = c.Enrollments.Count(e => e.SemesterId == semId),
                Kontenjan = c.Capacity
            }).ToList();

            gridMyCourses.DataSource = courseStats;
        }

        private void LoadStudentsForGrading()
        {
            if (cmbGradeCourse.SelectedValue == null) return;
            if (cmbSemester.SelectedValue == null) return;

            int semId = (int)cmbSemester.SelectedValue;
            int courseId = (int)cmbGradeCourse.SelectedValue;

            var enrollments = _enrollmentService.GetCourseEnrollments(courseId, semId);

            gridStudents.DataSource = enrollments.Select(e => new {
                KayıtId = e.Id,
                Numara = e.Student != null ? e.Student.StudentNumber : "-",
                Öğrenci = e.Student != null ? e.Student.FirstName + " " + e.Student.LastName : "-",
                Puan = e.Grade != null && e.Grade.Score.HasValue ? e.Grade.Score.Value.ToString("F1") : "-",
                HarfNotu = e.Grade != null && !string.IsNullOrEmpty(e.Grade.LetterGrade) ? e.Grade.LetterGrade : "-"
            }).ToList();

            if (gridStudents.Columns.Contains("KayıtId"))
                gridStudents.Columns["KayıtId"].Visible = false;

            txtEnrollmentId.Text = "";
            txtScore.Text = "";
        }

        private void GridStudents_SelectionChanged(object sender, EventArgs e)
        {
            if (gridStudents.SelectedRows.Count > 0)
            {
                var row = gridStudents.SelectedRows[0];
                txtEnrollmentId.Text = row.Cells["KayıtId"].Value?.ToString();
                string puan = row.Cells["Puan"].Value?.ToString() ?? "";
                txtScore.Text = puan == "-" ? "" : puan;
            }
        }

        private void BtnSaveGrade_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtEnrollmentId.Text))
            {
                MessageBox.Show("Lütfen listeden bir öğrenci seçiniz.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!int.TryParse(txtEnrollmentId.Text, out int enrollmentId))
            {
                MessageBox.Show("Geçersiz kayıt kimliği.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            string scoreText = txtScore.Text.Trim().Replace(',', '.');
            if (!double.TryParse(scoreText, System.Globalization.NumberStyles.Any,
                System.Globalization.CultureInfo.InvariantCulture, out double score))
            {
                MessageBox.Show("Lütfen geçerli bir sayısal not giriniz (0-100).", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var result = _enrollmentService.SaveGrade(enrollmentId, score);
            if (result.Success)
            {
                LoadStudentsForGrading();
                MessageBox.Show(result.Message, "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show(result.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
