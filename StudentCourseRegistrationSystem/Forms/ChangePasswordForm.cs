using System;
using System.Drawing;
using System.Windows.Forms;
using StudentCourseRegistrationSystem.Services;

namespace StudentCourseRegistrationSystem.Forms
{
    public class ChangePasswordForm : Form
    {
        private readonly AuthService _authService;
        private TextBox txtOldPassword;
        private TextBox txtNewPassword;
        private TextBox txtConfirmPassword;
        private Label lblError;

        public ChangePasswordForm(AuthService authService)
        {
            _authService = authService;
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.Text = "Şifre Değiştir";
            this.Size = new Size(400, 380);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.BackColor = Color.White;
            this.Font = new Font("Segoe UI", 10F);

            Label lblTitle = new Label() { Text = "Şifrenizi Güncelleyin", Location = new Point(100, 20), AutoSize = true, Font = new Font("Segoe UI", 14F, FontStyle.Bold), ForeColor = Color.FromArgb(67, 97, 238) };

            Label lblOld = new Label() { Text = "Mevcut Şifre:", Location = new Point(50, 70), AutoSize = true, ForeColor = Color.Gray };
            txtOldPassword = new TextBox() { Location = new Point(50, 95), Width = 280, PasswordChar = '•', Font = new Font("Segoe UI", 11F), BorderStyle = BorderStyle.FixedSingle };

            Label lblNew = new Label() { Text = "Yeni Şifre:", Location = new Point(50, 140), AutoSize = true, ForeColor = Color.Gray };
            txtNewPassword = new TextBox() { Location = new Point(50, 165), Width = 280, PasswordChar = '•', Font = new Font("Segoe UI", 11F), BorderStyle = BorderStyle.FixedSingle };

            Label lblConfirm = new Label() { Text = "Yeni Şifre (Tekrar):", Location = new Point(50, 210), AutoSize = true, ForeColor = Color.Gray };
            txtConfirmPassword = new TextBox() { Location = new Point(50, 235), Width = 280, PasswordChar = '•', Font = new Font("Segoe UI", 11F), BorderStyle = BorderStyle.FixedSingle };

            Button btnSave = new Button() { 
                Text = "Kaydet", 
                Location = new Point(50, 280), 
                Width = 280, 
                Height = 40,
                BackColor = Color.FromArgb(46, 204, 113),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 11F, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnSave.FlatAppearance.BorderSize = 0;
            btnSave.Click += BtnSave_Click;

            lblError = new Label() { ForeColor = Color.Red, Location = new Point(50, 325), AutoSize = true, Visible = false };

            this.Controls.Add(lblTitle);
            this.Controls.Add(lblOld); this.Controls.Add(txtOldPassword);
            this.Controls.Add(lblNew); this.Controls.Add(txtNewPassword);
            this.Controls.Add(lblConfirm); this.Controls.Add(txtConfirmPassword);
            this.Controls.Add(btnSave);
            this.Controls.Add(lblError);

            this.AcceptButton = btnSave;
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            lblError.Visible = false;

            if (string.IsNullOrWhiteSpace(txtOldPassword.Text) || string.IsNullOrWhiteSpace(txtNewPassword.Text))
            {
                lblError.Text = "Lütfen tüm alanları doldurun.";
                lblError.Visible = true;
                return;
            }

            if (txtNewPassword.Text != txtConfirmPassword.Text)
            {
                lblError.Text = "Yeni şifreler eşleşmiyor.";
                lblError.Visible = true;
                return;
            }

            try
            {
                _authService.ChangePassword(txtOldPassword.Text, txtNewPassword.Text);
                MessageBox.Show("Şifreniz başarıyla güncellendi.", "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                lblError.Text = ex.Message;
                lblError.Visible = true;
            }
        }
    }
}
