using System;
using System.Drawing;
using System.Windows.Forms;
using StudentCourseRegistrationSystem.Services;

namespace StudentCourseRegistrationSystem.Forms
{
    public class MainForm : Form
    {
        private readonly AuthService _authService;
        private readonly StudentService _studentService;
        private readonly CourseService _courseService;
        private readonly EnrollmentService _enrollmentService;
        private readonly ReportService _reportService;
        private readonly InstructorService _instructorService;
        private readonly SemesterService _semesterService;

        public MainForm(AuthService authService, StudentService studentService, CourseService courseService, EnrollmentService enrollmentService, ReportService reportService, InstructorService instructorService, SemesterService semesterService)
        {
            _authService = authService;
            _studentService = studentService;
            _courseService = courseService;
            _enrollmentService = enrollmentService;
            _reportService = reportService;
            _instructorService = instructorService;
            _semesterService = semesterService;

            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.Text = "Öğrenci Kayıt Sistemi - Panel";
            this.Size = new Size(900, 550);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.FromArgb(244, 246, 249);
            this.Font = new Font("Segoe UI", 10F);

            Panel pnlSidebar = new Panel() { Dock = DockStyle.Left, Width = 280, BackColor = Color.White };
            pnlSidebar.Padding = new Padding(15);
            
            Label lblLogo = new Label() { Text = "Kontrol Paneli", Font = new Font("Segoe UI", 16F, FontStyle.Bold), ForeColor = Color.FromArgb(67, 97, 238), AutoSize = true, Location = new Point(15, 20) };
            pnlSidebar.Controls.Add(lblLogo);

            FlowLayoutPanel flowButtons = new FlowLayoutPanel() { Location = new Point(0, 80), Width = 280, Height = 400, FlowDirection = FlowDirection.TopDown, WrapContents = false };

            var currentUser = Helpers.SessionManager.CurrentUser;
            string role = currentUser?.Role?.Name ?? "";

            if (role == "Admin")
            {
                Button btnStudents = CreateMenuButton("👤 Öğrenci Yönetimi");
                btnStudents.Click += (s, e) => new StudentForm(_studentService, _authService).ShowDialog();

                Button btnInstructors = CreateMenuButton("👨‍🏫 Öğretmen Yönetimi");
                btnInstructors.Click += (s, e) => new InstructorForm(_instructorService, _authService).ShowDialog();

                Button btnCourses = CreateMenuButton("📚 Ders Yönetimi");
                btnCourses.Click += (s, e) => new CourseForm(_courseService).ShowDialog();

                Button btnEnrollments = CreateMenuButton("🎓 Kayıt Yönetimi");
                btnEnrollments.Click += (s, e) => new EnrollmentForm(_enrollmentService, _studentService, _courseService, _semesterService).ShowDialog();

                Button btnReports = CreateMenuButton("📊 Raporlar");
                btnReports.Click += (s, e) => new ReportForm(_reportService).ShowDialog();

                flowButtons.Controls.Add(btnStudents);
                flowButtons.Controls.Add(btnInstructors);
                flowButtons.Controls.Add(btnCourses);
                flowButtons.Controls.Add(btnEnrollments);
                flowButtons.Controls.Add(btnReports);
            }
            else if (role == "Instructor")
            {
                Button btnInstructorPanel = CreateMenuButton("👨‍🏫 Öğretmen İşlemleri");
                btnInstructorPanel.Click += (s, e) => new InstructorPanelForm(_enrollmentService, _courseService, _semesterService).ShowDialog();

                Button btnChangePw = CreateMenuButton("🔑 Şifremi Değiştir");
                btnChangePw.Click += (s, e) => new ChangePasswordForm(_authService).ShowDialog();

                flowButtons.Controls.Add(btnInstructorPanel);
                flowButtons.Controls.Add(btnChangePw);
            }
            else if (role == "Student")
            {
                Button btnStudentPanel = CreateMenuButton("🎓 Öğrenci İşlemleri");
                btnStudentPanel.Click += (s, e) => new StudentPanelForm(_enrollmentService, _courseService, _semesterService).ShowDialog();

                Button btnChangePw = CreateMenuButton("🔑 Şifremi Değiştir");
                btnChangePw.Click += (s, e) => new ChangePasswordForm(_authService).ShowDialog();

                flowButtons.Controls.Add(btnStudentPanel);
                flowButtons.Controls.Add(btnChangePw);
            }

            Button btnLogout = CreateMenuButton("🚪 Çıkış Yap");
            btnLogout.ForeColor = Color.IndianRed;
            btnLogout.Click += (s, e) => {
                _authService.Logout();
                this.Close();
            };
            flowButtons.Controls.Add(btnLogout);

            pnlSidebar.Controls.Add(flowButtons);

            Panel pnlMain = new Panel() { Dock = DockStyle.Fill, BackColor = Color.FromArgb(244, 246, 249) };
            string welcomeName = currentUser?.Username ?? "";
            if (currentUser?.Student != null) welcomeName = $"{currentUser.Student.FirstName} {currentUser.Student.LastName}";
            if (currentUser?.Instructor != null) welcomeName = $"{currentUser.Instructor.FirstName} {currentUser.Instructor.LastName}";

            Label lblWelcome = new Label() { Text = $"Hoş geldiniz, {welcomeName}.\nLütfen sol taraftaki menüden bir işlem seçin.", Location = new Point(50, 50), AutoSize = true, Font = new Font("Segoe UI", 14F), ForeColor = Color.Gray };
            pnlMain.Controls.Add(lblWelcome);

            this.Controls.Add(pnlMain);
            this.Controls.Add(pnlSidebar);
        }

        private Button CreateMenuButton(string text)
        {
            Button btn = new Button() { 
                Text = "  " + text, 
                Width = 280, 
                Height = 50, 
                Margin = new Padding(0),
                FlatStyle = FlatStyle.Flat,
                TextAlign = ContentAlignment.MiddleLeft,
                Font = new Font("Segoe UI", 11F, FontStyle.Regular),
                BackColor = Color.White,
                ForeColor = Color.FromArgb(50, 50, 50),
                Cursor = Cursors.Hand
            };
            btn.FlatAppearance.BorderSize = 0;
            btn.FlatAppearance.MouseOverBackColor = Color.FromArgb(240, 242, 245);
            return btn;
        }
    }
}
