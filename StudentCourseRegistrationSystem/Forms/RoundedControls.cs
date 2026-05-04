using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace StudentCourseRegistrationSystem.Forms
{
    public class RoundedButton : Button
    {
        private int _borderRadius = 8;
        private Color _hoverColor;
        private Color _normalColor;
        private bool _isHovering = false;

        [System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Hidden)]
        public int BorderRadius
        {
            get { return _borderRadius; }
            set { _borderRadius = value; this.Invalidate(); }
        }

        public RoundedButton()
        {
            this.FlatStyle = FlatStyle.Flat;
            this.FlatAppearance.BorderSize = 0;
            this.BackColor = ThemeManager.PrimaryButton;
            this.ForeColor = Color.White;
            this._normalColor = this.BackColor;
            this._hoverColor = ThemeManager.PrimaryButtonHover;
            this.Font = ThemeManager.RegularFont;
            this.Cursor = Cursors.Hand;
            
            this.MouseEnter += (s, e) => { _isHovering = true; this.Invalidate(); };
            this.MouseLeave += (s, e) => { _isHovering = false; this.Invalidate(); };
        }

        public void SetColors(Color normal, Color hover)
        {
            this.BackColor = normal;
            this._normalColor = normal;
            this._hoverColor = hover;
            this.Invalidate();
        }

        protected override void OnPaint(PaintEventArgs pevent)
        {
            Graphics g = pevent.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            Rectangle rect = new Rectangle(0, 0, this.Width, this.Height);
            using (GraphicsPath path = GetRoundedPath(rect, _borderRadius))
            {
                this.Region = new Region(path);
                
                Color fill = _isHovering ? _hoverColor : _normalColor;
                using (SolidBrush brush = new SolidBrush(fill))
                {
                    g.FillPath(brush, path);
                }

                TextRenderer.DrawText(g, this.Text, this.Font, rect, this.ForeColor, TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
            }
        }

        private GraphicsPath GetRoundedPath(Rectangle rect, int radius)
        {
            GraphicsPath path = new GraphicsPath();
            int curveSize = radius * 2;
            path.AddArc(rect.X, rect.Y, curveSize, curveSize, 180, 90);
            path.AddArc(rect.Right - curveSize, rect.Y, curveSize, curveSize, 270, 90);
            path.AddArc(rect.Right - curveSize - 1, rect.Bottom - curveSize - 1, curveSize, curveSize, 0, 90);
            path.AddArc(rect.X, rect.Bottom - curveSize - 1, curveSize, curveSize, 90, 90);
            path.CloseFigure();
            return path;
        }
    }
}
