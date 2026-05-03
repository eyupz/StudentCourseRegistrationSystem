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
        private Panel pnlCard;

        public LoginForm()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.Text = "Login - Student Course Registration System";
            this.Size = new Size(500, 400);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.BackColor = Color.FromArgb(236, 240, 241); // Light grayish background

            pnlCard = new Panel
            {
                Size = new Size(350, 250),
                Location = new Point(65, 50),
                BackColor = Color.White,
                Padding = new Padding(20)
            };

            lblTitle = new Label
            {
                Text = "Welcome Back",
                Font = new Font("Segoe UI Semibold", 18),
                ForeColor = Color.FromArgb(44, 62, 80),
                AutoSize = true,
                Location = new Point(80, 20)
            };

            var lblUsername = new Label { Text = "Username", Font = new Font("Segoe UI", 10), ForeColor = Color.Gray, Location = new Point(40, 70), AutoSize = true };
            txtUsername = new TextBox { Location = new Point(40, 95), Width = 270, Font = new Font("Segoe UI", 11) };

            var lblPassword = new Label { Text = "Password", Font = new Font("Segoe UI", 10), ForeColor = Color.Gray, Location = new Point(40, 130), AutoSize = true };
            txtPassword = new TextBox { Location = new Point(40, 155), Width = 270, Font = new Font("Segoe UI", 11), PasswordChar = '•' };

            btnLogin = new Button
            {
                Text = "LOGIN",
                Location = new Point(40, 200),
                Width = 270,
                Height = 35,
                BackColor = Color.FromArgb(26, 188, 156), // Teal
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI Semibold", 11),
                Cursor = Cursors.Hand
            };
            btnLogin.FlatAppearance.BorderSize = 0;
            btnLogin.Click += BtnLogin_Click;

            pnlCard.Controls.Add(lblTitle);
            pnlCard.Controls.Add(lblUsername);
            pnlCard.Controls.Add(txtUsername);
            pnlCard.Controls.Add(lblPassword);
            pnlCard.Controls.Add(txtPassword);
            pnlCard.Controls.Add(btnLogin);

            this.Controls.Add(pnlCard);
        }

        private void BtnLogin_Click(object sender, EventArgs e)
        {
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
