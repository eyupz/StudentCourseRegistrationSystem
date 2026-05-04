using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using StudentCourseRegistrationSystem.Models;
using StudentCourseRegistrationSystem.Services;

namespace StudentCourseRegistrationSystem.Forms
{
    public class CourseForm : Form
    {
        private readonly CourseService _courseService;
        private DataGridView gridCourses;
        private TextBox txtSearch, txtId, txtCourseCode, txtTitle, txtCredits, txtCapacity, txtDepartmentId, txtInstructorId;
        
        public CourseForm(CourseService courseService)
        {
            _courseService = courseService;
            InitializeComponent();
            LoadData();
        }

        private void InitializeComponent()
        {
            this.Text = "Ders Yönetimi";
            this.Size = new Size(950, 650);
            this.StartPosition = FormStartPosition.CenterParent;
            this.BackColor = Color.FromArgb(244, 246, 249);
            this.Font = new Font("Segoe UI", 10F);

            // Search Panel
            Panel pnlSearch = new Panel() { Dock = DockStyle.Top, Height = 60, Padding = new Padding(15), BackColor = Color.White };
            txtSearch = new TextBox() { Width = 250, Location = new Point(15, 18), Font = new Font("Segoe UI", 11F), BorderStyle = BorderStyle.FixedSingle };
            
            Button btnSearch = CreateButton("🔍 Ara", 280, 15, 100);
            btnSearch.Click += (s, e) => LoadData(txtSearch.Text);
            
            Button btnReset = CreateButton("🔄 Temizle", 390, 15, 100);
            btnReset.Click += (s, e) => { txtSearch.Text = ""; LoadData(); };
            
            pnlSearch.Controls.Add(txtSearch);
            pnlSearch.Controls.Add(btnSearch);
            pnlSearch.Controls.Add(btnReset);

            // Grid
            gridCourses = new DataGridView() { 
                Dock = DockStyle.Fill, 
                SelectionMode = DataGridViewSelectionMode.FullRowSelect, 
                MultiSelect = false,
                ReadOnly = true,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.None,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                EnableHeadersVisualStyles = false,
                RowHeadersVisible = false,
                GridColor = Color.FromArgb(230, 230, 230)
            };
            gridCourses.ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle() { BackColor = Color.FromArgb(67, 97, 238), ForeColor = Color.White, Font = new Font("Segoe UI", 10F, FontStyle.Bold), Padding = new Padding(5) };
            gridCourses.DefaultCellStyle = new DataGridViewCellStyle() { SelectionBackColor = Color.FromArgb(200, 210, 255), SelectionForeColor = Color.Black, Padding = new Padding(5) };
            gridCourses.AlternatingRowsDefaultCellStyle = new DataGridViewCellStyle() { BackColor = Color.FromArgb(245, 245, 245) };
            gridCourses.SelectionChanged += GridCourses_SelectionChanged;

            // Input Panel
            Panel pnlInput = new Panel() { Dock = DockStyle.Bottom, Height = 180, Padding = new Padding(15), BackColor = Color.White };
            
            txtId = new TextBox() { Visible = false };
            
            Label lblCode = new Label() { Text = "Ders Kodu:", Location = new Point(15, 20), AutoSize = true, ForeColor = Color.Gray };
            txtCourseCode = new TextBox() { Location = new Point(120, 18), Width = 150, Font = new Font("Segoe UI", 11F), BorderStyle = BorderStyle.FixedSingle };

            Label lblTitle = new Label() { Text = "Ders Adı:", Location = new Point(15, 60), AutoSize = true, ForeColor = Color.Gray };
            txtTitle = new TextBox() { Location = new Point(120, 58), Width = 150, Font = new Font("Segoe UI", 11F), BorderStyle = BorderStyle.FixedSingle };

            Label lblCredits = new Label() { Text = "Kredi:", Location = new Point(15, 100), AutoSize = true, ForeColor = Color.Gray };
            txtCredits = new TextBox() { Location = new Point(120, 98), Width = 150, Font = new Font("Segoe UI", 11F), BorderStyle = BorderStyle.FixedSingle };

            Label lblCapacity = new Label() { Text = "Kapasite:", Location = new Point(320, 20), AutoSize = true, ForeColor = Color.Gray };
            txtCapacity = new TextBox() { Location = new Point(420, 18), Width = 120, Font = new Font("Segoe UI", 11F), BorderStyle = BorderStyle.FixedSingle };

            Label lblDept = new Label() { Text = "Bölüm ID:", Location = new Point(320, 60), AutoSize = true, ForeColor = Color.Gray };
            txtDepartmentId = new TextBox() { Location = new Point(420, 58), Width = 120, Font = new Font("Segoe UI", 11F), BorderStyle = BorderStyle.FixedSingle };

            Label lblInstructor = new Label() { Text = "Hoca ID:", Location = new Point(320, 100), AutoSize = true, ForeColor = Color.Gray };
            txtInstructorId = new TextBox() { Location = new Point(420, 98), Width = 120, Font = new Font("Segoe UI", 11F), BorderStyle = BorderStyle.FixedSingle };

            Button btnAdd = CreateButton("➕ Ekle", 600, 18, 110);
            btnAdd.BackColor = Color.FromArgb(46, 204, 113);
            btnAdd.Click += BtnAdd_Click;

            Button btnUpdate = CreateButton("✏️ Güncelle", 600, 58, 110);
            btnUpdate.BackColor = Color.FromArgb(243, 156, 18);
            btnUpdate.Click += BtnUpdate_Click;

            Button btnDelete = CreateButton("🗑️ Sil", 600, 98, 110);
            btnDelete.BackColor = Color.FromArgb(231, 76, 60);
            btnDelete.Click += BtnDelete_Click;

            pnlInput.Controls.Add(txtId);
            pnlInput.Controls.Add(lblCode); pnlInput.Controls.Add(txtCourseCode);
            pnlInput.Controls.Add(lblTitle); pnlInput.Controls.Add(txtTitle);
            pnlInput.Controls.Add(lblCredits); pnlInput.Controls.Add(txtCredits);
            pnlInput.Controls.Add(lblCapacity); pnlInput.Controls.Add(txtCapacity);
            pnlInput.Controls.Add(lblDept); pnlInput.Controls.Add(txtDepartmentId);
            pnlInput.Controls.Add(lblInstructor); pnlInput.Controls.Add(txtInstructorId);
            pnlInput.Controls.Add(btnAdd); pnlInput.Controls.Add(btnUpdate); pnlInput.Controls.Add(btnDelete);

            this.Controls.Add(gridCourses);
            this.Controls.Add(pnlInput);
            this.Controls.Add(pnlSearch);
        }

