using System;
using System.Drawing;
using System.Windows.Forms;

namespace StudentCourseRegistrationSystem.Forms
{
    public partial class LoginForm : Form
    {
        private TextBox txtUsername;
        private TextBox txtPassword;
        private Button btnLogin;
        private Label lblTitle;

        public LoginForm()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.Text = "Login - Student Course Registration System";
            this.Size = new Size(400, 300);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;

            lblTitle = new Label
            {
                Text = "System Login",
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                AutoSize = true,
                Location = new Point(130, 30)
            };

            var lblUsername = new Label { Text = "Username:", Location = new Point(50, 90), AutoSize = true };
            txtUsername = new TextBox { Location = new Point(150, 90), Width = 180 };

            var lblPassword = new Label { Text = "Password:", Location = new Point(50, 130), AutoSize = true };
            txtPassword = new TextBox { Location = new Point(150, 130), Width = 180, PasswordChar = '*' };

            btnLogin = new Button
            {
                Text = "Login",
                Location = new Point(150, 180),
                Width = 100,
                Height = 35,
                BackColor = Color.FromArgb(0, 122, 204),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10, FontStyle.Bold)
            };
            btnLogin.Click += BtnLogin_Click;

            this.Controls.Add(lblTitle);
            this.Controls.Add(lblUsername);
            this.Controls.Add(txtUsername);
            this.Controls.Add(lblPassword);
            this.Controls.Add(txtPassword);
            this.Controls.Add(btnLogin);
        }

        private void BtnLogin_Click(object sender, EventArgs e)
        {
            // TODO: Call AuthService to validate user
            if (!string.IsNullOrWhiteSpace(txtUsername.Text))
            {
                this.Hide();
                var mainForm = new MainForm();
                mainForm.FormClosed += (s, args) => this.Close();
                mainForm.Show();
            }
            else
            {
                MessageBox.Show("Please enter a valid username.", "Login Failed", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
    }
}
