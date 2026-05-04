using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Microsoft.EntityFrameworkCore;
using StudentCourseRegistrationSystem.Data;
using StudentCourseRegistrationSystem.Helpers;
using StudentCourseRegistrationSystem.Models;

namespace StudentCourseRegistrationSystem.Forms
{
    /// <summary>Admin paneli — Öğretmen yönetimi (CRUD + kullanıcı oluşturma)</summary>
    public partial class InstructorForm : Form
    {
        private DataGridView dgv;
        private TextBox txtAd, txtSoyad, txtUser, txtPass;

        public InstructorForm()
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

            sideInner.Controls.Add(MakeTitle("Öğretmen Ekle / Güncelle"));

            txtAd    = MakeTxt(); txtSoyad = MakeTxt();
            txtUser  = MakeTxt(); txtPass  = MakeTxt(); txtPass.UseSystemPasswordChar = true;

            // Bilgi notu - kullanıcı adı/şifre otomatik oluşturulur
            var infoCard = new Panel { Dock = DockStyle.Top, Height = 76, BackColor = Color.FromArgb(239, 246, 255), Margin = new Padding(0, 8, 0, 12) };
            infoCard.Controls.Add(new Label { Text = "💡 Kullanıcı adı ve şifre otomatik\noluşturulur. İsterseniz değiştirebilirsiniz.", Font = ThemeManager.SmallFont, ForeColor = ThemeManager.PrimaryButton, Location = new Point(10, 10), AutoSize = true });
            sideInner.Controls.Add(infoCard);

            // Alanlar ters sırayla Dock:Top ile eklenir
            sideInner.Controls.Add(LField("Kullanıcı Adı (Otomatik)", txtUser));
            sideInner.Controls.Add(LField("Şifre (Otomatik)", txtPass));
            sideInner.Controls.Add(LField("Soyad", txtSoyad));
            sideInner.Controls.Add(LField("Ad", txtAd));

            // Ad/Soyad girilince otomatik kullanıcı adı ve şifre doldur
            void AutoFill(object? s, EventArgs e)
            {
                string ad    = txtAd.Text.Trim().ToLowerInvariant().Replace(" ", "");
                string soyad = txtSoyad.Text.Trim().ToLowerInvariant().Replace(" ", "");
                if (!string.IsNullOrEmpty(ad) && !string.IsNullOrEmpty(soyad))
                {
                    txtUser.Text = ad + soyad;
                    // Baş harfleri al
                    string allNames = (txtAd.Text.Trim() + " " + txtSoyad.Text.Trim());
                    string initials = string.Join("", allNames.Split(' ', StringSplitOptions.RemoveEmptyEntries)
                        .Select(w => char.ToLower(w[0])));
                    txtPass.Text = initials + "123";
                }
            }
            txtAd.TextChanged    += AutoFill;
            txtSoyad.TextChanged += AutoFill;

            // Buton grubu
            var btnAdd = MakeBtn("＋  Öğretmen Ekle",    ThemeManager.SuccessButton, ThemeManager.SuccessButtonHover);
            var btnUpd = MakeBtn("✎  Güncelle",          ThemeManager.PrimaryButton, ThemeManager.PrimaryButtonHover);
            var btnDel = MakeBtn("✖  Sil",               ThemeManager.DangerButton,  ThemeManager.DangerButtonHover);

            btnAdd.Click += BtnAdd_Click;
            btnUpd.Click += BtnUpd_Click;
            btnDel.Click += BtnDel_Click;

            // Butonları tek tek Dock:Top olarak ekle (sıralı)
            var btnDock = new Panel { Dock = DockStyle.Top, Height = 152, Padding = new Padding(0, 8, 0, 0) };
            btnAdd.Dock = DockStyle.Top; btnAdd.Margin = new Padding(0, 0, 0, 8);
            btnUpd.Dock = DockStyle.Top; btnUpd.Margin = new Padding(0, 0, 0, 8);
            btnDel.Dock = DockStyle.Top;
            // Ekleme sırası: son eklenen en üste gelir (Dock:Top ile)
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
                var instr = ctx.Instructors
                    .Include(i => i.Courses)
                    .Include(i => i.User)
                    .ToList();

                dgv.DataSource = instr.Select(i => new {
                    Id         = i.Id,
                    Ad         = i.FirstName,
                    Soyad      = i.LastName,
                    KullaniciAdi = i.User?.Username ?? "—",
                    DersSayısı = i.Courses.Count
                }).ToList();

