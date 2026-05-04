using System.Drawing;
using System.Windows.Forms;

namespace StudentCourseRegistrationSystem.Forms
{
    public static class ThemeManager
    {
        // ─── Renk Paleti ────────────────────────────────────────────────────────
        public static readonly Color BackgroundColor    = Color.FromArgb(245, 247, 250);
        public static readonly Color SidebarColor       = Color.FromArgb(24, 32, 56);
        public static readonly Color SidebarHover       = Color.FromArgb(38, 50, 80);
        public static readonly Color SidebarActive      = Color.FromArgb(59, 130, 246);
        public static readonly Color TextPrimary        = Color.FromArgb(15, 23, 42);
        public static readonly Color TextSecondary      = Color.FromArgb(100, 116, 139);
        public static readonly Color TextLight          = Color.FromArgb(200, 210, 230);
        public static readonly Color PrimaryButton      = Color.FromArgb(59, 130, 246);
        public static readonly Color PrimaryButtonHover = Color.FromArgb(37, 99, 235);
        public static readonly Color SuccessButton      = Color.FromArgb(16, 185, 129);
        public static readonly Color SuccessButtonHover = Color.FromArgb(5, 150, 105);
        public static readonly Color DangerButton       = Color.FromArgb(239, 68, 68);
        public static readonly Color DangerButtonHover  = Color.FromArgb(185, 28, 28);
        public static readonly Color BorderColor        = Color.FromArgb(226, 232, 240);
        public static readonly Color InputBackground    = Color.FromArgb(248, 250, 252);
        public static readonly Color CardBackground     = Color.White;

        // ─── Tipografi ───────────────────────────────────────────────────────────
        public static readonly Font HeaderFont  = new Font("Segoe UI", 18, FontStyle.Bold);
        public static readonly Font TitleFont   = new Font("Segoe UI", 13, FontStyle.Bold);
        public static readonly Font RegularFont = new Font("Segoe UI", 10, FontStyle.Regular);
        public static readonly Font LabelFont   = new Font("Segoe UI", 9,  FontStyle.Regular);
        public static readonly Font SmallFont   = new Font("Segoe UI", 8,  FontStyle.Regular);

        // ─── DataGridView Stillemesi ─────────────────────────────────────────────
        public static void StyleDataGrid(DataGridView dgv)
        {
            dgv.BackgroundColor    = CardBackground;
            dgv.BorderStyle        = BorderStyle.None;
            dgv.CellBorderStyle    = DataGridViewCellBorderStyle.SingleHorizontal;
            dgv.GridColor          = BorderColor;
            dgv.RowHeadersVisible  = false;
            dgv.AllowUserToAddRows = false;
            dgv.ReadOnly           = true;
            dgv.SelectionMode      = DataGridViewSelectionMode.FullRowSelect;
            dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            dgv.DefaultCellStyle.Font             = RegularFont;
            dgv.DefaultCellStyle.BackColor        = CardBackground;
            dgv.DefaultCellStyle.ForeColor        = TextPrimary;
            dgv.DefaultCellStyle.SelectionBackColor = Color.FromArgb(239, 246, 255);
            dgv.DefaultCellStyle.SelectionForeColor = PrimaryButton;
            dgv.DefaultCellStyle.Padding          = new Padding(8, 4, 8, 4);

            dgv.EnableHeadersVisualStyles    = false;
            dgv.ColumnHeadersBorderStyle     = DataGridViewHeaderBorderStyle.None;
            dgv.ColumnHeadersHeight          = 44;
            dgv.ColumnHeadersDefaultCellStyle.BackColor        = Color.FromArgb(248, 250, 252);
            dgv.ColumnHeadersDefaultCellStyle.ForeColor        = TextSecondary;
            dgv.ColumnHeadersDefaultCellStyle.Font             = new Font("Segoe UI", 9, FontStyle.Bold);
            dgv.ColumnHeadersDefaultCellStyle.SelectionBackColor = Color.FromArgb(248, 250, 252);
            dgv.ColumnHeadersDefaultCellStyle.Padding          = new Padding(8, 4, 8, 4);

            dgv.RowTemplate.Height = 44;
        }

        // ─── TextBox Stillemesi ──────────────────────────────────────────────────
        public static void StyleTextBox(TextBox txt)
        {
            txt.Font        = RegularFont;
            txt.BackColor   = InputBackground;
            txt.ForeColor   = TextPrimary;
            txt.BorderStyle = BorderStyle.FixedSingle;
            txt.Height      = 34;
        }

        // ─── ComboBox Stillemesi ─────────────────────────────────────────────────
        public static void StyleComboBox(ComboBox cmb)
        {
            cmb.Font          = RegularFont;
            cmb.BackColor     = InputBackground;
            cmb.ForeColor     = TextPrimary;
            cmb.FlatStyle     = FlatStyle.Flat;
            cmb.DropDownStyle = ComboBoxStyle.DropDownList;
            cmb.Height        = 34;
        }

        // ─── Kart Paneli Oluşturma ───────────────────────────────────────────────
        public static Panel MakeCard(int width, int height, Color? accent = null)
        {
            var card = new Panel
            {
                Size      = new System.Drawing.Size(width, height),
                BackColor = CardBackground,
                Margin    = new Padding(0, 0, 20, 20)
            };
            card.Paint += (s, e) =>
                ControlPaint.DrawBorder(e.Graphics, card.ClientRectangle, BorderColor, ButtonBorderStyle.Solid);

            if (accent.HasValue)
            {
                var bar = new Panel { Dock = DockStyle.Top, Height = 4, BackColor = accent.Value };
                card.Controls.Add(bar);
            }
            return card;
        }
    }
}
