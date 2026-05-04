using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Microsoft.EntityFrameworkCore;
using StudentCourseRegistrationSystem.Data;
using StudentCourseRegistrationSystem.Helpers;
using StudentCourseRegistrationSystem.Models;

namespace StudentCourseRegistrationSystem.Forms
{
    /// <summary>Öğretmen paneli — 15 haftalık yoklama yönetimi</summary>
    public partial class AttendanceForm : Form
    {
        private DataGridView dgv;
        private ComboBox cmbCourse, cmbWeek, cmbHour;
        private Label lblStatus;
        private Button btnLoad, btnSave;
        private int _instructorId;

        public AttendanceForm()
        {
            this.BackColor       = ThemeManager.BackgroundColor;
            this.FormBorderStyle = FormBorderStyle.None;
            this.TopLevel        = false;
            this.Dock            = DockStyle.Fill;

            // Öğretmen ID'sini bul
            try
            {
                using var ctx = new AppDbContext();
                var u = ctx.Users.Include(x => x.Instructor).FirstOrDefault(x => x.Id == SessionManager.CurrentUser.Id);
                _instructorId = u?.Instructor?.Id ?? 0;
            }
            catch { }

            Build();
        }

        private void Build()
        {
            // ── Üst Filtre Çubuğu ────────────────────────────────────────────────
            var topBar = new Panel
            {
                Dock      = DockStyle.Top,
                Height    = 80,
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

            // Ders
            cmbCourse = new ComboBox(); ThemeManager.StyleComboBox(cmbCourse); cmbCourse.Width = 280;
            filterFlow.Controls.Add(LabeledField("Ders", cmbCourse, 280));

            // Hafta (1-15)
            cmbWeek = new ComboBox(); ThemeManager.StyleComboBox(cmbWeek); cmbWeek.Width = 100;
            for (int w = 1; w <= 15; w++) cmbWeek.Items.Add($"Hafta {w}");
            cmbWeek.SelectedIndex = 0;
            filterFlow.Controls.Add(LabeledField("Hafta", cmbWeek, 110));

            // Saat (ders saati - ders kredisine göre)
            cmbHour = new ComboBox(); ThemeManager.StyleComboBox(cmbHour); cmbHour.Width = 110;
            for (int h = 1; h <= 4; h++) cmbHour.Items.Add($"{h}. Saat");
            cmbHour.SelectedIndex = 0;
            filterFlow.Controls.Add(LabeledField("Ders Saati", cmbHour, 120));

            // Listele butonu
            btnLoad = new RoundedButton { Text = "  🔍  Listele", Width = 150, Height = 38, BorderRadius = 8, Font = ThemeManager.RegularFont };
            ((RoundedButton)btnLoad).SetColors(ThemeManager.PrimaryButton, ThemeManager.PrimaryButtonHover);
            var btnWrap = new Panel { Width = 150, Height = 56, Margin = new Padding(16, 0, 0, 0) };
            btnWrap.Controls.Add(new Label { Text = "", Height = 20, Dock = DockStyle.Top });
            btnLoad.Dock = DockStyle.Bottom;
            btnWrap.Controls.Add(btnLoad);
            filterFlow.Controls.Add(btnWrap);
            btnLoad.Click += (s, e) => LoadAttendance();

            topBar.Controls.Add(filterFlow);

            // ── Sağ Bilgi Paneli ────────────────────────────────────────────────
            var sidePanel = new Panel
            {
                Dock      = DockStyle.Right,
                Width     = 280,
                BackColor = ThemeManager.CardBackground
            };
            sidePanel.Controls.Add(new Panel { Dock = DockStyle.Left, Width = 1, BackColor = ThemeManager.BorderColor });

            var sideInner = new Panel { Dock = DockStyle.Fill, Padding = new Padding(20, 22, 20, 22) };

            // Başlık
            sideInner.Controls.Add(new Label
            {
                Text      = "📋  Yoklama Bilgisi",
                Font      = ThemeManager.TitleFont,
                ForeColor = ThemeManager.TextPrimary,
                AutoSize  = false, Height = 36, Dock = DockStyle.Top
            });

            // Bilgi kartı
            var infoCard = new Panel { Dock = DockStyle.Top, Height = 110, BackColor = Color.FromArgb(239, 246, 255), Margin = new Padding(0, 10, 0, 0) };
            infoCard.Controls.Add(new Label
            {
                Text      = "✅  Var: Yoklamayı işaretleyin\n\n📌  Her ders saati için ayrı\n     yoklama alınır.\n\n🔁  Kaydedilen yoklama\n     değiştirilebilir.",
                Font      = ThemeManager.SmallFont,
                ForeColor = ThemeManager.TextSecondary,
                Location  = new Point(12, 8),
                AutoSize  = true
            });
            sideInner.Controls.Add(infoCard);

            // İstatistik
            lblStatus = new Label
            {
                Text      = "⬆ Ders, hafta ve saat seçip\n'Listele'ye tıklayın.",
                Font      = ThemeManager.SmallFont,
                ForeColor = ThemeManager.TextSecondary,
                Dock      = DockStyle.Top,
                Height    = 50,
                Margin    = new Padding(0, 12, 0, 0)
            };
            sideInner.Controls.Add(lblStatus);

            // Kaydet butonu
            btnSave = new RoundedButton { Text = "  ✔  Yoklamayı Kaydet", Width = 230, Height = 44, BorderRadius = 8, Font = ThemeManager.RegularFont };
            ((RoundedButton)btnSave).SetColors(ThemeManager.SuccessButton, ThemeManager.SuccessButtonHover);
            btnSave.Dock = DockStyle.Top;
            btnSave.Margin = new Padding(0, 16, 0, 0);
            btnSave.Click += SaveAttendance;
            sideInner.Controls.Add(btnSave);

            sidePanel.Controls.Add(sideInner);

            // ── Ana Grid ─────────────────────────────────────────────────────────
            dgv = new DataGridView();
            ThemeManager.StyleDataGrid(dgv);
            dgv.Dock          = DockStyle.Fill;
            dgv.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgv.ReadOnly      = false;
            dgv.AllowUserToAddRows = false;

            // Checkbox sütunu ekle
            var colPresent = new DataGridViewCheckBoxColumn
            {
                Name       = "Var",
                HeaderText = "✓ Var",
                Width      = 60,
                ReadOnly   = false
            };

            var pnlGrid = new Panel { Dock = DockStyle.Fill, BackColor = ThemeManager.BackgroundColor, Padding = new Padding(16, 12, 16, 12) };
            pnlGrid.Controls.Add(dgv);

            this.Controls.Add(pnlGrid);
            this.Controls.Add(sidePanel);
            this.Controls.Add(topBar);

            LoadCourses();
        }

        private void LoadCourses()
        {
            try
            {
                using var ctx = new AppDbContext();
                var q = ctx.Courses.AsQueryable();
                if (_instructorId > 0)
                    q = q.Where(c => c.InstructorId == _instructorId);
                else if (SessionManager.IsAdmin)
                    q = ctx.Courses; // Admin tümünü görebilir

                cmbCourse.DataSource    = q.ToList();
                cmbCourse.DisplayMember = "Title";
                cmbCourse.ValueMember   = "Id";
            }
            catch (Exception ex) { ErrorHelper.Show(ex, "Ders listesi yüklenirken"); }
        }

        private void LoadAttendance()
        {
            if (cmbCourse.SelectedValue == null) { ErrorHelper.ShowWarning("Ders seçin."); return; }

            int courseId = (int)cmbCourse.SelectedValue;
            int week     = cmbWeek.SelectedIndex + 1;
            int hour     = cmbHour.SelectedIndex + 1;

            try
            {
                using var ctx = new AppDbContext();

                // Aktif dönemin kaydı olan öğrenciler
                var activeSem = ctx.Semesters.FirstOrDefault(s => s.IsActive);
                if (activeSem == null) { ErrorHelper.ShowWarning("Aktif dönem bulunamadı."); return; }

                var enrollments = ctx.Enrollments
                    .Include(e => e.Student)
                    .Include(e => e.Attendances)
                    .Where(e => e.CourseId == courseId && e.SemesterId == activeSem.Id && e.Status == EnrollmentStatus.Enrolled)
                    .ToList();

                if (!enrollments.Any())
                {
                    lblStatus.Text = "⚠️ Bu derse kayıtlı öğrenci yok.";
                    dgv.DataSource = null;
                    return;
                }

                // Mevcut yoklamaları al
                var existing = ctx.Attendances
                    .Where(a => enrollments.Select(e => e.Id).Contains(a.EnrollmentId) && a.Week == week && a.Hour == hour)
                    .ToDictionary(a => a.EnrollmentId, a => a.IsPresent);

                // Grid'i doldur (Tag'e EnrollmentId saklayacağız)
                dgv.Columns.Clear();
                dgv.AutoGenerateColumns = false;

                dgv.Columns.Add(new DataGridViewTextBoxColumn { Name = "EnrollmentId", HeaderText = "ID",          Visible = false });
                dgv.Columns.Add(new DataGridViewTextBoxColumn { Name = "No",           HeaderText = "Öğrenci No",  Width = 110 });
                dgv.Columns.Add(new DataGridViewTextBoxColumn { Name = "AdSoyad",      HeaderText = "Ad Soyad",    Width = 200, ReadOnly = true });
                dgv.Columns.Add(new DataGridViewCheckBoxColumn{ Name = "Var",          HeaderText = "✓ Var",       Width = 70,  ReadOnly = false });

                dgv.Rows.Clear();
                foreach (var e in enrollments)
                {
                    bool isPresent = existing.TryGetValue(e.Id, out bool p) && p;
                    dgv.Rows.Add(e.Id, e.Student?.StudentNumber, $"{e.Student?.FirstName} {e.Student?.LastName}", isPresent);
                }

                // Yoklama satırlarını readonly'den koru
                foreach (DataGridViewColumn col in dgv.Columns)
                    col.ReadOnly = (col.Name != "Var");

                int present = existing.Values.Count(v => v);
                lblStatus.Text = $"✅  {enrollments.Count} öğrenci\n📅  Hafta {week}, {hour}. Saat\n✓   {present} kişi var olarak işaretli";
            }
            catch (Exception ex) { ErrorHelper.Show(ex, "Yoklama yüklenirken"); }
        }

        private void SaveAttendance(object? sender, EventArgs e)
        {
            if (dgv.Rows.Count == 0) { ErrorHelper.ShowWarning("Önce listeyi yükleyin."); return; }
            if (cmbCourse.SelectedValue == null) return;

            int week = cmbWeek.SelectedIndex + 1;
            int hour = cmbHour.SelectedIndex + 1;

            try
            {
                using var ctx = new AppDbContext();

                foreach (DataGridViewRow row in dgv.Rows)
                {
                    if (!int.TryParse(row.Cells["EnrollmentId"].Value?.ToString(), out int eid)) continue;
                    bool isPresent = Convert.ToBoolean(row.Cells["Var"].Value ?? false);

                    var existing = ctx.Attendances.FirstOrDefault(a => a.EnrollmentId == eid && a.Week == week && a.Hour == hour);
                    if (existing != null)
                    {
                        existing.IsPresent = isPresent;
                        existing.Date      = DateTime.Now;
                    }
                    else
                    {
                        ctx.Attendances.Add(new Attendance
                        {
                            EnrollmentId = eid,
                            Week         = week,
                            Hour         = hour,
                            IsPresent    = isPresent,
                            Date         = DateTime.Now
                        });
                    }
                }
                ctx.SaveChanges();
                MessageBox.Show($"✅ Hafta {week}, {hour}. Saat yoklaması kaydedildi.", "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
                LoadAttendance(); // Yenile
            }
            catch (Exception ex) { ErrorHelper.Show(ex, "Yoklama kaydedilirken"); }
        }

        private static Panel LabeledField(string label, Control ctrl, int width)
        {
            var col = new Panel { AutoSize = false, Width = width + 10, Height = 56, Margin = new Padding(0, 0, 20, 0) };
            col.Controls.Add(new Label { Text = label, Font = ThemeManager.LabelFont, ForeColor = ThemeManager.TextSecondary, AutoSize = true, Location = new Point(0, 2) });
            ctrl.Width    = width - 4;
            ctrl.Location = new Point(0, 22);
            col.Controls.Add(ctrl);
            return col;
        }
    }
}
