using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using StudentCourseRegistrationSystem.Data;
using StudentCourseRegistrationSystem.Services;
using StudentCourseRegistrationSystem.Helpers;

namespace StudentCourseRegistrationSystem.Forms
{
    /// <summary>
    /// Ana giriş ekranı — Rol seçimi (Admin / Öğretmen / Öğrenci) + Kullanıcı adı/şifre girişi
    /// </summary>
    public partial class LoginForm : Form
    {
        private Panel pnlRoleSelect;
        private Panel pnlLogin;
        private TextBox txtUsername, txtPassword;
        private Label lblRoleTitle, lblLoginSubtitle;
        private string _selectedRole = "";

        public LoginForm()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.Text            = "OBS — Öğrenci Bilgi Sistemi | Giriş";
            this.Size            = new Size(1100, 720);
            this.StartPosition   = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox     = false;
            this.BackColor       = ThemeManager.BackgroundColor;
            this.Font            = ThemeManager.RegularFont;

            // ── Sol panel: Logo + Slogan ──────────────────────────────────────────
            var pnlLeft = new Panel { Dock = DockStyle.Left, Width = 440, BackColor = ThemeManager.SidebarColor };
            pnlLeft.Paint += DrawLeftGradient;

            var lblLogo = new Label { Text = "OBS", Font = new Font("Segoe UI", 62, FontStyle.Bold), ForeColor = Color.White, TextAlign = ContentAlignment.MiddleCenter, AutoSize = false, Size = new Size(440, 120), Location = new Point(0, 160) };
            var lblPortal = new Label { Text = "Portal", Font = new Font("Segoe UI", 36, FontStyle.Regular), ForeColor = Color.FromArgb(148, 163, 184), TextAlign = ContentAlignment.MiddleCenter, AutoSize = false, Size = new Size(440, 60), Location = new Point(0, 270) };
            var lblSub = new Label { Text = "Öğrenci Bilgi Sistemi", Font = new Font("Segoe UI", 13, FontStyle.Regular), ForeColor = Color.FromArgb(100, 116, 139), TextAlign = ContentAlignment.MiddleCenter, AutoSize = false, Size = new Size(440, 40), Location = new Point(0, 340) };
            var line = new Panel { BackColor = ThemeManager.SidebarActive, Size = new Size(60, 4), Location = new Point(190, 410) };
            var lblFeats = new Label { Text = "✔  Yoklama Takibi\n✔  Not Yönetimi\n✔  Haftalık Program\n✔  Dönem Raporları", Font = new Font("Segoe UI", 11, FontStyle.Regular), ForeColor = Color.FromArgb(148, 163, 184), AutoSize = false, Size = new Size(380, 120), Location = new Point(30, 436), TextAlign = ContentAlignment.TopLeft };

            pnlLeft.Controls.AddRange(new Control[] { lblLogo, lblPortal, lblSub, line, lblFeats });

            // ── Sağ panel: Giriş Konteyneri ──────────────────────────────────────
            var pnlRight = new Panel { Dock = DockStyle.Fill, BackColor = ThemeManager.BackgroundColor };
            
            // Merkezleme Paneli (İçerikleri ortalamak için)
            var pnlCenter = new Panel { Size = new Size(460, 500), BackColor = Color.Transparent };
            pnlRight.Controls.Add(pnlCenter);

            // ROL SEÇİM PANELI
            pnlRoleSelect = new Panel { Dock = DockStyle.Fill, Visible = true };
            var lblWelcome = new Label { Text = "Hoş Geldiniz", Font = new Font("Segoe UI", 28, FontStyle.Bold), ForeColor = ThemeManager.TextPrimary, AutoSize = true, Location = new Point(0, 0) };
            var lblChoose = new Label { Text = "Lütfen giriş yapmak istediğiniz rolü seçin.", Font = ThemeManager.RegularFont, ForeColor = ThemeManager.TextSecondary, AutoSize = true, Location = new Point(4, 52) };
            
