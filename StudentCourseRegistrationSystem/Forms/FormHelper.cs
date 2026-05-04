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
    /// <summary>
    /// Ortak yardımcı metodlar — tüm formlar bu sınıfı kullanır.
    /// </summary>
    public static class FormHelper
    {
        /// <summary>Label + input çiftini içeren satır paneli oluşturur.</summary>
        public static Panel FieldRow(string labelText, Control input, int labelWidth = 120)
        {
            var row = new Panel { Height = 62, Dock = DockStyle.Top };
            var lbl = new Label
            {
                Text      = labelText,
                Font      = ThemeManager.LabelFont,
                ForeColor = ThemeManager.TextSecondary,
                AutoSize  = false,
                Width     = labelWidth,
                Height    = 20,
                Location  = new Point(0, 6)
            };
            input.Location = new Point(0, 26);
            row.Controls.Add(lbl);
            row.Controls.Add(input);
            return row;
        }

        /// <summary>İçerik alanına tam genişlik beyaz kart ekler.</summary>
        public static Panel ContentCard(string title = null)
        {
            var card = new Panel
            {
                BackColor = ThemeManager.CardBackground,
                Dock      = DockStyle.Fill,
                Padding   = new Padding(28)
            };
            if (!string.IsNullOrEmpty(title))
            {
                var lbl = new Label
                {
                    Text      = title,
                    Font      = ThemeManager.TitleFont,
                    ForeColor = ThemeManager.TextPrimary,
                    AutoSize  = true,
                    Dock      = DockStyle.Top,
                    Height    = 36,
                    Margin    = new Padding(0, 0, 0, 16)
                };
                card.Controls.Add(lbl);
            }
            return card;
        }

        /// <summary>Üst filtre bölümü paneli.</summary>
        public static Panel FilterBar(int height = 90)
        {
            var pnl = new Panel
            {
                Dock      = DockStyle.Top,
                Height    = height,
                BackColor = ThemeManager.CardBackground,
                Padding   = new Padding(24, 16, 24, 12)
            };
            pnl.Controls.Add(new Panel { Dock = DockStyle.Bottom, Height = 1, BackColor = ThemeManager.BorderColor });
            return pnl;
        }

        /// <summary>Alt buton çubuğu.</summary>
        public static Panel ActionBar()
        {
            var pnl = new Panel
            {
                Dock      = DockStyle.Bottom,
                Height    = 64,
                BackColor = ThemeManager.CardBackground,
                Padding   = new Padding(24, 12, 24, 12)
            };
            pnl.Controls.Add(new Panel { Dock = DockStyle.Top, Height = 1, BackColor = ThemeManager.BorderColor });
            return pnl;
        }

        /// <summary>Hata mesajı Label'ı.</summary>
        public static Label ErrorLabel(string msg)
            => new Label { Text = "⚠️  " + msg, Font = ThemeManager.RegularFont, ForeColor = ThemeManager.DangerButton, AutoSize = true, Location = new Point(28, 28) };
    }
}
