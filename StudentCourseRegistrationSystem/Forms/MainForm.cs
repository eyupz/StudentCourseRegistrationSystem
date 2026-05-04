using System;
using System.Drawing;
using System.Windows.Forms;
using StudentCourseRegistrationSystem.Helpers;

namespace StudentCourseRegistrationSystem.Forms
{
    public partial class MainForm : Form
    {
        private Panel pnlContent;
        private Panel panelMenu;
        private Label lblHeaderTitle;
        private Button activeBtn = null;
        private Form activeForm = null;

        public MainForm()
        {
            BuildWindow();
            BuildSidebar();
            BuildHeader();

            if (SessionManager.IsAdmin)
                NavigateTo(new AdminDashboardForm(), "Yönetici Paneli");
            else if (SessionManager.IsInstructor)
                NavigateTo(new InstructorDashboardForm(), "Eğitmen Paneli");
            else if (SessionManager.IsStudent)
                NavigateTo(new StudentDashboardForm(), "Öğrenci Paneli");
        }

        // ── Ana Pencere ──────────────────────────────────────────────────────────
        private void BuildWindow()
        {
            this.Text            = "Öğrenci Bilgi Sistemi – OBS";
            this.Size            = new Size(1366, 820);
            this.MinimumSize     = new Size(1100, 700);
            this.StartPosition   = FormStartPosition.CenterScreen;
            this.BackColor       = ThemeManager.BackgroundColor;
            this.Font            = ThemeManager.RegularFont;

            panelMenu = new Panel
            {
                Dock      = DockStyle.Left,
                Width     = 240,
                BackColor = ThemeManager.SidebarColor
            };

            pnlContent = new Panel { Dock = DockStyle.Fill, BackColor = ThemeManager.BackgroundColor };

            this.Controls.Add(pnlContent);
            this.Controls.Add(panelMenu);
        }

        // ── Üst Header ───────────────────────────────────────────────────────────
        private void BuildHeader()
        {
            var header = new Panel { Dock = DockStyle.Top, Height = 62, BackColor = ThemeManager.CardBackground };
            header.Controls.Add(new Panel { Dock = DockStyle.Bottom, Height = 1, BackColor = ThemeManager.BorderColor });

            lblHeaderTitle = new Label
            {
                Text      = "",
                Font      = ThemeManager.TitleFont,
                ForeColor = ThemeManager.TextPrimary,
                AutoSize  = false,
                Dock      = DockStyle.Left,
                Width     = 600,
                Padding   = new Padding(28, 0, 0, 0),
                TextAlign = ContentAlignment.MiddleLeft
            };

            var roleText  = $"👤  {SessionManager.CurrentUser?.Username}  ·  {SessionManager.CurrentUser?.Role?.Name}";
            var dateText  = DateTime.Now.ToString("dd MMMM yyyy");

            var pnlRight = new Panel
            {
                Dock      = DockStyle.Right,
                Width     = 320,
                BackColor = ThemeManager.CardBackground,
                Padding   = new Padding(0, 10, 24, 0) // Sağdan padding
            };
            var lblUser = new Label { Text = roleText, Font = ThemeManager.RegularFont, ForeColor = ThemeManager.TextPrimary, AutoSize = false, Height = 26, Dock = DockStyle.Top, TextAlign = ContentAlignment.MiddleRight };
            var lblDate = new Label { Text = dateText, Font = ThemeManager.SmallFont,   ForeColor = ThemeManager.TextSecondary, AutoSize = false, Height = 20, Dock = DockStyle.Top, TextAlign = ContentAlignment.MiddleRight };

            pnlRight.Controls.Add(lblDate); // Tersten ekliyoruz çünkü Dock.Top
            pnlRight.Controls.Add(lblUser);

            header.Controls.Add(lblHeaderTitle);
            header.Controls.Add(pnlRight);

            pnlContent.Controls.Add(header);
        }

        // ── Sidebar ──────────────────────────────────────────────────────────────
        private void BuildSidebar()
        {
            // Logo
            var pnlLogo = new Panel { Dock = DockStyle.Top, Height = 75, BackColor = ThemeManager.SidebarColor };
            var lblLogo = new Label
            {
                Text      = "OBS Portal",
                Font      = new Font("Segoe UI", 16, FontStyle.Bold),
                ForeColor = Color.White,
                Dock      = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter
            };
            pnlLogo.Controls.Add(lblLogo);
            panelMenu.Controls.Add(pnlLogo);

            // Separator
            panelMenu.Controls.Add(new Panel { Dock = DockStyle.Top, Height = 1, BackColor = ThemeManager.SidebarHover });

            // Menü öğeleri
            var menuPanel = new FlowLayoutPanel
            {
                Dock          = DockStyle.Top,
                AutoSize      = true,
                FlowDirection = FlowDirection.TopDown,
                WrapContents  = false,
                Padding       = new Padding(12, 10, 12, 0)
            };

            foreach (var (icon, text, action) in BuildMenuItems())
            {
                var btn = MakeSidebarButton(icon, text, action);
                menuPanel.Controls.Add(btn);
            }

            panelMenu.Controls.Add(menuPanel);

            // Çıkış — alta yapışık
            var pnlLogout = new Panel { Dock = DockStyle.Bottom, Height = 64, BackColor = ThemeManager.SidebarColor, Padding = new Padding(12, 10, 12, 10) };
            pnlLogout.Controls.Add(new Panel { Dock = DockStyle.Top, Height = 1, BackColor = ThemeManager.SidebarHover, Location = new Point(0, 0) });

            var btnLogout = MakeSidebarButton("🚪", "Çıkış Yap", () => { SessionManager.Logout(); this.Close(); });
            btnLogout.ForeColor = Color.FromArgb(252, 165, 165);
            btnLogout.Dock = DockStyle.Fill;
            pnlLogout.Controls.Add(btnLogout);

            panelMenu.Controls.Add(pnlLogout);
        }

