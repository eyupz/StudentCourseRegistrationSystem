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
    public partial class EnrollmentForm : Form
    {
        private DataGridView dgv;
        private ComboBox cmbStudent, cmbCourse, cmbSemester;

        public EnrollmentForm()
        {
            this.BackColor = ThemeManager.BackgroundColor; this.FormBorderStyle = FormBorderStyle.None;
            this.TopLevel = false; this.Dock = DockStyle.Fill;
            Build(); LoadCombos(); LoadData();
        }

        private void Build()
        {
            var bar = FormHelper.FilterBar(120);
            var flow = new FlowLayoutPanel { Dock = DockStyle.Fill, FlowDirection = FlowDirection.LeftToRight, WrapContents = false };

            void Field(string lbl, ComboBox cmb, int w = 230)
            {
                var col = new Panel { AutoSize = false, Width = w, Height = 80, Margin = new Padding(0, 0, 20, 0) };
                col.Controls.Add(new Label { Text = lbl, Font = ThemeManager.LabelFont, ForeColor = ThemeManager.TextSecondary, AutoSize = true, Location = new Point(0, 0) });
                cmb.Width = w - 8; cmb.Location = new Point(0, 22);
                col.Controls.Add(cmb); flow.Controls.Add(col);
            }

            cmbStudent  = new ComboBox(); ThemeManager.StyleComboBox(cmbStudent);
            cmbCourse   = new ComboBox(); ThemeManager.StyleComboBox(cmbCourse);
            cmbSemester = new ComboBox(); ThemeManager.StyleComboBox(cmbSemester);

            Field("Öğrenci", cmbStudent,  240);
            Field("Ders",    cmbCourse,   260);
            Field("Dönem",   cmbSemester, 200);

            var btnCol = new Panel { AutoSize = false, Width = 320, Height = 80 };
            btnCol.Controls.Add(new Label { Text = "İşlem", Font = ThemeManager.LabelFont, ForeColor = ThemeManager.TextSecondary, AutoSize = true, Location = new Point(0, 0) });
            var btnFlow = new FlowLayoutPanel { Location = new Point(0, 22), AutoSize = true };

            var btnEnroll = new RoundedButton { Text = "✔ Kayıt Yap",  Width = 130, Height = 34, BorderRadius = 6, Margin = new Padding(0, 0, 10, 0) };
            btnEnroll.SetColors(ThemeManager.SuccessButton, ThemeManager.SuccessButtonHover);
            btnEnroll.Click += BtnEnroll_Click;

            var btnDrop = new RoundedButton { Text = "✖ Dersi Bırak", Width = 140, Height = 34, BorderRadius = 6 };
            btnDrop.SetColors(ThemeManager.DangerButton, ThemeManager.DangerButtonHover);
            btnDrop.Click += BtnDrop_Click;

            btnFlow.Controls.Add(btnEnroll);
            btnFlow.Controls.Add(btnDrop);
            btnCol.Controls.Add(btnFlow);
            flow.Controls.Add(btnCol);
            bar.Controls.Add(flow);

            dgv = new DataGridView(); ThemeManager.StyleDataGrid(dgv); dgv.Dock = DockStyle.Fill;

            var pnlGrid = new Panel { Dock = DockStyle.Fill, BackColor = ThemeManager.BackgroundColor, Padding = new Padding(16) };
            pnlGrid.Controls.Add(dgv);

            this.Controls.Add(pnlGrid);
            this.Controls.Add(bar);
        }

        private void LoadCombos()
        {
            try
            {
                using var ctx = new AppDbContext();
                var students = ctx.Students.ToList();
                cmbStudent.DataSource = students; cmbStudent.DisplayMember = "FirstName"; cmbStudent.ValueMember = "Id";

                var courses = ctx.Courses.ToList();
                cmbCourse.DataSource = courses; cmbCourse.DisplayMember = "Title"; cmbCourse.ValueMember = "Id";

                var semesters = ctx.Semesters.ToList();
                cmbSemester.DataSource = semesters; cmbSemester.DisplayMember = "Name"; cmbSemester.ValueMember = "Id";

                var active = semesters.FirstOrDefault(s => s.IsActive);
                if (active != null) cmbSemester.SelectedValue = active.Id;
            }
            catch (Exception ex) { ErrorHelper.Show(ex, "Combolar yüklenirken"); }
        }

        private void LoadData()
        {
            try
            {
                using var ctx = new AppDbContext();
                dgv.DataSource = new EnrollmentService(ctx).GetAllEnrollments().Select(e => new {
                    Id      = e.Id,
                    Öğrenci = $"{e.Student?.FirstName} {e.Student?.LastName}",
                    Ders    = e.Course?.Title,
                    Dönem   = e.Semester?.Name,
                    Not     = e.Grade ?? "—",
                    Durum   = e.Status == EnrollmentStatus.Enrolled ? "Devam" :
                              e.Status == EnrollmentStatus.Completed ? "Tamamlandı" : "Bırakıldı"
                }).ToList();
                if (dgv.Columns["Id"] != null) dgv.Columns["Id"].Visible = false;
            }
            catch (Exception ex) { ErrorHelper.Show(ex, "Kayıtlar yüklenirken"); }
        }

        private void BtnEnroll_Click(object sender, EventArgs e)
        {
            if (cmbStudent.SelectedValue == null || cmbCourse.SelectedValue == null || cmbSemester.SelectedValue == null)
            { ErrorHelper.ShowWarning("Öğrenci, ders ve dönem seçmelisiniz."); return; }
            try
            {
                using var ctx = new AppDbContext();
                new EnrollmentService(ctx).Enroll((int)cmbStudent.SelectedValue, (int)cmbCourse.SelectedValue, (int)cmbSemester.SelectedValue);
                LoadData();
                MessageBox.Show("Kayıt başarılı.", "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (InvalidOperationException ex) { ErrorHelper.ShowWarning(ex.Message); }
            catch (Exception ex) { ErrorHelper.Show(ex, "Kayıt yapılırken"); }
        }

        private void BtnDrop_Click(object sender, EventArgs e)
        {
            if (dgv.SelectedRows.Count == 0 || !int.TryParse(dgv.SelectedRows[0].Cells["Id"].Value?.ToString(), out int id)) return;
            if (MessageBox.Show("Dersi bırakmak istiyor musunuz?", "Onay", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;
            try { using var ctx = new AppDbContext(); new EnrollmentService(ctx).Drop(id); LoadData(); }
            catch (Exception ex) { ErrorHelper.Show(ex, "Ders bırakılırken"); }
        }
    }
}
