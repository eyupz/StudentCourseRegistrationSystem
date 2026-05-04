using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using StudentCourseRegistrationSystem.Models;
using StudentCourseRegistrationSystem.Services;

namespace StudentCourseRegistrationSystem.Forms
{
    public class StudentForm : Form
    {
        private readonly StudentService _studentService;
        private readonly AuthService _authService;
        private DataGridView gridStudents;
        private TextBox txtSearch, txtId, txtStudentNumber, txtFirstName, txtLastName, txtDepartmentId;
        
        public StudentForm(StudentService studentService, AuthService authService)
        {
            _studentService = studentService;
            _authService = authService;
            InitializeComponent();
            LoadData();
        }

        private void InitializeComponent()
        {
            this.Text = "Öğrenci Yönetimi";
            this.Size = new Size(850, 650);
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
            gridStudents = new DataGridView() { 
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
            gridStudents.ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle() { BackColor = Color.FromArgb(67, 97, 238), ForeColor = Color.White, Font = new Font("Segoe UI", 10F, FontStyle.Bold), Padding = new Padding(5) };
            gridStudents.DefaultCellStyle = new DataGridViewCellStyle() { SelectionBackColor = Color.FromArgb(200, 210, 255), SelectionForeColor = Color.Black, Padding = new Padding(5) };
            gridStudents.AlternatingRowsDefaultCellStyle = new DataGridViewCellStyle() { BackColor = Color.FromArgb(245, 245, 245) };
            gridStudents.SelectionChanged += GridStudents_SelectionChanged;

            // Input Panel
            Panel pnlInput = new Panel() { Dock = DockStyle.Bottom, Height = 180, Padding = new Padding(15), BackColor = Color.White };
            
            txtId = new TextBox() { Visible = false };
            txtStudentNumber = new TextBox() { Visible = false }; // Güncellemeler için dahili olarak saklanır, UI'da gösterilmez

            Label lblFirst = new Label() { Text = "Adı:", Location = new Point(15, 20), AutoSize = true, ForeColor = Color.Gray };
            txtFirstName = new TextBox() { Location = new Point(110, 18), Width = 200, Font = new Font("Segoe UI", 11F), BorderStyle = BorderStyle.FixedSingle };

            Label lblLast = new Label() { Text = "Soyadı:", Location = new Point(15, 60), AutoSize = true, ForeColor = Color.Gray };
            txtLastName = new TextBox() { Location = new Point(110, 58), Width = 200, Font = new Font("Segoe UI", 11F), BorderStyle = BorderStyle.FixedSingle };

            Label lblDept = new Label() { Text = "Bölüm ID:", Location = new Point(340, 20), AutoSize = true, ForeColor = Color.Gray };
            txtDepartmentId = new TextBox() { Location = new Point(420, 18), Width = 150, Font = new Font("Segoe UI", 11F), BorderStyle = BorderStyle.FixedSingle };

            Button btnAdd = CreateButton("➕ Ekle", 110, 110, 100);
            btnAdd.BackColor = Color.FromArgb(46, 204, 113);
            btnAdd.Click += BtnAdd_Click;

            Button btnUpdate = CreateButton("✏️ Güncelle", 220, 110, 100);
            btnUpdate.BackColor = Color.FromArgb(243, 156, 18);
            btnUpdate.Click += BtnUpdate_Click;

            Button btnDelete = CreateButton("🗑️ Sil", 330, 110, 100);
            btnDelete.BackColor = Color.FromArgb(231, 76, 60);
            btnDelete.Click += BtnDelete_Click;

            Button btnResetPw = CreateButton("🔑 Şifre Sıfırla", 440, 110, 130);
            btnResetPw.BackColor = Color.FromArgb(142, 68, 173);
            btnResetPw.Click += BtnResetPw_Click;

            pnlInput.Controls.Add(txtId);
            pnlInput.Controls.Add(txtStudentNumber);
            pnlInput.Controls.Add(lblFirst); pnlInput.Controls.Add(txtFirstName);
            pnlInput.Controls.Add(lblLast); pnlInput.Controls.Add(txtLastName);
            pnlInput.Controls.Add(lblDept); pnlInput.Controls.Add(txtDepartmentId);
            pnlInput.Controls.Add(btnAdd); pnlInput.Controls.Add(btnUpdate); pnlInput.Controls.Add(btnDelete); pnlInput.Controls.Add(btnResetPw);

            this.Controls.Add(gridStudents);
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
                ? _studentService.GetAllStudents() 
                : _studentService.SearchStudents(searchTerm);

            gridStudents.DataSource = data.Select(s => new {
                s.Id,
                Numara = s.StudentNumber,
                Ad = s.FirstName,
                Soyad = s.LastName,
                DeptId = s.DepartmentId,
                Bolum = s.Department?.Name
            }).ToList();
        }

        private void GridStudents_SelectionChanged(object? sender, EventArgs e)
        {
            if (gridStudents.SelectedRows.Count > 0)
            {
                var row = gridStudents.SelectedRows[0];
                txtId.Text = row.Cells["Id"].Value?.ToString();
                txtStudentNumber.Text = row.Cells["Numara"].Value?.ToString(); // Kept internally
                txtFirstName.Text = row.Cells["Ad"].Value?.ToString();
                txtLastName.Text = row.Cells["Soyad"].Value?.ToString();
                txtDepartmentId.Text = row.Cells["DeptId"].Value?.ToString();
            }
        }

        private void BtnResetPw_Click(object? sender, EventArgs e)
        {
            if (int.TryParse(txtId.Text, out int id))
            {
                var input = Microsoft.VisualBasic.Interaction.InputBox("Yeni şifreyi girin:", "Şifre Sıfırlama", "");
                if (!string.IsNullOrWhiteSpace(input))
                {
                    try
                    {
                        using (var ctx = new Data.AppDbContext())
                        {
                            var user = ctx.Users.FirstOrDefault(u => u.StudentId == id);
                            if (user != null)
                            {
                                _authService.ResetUserPassword(user.Id, input);
                                MessageBox.Show("Şifre başarıyla sıfırlandı.", "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            }
                            else
                            {
                                MessageBox.Show("Kullanıcı hesabı bulunamadı.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void BtnAdd_Click(object? sender, EventArgs e)
        {
            var student = new Student
            {
                FirstName = txtFirstName.Text.Trim(),
                LastName = txtLastName.Text.Trim(),
                DepartmentId = int.TryParse(txtDepartmentId.Text, out int deptId) ? deptId : 0
            };

            var result = _studentService.AddStudent(student);
            if (result.Success)
            {
                LoadData();
                MessageBox.Show(result.Message, "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show(result.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnUpdate_Click(object? sender, EventArgs e)
        {
            if (!int.TryParse(txtId.Text, out int id))
            {
                MessageBox.Show("Lütfen güncellenecek öğrenciyi seçiniz.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var student = new Student
            {
                Id = id,
                StudentNumber = txtStudentNumber.Text,
                FirstName = txtFirstName.Text.Trim(),
                LastName = txtLastName.Text.Trim(),
                DepartmentId = int.TryParse(txtDepartmentId.Text, out int deptId) ? deptId : 0
            };

            var result = _studentService.UpdateStudent(student);
            if (result.Success)
            {
                LoadData();
                MessageBox.Show(result.Message, "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show(result.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnDelete_Click(object? sender, EventArgs e)
        {
            if (!int.TryParse(txtId.Text, out int id))
            {
                MessageBox.Show("Lütfen silinecek öğrenciyi seçiniz.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (MessageBox.Show("Bu öğrenciyi silmek istediğinize emin misiniz?", "Onay",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;

            var result = _studentService.DeleteStudent(id);
            if (result.Success)
            {
                LoadData();
                MessageBox.Show(result.Message, "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show(result.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
