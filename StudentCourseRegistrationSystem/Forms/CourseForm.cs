using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using StudentCourseRegistrationSystem.Data;
using StudentCourseRegistrationSystem.Models;
using StudentCourseRegistrationSystem.Services;

namespace StudentCourseRegistrationSystem.Forms
{
    public partial class CourseForm : Form
    {
        private DataGridView dgvCourses;
        private TextBox txtCode, txtTitle, txtCredits, txtCapacity, txtSchedule;
        private ComboBox cmbPrerequisite;

        public CourseForm()
        {
            this.BackColor = ThemeManager.BackgroundColor;
            this.FormBorderStyle = FormBorderStyle.None;
            this.TopLevel = false;
            this.Dock = DockStyle.Fill;
            BuildUI();
            LoadCourses();
        }

        private void BuildUI()
        {
            var pnlTop = new Panel { Dock = DockStyle.Top, Height = 230, BackColor = Color.White };
            pnlTop.Controls.Add(new Panel { Dock = DockStyle.Bottom, Height = 1, BackColor = ThemeManager.BorderColor });

            var grid = new TableLayoutPanel { Location = new Point(30, 25), Width = 1100, AutoSize = true, ColumnCount = 3, RowCount = 4 };
            grid.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33F));
            grid.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33F));
            grid.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33F));

            void AddField(string label, TextBox txt, int col, int row)
            {
                grid.Controls.Add(new Label { Text = label, Font = ThemeManager.LabelFont, ForeColor = ThemeManager.TextSecondary, AutoSize = true, Margin = new Padding(0, 0, 0, 4) }, col, row * 2);
                ThemeManager.StyleTextBox(txt);
                grid.Controls.Add(txt, col, row * 2 + 1);
            }

            txtCode = new TextBox { Width = 300 }; AddField("Ders Kodu", txtCode, 0, 0);
            txtTitle = new TextBox { Width = 300 }; AddField("Ders Adı", txtTitle, 1, 0);
            txtCredits = new TextBox { Width = 300 }; AddField("Kredi (AKTS)", txtCredits, 2, 0);
            txtCapacity = new TextBox { Width = 300 }; AddField("Kontenjan", txtCapacity, 0, 1);
            txtSchedule = new TextBox { Width = 300 }; AddField("Program (Örn: Pzt 09:00-11:00)", txtSchedule, 1, 1);

            grid.Controls.Add(new Label { Text = "Ön Koşul Ders", Font = ThemeManager.LabelFont, ForeColor = ThemeManager.TextSecondary, AutoSize = true, Margin = new Padding(0, 0, 0, 4) }, 2, 2);
            cmbPrerequisite = new ComboBox { Width = 300, DropDownStyle = ComboBoxStyle.DropDownList };
            ThemeManager.StyleComboBox(cmbPrerequisite);
            grid.Controls.Add(cmbPrerequisite, 2, 3);

            var btnFlow = new FlowLayoutPanel { AutoSize = true, FlowDirection = FlowDirection.LeftToRight, Margin = new Padding(0, 15, 0, 0) };
            void MakeBtn(string text, Action onClick, Color color, Color hover)
            {
                var b = new RoundedButton { Text = text, Width = 130, Height = 40, BorderRadius = 6, Margin = new Padding(0, 0, 12, 0) };
                b.SetColors(color, hover); b.Click += (s, e) => onClick(); btnFlow.Controls.Add(b);
            }
            MakeBtn("Ekle", BtnAdd_Click, ThemeManager.SuccessButton, ThemeManager.SuccessButtonHover);
            MakeBtn("Güncelle", BtnUpdate_Click, ThemeManager.PrimaryButton, ThemeManager.PrimaryButtonHover);
            MakeBtn("Sil", BtnDelete_Click, ThemeManager.DangerButton, ThemeManager.DangerButtonHover);

            grid.Controls.Add(btnFlow, 0, 7); grid.SetColumnSpan(btnFlow, 3);
            pnlTop.Controls.Add(grid);

            dgvCourses = new DataGridView();
            ThemeManager.StyleDataGrid(dgvCourses);
            dgvCourses.Dock = DockStyle.Fill;
            dgvCourses.SelectionChanged += (s, e) => PopulateFields();

            var pnlGrid = new Panel { Dock = DockStyle.Fill, Padding = new Padding(25) };
            pnlGrid.Controls.Add(dgvCourses);

            this.Controls.Add(pnlGrid);
            this.Controls.Add(pnlTop);

            LoadPrerequisiteCombo();
        }

        private void LoadPrerequisiteCombo()
        {
            try
            {
                using var ctx = new AppDbContext();
                var courses = ctx.Courses.ToList();
                courses.Insert(0, new Course { Id = 0, CourseCode = "—", Title = "Yok" });
                cmbPrerequisite.DataSource = courses;
                cmbPrerequisite.DisplayMember = "CourseCode";
                cmbPrerequisite.ValueMember = "Id";
            }
            catch { }
        }

        private void LoadCourses()
        {
            try
            {
                using var ctx = new AppDbContext();
                var svc = new CourseService(ctx);
                dgvCourses.DataSource = svc.GetAllCourses().Select(c => new
                {
                    Id       = c.Id,
                    Kod      = c.CourseCode,
                    Adi      = c.Title,
                    Kredi    = c.Credits,
                    Kapasite = c.Capacity,
                    Program  = c.Schedule,
                    Bölüm    = c.Department?.Name ?? "-",
                    ÖnKoşul  = c.PrerequisiteCourseId.HasValue ? ctx.Courses.FirstOrDefault(x => x.Id == c.PrerequisiteCourseId)?.CourseCode ?? "-" : "-"
                }).ToList();
                if (dgvCourses.Columns["Id"] != null) dgvCourses.Columns["Id"].Visible = false;
            }
            catch (Exception ex) { MessageBox.Show(ex.Message, "Hata"); }
        }

        private void PopulateFields()
        {
            if (dgvCourses.SelectedRows.Count == 0) return;
            var row = dgvCourses.SelectedRows[0];
            txtCode.Text = row.Cells["Kod"].Value?.ToString();
            txtTitle.Text = row.Cells["Adi"].Value?.ToString();
            txtCredits.Text = row.Cells["Kredi"].Value?.ToString();
            txtCapacity.Text = row.Cells["Kapasite"].Value?.ToString();
            txtSchedule.Text = row.Cells["Program"].Value?.ToString();
        }

        private void BtnAdd_Click()
        {
            if (!int.TryParse(txtCredits.Text, out int credits)) credits = 3;
            if (!int.TryParse(txtCapacity.Text, out int cap)) cap = 30;

            try
            {
                using var ctx = new AppDbContext();
                var svc = new CourseService(ctx);
                var dept = ctx.Departments.First();
                var instructor = ctx.Instructors.First();
                int? prereq = cmbPrerequisite.SelectedValue is int pid && pid != 0 ? pid : (int?)null;

                svc.AddCourse(new Course
                {
                    CourseCode = txtCode.Text, Title = txtTitle.Text,
                    Credits = credits, Capacity = cap, Schedule = txtSchedule.Text,
                    DepartmentId = dept.Id, InstructorId = instructor.Id,
                    PrerequisiteCourseId = prereq
                });
                LoadCourses(); LoadPrerequisiteCombo();
                MessageBox.Show("Ders eklendi.", "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex) { MessageBox.Show(ex.Message, "Hata"); }
        }

        private void BtnUpdate_Click()
        {
            if (dgvCourses.SelectedRows.Count == 0) return;
            int id = (int)dgvCourses.SelectedRows[0].Cells["Id"].Value;
            if (!int.TryParse(txtCredits.Text, out int credits)) credits = 3;
            if (!int.TryParse(txtCapacity.Text, out int cap)) cap = 30;

            try
            {
                using var ctx = new AppDbContext();
                var svc = new CourseService(ctx);
                var course = svc.GetCourseById(id);
                if (course == null) return;
                course.CourseCode = txtCode.Text; course.Title = txtTitle.Text;
                course.Credits = credits; course.Capacity = cap; course.Schedule = txtSchedule.Text;
                course.PrerequisiteCourseId = cmbPrerequisite.SelectedValue is int pid && pid != 0 ? pid : (int?)null;
                svc.UpdateCourse(course);
                LoadCourses(); LoadPrerequisiteCombo();
            }
            catch (Exception ex) { MessageBox.Show(ex.Message, "Hata"); }
        }

        private void BtnDelete_Click()
        {
            if (dgvCourses.SelectedRows.Count == 0) return;
            int id = (int)dgvCourses.SelectedRows[0].Cells["Id"].Value;
            if (MessageBox.Show("Dersi silmek istiyor musunuz?", "Sil", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;
            try
            {
                using var ctx = new AppDbContext();
                new CourseService(ctx).DeleteCourse(id);
                LoadCourses(); LoadPrerequisiteCombo();
            }
            catch (Exception ex) { MessageBox.Show(ex.Message, "Hata"); }
        }
    }
}
