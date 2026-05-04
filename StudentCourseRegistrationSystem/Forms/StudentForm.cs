using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Microsoft.EntityFrameworkCore;
using StudentCourseRegistrationSystem.Data;
using StudentCourseRegistrationSystem.Helpers;
using StudentCourseRegistrationSystem.Models;
using StudentCourseRegistrationSystem.Services;

namespace StudentCourseRegistrationSystem.Forms
{
    /// <summary>Admin paneli — Öğrenci yönetimi (CRUD + kullanıcı oluşturma)</summary>
    public partial class StudentForm : Form
    {
        private DataGridView dgv;
        private TextBox txtNo, txtAd, txtSoyad, txtUser, txtPass;

        public StudentForm()
        {
            this.BackColor       = ThemeManager.BackgroundColor;
            this.FormBorderStyle = FormBorderStyle.None;
            this.TopLevel        = false;
            this.Dock            = DockStyle.Fill;
            Build();
            Refresh_();
        }

        private void Build()
        {
            // ── Sağ Panel: Form Alanları ─────────────────────────────────────────
            var sidePanel = new Panel { Dock = DockStyle.Right, Width = 340, BackColor = ThemeManager.CardBackground };
            sidePanel.Controls.Add(new Panel { Dock = DockStyle.Left, Width = 1, BackColor = ThemeManager.BorderColor });

            var sideInner = new Panel { Dock = DockStyle.Fill, Padding = new Padding(24, 22, 24, 22), AutoScroll = true };

            sideInner.Controls.Add(MakeTitle("Öğrenci Ekle / Güncelle"));

            txtNo    = MakeTxt();
            txtAd    = MakeTxt();
            txtSoyad = MakeTxt();
            txtUser  = MakeTxt();
            txtPass  = MakeTxt(); txtPass.UseSystemPasswordChar = true;

            // Bilgi notu
            var infoCard = new Panel { Dock = DockStyle.Top, Height = 76, BackColor = Color.FromArgb(239, 246, 255), Margin = new Padding(0, 8, 0, 12) };
            infoCard.Controls.Add(new Label { Text = "💡 Kullanıcı adı ve şifre otomatik\noluşturulur. İsterseniz değiştirebilirsiniz.", Font = ThemeManager.SmallFont, ForeColor = ThemeManager.PrimaryButton, Location = new Point(10, 10), AutoSize = true });
            sideInner.Controls.Add(infoCard);

            sideInner.Controls.Add(LField("Öğrenci No", txtNo));
            sideInner.Controls.Add(LField("Ad", txtAd));
            sideInner.Controls.Add(LField("Soyad", txtSoyad));
            sideInner.Controls.Add(LField("Kullanıcı Adı", txtUser));
            sideInner.Controls.Add(LField("Şifre", txtPass));

            // Otomatik doldurma
            void AutoFill(object? s, EventArgs e)
            {
                string ad    = txtAd.Text.Trim().ToLowerInvariant().Replace(" ", "");
                string soyad = txtSoyad.Text.Trim().ToLowerInvariant().Replace(" ", "");
                if (!string.IsNullOrEmpty(ad) && !string.IsNullOrEmpty(soyad))
                {
                    txtUser.Text = ad + soyad;
                    string initials = string.Join("", (txtAd.Text.Trim() + " " + txtSoyad.Text.Trim())
                        .Split(' ', StringSplitOptions.RemoveEmptyEntries)
                        .Select(w => char.ToLower(w[0])));
                    txtPass.Text = initials + "123";
                }
            }
            txtAd.TextChanged    += AutoFill;
            txtSoyad.TextChanged += AutoFill;

            // Butonlar
            var btnAdd = MakeBtn("＋  Öğrenci Ekle",    ThemeManager.SuccessButton, ThemeManager.SuccessButtonHover);
            var btnUpd = MakeBtn("✎  Güncelle",         ThemeManager.PrimaryButton, ThemeManager.PrimaryButtonHover);
            var btnDel = MakeBtn("✖  Sil",              ThemeManager.DangerButton,  ThemeManager.DangerButtonHover);

            btnAdd.Click += (s, e) => Add();
            btnUpd.Click += (s, e) => Update_();
            btnDel.Click += (s, e) => Delete();

            var btnDock = new Panel { Dock = DockStyle.Top, Height = 152, Padding = new Padding(0, 8, 0, 0) };
            btnAdd.Dock = DockStyle.Top; btnAdd.Margin = new Padding(0, 0, 0, 8);
            btnUpd.Dock = DockStyle.Top; btnUpd.Margin = new Padding(0, 0, 0, 8);
            btnDel.Dock = DockStyle.Top;
            btnDock.Controls.Add(btnDel);
            btnDock.Controls.Add(btnUpd);
            btnDock.Controls.Add(btnAdd);
            sideInner.Controls.Add(btnDock);

            sidePanel.Controls.Add(sideInner);

            // ── Grid ─────────────────────────────────────────────────────────────
            dgv = new DataGridView();
            ThemeManager.StyleDataGrid(dgv);
            dgv.Dock = DockStyle.Fill;
            dgv.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgv.SelectionChanged += (s, e) => FillForm();

            var pnlGrid = new Panel { Dock = DockStyle.Fill, BackColor = ThemeManager.BackgroundColor, Padding = new Padding(16, 12, 16, 12) };
            pnlGrid.Controls.Add(dgv);

            this.Controls.Add(pnlGrid);
            this.Controls.Add(sidePanel);
        }

