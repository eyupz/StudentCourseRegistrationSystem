using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using StudentCourseRegistrationSystem.Services;

namespace StudentCourseRegistrationSystem.Forms
{
    public class StudentPanelForm : Form
    {
        private readonly EnrollmentService _enrollmentService;
        private readonly CourseService _courseService;
        private readonly SemesterService _semesterService;

        private TabControl tabControl;
        
        // Tab 1: Ders Seçimi
        private DataGridView gridAvailableCourses;
        private DataGridView gridMyCourses;
        private ComboBox cmbSemester;
        private Button btnEnroll, btnDrop;
        private TextBox txtSearchCourses;

        // Tab 2: Transkript
        private DataGridView gridTranscript;
        private Label lblGNO, lblAGNO;
        private ComboBox cmbTranscriptSemester;

        private int _studentId;

        public StudentPanelForm(EnrollmentService enrollmentService, CourseService courseService, SemesterService semesterService)
        {
            _enrollmentService = enrollmentService;
            _courseService = courseService;
            _semesterService = semesterService;
            
            _studentId = Helpers.SessionManager.CurrentUser?.StudentId ?? 0;

            InitializeComponent();
            LoadDropdowns();
            LoadAvailableCourses();
            LoadMyCourses();
            LoadTranscript();
        }

