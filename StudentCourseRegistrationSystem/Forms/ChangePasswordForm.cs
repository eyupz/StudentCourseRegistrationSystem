using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using StudentCourseRegistrationSystem.Data;
using StudentCourseRegistrationSystem.Helpers;

namespace StudentCourseRegistrationSystem.Forms
{
    /// <summary>Kullanıcının kendi şifresini değiştirdiği form (hem öğrenci hem öğretmen kullanır)</summary>
    public partial class ChangePasswordForm : Form
    {
        private TextBox txtCurrent, txtNew, txtConfirm;

        public ChangePasswordForm()
        {
            this.BackColor       = ThemeManager.BackgroundColor;
            this.FormBorderStyle = FormBorderStyle.None;
            this.TopLevel        = false;
            this.Dock            = DockStyle.Fill;
            Build();
        }

        private void Build()
        {
            // ── İçerik kartı ────────────────────────────────────────────────────
            var scroll = new Panel { Dock = DockStyle.Fill, AutoScroll = true, BackColor = ThemeManager.BackgroundColor };
            var card   = new Panel
            {
                BackColor = ThemeManager.CardBackground,
                Size      = new Size(480, 440),
                Location  = new Point(60, 50)
            };
            card.Paint += (s, e) => ControlPaint.DrawBorder(e.Graphics, card.ClientRectangle, ThemeManager.BorderColor, ButtonBorderStyle.Solid);

            // Accent çizgisi
            card.Controls.Add(new Panel { Dock = DockStyle.Top, Height = 5, BackColor = ThemeManager.PrimaryButton });

            var inner = new Panel { Location = new Point(32, 24), Size = new Size(416, 380) };

            inner.Controls.Add(new Label
            {
                Text      = "🔒  Şifre Değiştir",
                Font      = ThemeManager.TitleFont,
                ForeColor = ThemeManager.TextPrimary,
                Location  = new Point(0, 0),
                AutoSize  = true
            });

            inner.Controls.Add(new Label
            {
                Text      = "Güvenliğiniz için şifrenizi düzenli değiştirin.",
                Font      = ThemeManager.SmallFont,
                ForeColor = ThemeManager.TextSecondary,
                Location  = new Point(0, 34),
                AutoSize  = true
            });

            // Mevcut şifre
            txtCurrent = MakeTxt(true);
            AddField(inner, "Mevcut Şifre",    txtCurrent, 80);

            // Yeni şifre
            txtNew = MakeTxt(true);
            AddField(inner, "Yeni Şifre",      txtNew,     156);

            // Onay şifre
            txtConfirm = MakeTxt(true);
            AddField(inner, "Yeni Şifre (Tekrar)", txtConfirm, 232);

            // Kaydet butonu
            var btnSave = new RoundedButton { Text = "  ✔  Şifremi Güncelle", Width = 200, Height = 44, BorderRadius = 8, Font = ThemeManager.RegularFont, Location = new Point(0, 318) };
            btnSave.SetColors(ThemeManager.SuccessButton, ThemeManager.SuccessButtonHover);
            btnSave.Click += BtnSave_Click;
            inner.Controls.Add(btnSave);

            card.Controls.Add(inner);
            scroll.Controls.Add(card);
            this.Controls.Add(scroll);
        }

        private static void AddField(Panel parent, string label, TextBox txt, int top)
        {
            parent.Controls.Add(new Label
            {
                Text      = label,
                Font      = ThemeManager.LabelFont,
                ForeColor = ThemeManager.TextSecondary,
                Location  = new Point(0, top),
                AutoSize  = true
            });
            txt.Location = new Point(0, top + 22);
            txt.Width    = 400;
            parent.Controls.Add(txt);
        }

        private static TextBox MakeTxt(bool isPass)
        {
            var t = new TextBox();
            ThemeManager.StyleTextBox(t);
            if (isPass) t.UseSystemPasswordChar = true;
            return t;
        }

        private void BtnSave_Click(object? sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtCurrent.Text) || string.IsNullOrWhiteSpace(txtNew.Text) || string.IsNullOrWhiteSpace(txtConfirm.Text))
            { ErrorHelper.ShowWarning("Tüm alanları doldurun."); return; }

            if (txtNew.Text != txtConfirm.Text)
            { ErrorHelper.ShowWarning("Yeni şifreler eşleşmiyor."); return; }

            if (txtNew.Text.Length < 4)
            { ErrorHelper.ShowWarning("Şifre en az 4 karakter olmalıdır."); return; }

            try
            {
                using var ctx = new AppDbContext();
                var user = ctx.Users.Find(SessionManager.CurrentUser.Id);
                if (user == null) { ErrorHelper.ShowWarning("Kullanıcı bulunamadı."); return; }

                if (!PasswordHelper.VerifyPassword(txtCurrent.Text, user.PasswordHash))
                { ErrorHelper.ShowWarning("Mevcut şifre hatalı."); return; }

                user.PasswordHash = PasswordHelper.HashPassword(txtNew.Text);
                ctx.SaveChanges();

                MessageBox.Show("✅ Şifreniz başarıyla güncellendi.", "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
                txtCurrent.Text = txtNew.Text = txtConfirm.Text = "";
            }
            catch (Exception ex) { ErrorHelper.Show(ex, "Şifre güncellenirken"); }
        }
    }
}