        private void Refresh_()
        {
            try
            {
                using var ctx = new AppDbContext();
                var students = ctx.Students
                    .Include(s => s.Department)
                    .Include(s => s.User)
                    .ToList();

                dgv.DataSource = students.Select(s => new {
                    Id         = s.Id,
                    No         = s.StudentNumber,
                    Ad         = s.FirstName,
                    Soyad      = s.LastName,
                    Bölüm      = s.Department?.Name ?? "-",
                    KullaniciAdi = s.User?.Username ?? "—",
                    GPA        = s.GPA.ToString("F2")
                }).ToList();

                if (dgv.Columns["Id"] != null) dgv.Columns["Id"].Visible = false;
            }
            catch (Exception ex) { ErrorHelper.Show(ex, "Öğrenciler yüklenirken"); }
        }

        private void FillForm()
        {
            if (dgv.SelectedRows.Count == 0) return;
            var r = dgv.SelectedRows[0];
            txtNo.Text    = r.Cells["No"].Value?.ToString();
            txtAd.Text    = r.Cells["Ad"].Value?.ToString();
            txtSoyad.Text = r.Cells["Soyad"].Value?.ToString();
            txtUser.Text  = r.Cells["KullaniciAdi"].Value?.ToString();
            txtPass.Text  = "";
        }