        private void InitializeComponent()
        {
            this.Text = "Öğrenci İşlemleri";
            this.Size = new Size(1000, 700);
            this.StartPosition = FormStartPosition.CenterParent;
            this.BackColor = Color.FromArgb(244, 246, 249);
            this.Font = new Font("Segoe UI", 10F);

            tabControl = new TabControl() { Dock = DockStyle.Fill, ItemSize = new Size(150, 40) };

            // Tab 1: Ders Kayıt
            TabPage tabEnrollment = new TabPage("Ders Seçimi") { BackColor = Color.White, Padding = new Padding(15) };
            
            Panel pnlEnrollTop = new Panel() { Dock = DockStyle.Top, Height = 60 };
            Label lblSem = new Label() { Text = "Dönem:", Location = new Point(0, 15), AutoSize = true, ForeColor = Color.Gray };
            cmbSemester = new ComboBox() { Location = new Point(60, 12), Width = 150, DisplayMember = "Name", ValueMember = "Id", DropDownStyle = ComboBoxStyle.DropDownList };
            cmbSemester.SelectedIndexChanged += (s, e) => { LoadAvailableCourses(); LoadMyCourses(); };
            
            txtSearchCourses = new TextBox() { Location = new Point(230, 12), Width = 200 };
            Button btnSearch = new Button() { Text = "Ara", Location = new Point(440, 10), Width = 80, Height = 30, BackColor = Color.FromArgb(67, 97, 238), ForeColor = Color.White, FlatStyle = FlatStyle.Flat };
            btnSearch.FlatAppearance.BorderSize = 0;
            btnSearch.Click += (s, e) => LoadAvailableCourses(txtSearchCourses.Text);

            pnlEnrollTop.Controls.Add(lblSem); pnlEnrollTop.Controls.Add(cmbSemester);
            pnlEnrollTop.Controls.Add(txtSearchCourses); pnlEnrollTop.Controls.Add(btnSearch);

            SplitContainer splitContainer = new SplitContainer() { Dock = DockStyle.Fill, Orientation = Orientation.Horizontal, SplitterDistance = 250 };
            
            // Available Courses
            GroupBox gbAvailable = new GroupBox() { Text = "Açılan Dersler", Dock = DockStyle.Fill };
            gridAvailableCourses = CreateGrid();
            btnEnroll = new Button() { Text = "Seçili Dersi Ekle", Dock = DockStyle.Bottom, Height = 40, BackColor = Color.FromArgb(46, 204, 113), ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI", 10F, FontStyle.Bold) };
            btnEnroll.FlatAppearance.BorderSize = 0;
            btnEnroll.Click += BtnEnroll_Click;
            gbAvailable.Controls.Add(gridAvailableCourses);
            gbAvailable.Controls.Add(btnEnroll);

            // My Courses
            GroupBox gbMyCourses = new GroupBox() { Text = "Seçtiğim Dersler", Dock = DockStyle.Fill };
            gridMyCourses = CreateGrid();
            btnDrop = new Button() { Text = "Seçili Dersi Çıkar", Dock = DockStyle.Bottom, Height = 40, BackColor = Color.FromArgb(231, 76, 60), ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI", 10F, FontStyle.Bold) };
            btnDrop.FlatAppearance.BorderSize = 0;
            btnDrop.Click += BtnDrop_Click;
            gbMyCourses.Controls.Add(gridMyCourses);
            gbMyCourses.Controls.Add(btnDrop);

            splitContainer.Panel1.Controls.Add(gbAvailable);
            splitContainer.Panel2.Controls.Add(gbMyCourses);

            tabEnrollment.Controls.Add(splitContainer);
            tabEnrollment.Controls.Add(pnlEnrollTop);

            // Tab 2: Transkript
            TabPage tabTranscript = new TabPage("Notlarım ve Transkript") { BackColor = Color.White, Padding = new Padding(15) };

            Panel pnlTransTop = new Panel() { Dock = DockStyle.Top, Height = 60 };
            Label lblTSem = new Label() { Text = "Dönem Seç:", Location = new Point(0, 15), AutoSize = true, ForeColor = Color.Gray };
            cmbTranscriptSemester = new ComboBox() { Location = new Point(80, 12), Width = 150, DisplayMember = "Name", ValueMember = "Id", DropDownStyle = ComboBoxStyle.DropDownList };
            cmbTranscriptSemester.SelectedIndexChanged += (s, e) => LoadTranscript();
            pnlTransTop.Controls.Add(lblTSem); pnlTransTop.Controls.Add(cmbTranscriptSemester);

            gridTranscript = CreateGrid();

            Panel pnlTransBottom = new Panel() { Dock = DockStyle.Bottom, Height = 60, Padding = new Padding(10), BackColor = Color.FromArgb(240, 242, 245) };
            lblGNO = new Label() { Text = "Dönem Ortalaması (GNO): 0.00", Location = new Point(20, 20), AutoSize = true, Font = new Font("Segoe UI", 12F, FontStyle.Bold) };
            lblAGNO = new Label() { Text = "Genel Ortalama (AGNO): 0.00", Location = new Point(400, 20), AutoSize = true, Font = new Font("Segoe UI", 12F, FontStyle.Bold), ForeColor = Color.FromArgb(67, 97, 238) };
            pnlTransBottom.Controls.Add(lblGNO); pnlTransBottom.Controls.Add(lblAGNO);

            tabTranscript.Controls.Add(gridTranscript);
            tabTranscript.Controls.Add(pnlTransTop);
            tabTranscript.Controls.Add(pnlTransBottom);

            tabControl.TabPages.Add(tabEnrollment);
            tabControl.TabPages.Add(tabTranscript);

            this.Controls.Add(tabControl);
        }

        private DataGridView CreateGrid()
        {
            var grid = new DataGridView() { 
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
            
            cmbSemester.DataSource = semesters.ToList();
            cmbTranscriptSemester.DataSource = semesters.ToList();
        }

        private void LoadAvailableCourses(string search = "")
        {
            if (cmbSemester.SelectedValue == null) return;
            int semId = (int)cmbSemester.SelectedValue;

            var courses = _courseService.GetAllCourses();
            
            if (!string.IsNullOrWhiteSpace(search))
            {
                var s = search.ToLower();
                courses = courses.Where(c => c.CourseCode.ToLower().Contains(s) || c.Title.ToLower().Contains(s)).ToList();
            }

            var myEnrolledCourseIds = _enrollmentService.GetStudentEnrollments(_studentId)
                .Where(e => e.SemesterId == semId)
                .Select(e => e.CourseId)
                .ToList();

            var available = courses.Where(c => !myEnrolledCourseIds.Contains(c.Id)).ToList();

            gridAvailableCourses.DataSource = available.Select(c => new {
                c.Id,
                Kod = c.CourseCode,
                Ders = c.Title,
                Kredi = c.Credits,
                Kontenjan = _courseService.GetAvailableSpots(c.Id, semId) + " / " + c.Capacity
            }).ToList();
        }

        private void LoadMyCourses()
        {
            if (cmbSemester.SelectedValue == null) return;
            int semId = (int)cmbSemester.SelectedValue;

            var enrollments = _enrollmentService.GetStudentEnrollments(_studentId)
                .Where(e => e.SemesterId == semId).ToList();

            gridMyCourses.DataSource = enrollments.Select(e => new {
                KayıtId = e.Id,
                DersId = e.CourseId,
                Kod = e.Course?.CourseCode,
                Ders = e.Course?.Title,
                Kredi = e.Course?.Credits
            }).ToList();
        }

        private void LoadTranscript()
        {
            if (cmbTranscriptSemester.SelectedValue == null) return;
            int semId = (int)cmbTranscriptSemester.SelectedValue;

            var enrollments = _enrollmentService.GetStudentEnrollments(_studentId)
                .Where(e => e.SemesterId == semId).ToList();

            gridTranscript.DataSource = enrollments.Select(e => new {
                Kod = e.Course?.CourseCode,
                Ders = e.Course?.Title,
                Kredi = e.Course?.Credits,
                Puan = e.Grade?.Score?.ToString() ?? "-",
                HarfNotu = e.Grade?.LetterGrade ?? "-"
            }).ToList();

            double gno = _enrollmentService.CalculateGNO(_studentId, semId);
            double agno = _enrollmentService.CalculateAGNO(_studentId);

            lblGNO.Text = $"Dönem Ortalaması (GNO): {gno:0.00}";
            lblAGNO.Text = $"Genel Ortalama (AGNO): {agno:0.00}";
        }

        private void BtnEnroll_Click(object sender, EventArgs e)
        {
            if (gridAvailableCourses.SelectedRows.Count == 0) return;
            if (cmbSemester.SelectedValue == null) return;

            int courseId = (int)gridAvailableCourses.SelectedRows[0].Cells["Id"].Value;
            int semId = (int)cmbSemester.SelectedValue;

            try
            {
                _enrollmentService.Enroll(_studentId, courseId, semId);
                LoadAvailableCourses(txtSearchCourses.Text);
                LoadMyCourses();
                LoadTranscript();
                MessageBox.Show("Ders eklendi.", "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnDrop_Click(object sender, EventArgs e)
        {
            if (gridMyCourses.SelectedRows.Count == 0) return;

            int enrollmentId = (int)gridMyCourses.SelectedRows[0].Cells["KayıtId"].Value;

            if (MessageBox.Show("Bu dersi bırakmak istediğinize emin misiniz?", "Onay", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                try
                {
                    _enrollmentService.Drop(enrollmentId);
                    LoadAvailableCourses(txtSearchCourses.Text);
                    LoadMyCourses();
                    LoadTranscript();
                    MessageBox.Show("Ders bırakıldı.", "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
}