        private Button CreateButton(string text, int x, int y, int width)
        {
            Button btn = new Button() { 
                Text = text, Location = new Point(x, y), Width = width, Height = 35,
                FlatStyle = FlatStyle.Flat, ForeColor = Color.White, BackColor = Color.FromArgb(67, 97, 238),
                Font = new Font("Segoe UI", 10F, FontStyle.Bold), Cursor = Cursors.Hand
            };
            btn.FlatAppearance.BorderSize = 0;
            return btn;
        }

        private void LoadData(string searchTerm = "")
        {
            var data = string.IsNullOrWhiteSpace(searchTerm) 
                ? _courseService.GetAllCourses() 
                : _courseService.SearchCourses(searchTerm);

            gridCourses.DataSource = data.Select(c => new {
                c.Id,
                Kod = c.CourseCode,
                Ad = c.Title,
                Kredi = c.Credits,
                Kapasite = c.Capacity,
                BolumId = c.DepartmentId,
                Bolum = c.Department?.Name,
                HocaId = c.InstructorId,
                Hoca = c.Instructor?.FirstName + " " + c.Instructor?.LastName
            }).ToList();
        }

        private void GridCourses_SelectionChanged(object? sender, EventArgs e)
        {
            if (gridCourses.SelectedRows.Count > 0)
            {
                var row = gridCourses.SelectedRows[0];
                txtId.Text = row.Cells["Id"].Value?.ToString();
                txtCourseCode.Text = row.Cells["Kod"].Value?.ToString();
                txtTitle.Text = row.Cells["Ad"].Value?.ToString();
                txtCredits.Text = row.Cells["Kredi"].Value?.ToString();
                txtCapacity.Text = row.Cells["Kapasite"].Value?.ToString();
                txtDepartmentId.Text = row.Cells["BolumId"].Value?.ToString();
                txtInstructorId.Text = row.Cells["HocaId"].Value?.ToString();
            }
        }

        private void BtnAdd_Click(object? sender, EventArgs e)
        {
            var course = new Course
            {
                CourseCode = txtCourseCode.Text.Trim(),
                Title = txtTitle.Text.Trim(),
                Credits = int.TryParse(txtCredits.Text, out int cred) ? cred : 0,
                Capacity = int.TryParse(txtCapacity.Text, out int cap) ? cap : 0,
                DepartmentId = int.TryParse(txtDepartmentId.Text, out int deptId) ? deptId : 0,
                InstructorId = int.TryParse(txtInstructorId.Text, out int instId) && instId > 0 ? (int?)instId : null
            };

            var result = _courseService.AddCourse(course);
            if (result.Success) { LoadData(); MessageBox.Show(result.Message, "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information); }
            else MessageBox.Show(result.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void BtnUpdate_Click(object? sender, EventArgs e)
        {
            if (!int.TryParse(txtId.Text, out int id))
            {
                MessageBox.Show("Lütfen güncellenecek dersi seçiniz.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var course = new Course
            {
                Id = id,
                CourseCode = txtCourseCode.Text.Trim(),
                Title = txtTitle.Text.Trim(),
                Credits = int.TryParse(txtCredits.Text, out int cred) ? cred : 0,
                Capacity = int.TryParse(txtCapacity.Text, out int cap) ? cap : 0,
                DepartmentId = int.TryParse(txtDepartmentId.Text, out int deptId) ? deptId : 0,
                InstructorId = int.TryParse(txtInstructorId.Text, out int instId) && instId > 0 ? (int?)instId : null
            };

            var result = _courseService.UpdateCourse(course);
            if (result.Success) { LoadData(); MessageBox.Show(result.Message, "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information); }
            else MessageBox.Show(result.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void BtnDelete_Click(object? sender, EventArgs e)
        {
            if (!int.TryParse(txtId.Text, out int id))
            {
                MessageBox.Show("Lütfen silinecek dersi seçiniz.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (MessageBox.Show("Bu dersi silmek istediğinize emin misiniz?", "Onay",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;

            var result = _courseService.DeleteCourse(id);
            if (result.Success) { LoadData(); MessageBox.Show(result.Message, "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information); }
            else MessageBox.Show(result.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}
