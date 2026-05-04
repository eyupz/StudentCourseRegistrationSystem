using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using StudentCourseRegistrationSystem.Models;
using StudentCourseRegistrationSystem.Services;

namespace StudentCourseRegistrationSystem.Forms
{
    public class InstructorForm : Form
    {
        private readonly InstructorService _instructorService;
        private readonly AuthService _authService;
        private DataGridView gridInstructors;
        private TextBox txtSearch;
        private TextBox txtId;
        private TextBox txtFirstName;
        private TextBox txtLastName;

        public InstructorForm(InstructorService instructorService, AuthService authService)
        {
            _instructorService = instructorService;
            _authService = authService;
            InitializeComponent();
            LoadData();
        }

        private void InitializeComponent()
        {
            this.Text = "Öğretmen Yönetimi";
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
            gridInstructors = new DataGridView() { 
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
            gridInstructors.ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle() { BackColor = Color.FromArgb(67, 97, 238), ForeColor = Color.White, Font = new Font("Segoe UI", 10F, FontStyle.Bold), Padding = new Padding(5) };
            gridInstructors.DefaultCellStyle = new DataGridViewCellStyle() { SelectionBackColor = Color.FromArgb(200, 210, 255), SelectionForeColor = Color.Black, Padding = new Padding(5) };
            gridInstructors.AlternatingRowsDefaultCellStyle = new DataGridViewCellStyle() { BackColor = Color.FromArgb(245, 245, 245) };
            gridInstructors.SelectionChanged += GridInstructors_SelectionChanged;

            // Input Panel
            Panel pnlInput = new Panel() { Dock = DockStyle.Bottom, Height = 180, Padding = new Padding(15), BackColor = Color.White };
            
            txtId = new TextBox() { Visible = false };

            Label lblFirst = new Label() { Text = "Adı:", Location = new Point(15, 20), AutoSize = true, ForeColor = Color.Gray };
            txtFirstName = new TextBox() { Location = new Point(110, 18), Width = 200, Font = new Font("Segoe UI", 11F), BorderStyle = BorderStyle.FixedSingle };

            Label lblLast = new Label() { Text = "Soyadı:", Location = new Point(15, 60), AutoSize = true, ForeColor = Color.Gray };
            txtLastName = new TextBox() { Location = new Point(110, 58), Width = 200, Font = new Font("Segoe UI", 11F), BorderStyle = BorderStyle.FixedSingle };

            Button btnAdd = CreateButton("➕ Ekle", 350, 18, 120);
            btnAdd.BackColor = Color.FromArgb(46, 204, 113);
            btnAdd.Click += BtnAdd_Click;

            Button btnUpdate = CreateButton("✏️ Güncelle", 350, 58, 120);
            btnUpdate.BackColor = Color.FromArgb(243, 156, 18);
            btnUpdate.Click += BtnUpdate_Click;

            Button btnDelete = CreateButton("🗑️ Sil", 480, 18, 120);
            btnDelete.BackColor = Color.FromArgb(231, 76, 60);
            btnDelete.Click += BtnDelete_Click;

            Button btnResetPw = CreateButton("🔑 Şifre Sıfırla", 480, 58, 120);
            btnResetPw.BackColor = Color.FromArgb(142, 68, 173);
            btnResetPw.Click += BtnResetPw_Click;

            pnlInput.Controls.Add(txtId);
            pnlInput.Controls.Add(lblFirst); pnlInput.Controls.Add(txtFirstName);
            pnlInput.Controls.Add(lblLast); pnlInput.Controls.Add(txtLastName);
            pnlInput.Controls.Add(btnAdd); pnlInput.Controls.Add(btnUpdate); 
            pnlInput.Controls.Add(btnDelete); pnlInput.Controls.Add(btnResetPw);

            this.Controls.Add(gridInstructors);
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
                ? _instructorService.GetAllInstructors() 
                : _instructorService.SearchInstructors(searchTerm);

            gridInstructors.DataSource = data.Select(i => new {
                i.Id,
                Ad = i.FirstName,
                Soyad = i.LastName
            }).ToList();
        }

        private void GridInstructors_SelectionChanged(object? sender, EventArgs e)
        {
            if (gridInstructors.SelectedRows.Count > 0)
            {
                var row = gridInstructors.SelectedRows[0];
                txtId.Text = row.Cells["Id"].Value?.ToString();
                txtFirstName.Text = row.Cells["Ad"].Value?.ToString();
                txtLastName.Text = row.Cells["Soyad"].Value?.ToString();
            }
        }

        private void BtnAdd_Click(object? sender, EventArgs e)
        {
            try
            {
                var inst = new Instructor
                {
                    FirstName = txtFirstName.Text,
                    LastName = txtLastName.Text
                };
                
                _instructorService.AddInstructor(inst);
                LoadData();
                MessageBox.Show("Öğretmen başarıyla eklendi ve kullanıcı hesabı oluşturuldu.", "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnUpdate_Click(object? sender, EventArgs e)
        {
            try
            {
                if (int.TryParse(txtId.Text, out int id))
                {
                    var inst = new Instructor
                    {
                        Id = id,
                        FirstName = txtFirstName.Text,
                        LastName = txtLastName.Text
                    };

                    _instructorService.UpdateInstructor(inst);
                    LoadData();
                    MessageBox.Show("Öğretmen başarıyla güncellendi.", "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnDelete_Click(object? sender, EventArgs e)
        {
            try
            {
                if (int.TryParse(txtId.Text, out int id))
                {
                    if (MessageBox.Show("Bu öğretmeni silmek istediğinize emin misiniz?", "Onay", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        _instructorService.DeleteInstructor(id);
                        LoadData();
                        MessageBox.Show("Öğretmen silindi.", "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                        // Öğretmen eklenirken kullanıcı hesabı oluşturulur.
                        // Öğretmenle ilişkili kullanıcıyı çekiyoruz.
                        using (var ctx = new Data.AppDbContext())
                        {
                            var user = ctx.Users.FirstOrDefault(u => u.InstructorId == id);
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
    }
}
