using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Windows.Forms;
using Microsoft.EntityFrameworkCore;
using StudentCourseRegistrationSystem.Data;
using StudentCourseRegistrationSystem.Helpers;
using StudentCourseRegistrationSystem.Services;

namespace StudentCourseRegistrationSystem.Forms
{
    public partial class GradeEntryForm : Form
    {
        private DataGridView dgv;
        private ComboBox cmbCourse, cmbSemester;
        private Label lblStatus;

        public GradeEntryForm()
        {
            this.BackColor       = ThemeManager.BackgroundColor;
            this.FormBorderStyle = FormBorderStyle.None;
            this.TopLevel        = false;
            this.Dock            = DockStyle.Fill;
            Build();
        }

        private void Build()
        {
            // ══════════════════════════════════════════════
            // ÜST FİLTRE ÇUBUĞU
            // ══════════════════════════════════════════════
            var topBar = new Panel
            {
                Dock      = DockStyle.Top,
                Height    = 78,
                BackColor = ThemeManager.CardBackground,
                Padding   = new Padding(24, 0, 24, 0)
            };
            topBar.Controls.Add(new Panel { Dock = DockStyle.Bottom, Height = 1, BackColor = ThemeManager.BorderColor });

            var filterFlow = new FlowLayoutPanel
            {
                Dock          = DockStyle.Fill,
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents  = false,
                Padding       = new Padding(0, 16, 0, 0)
            };

            cmbCourse   = new ComboBox(); ThemeManager.StyleComboBox(cmbCourse);   cmbCourse.Width = 300;
            cmbSemester = new ComboBox(); ThemeManager.StyleComboBox(cmbSemester); cmbSemester.Width = 200;

            filterFlow.Controls.Add(LabeledField("Ders",  cmbCourse,   300));
            filterFlow.Controls.Add(LabeledField("Dönem", cmbSemester, 200));

            var btnList = MakeButton("  🔍  Öğrencileri Listele", ThemeManager.PrimaryButton, ThemeManager.PrimaryButtonHover, 190, 38);
            var btnWrap = new Panel { Width = 190, Height = 56, Margin = new Padding(16, 0, 0, 0) };
            btnWrap.Controls.Add(new Label { Text = "", Height = 20, Dock = DockStyle.Top });
            btnList.Dock = DockStyle.Bottom;
            btnWrap.Controls.Add(btnList);
            filterFlow.Controls.Add(btnWrap);
            btnList.Click += (s, e) => LoadStudents();

            topBar.Controls.Add(filterFlow);

            // ══════════════════════════════════════════════
            // NOT GİRİŞ PANELI — sağ tarafta sabit panel
            // ══════════════════════════════════════════════
            var sidePanel = new Panel
            {
                Dock      = DockStyle.Right,
                Width     = 340,
                BackColor = ThemeManager.CardBackground
            };
            sidePanel.Controls.Add(new Panel { Dock = DockStyle.Left, Width = 1, BackColor = ThemeManager.BorderColor });

            var sideInner = new Panel { Dock = DockStyle.Fill, Padding = new Padding(24, 20, 24, 20) };

            // Başlık
            sideInner.Controls.Add(new Label
            {
                Text      = "📝  Not Girişi",
                Font      = ThemeManager.TitleFont,
                ForeColor = ThemeManager.TextPrimary,
                AutoSize  = true,
                Dock      = DockStyle.Top,
                Height    = 36
            });

            // Ağırlık bilgisi kartı
            var infoCard = new Panel { Dock = DockStyle.Top, Height = 76, BackColor = Color.FromArgb(239, 246, 255), Margin = new Padding(0, 8, 0, 0) };
            infoCard.Controls.Add(new Label { Text = "📊  Final %50  ·  Vize %35  ·  Ödev %15", Font = ThemeManager.LabelFont, ForeColor = ThemeManager.PrimaryButton, Location = new Point(12, 10), AutoSize = true });
            infoCard.Controls.Add(new Label { Text = "Her alan 0 – 100 arasında olmalıdır.", Font = ThemeManager.SmallFont, ForeColor = ThemeManager.TextSecondary, Location = new Point(12, 34), AutoSize = true });
            infoCard.Controls.Add(new Label { Text = "Ağırlıklı ort. otomatik hesaplanır.", Font = ThemeManager.SmallFont, ForeColor = ThemeManager.TextSecondary, Location = new Point(12, 54), AutoSize = true });
            sideInner.Controls.Add(infoCard);

            // Seçili öğrenci bilgisi
            var lblSelected = new Label { Text = "Seçili öğrenci yok", Font = ThemeManager.LabelFont, ForeColor = ThemeManager.TextSecondary, Dock = DockStyle.Top, Height = 28, TextAlign = System.Drawing.ContentAlignment.MiddleLeft, Margin = new Padding(0, 12, 0, 0) };
            sideInner.Controls.Add(lblSelected);
            dgv = new DataGridView(); // dgv sonra atanacak
            // dgv seçim değişince güncelle

            // Notlar
            var txtVize  = MakeNumericField(); txtVize.Width  = 260;
            var txtFinal = MakeNumericField(); txtFinal.Width = 260;
            var txtOdev  = MakeNumericField(); txtOdev.Width  = 260;

            sideInner.Controls.Add(LabeledField("Vize  (0-100)",  txtVize,  260, top: 8));
            sideInner.Controls.Add(LabeledField("Final  (0-100)", txtFinal, 260, top: 8));
            sideInner.Controls.Add(LabeledField("Ödev  (0-100)",  txtOdev,  260, top: 8));

            // Canlı önizleme
            var lblPreview = new Label { Text = "Ağırlıklı: —   Harf: —", Font = new Font("Segoe UI", 11, FontStyle.Bold), ForeColor = ThemeManager.PrimaryButton, Dock = DockStyle.Top, Height = 32, TextAlign = System.Drawing.ContentAlignment.MiddleLeft, Margin = new Padding(0, 6, 0, 0) };
            sideInner.Controls.Add(lblPreview);

            void UpdatePreview(object s, EventArgs e)
            {
                if (decimal.TryParse(txtVize.Text,  out decimal v) &&
                    decimal.TryParse(txtFinal.Text, out decimal f) &&
                    decimal.TryParse(txtOdev.Text,  out decimal o))
                {
                    decimal ort = f * 0.50m + v * 0.35m + o * 0.15m;
                    string harf = EnrollmentService.HarfNotu(ort);
                    lblPreview.Text      = $"Ağırlıklı: {ort:F1}   →   {harf}";
                    lblPreview.ForeColor = harf is "FF" or "FD" ? ThemeManager.DangerButton : ThemeManager.SuccessButton;
                }
                else { lblPreview.Text = "Ağırlıklı: —   Harf: —"; lblPreview.ForeColor = ThemeManager.TextSecondary; }
            }
            txtVize.TextChanged  += UpdatePreview;
            txtFinal.TextChanged += UpdatePreview;
            txtOdev.TextChanged  += UpdatePreview;

            // Kaydet Butonu
            var btnSave = MakeButton("  ✔  Notu Kaydet", ThemeManager.SuccessButton, ThemeManager.SuccessButtonHover, 260, 44);
            btnSave.Dock = DockStyle.Top;
            btnSave.Margin = new Padding(0, 12, 0, 0);
            btnSave.Click += (s, e) =>
            {
                if (dgv.SelectedRows.Count == 0) { ErrorHelper.ShowWarning("Listeden bir öğrenci seçin."); return; }
                if (!int.TryParse(dgv.SelectedRows[0].Cells["Id"].Value?.ToString(), out int eid)) return;
                if (!decimal.TryParse(txtVize.Text,  out decimal v)) { ErrorHelper.ShowWarning("Geçerli bir Vize notu girin."); return; }
                if (!decimal.TryParse(txtFinal.Text, out decimal f)) { ErrorHelper.ShowWarning("Geçerli bir Final notu girin."); return; }
                if (!decimal.TryParse(txtOdev.Text,  out decimal o)) { ErrorHelper.ShowWarning("Geçerli bir Ödev notu girin."); return; }
                try
                {
                    using var ctx = new AppDbContext();
                    new EnrollmentService(ctx).SetNumericGrades(eid, v, f, o);
                    decimal ort  = f * 0.50m + v * 0.35m + o * 0.15m;
                    string  harf = EnrollmentService.HarfNotu(ort);
                    MessageBox.Show($"Not kaydedildi!\n\nVize: {v}  Final: {f}  Ödev: {o}\nAğırlıklı: {ort:F1}  →  {harf}", "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    txtVize.Text = txtFinal.Text = txtOdev.Text = "";
                    LoadStudents();
                }
                catch (Exception ex) { ErrorHelper.Show(ex, "Not kaydedilirken"); }
            };
            sideInner.Controls.Add(btnSave);

            // Durum
            lblStatus = new Label { Text = "⬆ Ders ve dönem seçin, ardından 'Listele'ye tıklayın.", Font = ThemeManager.SmallFont, ForeColor = ThemeManager.TextSecondary, Dock = DockStyle.Bottom, Height = 28 };
            sideInner.Controls.Add(lblStatus);

            sidePanel.Controls.Add(sideInner);

            // ══════════════════════════════════════════════
            // ANA TABLO
            // ══════════════════════════════════════════════
            dgv = new DataGridView();
            ThemeManager.StyleDataGrid(dgv);
            dgv.Dock          = DockStyle.Fill;
            dgv.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgv.MultiSelect   = false;
            dgv.SelectionChanged += (s, e) =>
            {
                if (dgv.SelectedRows.Count > 0)
                    lblSelected.Text = "Seçili: " + dgv.SelectedRows[0].Cells["AdSoyad"].Value?.ToString();
            };

            var pnlGrid = new Panel { Dock = DockStyle.Fill, BackColor = ThemeManager.BackgroundColor, Padding = new Padding(16, 12, 16, 12) };
            pnlGrid.Controls.Add(dgv);

            this.Controls.Add(pnlGrid);
            this.Controls.Add(sidePanel);
            this.Controls.Add(topBar);

            LoadComboBoxes();
        }

        // ── Yardımcılar ──────────────────────────────────────────────────────────

        private static TextBox MakeNumericField()
        {
            var t = new TextBox();
            ThemeManager.StyleTextBox(t);
            t.KeyPress += (s, e) =>
            {
                if (!char.IsDigit(e.KeyChar) && e.KeyChar != '.' && e.KeyChar != ',' && e.KeyChar != '\b')
                    e.Handled = true;
            };
            return t;
        }

        private static Button MakeButton(string text, Color back, Color hover, int w, int h)
        {
            var btn = new RoundedButton { Text = text, Width = w, Height = h, BorderRadius = 8, Font = ThemeManager.RegularFont };
            btn.SetColors(back, hover);
            return btn;
        }

        private static Panel LabeledField(string label, Control ctrl, int width, int top = 0)
        {
            var col = new Panel { AutoSize = false, Width = width, Height = 58, Margin = new Padding(0, top, 20, 0), Dock = DockStyle.Top };
            col.Controls.Add(new Label { Text = label, Font = ThemeManager.LabelFont, ForeColor = ThemeManager.TextSecondary, AutoSize = true, Location = new Point(0, 2) });
            ctrl.Width    = width - 4;
            ctrl.Location = new Point(0, 22);
            col.Controls.Add(ctrl);
            return col;
        }

        private void LoadComboBoxes()
        {
            try
            {
                using var ctx = new AppDbContext();
                var q = ctx.Courses.AsQueryable();
                if (SessionManager.IsInstructor)
                {
                    var u = ctx.Users.Include(x => x.Instructor).FirstOrDefault(x => x.Id == SessionManager.CurrentUser.Id);
                    int id = u?.Instructor?.Id ?? 0;
                    q = q.Where(c => c.InstructorId == id);
                }
                cmbCourse.DataSource = q.ToList(); cmbCourse.DisplayMember = "Title"; cmbCourse.ValueMember = "Id";
                cmbSemester.DataSource = ctx.Semesters.ToList(); cmbSemester.DisplayMember = "Name"; cmbSemester.ValueMember = "Id";
                var active = ctx.Semesters.FirstOrDefault(s => s.IsActive);
                if (active != null) cmbSemester.SelectedValue = active.Id;
            }
            catch (Exception ex) { ErrorHelper.Show(ex, "Yükleme hatası"); }
        }

        private void LoadStudents()
        {
            if (cmbCourse.SelectedValue == null || cmbSemester.SelectedValue == null) return;
            try
            {
                using var ctx = new AppDbContext();
                var list = new EnrollmentService(ctx).GetCourseEnrollments(
                    (int)cmbCourse.SelectedValue, (int)cmbSemester.SelectedValue);

                dgv.DataSource = list.Select(e => new {
                    Id       = e.Id,
                    No       = e.Student?.StudentNumber,
                    AdSoyad  = $"{e.Student?.FirstName} {e.Student?.LastName}",
                    Vize     = e.Vize?.ToString("F0") ?? "—",
                    Final    = e.Final?.ToString("F0") ?? "—",
                    Ödev     = e.Odev?.ToString("F0") ?? "—",
                    HarfNotu = e.Grade ?? "—",
                    Durum    = e.Status == Models.EnrollmentStatus.Enrolled ? "Devam" :
                               e.Status == Models.EnrollmentStatus.Completed ? "✓ Tamamlandı" : "✗ Bırakıldı"
                }).ToList();

                if (dgv.Columns["Id"] != null) dgv.Columns["Id"].Visible = false;
                lblStatus.Text = list.Any()
                    ? $"✅  {list.Count} öğrenci listelendi"
                    : "⚠️  Bu ders/dönem için kayıtlı öğrenci yok";
            }
            catch (Exception ex) { ErrorHelper.Show(ex, "Yükleme hatası"); }
        }
    }
}
