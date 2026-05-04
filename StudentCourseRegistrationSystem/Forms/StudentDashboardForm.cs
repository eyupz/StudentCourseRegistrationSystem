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
    public partial class StudentDashboardForm : Form
    {
        public StudentDashboardForm()
        {
            this.BackColor = ThemeManager.BackgroundColor; this.FormBorderStyle = FormBorderStyle.None;
            this.TopLevel = false; this.Dock = DockStyle.Fill;
            Build();
        }

        private void Build()
        {
            try
            {
                using var ctx = new AppDbContext();
                var student = new StudentService(ctx).GetStudentByUserId(SessionManager.CurrentUser.Id);
                if (student == null) { this.Controls.Add(FormHelper.ErrorLabel("Öğrenci profili bulunamadı.")); return; }

                var enrollSvc  = new EnrollmentService(ctx);
                var allEnroll  = enrollSvc.GetStudentEnrollments(student.Id);
                var active     = allEnroll.Where(e => e.Status == EnrollmentStatus.Enrolled).ToList();
                var completed  = allEnroll.Count(e => e.Status == EnrollmentStatus.Completed);
                int credits    = active.Sum(e => e.Course?.Credits ?? 0);

                // ── ScrollPanel
                var scroll = new Panel { Dock = DockStyle.Fill, AutoScroll = true, BackColor = ThemeManager.BackgroundColor };
                var inner  = new Panel { AutoSize = true, AutoSizeMode = AutoSizeMode.GrowAndShrink, Width = 1100, Location = new Point(24, 24), BackColor = ThemeManager.BackgroundColor };

                // ── Profil Kartı
                var cardProfile = new Panel { Size = new Size(740, 110), Location = new Point(0, 0), BackColor = ThemeManager.CardBackground };
                cardProfile.Paint += (s, e) => ControlPaint.DrawBorder(e.Graphics, cardProfile.ClientRectangle, ThemeManager.BorderColor, ButtonBorderStyle.Solid);
                cardProfile.Controls.Add(new Panel { Dock = DockStyle.Left, Width = 5, BackColor = ThemeManager.PrimaryButton });

                var cardInner = new Panel { Location = new Point(5, 0), Size = new Size(735, 110) };
                cardInner.Controls.Add(new Label { Text = $"{student.FirstName} {student.LastName}", Font = new Font("Segoe UI", 15, FontStyle.Bold), ForeColor = ThemeManager.TextPrimary, AutoSize = true, Location = new Point(18, 15) });
                cardInner.Controls.Add(new Label { Text = $"No: {student.StudentNumber}   |   Bölüm: {student.Department?.Name ?? "—"}", Font = ThemeManager.RegularFont, ForeColor = ThemeManager.TextSecondary, AutoSize = true, Location = new Point(20, 48) });
                var lblGpa = new Label { Text = $"GPA  {student.GPA:F2}", Font = new Font("Segoe UI", 14, FontStyle.Bold), ForeColor = GpaColor(student.GPA), AutoSize = true, Location = new Point(580, 38) };
                cardInner.Controls.Add(lblGpa);
                cardProfile.Controls.Add(cardInner);
                inner.Controls.Add(cardProfile);

                // ── İstatistik Kartları
                var cardFlow = new FlowLayoutPanel { Location = new Point(0, 128), AutoSize = true, FlowDirection = FlowDirection.LeftToRight };
                cardFlow.Controls.Add(StatCard("Aktif Ders",   active.Count.ToString(),     Color.FromArgb(59, 130, 246)));
                cardFlow.Controls.Add(StatCard("Tamamlanan",   completed.ToString(),         Color.FromArgb(16, 185, 129)));
                cardFlow.Controls.Add(StatCard("Dönem AKTS",   credits.ToString(),           Color.FromArgb(245, 158, 11)));
                inner.Controls.Add(cardFlow);

                // ── Aktif Dersler Tablosu
                inner.Controls.Add(new Label { Text = "Bu Dönem Aktif Derslerim", Font = ThemeManager.TitleFont, ForeColor = ThemeManager.TextPrimary, AutoSize = true, Location = new Point(0, 268) });

                var dgv = new DataGridView();
                ThemeManager.StyleDataGrid(dgv);
                dgv.Location = new Point(0, 300);
                dgv.Size     = new Size(1060, Math.Max(100, active.Count * 44 + 48));
                dgv.Anchor   = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
                dgv.DataSource = active.Select(e => new {
                    Kod     = e.Course?.CourseCode,
                    Ders    = e.Course?.Title,
                    Kredi   = e.Course?.Credits,
                    Program = e.Course?.Schedule,
                    Eğitmen = $"{e.Course?.Instructor?.FirstName} {e.Course?.Instructor?.LastName}"
                }).ToList();

                inner.Controls.Add(dgv);
                scroll.Controls.Add(inner);
                this.Controls.Add(scroll);
            }
            catch (Exception ex)
            {
                this.Controls.Add(FormHelper.ErrorLabel(ErrorHelper.GetFullMessage(ex)));
            }
        }

        private Color GpaColor(decimal gpa) =>
            gpa >= 3.0m ? ThemeManager.SuccessButton :
            gpa >= 2.0m ? Color.FromArgb(245, 158, 11) :
            ThemeManager.DangerButton;

        private Panel StatCard(string title, string value, Color color)
        {
            var p = new Panel { Size = new Size(185, 90), BackColor = ThemeManager.CardBackground, Margin = new Padding(0, 0, 18, 0) };
            p.Paint += (s, e) => ControlPaint.DrawBorder(e.Graphics, p.ClientRectangle, ThemeManager.BorderColor, ButtonBorderStyle.Solid);
            p.Controls.Add(new Panel { Dock = DockStyle.Top, Height = 4, BackColor = color });
            p.Controls.Add(new Label { Text = title, Font = ThemeManager.LabelFont, ForeColor = ThemeManager.TextSecondary, Location = new Point(14, 16), AutoSize = true });
            p.Controls.Add(new Label { Text = value, Font = new Font("Segoe UI", 20, FontStyle.Bold), ForeColor = color, Location = new Point(12, 38), AutoSize = true });
            return p;
        }
    }
}