        private void Add()
        {
            if (string.IsNullOrWhiteSpace(txtNo.Text) || string.IsNullOrWhiteSpace(txtAd.Text) || 
                string.IsNullOrWhiteSpace(txtUser.Text) || string.IsNullOrWhiteSpace(txtPass.Text)) 
            { ErrorHelper.ShowWarning("No, Ad, Soyad, Kullanıcı Adı ve Şifre zorunludur."); return; }

            try
            {
                using var ctx = new AppDbContext();
                var studentRole = ctx.Roles.FirstOrDefault(r => r.Name == "Student") 
                    ?? throw new InvalidOperationException("Student rolü bulunamadı.");

                if (ctx.Users.Any(u => u.Username == txtUser.Text))
                    throw new InvalidOperationException($"'{txtUser.Text}' kullanıcı adı zaten alınmış.");

                var dept = ctx.Departments.FirstOrDefault() ?? throw new InvalidOperationException("Hiç departman bulunamadı.");

                // 1. User
                var user = new User
                {
                    Username     = txtUser.Text.ToLowerInvariant(),
                    PasswordHash = PasswordHelper.HashPassword(txtPass.Text),
                    Email        = $"{txtNo.Text}@edu.tr",
                    RoleId       = studentRole.Id
                };
                ctx.Users.Add(user);
                ctx.SaveChanges();

                // 2. Student
                var student = new Student 
                { 
                    StudentNumber = txtNo.Text, 
                    FirstName     = txtAd.Text, 
                    LastName      = txtSoyad.Text, 
                    DepartmentId  = dept.Id, 
                    GPA           = 0,
                    UserId        = user.Id
                };
                ctx.Students.Add(student);
                ctx.SaveChanges();

                // 3. Link
                user.StudentId = student.Id;
                ctx.SaveChanges();

                MessageBox.Show($"✅ {txtAd.Text} {txtSoyad.Text} eklendi.\nGiriş: {txtUser.Text} / {txtPass.Text}", "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
                txtNo.Text = txtAd.Text = txtSoyad.Text = txtUser.Text = txtPass.Text = "";
                Refresh_();
            }
            catch (Exception ex) { ErrorHelper.Show(ex, "Ekleme hatası"); }
        }

        private void Update_()
        {
            if (dgv.SelectedRows.Count == 0 || !int.TryParse(dgv.SelectedRows[0].Cells["Id"].Value?.ToString(), out int id)) return;
            try
            {
                using var ctx = new AppDbContext();
                var student = ctx.Students.Find(id);
                if (student == null) return;

                student.StudentNumber = txtNo.Text;
                student.FirstName     = txtAd.Text;
                student.LastName      = txtSoyad.Text;

                if (!string.IsNullOrWhiteSpace(txtPass.Text))
                {
                    var user = ctx.Users.FirstOrDefault(u => u.StudentId == id);
                    if (user != null) user.PasswordHash = PasswordHelper.HashPassword(txtPass.Text);
                }

                ctx.SaveChanges();
                MessageBox.Show("✅ Güncellendi.", "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
                Refresh_();
            }
            catch (Exception ex) { ErrorHelper.Show(ex, "Güncelleme hatası"); }
        }

        private void Delete()
        {
            if (dgv.SelectedRows.Count == 0 || !int.TryParse(dgv.SelectedRows[0].Cells["Id"].Value?.ToString(), out int id)) return;
            if (MessageBox.Show("Bu öğrenciyi silmek istiyor musunuz?", "Onay", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes) return;
            try
            {
                using var ctx = new AppDbContext();
                var student = ctx.Students.Find(id);
                if (student == null) return;

                var user = ctx.Users.FirstOrDefault(u => u.StudentId == id);
                if (user != null) { user.StudentId = null; ctx.SaveChanges(); }

                ctx.Students.Remove(student);
                ctx.SaveChanges();
                Refresh_();
            }
            catch (Exception ex) { ErrorHelper.Show(ex, "Silme hatası"); }
        }

        // ── Yardımcılar ──────────────────────────────────────────────────────────
        private static Label MakeTitle(string t) => new Label { Text = t, Font = ThemeManager.TitleFont, ForeColor = ThemeManager.TextPrimary, AutoSize = false, Height = 36, Dock = DockStyle.Top };
        private static TextBox MakeTxt() { var t = new TextBox(); ThemeManager.StyleTextBox(t); t.Width = 270; return t; }
        private static Button MakeBtn(string t, Color c, Color h) { var b = new RoundedButton { Text = t, Width = 270, Height = 42, BorderRadius = 8, Font = ThemeManager.RegularFont }; b.SetColors(c, h); return b; }
        private static Panel LField(string label, Control ctrl) { var p = new Panel { Dock = DockStyle.Top, Height = 64, Margin = new Padding(0, 4, 0, 0) }; p.Controls.Add(new Label { Text = label, Font = ThemeManager.LabelFont, ForeColor = ThemeManager.TextSecondary, AutoSize = true, Location = new Point(0, 2) }); ctrl.Location = new Point(0, 24); ctrl.Width = 270; p.Controls.Add(ctrl); return p; }
    }
}
