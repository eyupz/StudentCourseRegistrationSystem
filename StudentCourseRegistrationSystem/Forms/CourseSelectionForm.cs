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
    /// <summary>
    /// Öğrenci – Ders Seçimi. Çakışma, Ön Koşul ve Kapasite kontrolü içerir.
    /// </summary>
    public partial class CourseSelectionForm : Form
    {
        private DataGridView dgvCourses;
        private ComboBox cmbSemester, cmbDept;
        private TextBox txtSearch;
        private RoundedButton btnEnroll, btnRefresh;

        public CourseSelectionForm()
        {
            this.BackColor = ThemeManager.BackgroundColor;
            this.FormBorderStyle = FormBorderStyle.None;
            this.TopLevel = false;
            this.Dock = DockStyle.Fill;
            BuildUI();
        }

        private void BuildUI()
        {
            // ── Filtre paneli
            var pnlFilter = new Panel { Dock = DockStyle.Top, Height = 130, BackColor = Color.White, Padding = new Padding(25, 20, 25, 15) };
            pnlFilter.Controls.Add(new Panel { Dock = DockStyle.Bottom, Height = 1, BackColor = ThemeManager.BorderColor });

            var filterLayout = new TableLayoutPanel
            {
                Location = new Point(25, 20),
                AutoSize = true,
                ColumnCount = 5,
                RowCount = 2
            };
            for (int i = 0; i < 5; i++) filterLayout.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));

            var lblSem = new Label { Text = "Dönem", Font = ThemeManager.LabelFont, ForeColor = ThemeManager.TextSecondary, AutoSize = true, Margin = new Padding(0, 0, 15, 4) };
            cmbSemester = new ComboBox { Width = 180, DropDownStyle = ComboBoxStyle.DropDownList, Margin = new Padding(0, 0, 25, 0) };
            ThemeManager.StyleComboBox(cmbSemester);

            var lblDept = new Label { Text = "Bölüm", Font = ThemeManager.LabelFont, ForeColor = ThemeManager.TextSecondary, AutoSize = true, Margin = new Padding(0, 0, 15, 4) };
            cmbDept = new ComboBox { Width = 220, DropDownStyle = ComboBoxStyle.DropDownList, Margin = new Padding(0, 0, 25, 0) };
            ThemeManager.StyleComboBox(cmbDept);

            var lblSearch = new Label { Text = "Ders Ara", Font = ThemeManager.LabelFont, ForeColor = ThemeManager.TextSecondary, AutoSize = true, Margin = new Padding(0, 0, 15, 4) };
            txtSearch = new TextBox { Width = 200, Margin = new Padding(0, 0, 25, 0) };
            ThemeManager.StyleTextBox(txtSearch);

            btnRefresh = new RoundedButton { Text = "🔍 Filtrele", Width = 130, Height = 36, BorderRadius = 6, Margin = new Padding(0, 20, 0, 0) };
            btnRefresh.SetColors(ThemeManager.PrimaryButton, ThemeManager.PrimaryButtonHover);
            btnRefresh.Click += (s, e) => LoadCourses();

            filterLayout.Controls.Add(lblSem, 0, 0);  filterLayout.Controls.Add(cmbSemester, 0, 1);
            filterLayout.Controls.Add(lblDept, 1, 0); filterLayout.Controls.Add(cmbDept, 1, 1);
            filterLayout.Controls.Add(lblSearch, 2, 0); filterLayout.Controls.Add(txtSearch, 2, 1);
            filterLayout.Controls.Add(btnRefresh, 3, 1);

            pnlFilter.Controls.Add(filterLayout);

            // ── Kayıt butonu + tablo
            var pnlBottom = new Panel { Dock = DockStyle.Bottom, Height = 65, BackColor = Color.White };
            pnlBottom.Controls.Add(new Panel { Dock = DockStyle.Top, Height = 1, BackColor = ThemeManager.BorderColor });

            btnEnroll = new RoundedButton { Text = "✔ Seçilen Derse Kayıt Ol", Width = 220, Height = 42, BorderRadius = 6 };
            btnEnroll.SetColors(ThemeManager.SuccessButton, ThemeManager.SuccessButtonHover);
            btnEnroll.Location = new Point(25, 12);
            btnEnroll.Click += BtnEnroll_Click;
            pnlBottom.Controls.Add(btnEnroll);

            dgvCourses = new DataGridView();
            ThemeManager.StyleDataGrid(dgvCourses);
            dgvCourses.Dock = DockStyle.Fill;

            var pnlGrid = new Panel { Dock = DockStyle.Fill, Padding = new Padding(25) };
            pnlGrid.Controls.Add(dgvCourses);

            this.Controls.Add(pnlGrid);
            this.Controls.Add(pnlBottom);
            this.Controls.Add(pnlFilter);

            LoadComboBoxes();
            LoadCourses();
        }

        private void LoadComboBoxes()
        {
            try
            {
                using var ctx = new AppDbContext();
                cmbSemester.DataSource = ctx.Semesters.ToList();
                cmbSemester.DisplayMember = "Name";
                cmbSemester.ValueMember = "Id";
                // Aktif dönemi seç
                var active = ctx.Semesters.FirstOrDefault(s => s.IsActive);
                if (active != null) cmbSemester.SelectedValue = active.Id;

                var depts = ctx.Departments.ToList();
                depts.Insert(0, new Models.Department { Id = 0, Name = "Tümü", Code = "" });
                cmbDept.DataSource = depts;
                cmbDept.DisplayMember = "Name";
                cmbDept.ValueMember = "Id";
            }
            catch (Exception ex) { MessageBox.Show(ex.Message, "Hata"); }
        }

        private void LoadCourses()
        {
            try
            {
                using var ctx = new AppDbContext();
                var query = ctx.Courses
                    .Include(c => c.Department)
                    .Include(c => c.Instructor)
                    .Include(c => c.Enrollments)
                    .AsQueryable();

                if (cmbDept.SelectedValue is int deptId && deptId != 0)
                    query = query.Where(c => c.DepartmentId == deptId);

                if (!string.IsNullOrWhiteSpace(txtSearch.Text))
                    query = query.Where(c => c.Title.Contains(txtSearch.Text) || c.CourseCode.Contains(txtSearch.Text));

                int semId = (int)(cmbSemester.SelectedValue ?? 0);

                dgvCourses.DataSource = query.ToList().Select(c => new
                {
                    Id            = c.Id,
                    Kod           = c.CourseCode,
                    DersAdi       = c.Title,
                    Kredi         = c.Credits,
                    Program       = c.Schedule,
                    Kapasite      = c.Capacity,
                    Doluluk       = $"{c.Enrollments.Count(e => e.SemesterId == semId && e.Status == EnrollmentStatus.Enrolled)}/{c.Capacity}",
                    Bölüm         = c.Department?.Name,
                    Eğitmen       = c.Instructor?.FirstName + " " + c.Instructor?.LastName,
                    ÖnKoşul       = c.PrerequisiteCourseId.HasValue ? ctx.Courses.FirstOrDefault(x => x.Id == c.PrerequisiteCourseId)?.CourseCode ?? "-" : "-"
                }).ToList();

                if (dgvCourses.Columns["Id"] != null) dgvCourses.Columns["Id"].Visible = false;
            }
            catch (Exception ex) { MessageBox.Show(ex.Message, "Hata"); }
        }

        private void BtnEnroll_Click(object sender, EventArgs e)
        {
            if (dgvCourses.SelectedRows.Count == 0) { MessageBox.Show("Lütfen bir ders seçin.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }
            if (cmbSemester.SelectedValue == null) return;

            int courseId = (int)dgvCourses.SelectedRows[0].Cells["Id"].Value;
            int semId = (int)cmbSemester.SelectedValue;

            try
            {
                using var ctx = new AppDbContext();
                var studentSvc = new StudentService(ctx);
                var student = studentSvc.GetStudentByUserId(SessionManager.CurrentUser.Id);
                if (student == null) { MessageBox.Show("Öğrenci profili bulunamadı.", "Hata"); return; }

                var svc = new EnrollmentService(ctx);
                svc.Enroll(student.Id, courseId, semId);
                MessageBox.Show("✅ Ders kaydınız başarıyla yapıldı!", "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
                LoadCourses();
            }
            catch (InvalidOperationException ex)
            {
                MessageBox.Show("⚠️ " + ex.Message, "Kayıt Uyarısı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Hata: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