                if (dgv.Columns["Id"] != null) dgv.Columns["Id"].Visible = false;
            }
            catch (Exception ex) { ErrorHelper.Show(ex, "Liste yüklenirken"); }
        }

        private void FillForm()
        {
            if (dgv.SelectedRows.Count == 0) return;
            var r = dgv.SelectedRows[0];
            txtAd.Text    = r.Cells["Ad"].Value?.ToString();
            txtSoyad.Text = r.Cells["Soyad"].Value?.ToString();
            txtUser.Text  = r.Cells["KullaniciAdi"].Value?.ToString();
            txtPass.Text  = "";
        }

        private void BtnAdd_Click(object? sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtAd.Text)   || string.IsNullOrWhiteSpace(txtSoyad.Text) ||
                string.IsNullOrWhiteSpace(txtUser.Text)  || string.IsNullOrWhiteSpace(txtPass.Text))
            { ErrorHelper.ShowWarning("Ad, Soyad, Kullanıcı Adı ve Şifre zorunludur."); return; }

            try
            {
                using var ctx = new AppDbContext();
                var instrRole = ctx.Roles.FirstOrDefault(r => r.Name == "Instructor")
                    ?? throw new InvalidOperationException("Instructor rolü bulunamadı.");

                if (ctx.Users.Any(u => u.Username == txtUser.Text))
                    throw new InvalidOperationException($"'{txtUser.Text}' kullanıcı adı zaten alınmış.");

                // 1. User oluştur
                var user = new User
                {
                    Username     = txtUser.Text.ToLowerInvariant(),
                    PasswordHash = PasswordHelper.HashPassword(txtPass.Text),
                    Email        = $"{txtUser.Text}@obs.edu.tr",
                    RoleId       = instrRole.Id
                };
                ctx.Users.Add(user);
                ctx.SaveChanges();

                // 2. Instructor oluştur
                var instr = new Instructor { FirstName = txtAd.Text, LastName = txtSoyad.Text, UserId = user.Id };
                ctx.Instructors.Add(instr);
                ctx.SaveChanges();

                // 3. Çift yönlü link
                user.InstructorId = instr.Id;
                ctx.SaveChanges();

                MessageBox.Show($"✅ {txtAd.Text} {txtSoyad.Text} eklendi.\nGiriş: {txtUser.Text} / {txtPass.Text}", "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
                txtAd.Text = txtSoyad.Text = txtUser.Text = txtPass.Text = "";
                Refresh_();
            }
            catch (Exception ex) { ErrorHelper.Show(ex, "Ekleme hatası"); }
        }

        private void BtnUpd_Click(object? sender, EventArgs e)
        {
            if (dgv.SelectedRows.Count == 0 || !int.TryParse(dgv.SelectedRows[0].Cells["Id"].Value?.ToString(), out int id)) return;
            if (string.IsNullOrWhiteSpace(txtAd.Text) || string.IsNullOrWhiteSpace(txtSoyad.Text))
            { ErrorHelper.ShowWarning("Ad ve Soyad zorunludur."); return; }
            try
            {
                using var ctx = new AppDbContext();
                var instr = ctx.Instructors.Find(id);
                if (instr == null) return;
                instr.FirstName = txtAd.Text;
                instr.LastName  = txtSoyad.Text;
                // Şifre güncelleme
                if (!string.IsNullOrWhiteSpace(txtPass.Text))
                {
                    var user = ctx.Users.FirstOrDefault(u => u.InstructorId == id);
                    if (user != null) user.PasswordHash = PasswordHelper.HashPassword(txtPass.Text);
                }
                ctx.SaveChanges();
                MessageBox.Show("✅ Güncellendi.", "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
                Refresh_();
            }
            catch (Exception ex) { ErrorHelper.Show(ex, "Güncelleme hatası"); }
        }

        private void BtnDel_Click(object? sender, EventArgs e)
        {
            if (dgv.SelectedRows.Count == 0 || !int.TryParse(dgv.SelectedRows[0].Cells["Id"].Value?.ToString(), out int id)) return;
            if (MessageBox.Show("Bu öğretmeni silmek istiyor musunuz?", "Onay", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes) return;
            try
            {
                using var ctx = new AppDbContext();
                var instr = ctx.Instructors.Find(id);
                if (instr == null) return;

                // Kullanıcısını bul ve InstructorId'yi temizle önce
                var user = ctx.Users.FirstOrDefault(u => u.InstructorId == id);
                if (user != null) { user.InstructorId = null; ctx.SaveChanges(); }

                // Derslerden instructor referansını kaldır — InstructorId non-nullable,
                // bu yüzden derslere dummy (ilk geçerli) instructor ata veya instructor'ı sil
                // Basit yaklaşım: dersleri de sil veya admin manuel düzeltir.
                // Burada güvenli yol: önce dersleri terk et (başka öğretmene ata ya da sil)
                var courses = ctx.Courses.Where(c => c.InstructorId == id).ToList();
                ctx.Courses.RemoveRange(courses); // Kurs silinirse enrollment cascade ile silinir
                ctx.SaveChanges();

                ctx.Instructors.Remove(instr);
                ctx.SaveChanges();
                Refresh_();
            }
            catch (Exception ex) { ErrorHelper.Show(ex, "Silme hatası"); }
        }

        // ── Yardımcılar ──────────────────────────────────────────────────────────

        private static Label MakeTitle(string t) => new Label
        {
            Text = t, Font = ThemeManager.TitleFont, ForeColor = ThemeManager.TextPrimary,
            AutoSize = false, Height = 36, Dock = DockStyle.Top
        };

        private static TextBox MakeTxt() { var t = new TextBox(); ThemeManager.StyleTextBox(t); t.Width = 262; return t; }

        private static Button MakeBtn(string t, Color c, Color h)
        {
            var b = new RoundedButton { Text = t, Width = 262, Height = 42, BorderRadius = 8, Font = ThemeManager.RegularFont };
            b.SetColors(c, h); return b;
        }

        private static Panel LField(string label, Control ctrl)
        {
            var p = new Panel { Dock = DockStyle.Top, Height = 64, Margin = new Padding(0, 4, 0, 0) };
            p.Controls.Add(new Label { Text = label, Font = ThemeManager.LabelFont, ForeColor = ThemeManager.TextSecondary, AutoSize = true, Location = new Point(0, 2) });
            ctrl.Location = new Point(0, 24); ctrl.Width = 270;
            p.Controls.Add(ctrl); return p;
        }
    }
}
