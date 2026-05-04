using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using StudentCourseRegistrationSystem.Data;
using StudentCourseRegistrationSystem.Helpers;
using StudentCourseRegistrationSystem.Models;
using StudentCourseRegistrationSystem.Services;

namespace StudentCourseRegistrationSystem.Forms
{
    public partial class TranscriptForm : Form
    {
        public TranscriptForm()
        {
            this.BackColor = ThemeManager.BackgroundColor;
            this.FormBorderStyle = FormBorderStyle.None;
            this.TopLevel = false;
            this.Dock = DockStyle.Fill;
            BuildUI();
        }

        private void BuildUI()
        {
            try
            {
                using var ctx = new AppDbContext();
                var studentSvc = new StudentService(ctx);
                var student = studentSvc.GetStudentByUserId(SessionManager.CurrentUser.Id);
                if (student == null) { AddError("Öğrenci profili bulunamadı."); return; }

                decimal gpa = studentSvc.CalculateAndSaveGPA(student.Id);
                var enrollSvc = new EnrollmentService(ctx);
                var enrollments = enrollSvc.GetStudentEnrollments(student.Id);

                var pnlHeader = new Panel { Location = new Point(30, 30), Size = new Size(900, 100), BackColor = Color.White };
                pnlHeader.Paint += (s, e) => ControlPaint.DrawBorder(e.Graphics, pnlHeader.ClientRectangle, ThemeManager.BorderColor, ButtonBorderStyle.Solid);
                pnlHeader.Controls.Add(new Panel { Dock = DockStyle.Top, Height = 6, BackColor = ThemeManager.PrimaryButton });
                pnlHeader.Controls.Add(new Label { Text = $"📋  {student.FirstName} {student.LastName}  —  {student.StudentNumber}", Font = ThemeManager.TitleFont, ForeColor = ThemeManager.TextPrimary, Location = new Point(25, 22), AutoSize = true });
                pnlHeader.Controls.Add(new Label { Text = $"Bölüm: {student.Department?.Name}", Font = ThemeManager.RegularFont, ForeColor = ThemeManager.TextSecondary, Location = new Point(28, 58), AutoSize = true });

                var gpaColor = gpa >= 3.0m ? Color.FromArgb(76, 201, 140) : gpa >= 2.0m ? Color.FromArgb(248, 150, 30) : Color.FromArgb(220, 53, 69);
                pnlHeader.Controls.Add(new Label { Text = $"GPA: {gpa:F2}", Font = new Font("Segoe UI Semibold", 20, FontStyle.Bold), ForeColor = gpaColor, Location = new Point(700, 25), AutoSize = true });

                var lblT = new Label { Text = "Ders Transkripti", Font = ThemeManager.TitleFont, ForeColor = ThemeManager.TextPrimary, Location = new Point(30, 150), AutoSize = true };

                var dgv = new DataGridView();
                ThemeManager.StyleDataGrid(dgv);
                dgv.Location = new Point(30, 185);
                dgv.Size = new Size(1150, 450);
                dgv.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom;
                dgv.DataSource = enrollments.Select(e => new
                {
                    Dönem    = e.Semester?.Name,
                    Kod      = e.Course?.CourseCode,
                    DersAdı  = e.Course?.Title,
                    Kredi    = e.Course?.Credits,
                    Not      = string.IsNullOrEmpty(e.Grade) ? "—" : e.Grade,
                    Durum    = e.Status == EnrollmentStatus.Enrolled ? "Devam Ediyor" :
                               e.Status == EnrollmentStatus.Completed ? "Tamamlandı" : "Bırakıldı"
                }).ToList();

                dgv.CellFormatting += (s, e) =>
                {
                    if (dgv.Columns[e.ColumnIndex]?.Name == "Not" && e.Value?.ToString() != "—")
                    {
                        string g = e.Value?.ToString();
                        e.CellStyle.ForeColor = (g == "FF" || g == "FD") ? Color.FromArgb(220, 53, 69) :
                                                (g == "AA" || g == "BA") ? Color.FromArgb(76, 201, 140) : ThemeManager.TextPrimary;
                        e.CellStyle.Font = new Font("Segoe UI Semibold", 9);
                    }
                };

                this.Controls.AddRange(new Control[] { pnlHeader, lblT, dgv });
            }
            catch (Exception ex) { AddError(ex.Message); }
        }

        private void AddError(string msg) =>
            this.Controls.Add(new Label { Text = "⚠️ " + msg, Font = ThemeManager.RegularFont, ForeColor = ThemeManager.DangerButton, Location = new Point(40, 40), AutoSize = true });
    }
}