        private Button MakeSidebarButton(string icon, string text, Action action)
        {
            var btn = new Button
            {
                Text      = $"  {icon}  {text}",
                Width     = 216,
                Height    = 44,
                FlatStyle = FlatStyle.Flat,
                ForeColor = ThemeManager.TextLight,
                Font      = ThemeManager.RegularFont,
                TextAlign = ContentAlignment.MiddleLeft,
                Cursor    = Cursors.Hand,
                BackColor = ThemeManager.SidebarColor,
                Margin    = new Padding(0, 2, 0, 2)
            };
            btn.FlatAppearance.BorderSize        = 0;
            btn.FlatAppearance.MouseOverBackColor = ThemeManager.SidebarHover;

            btn.Click += (s, e) =>
            {
                // Aktif butonu vurgula
                if (activeBtn != null)
                {
                    activeBtn.BackColor = ThemeManager.SidebarColor;
                    activeBtn.ForeColor = ThemeManager.TextLight;
                }
                activeBtn = btn;
                btn.BackColor = ThemeManager.SidebarHover;
                btn.ForeColor = Color.White;
                action?.Invoke();
            };
            return btn;
        }

        private (string icon, string text, Action action)[] BuildMenuItems()
        {
            if (SessionManager.IsAdmin)
                return new[]
                {
                    ("🏠", "Ana Sayfa",          (Action)(() => NavigateTo(new AdminDashboardForm(),  "Yönetici Paneli"))),
                    ("🎓", "Öğrenci Yönetimi",   (Action)(() => NavigateTo(new StudentForm(),         "Öğrenci Yönetimi"))),
                    ("👨‍🏫","Öğretmen Yönetimi",  (Action)(() => NavigateTo(new InstructorForm(),      "Öğretmen Yönetimi"))),
                    ("📚", "Ders Yönetimi",       (Action)(() => NavigateTo(new CourseForm(),          "Ders Yönetimi"))),
                    ("📝", "Kayıt İşlemleri",    (Action)(() => NavigateTo(new EnrollmentForm(),      "Kayıt İşlemleri"))),
                    ("🏅", "Not Girişi",          (Action)(() => NavigateTo(new GradeEntryForm(),      "Not Girişi"))),
                    ("📊", "Raporlar",            (Action)(() => NavigateTo(new ReportForm(),          "Raporlar"))),
                };
            else if (SessionManager.IsInstructor)
                return new[]
                {
                    ("🏠", "Ana Sayfa",   (Action)(() => NavigateTo(new InstructorDashboardForm(), "Eğitmen Paneli"))),
                    ("📋", "Yoklama Al",  (Action)(() => NavigateTo(new AttendanceForm(),          "Yoklama Yönetimi"))),
                    ("🏅", "Not Girişi", (Action)(() => NavigateTo(new GradeEntryForm(),          "Not Girişi"))),
                    ("🔒", "Şifre Değiştir", (Action)(() => NavigateTo(new ChangePasswordForm(), "Şifre Değiştir"))),
                };
            else
                return new[]
                {
                    ("🏠", "Ana Sayfa",         (Action)(() => NavigateTo(new StudentDashboardForm(),  "Öğrenci Paneli"))),
                    ("👤", "Profilim",           (Action)(() => NavigateTo(new StudentProfileForm(),    "Profilim & Program"))),
                    ("📚", "Ders Seçimi",        (Action)(() => NavigateTo(new CourseSelectionForm(),   "Ders Seçimi"))),
                    ("📋", "Transkript",         (Action)(() => NavigateTo(new TranscriptForm(),         "Transkript"))),
                    ("🔒", "Şifre Değiştir",    (Action)(() => NavigateTo(new ChangePasswordForm(),    "Şifre Değiştir"))),
                };
        }

        // ── Sayfa Geçişi ─────────────────────────────────────────────────────────
        public void NavigateTo(Form frm, string title)
        {
            activeForm?.Close();
            activeForm = frm;
            frm.TopLevel        = false;
            frm.FormBorderStyle = FormBorderStyle.None;
            frm.Dock            = DockStyle.Fill;
            pnlContent.Controls.Add(frm);
            pnlContent.Controls.SetChildIndex(frm, 0);
            if (lblHeaderTitle != null) lblHeaderTitle.Text = title;
            frm.Show();
        }
    }
}
