using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using StudentCourseRegistrationSystem.Data;
using StudentCourseRegistrationSystem.Services;

namespace StudentCourseRegistrationSystem.Forms
{
    public partial class ReportForm : Form
    {
        private DataGridView dgvReports;
        private RoundedButton btnStudentReport, btnCourseReport;

        public ReportForm()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.BackColor = ThemeManager.BackgroundColor;
            this.FormBorderStyle = FormBorderStyle.None;
            this.TopLevel = false;
            this.Dock = DockStyle.Fill;

            var pnlTop = new Panel { Dock = DockStyle.Top, Height = 100, BackColor = Color.White };
            var borderBottom = new Panel { Dock = DockStyle.Bottom, Height = 1, BackColor = ThemeManager.BorderColor };
            pnlTop.Controls.Add(borderBottom);

            var pnlButtons = new FlowLayoutPanel { Location = new Point(30, 30), AutoSize = true, FlowDirection = FlowDirection.LeftToRight };

            btnStudentReport = new RoundedButton { Text = "Öğrenci Raporu", Width = 180, Height = 45, BorderRadius = 6, Margin = new Padding(0, 0, 15, 0) };
            btnStudentReport.SetColors(ThemeManager.PrimaryButton, ThemeManager.PrimaryButtonHover);
            
            btnCourseReport = new RoundedButton { Text = "Ders Raporu", Width = 180, Height = 45, BorderRadius = 6 };
            btnCourseReport.SetColors(ThemeManager.PrimaryButton, ThemeManager.PrimaryButtonHover);

            btnStudentReport.Click += BtnStudentReport_Click;
            btnCourseReport.Click += BtnCourseReport_Click;

            pnlButtons.Controls.Add(btnStudentReport);
            pnlButtons.Controls.Add(btnCourseReport);
            pnlTop.Controls.Add(pnlButtons);

            var pnlGridWrapper = new Panel { Dock = DockStyle.Fill, Padding = new Padding(30) };
            var pnlGridBg = new Panel { Dock = DockStyle.Fill, BackColor = Color.White, Padding = new Padding(1) };
            pnlGridBg.Paint += (s, e) => { ControlPaint.DrawBorder(e.Graphics, pnlGridBg.ClientRectangle, ThemeManager.BorderColor, ButtonBorderStyle.Solid); };
            
            dgvReports = new DataGridView();
            ThemeManager.StyleDataGrid(dgvReports);
            dgvReports.Dock = DockStyle.Fill;
            
            pnlGridBg.Controls.Add(dgvReports);
            pnlGridWrapper.Controls.Add(pnlGridBg);

            this.Controls.Add(pnlGridWrapper);
            this.Controls.Add(pnlTop);
        }

        private void BtnStudentReport_Click(object? sender, EventArgs e)
        {
            try
            {
                using (var context = new AppDbContext())
                {
                    var service = new ReportService(context);
                    var report = service.GetStudentReport();
                    
                    dgvReports.DataSource = report.Select(r => new {
                        ÖğrenciNo = r.StudentNumber,
                        AdSoyad = r.FullName,
                        Bölüm = r.DepartmentName,
                        KayıtlıDersSayısı = r.TotalEnrolledCourses
                    }).ToList();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Rapor yüklenirken hata oluştu: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnCourseReport_Click(object? sender, EventArgs e)
        {
            try
            {
                using (var context = new AppDbContext())
                {
                    var service = new ReportService(context);
                    var report = service.GetCourseReport(null);
                    
                    dgvReports.DataSource = report.Select(r => new {
                        DersKodu = r.CourseCode,
                        DersAdı = r.Title,
                        Eğitmen = r.InstructorName,
                        Kapasite = r.Capacity,
                        MevcutKayıt = r.CurrentEnrollment,
                        BoşKontenjan = r.AvailableSpots
                    }).ToList();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Rapor yüklenirken hata oluştu: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