            var cardAdmin   = MakeRoleCard("🛡️",  "Admin",       "Sistem yönetimi",         Color.FromArgb(67, 97, 238),   "Admin");
            var cardTeacher = MakeRoleCard("👨‍🏫", "Öğretmen",    "Ders ve not yönetimi",     Color.FromArgb(16, 185, 129),  "Instructor");
            var cardStudent = MakeRoleCard("🎓",  "Öğrenci",     "Dersler ve transkript",    Color.FromArgb(245, 158, 11),  "Student");

            cardAdmin.Location   = new Point(0, 110);
            cardTeacher.Location = new Point(0, 220);
            cardStudent.Location = new Point(0, 330);

            pnlRoleSelect.Controls.AddRange(new Control[] { lblWelcome, lblChoose, cardAdmin, cardTeacher, cardStudent });

            // GİRİŞ FORM PANELI
            pnlLogin = new Panel { Dock = DockStyle.Fill, Visible = false };
            var btnBack = new Button { Text = "← Rol Seçimine Dön", Font = ThemeManager.SmallFont, ForeColor = ThemeManager.PrimaryButton, BackColor = Color.Transparent, FlatStyle = FlatStyle.Flat, AutoSize = true, Location = new Point(0, 0), Cursor = Cursors.Hand };
            btnBack.FlatAppearance.BorderSize = 0;
            btnBack.Click += (s, e) => ShowRoleSelect();

            lblRoleTitle = new Label { Text = "", Font = new Font("Segoe UI", 24, FontStyle.Bold), ForeColor = ThemeManager.TextPrimary, AutoSize = true, Location = new Point(0, 44) };
            lblLoginSubtitle = new Label { Text = "Kullanıcı adı ve şifrenizi girin.", Font = ThemeManager.RegularFont, ForeColor = ThemeManager.TextSecondary, AutoSize = true, Location = new Point(4, 90) };
            
            var lblU = new Label { Text = "Kullanıcı Adı", Font = ThemeManager.LabelFont, ForeColor = ThemeManager.TextSecondary, AutoSize = true, Location = new Point(4, 145) };
            txtUsername = new TextBox { Location = new Point(0, 168), Width = 440 };
            ThemeManager.StyleTextBox(txtUsername);

            var lblP = new Label { Text = "Şifre", Font = ThemeManager.LabelFont, ForeColor = ThemeManager.TextSecondary, AutoSize = true, Location = new Point(4, 225) };
            txtPassword = new TextBox { Location = new Point(0, 248), Width = 440, UseSystemPasswordChar = true };
            ThemeManager.StyleTextBox(txtPassword);
            txtPassword.KeyDown += (s, e) => { if (e.KeyCode == Keys.Enter) DoLogin(); };

            var btnLogin = new RoundedButton { Text = "GİRİŞ YAP  →", Width = 440, Height = 52, BorderRadius = 10, Location = new Point(0, 320), Font = new Font("Segoe UI", 12, FontStyle.Bold) };
            btnLogin.SetColors(ThemeManager.PrimaryButton, ThemeManager.PrimaryButtonHover);
            btnLogin.Click += (s, e) => DoLogin();

            pnlLogin.Controls.AddRange(new Control[] { btnBack, lblRoleTitle, lblLoginSubtitle, lblU, txtUsername, lblP, txtPassword, btnLogin });

            pnlCenter.Controls.Add(pnlLogin);
            pnlCenter.Controls.Add(pnlRoleSelect);

            this.Controls.Add(pnlRight);
            this.Controls.Add(pnlLeft);

            // Merkezleme logic'i
            this.Resize += (s, e) => {
                pnlCenter.Left = (pnlRight.Width - pnlCenter.Width) / 2;
                pnlCenter.Top  = (pnlRight.Height - pnlCenter.Height) / 2;
            };
            this.Load += (s, e) => this.OnResize(EventArgs.Empty);
        }

        private void DrawLeftGradient(object? sender, PaintEventArgs e)
        {
            var panel = (Panel)sender;
            using var brush = new LinearGradientBrush(new Point(0, 0), new Point(panel.Width, panel.Height), ThemeManager.SidebarColor, Color.FromArgb(18, 30, 60));
            e.Graphics.FillRectangle(brush, panel.ClientRectangle);
        }

