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
    /// <summary>Öğrenci paneli — Detaylı profil bilgileri ve haftalık ders programı</summary>
    public partial class StudentProfileForm : Form
    {
        public StudentProfileForm()
        {
            this.BackColor       = ThemeManager.BackgroundColor;
            this.FormBorderStyle = FormBorderStyle.None;
            this.TopLevel        = false;
            this.Dock            = DockStyle.Fill;
            Build();
        }

        private void Build()
        {
            try
            {
                using var ctx = new AppDbContext();
                var svc = new StudentService(ctx);
                var student = svc.GetStudentByUserId(SessionManager.CurrentUser.Id);

                if (student == null)
                {
                    this.Controls.Add(FormHelper.ErrorLabel("Öğrenci profili bulunamadı."));
                    return;
                }

                // Departman ve fakülte bilgisini yükle
                var dept = ctx.Departments.FirstOrDefault(d => d.Id == student.DepartmentId);

                var scroll = new Panel { Dock = DockStyle.Fill, AutoScroll = true, BackColor = ThemeManager.BackgroundColor };
                var inner  = new Panel { AutoSize = true, AutoSizeMode = AutoSizeMode.GrowAndShrink, Width = 1050, Location = new Point(30, 30), BackColor = ThemeManager.BackgroundColor };

                // ── Başlık ───────────────────────────────────────────────────────
                inner.Controls.Add(new Label
                {
                    Text      = "👤  Öğrenci Profili",
                    Font      = ThemeManager.HeaderFont,
                    ForeColor = ThemeManager.TextPrimary,
                    Location  = new Point(0, 0),
                    AutoSize  = true
                });

                // ── Ana Profil Kartı ─────────────────────────────────────────────
                var cardProfile = new Panel { Size = new Size(1020, 180), Location = new Point(0, 46), BackColor = ThemeManager.CardBackground };
                cardProfile.Paint += (s, e) => ControlPaint.DrawBorder(e.Graphics, cardProfile.ClientRectangle, ThemeManager.BorderColor, ButtonBorderStyle.Solid);

                // Sol renkli şerit
                cardProfile.Controls.Add(new Panel { Dock = DockStyle.Left, Width = 6, BackColor = ThemeManager.PrimaryButton });

                var profileInner = new Panel { Location = new Point(6, 0), Size = new Size(1014, 180) };

                // Avatar dairesi
                var avatar = new Panel
                {
                    Size      = new Size(88, 88),
                    Location  = new Point(20, 46),
                    BackColor = ThemeManager.PrimaryButton
                };
                // Yuvarlak avatar
                using (var path = new System.Drawing.Drawing2D.GraphicsPath())
                {
                    path.AddEllipse(0, 0, 88, 88);
                    avatar.Region = new Region(path);
                }
                string initials = (string.IsNullOrEmpty(student.FirstName) ? "?" : student.FirstName[0].ToString().ToUpper())
                                + (string.IsNullOrEmpty(student.LastName)  ? "" : student.LastName[0].ToString().ToUpper());
                avatar.Controls.Add(new Label
                {
                    Text      = initials,
                    Font      = new Font("Segoe UI", 26, FontStyle.Bold),
                    ForeColor = Color.White,
                    TextAlign = ContentAlignment.MiddleCenter,
                    Dock      = DockStyle.Fill,
                    BackColor = Color.Transparent
                });
                profileInner.Controls.Add(avatar);

                // Bilgi satırları
                int lx = 128;
                profileInner.Controls.Add(MakeProfileLabel($"{student.FirstName} {student.LastName}",
                    new Font("Segoe UI", 18, FontStyle.Bold), ThemeManager.TextPrimary, new Point(lx, 22)));

                // GPA rengi
                Color gpaColor = student.GPA >= 3.0m ? ThemeManager.SuccessButton :
                                 student.GPA >= 2.0m ? Color.FromArgb(245, 158, 11) : ThemeManager.DangerButton;
                profileInner.Controls.Add(MakeProfileLabel($"GPA: {student.GPA:F2}",
                    new Font("Segoe UI", 13, FontStyle.Bold), gpaColor, new Point(lx, 60)));

                profileInner.Controls.Add(MakeProfileLabel($"Öğrenci No: {student.StudentNumber}",
                    ThemeManager.RegularFont, ThemeManager.TextSecondary, new Point(lx, 92)));

                cardProfile.Controls.Add(profileInner);
                inner.Controls.Add(cardProfile);

                // ── Bilgi Kartları Satırı ────────────────────────────────────────
                var infoFlow = new FlowLayoutPanel { Location = new Point(0, 238), AutoSize = true, FlowDirection = FlowDirection.LeftToRight };
                infoFlow.Controls.Add(InfoCard("🏛️ Fakülte",    dept?.Faculty ?? "—",                   Color.FromArgb(67, 97, 238)));
                infoFlow.Controls.Add(InfoCard("📚 Bölüm",       dept?.Name ?? "—",                       Color.FromArgb(16, 185, 129)));
                infoFlow.Controls.Add(InfoCard("🔢 Öğrenci No",  student.StudentNumber,                   Color.FromArgb(245, 158, 11)));
                infoFlow.Controls.Add(InfoCard("🎓 GPA",         student.GPA.ToString("F2"),              gpaColor));
                infoFlow.Controls.Add(InfoCard("📁 Bölüm Kodu",  dept?.Code ?? "—",                       Color.FromArgb(114, 9, 183)));
                infoFlow.Controls.Add(InfoCard("✉️ E-Posta",     SessionManager.CurrentUser?.Email ?? "—", Color.FromArgb(6, 182, 212)));
                inner.Controls.Add(infoFlow);

                // ── Haftalık Ders Programı ───────────────────────────────────────
                inner.Controls.Add(new Label
                {
                    Text      = "📅  Haftalık Ders Programı",
                    Font      = ThemeManager.TitleFont,
                    ForeColor = ThemeManager.TextPrimary,
                    AutoSize  = true,
                    Location  = new Point(0, 430)
                });

                // Aktif derse kayıtları getir
                var activeSem = ctx.Semesters.FirstOrDefault(s => s.IsActive);
                var enrollments = ctx.Enrollments
                    .Include(e => e.Course).ThenInclude(c => c.Instructor)
                    .Where(e => e.StudentId == student.Id &&
                                e.Status == EnrollmentStatus.Enrolled &&
                                (activeSem == null || e.SemesterId == activeSem.Id))
                    .ToList();

                var dgvSchedule = new DataGridView();
                ThemeManager.StyleDataGrid(dgvSchedule);
                dgvSchedule.Location = new Point(0, 462);
                dgvSchedule.Size     = new Size(1020, Math.Max(80, enrollments.Count * 44 + 50));
                dgvSchedule.Anchor   = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
                dgvSchedule.AutoGenerateColumns = false;

                dgvSchedule.Columns.Add(new DataGridViewTextBoxColumn { Name = "Kod",     HeaderText = "Ders Kodu",  Width = 110 });
                dgvSchedule.Columns.Add(new DataGridViewTextBoxColumn { Name = "Ders",    HeaderText = "Ders Adı",   Width = 260 });
                dgvSchedule.Columns.Add(new DataGridViewTextBoxColumn { Name = "Program", HeaderText = "Gün / Saat", Width = 180 });
                dgvSchedule.Columns.Add(new DataGridViewTextBoxColumn { Name = "Kredi",   HeaderText = "Kredi",      Width = 70  });
                dgvSchedule.Columns.Add(new DataGridViewTextBoxColumn { Name = "Hoca",    HeaderText = "Öğretmen",   Width = 200 });
                dgvSchedule.Columns.Add(new DataGridViewTextBoxColumn { Name = "Durum",   HeaderText = "Durum",      Width = 100 });

                foreach (var e in enrollments)
                {
                    dgvSchedule.Rows.Add(
                        e.Course?.CourseCode,
                        e.Course?.Title,
                        e.Course?.Schedule ?? "—",
                        e.Course?.Credits,
                        $"{e.Course?.Instructor?.FirstName} {e.Course?.Instructor?.LastName}".Trim(),
                        "Aktif ✓"
                    );
                }

                inner.Controls.Add(dgvSchedule);

                // ── Not Durumu ───────────────────────────────────────────────────
                int gradeTop = 462 + dgvSchedule.Height + 30;
                inner.Controls.Add(new Label
                {
                    Text      = "🏅  Not Durumu",
                    Font      = ThemeManager.TitleFont,
                    ForeColor = ThemeManager.TextPrimary,
                    AutoSize  = true,
                    Location  = new Point(0, gradeTop)
                });

                var dgvGrades = new DataGridView();
                ThemeManager.StyleDataGrid(dgvGrades);
                dgvGrades.Location = new Point(0, gradeTop + 32);
                dgvGrades.Size     = new Size(1020, Math.Max(80, enrollments.Count * 44 + 50));
                dgvGrades.AutoGenerateColumns = false;

                dgvGrades.Columns.Add(new DataGridViewTextBoxColumn { Name = "Ders",   HeaderText = "Ders",       Width = 280 });
                dgvGrades.Columns.Add(new DataGridViewTextBoxColumn { Name = "Vize",   HeaderText = "Vize (%35)", Width = 100 });
                dgvGrades.Columns.Add(new DataGridViewTextBoxColumn { Name = "Final",  HeaderText = "Final (%50)",Width = 100 });
                dgvGrades.Columns.Add(new DataGridViewTextBoxColumn { Name = "Odev",   HeaderText = "Ödev (%15)", Width = 100 });
                dgvGrades.Columns.Add(new DataGridViewTextBoxColumn { Name = "Ort",    HeaderText = "Ortalama",   Width = 110 });
                dgvGrades.Columns.Add(new DataGridViewTextBoxColumn { Name = "Harf",   HeaderText = "Harf Notu",  Width = 100 });

                foreach (var e in enrollments)
                {
                    string ort  = e.Average.HasValue ? e.Average.Value.ToString("F1") : "—";
                    string harf = e.Grade ?? "—";
                    dgvGrades.Rows.Add(
                        e.Course?.Title,
                        e.Vize?.ToString("F0")  ?? "—",
                        e.Final?.ToString("F0") ?? "—",
                        e.Odev?.ToString("F0")  ?? "—",
                        ort, harf
                    );
                }

                inner.Controls.Add(dgvGrades);

                scroll.Controls.Add(inner);
                this.Controls.Add(scroll);
            }
            catch (Exception ex)
            {
                this.Controls.Add(FormHelper.ErrorLabel(ErrorHelper.GetFullMessage(ex)));
            }
        }

        private static Label MakeProfileLabel(string text, Font font, Color color, Point loc) => new Label
        {
            Text      = text,
            Font      = font,
            ForeColor = color,
            Location  = loc,
            AutoSize  = true
        };

        private static Panel InfoCard(string title, string value, Color accent)
        {
            var p = new Panel { Size = new Size(165, 100), BackColor = ThemeManager.CardBackground, Margin = new Padding(0, 0, 16, 16) };
            p.Paint += (s, e) => ControlPaint.DrawBorder(e.Graphics, p.ClientRectangle, ThemeManager.BorderColor, ButtonBorderStyle.Solid);
            p.Controls.Add(new Panel { Dock = DockStyle.Top, Height = 4, BackColor = accent });
            p.Controls.Add(new Label { Text = title, Font = ThemeManager.LabelFont, ForeColor = ThemeManager.TextSecondary, Location = new Point(12, 16), AutoSize = true });
            p.Controls.Add(new Label { Text = value, Font = new Font("Segoe UI", 10, FontStyle.Bold), ForeColor = ThemeManager.TextPrimary, Location = new Point(12, 40), AutoSize = true, MaximumSize = new Size(140, 0) });
            return p;
        }
    }
}
