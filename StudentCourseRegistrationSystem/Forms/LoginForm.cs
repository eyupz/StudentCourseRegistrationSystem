using System;
using System.Drawing;
using System.Windows.Forms;
using StudentCourseRegistrationSystem.Services;

namespace StudentCourseRegistrationSystem.Forms
{
    public class LoginForm : Form
    {
        private readonly AuthService _authService;
        private readonly StudentService _studentService;
        private readonly CourseService _courseService;
        private readonly EnrollmentService _enrollmentService;
        private readonly ReportService _reportService;
        private readonly InstructorService _instructorService;
        private readonly SemesterService _semesterService;

        private TextBox txtUsername;
        private TextBox txtPassword;
        private Button btnLogin;
        private Label lblError;
        
        private RadioButton rbAdmin;
        private RadioButton rbInstructor;
        private RadioButton rbStudent;

        public LoginForm(AuthService authService, StudentService studentService, CourseService courseService, EnrollmentService enrollmentService, ReportService reportService, InstructorService instructorService, SemesterService semesterService)
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
            this.Text = "Öğrenci Kayıt Sistemi - Giriş";
            this.Size = new Size(400, 420);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.BackColor = Color.White;
            this.Font = new Font("Segoe UI", 10F, FontStyle.Regular, GraphicsUnit.Point);

            Label lblTitle = new Label() { Text = "Sisteme Giriş Yapın", Location = new Point(100, 20), AutoSize = true, Font = new Font("Segoe UI", 14F, FontStyle.Bold), ForeColor = Color.FromArgb(67, 97, 238) };

            // Role Selection Panel
            Panel pnlRoles = new Panel() { Location = new Point(50, 70), Size = new Size(300, 30) };
            rbAdmin = new RadioButton() { Text = "Yönetici", Location = new Point(0, 0), AutoSize = true, Checked = true };
            rbInstructor = new RadioButton() { Text = "Öğretmen", Location = new Point(90, 0), AutoSize = true };
            rbStudent = new RadioButton() { Text = "Öğrenci", Location = new Point(200, 0), AutoSize = true };
            pnlRoles.Controls.Add(rbAdmin);
            pnlRoles.Controls.Add(rbInstructor);
            pnlRoles.Controls.Add(rbStudent);

            Label lblUsername = new Label() { Text = "Kullanıcı Adı", Location = new Point(50, 110), AutoSize = true, ForeColor = Color.Gray };
            txtUsername = new TextBox() { Location = new Point(50, 135), Width = 280, Font = new Font("Segoe UI", 11F), BorderStyle = BorderStyle.FixedSingle };

            Label lblPassword = new Label() { Text = "Şifre", Location = new Point(50, 180), AutoSize = true, ForeColor = Color.Gray };
            txtPassword = new TextBox() { Location = new Point(50, 205), Width = 280, PasswordChar = '•', Font = new Font("Segoe UI", 11F), BorderStyle = BorderStyle.FixedSingle };

            btnLogin = new Button() { 
                Text = "Giriş Yap", 
                Location = new Point(50, 265), 
                Width = 280, 
                Height = 40,
                BackColor = Color.FromArgb(67, 97, 238),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 11F, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnLogin.FlatAppearance.BorderSize = 0;
            btnLogin.Click += BtnLogin_Click;

            lblError = new Label() { ForeColor = Color.Red, Location = new Point(50, 315), AutoSize = true, Visible = false };

            this.Controls.Add(lblTitle);
            this.Controls.Add(pnlRoles);
            this.Controls.Add(lblUsername);
            this.Controls.Add(txtUsername);
            this.Controls.Add(lblPassword);
            this.Controls.Add(txtPassword);
            this.Controls.Add(btnLogin);
            this.Controls.Add(lblError);

            this.AcceptButton = btnLogin;
        }

        private void BtnLogin_Click(object sender, EventArgs e)
        {
            lblError.Visible = false;

            string expectedRole = "Admin";
            if (rbInstructor.Checked) expectedRole = "Instructor";
            else if (rbStudent.Checked) expectedRole = "Student";

            try
            {
                var user = _authService.Login(txtUsername.Text, txtPassword.Text);
                if (user != null)
                {
                    if (user.Role?.Name != expectedRole)
                    {
                        _authService.Logout();
                        lblError.Text = "Seçilen rol ile yetkiniz uyuşmuyor.";
                        lblError.Visible = true;
                        return;
                    }

                    if (!_authService.IsUserActive(user))
                    {
                        lblError.Text = "Hesabınız aktif değil.";
                        lblError.Visible = true;
                        return;
                    }

                    // Check for first login (default password)
                    bool isDefaultPassword = false;
                    if (user.Role.Name == "Student" && user.Student != null)
                    {
                        string expected = $"{user.Student.FirstName[0]}{user.Student.LastName[0]}123".ToLower();
                        if (txtPassword.Text == expected) isDefaultPassword = true;
                    }
                    else if (user.Role.Name == "Instructor" && user.Instructor != null)
                    {
                        string expected = $"123{user.Instructor.FirstName[0]}{user.Instructor.LastName[0]}".ToLower();
                        if (txtPassword.Text == expected) isDefaultPassword = true;
                    }

                    if (isDefaultPassword)
                    {
                        MessageBox.Show("İlk girişiniz olduğu için güvenlik sebebiyle şifrenizi değiştirmelisiniz.", "Şifre Değiştir", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        var cpForm = new ChangePasswordForm(_authService);
                        if (cpForm.ShowDialog() != DialogResult.OK)
                        {
                            _authService.Logout();
                            return; // Kullanıcı şifre değişimini iptal etti
                        }
                    }

                    MainForm mainForm = new MainForm(_authService, _studentService, _courseService, _enrollmentService, _reportService, _instructorService, _semesterService);
                    this.Hide();
                    mainForm.FormClosed += (s, args) => {
                        this.Show();
                        txtPassword.Text = "";
                    };
                    mainForm.Show();
                }
                else
                {
                    lblError.Text = "Geçersiz kullanıcı adı veya şifre.";
                    lblError.Visible = true;
                }
            }
            catch (Exception ex)
            {
                lblError.Text = ex.Message;
                lblError.Visible = true;
            }
        }
    }
}