        private Panel MakeRoleCard(string icon, string title, string subtitle, Color accent, string role)
        {
            var card = new Panel { Size = new Size(440, 96), BackColor = ThemeManager.CardBackground, Cursor = Cursors.Hand };
            card.Paint += (s, e) => ControlPaint.DrawBorder(e.Graphics, card.ClientRectangle, ThemeManager.BorderColor, ButtonBorderStyle.Solid);
            card.Controls.Add(new Panel { Dock = DockStyle.Left, Width = 6, BackColor = accent });

            // İkon Paneli (Sabit genişlik)
            var pnlIcon = new Panel { Dock = DockStyle.Left, Width = 70, BackColor = Color.Transparent };
            var iconLbl = new Label { Text = icon, Font = new Font("Segoe UI", 26), Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleCenter };
            pnlIcon.Controls.Add(iconLbl);

            var titleLbl = new Label { Text = title, Font = new Font("Segoe UI", 15, FontStyle.Bold), ForeColor = ThemeManager.TextPrimary, Location = new Point(78, 20), AutoSize = true };
            var subLbl = new Label { Text = subtitle, Font = ThemeManager.LabelFont, ForeColor = ThemeManager.TextSecondary, Location = new Point(80, 50), AutoSize = true };
            var arrow = new Label { Text = "→", Font = new Font("Segoe UI", 18, FontStyle.Bold), ForeColor = accent, Location = new Point(395, 30), AutoSize = true };

            card.Controls.AddRange(new Control[] { pnlIcon, titleLbl, subLbl, arrow });

            void Hover(bool over) {
                card.BackColor = over ? Color.FromArgb(248, 250, 252) : ThemeManager.CardBackground;
                arrow.Left = over ? 402 : 395;
            }
            card.MouseEnter += (s, e) => Hover(true);
            card.MouseLeave += (s, e) => Hover(false);
            card.Click      += (s, e) => ShowLogin(role, title, accent);
            foreach (Control c in card.Controls) {
                c.MouseEnter += (s, e) => Hover(true);
                c.MouseLeave += (s, e) => Hover(false);
                c.Click      += (s, e) => ShowLogin(role, title, accent);
                foreach (Control subC in c.Controls) {
                    subC.MouseEnter += (s, e) => Hover(true);
                    subC.MouseLeave += (s, e) => Hover(false);
                    subC.Click      += (s, e) => ShowLogin(role, title, accent);
                }
            }
            return card;
        }

        private void ShowLogin(string role, string title, Color accent)
        {
            _selectedRole = role;
            lblRoleTitle.Text = $"Giriş: {title}";
            lblRoleTitle.ForeColor = accent;
            lblLoginSubtitle.Text = $"{title} kullanıcı adı ve şifrenizi girin.";
            txtUsername.Text = ""; txtPassword.Text = "";
            pnlRoleSelect.Visible = false;
            pnlLogin.Visible = true;
            txtUsername.Focus();
        }

        private void ShowRoleSelect()
        {
            pnlLogin.Visible = false;
            pnlRoleSelect.Visible = true;
        }

        private void DoLogin()
        {
            if (string.IsNullOrWhiteSpace(txtUsername.Text) || string.IsNullOrWhiteSpace(txtPassword.Text)) {
                MessageBox.Show("Lütfen kullanıcı adı ve şifrenizi girin.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            try {
                using var context = new AppDbContext();
                var user = new AuthService(context).Login(txtUsername.Text.Trim(), txtPassword.Text);
                if (user == null) {
                    MessageBox.Show("Kullanıcı adı veya şifre hatalı.", "Giriş Başarısız", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                bool roleMatch = _selectedRole switch {
                    "Admin" => user.Role?.Name == "Admin",
                    "Instructor" => user.Role?.Name == "Instructor",
                    "Student" => user.Role?.Name == "Student",
                    _ => true
                };
                if (!roleMatch) {
                    MessageBox.Show($"Bu hesap '{user.Role?.Name}' rolüne sahip.\nLütfen doğru paneli seçin.", "Rol Uyuşmazlığı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                this.Hide();
                var mainForm = new MainForm();
                mainForm.FormClosed += (s, args) => this.Close();
                mainForm.Show();
            }
            catch (Exception ex) {
                ErrorHelper.Show(ex, "Giriş hatası");
            }
        }
    }
}
