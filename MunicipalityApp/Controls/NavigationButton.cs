using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace MunicipalityApp.Controls
{
    internal enum NavigationIcon
    {
        Home,
        Issues,
        Calendar,
        Status,
        Exit
    }

    internal sealed class NavigationButton : Button
    {
        private bool isHovered;

        public bool IsSelected { get; set; }
        public string MainText { get; set; } = string.Empty;
        public string SecondaryText { get; set; } = string.Empty;
        public NavigationIcon IconKind { get; set; }

        public NavigationButton()
        {
            Cursor = Cursors.Hand;
            FlatStyle = FlatStyle.Flat;
            FlatAppearance.BorderSize = 0;
            Text = string.Empty;
            Size = new Size(260, 52);
            SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint |
                     ControlStyles.OptimizedDoubleBuffer, true);
        }

        protected override void OnEnabledChanged(EventArgs e)
        {
            Cursor = Enabled ? Cursors.Hand : Cursors.Default;
            Invalidate();
            base.OnEnabledChanged(e);
        }

        protected override void OnMouseEnter(EventArgs e)
        {
            isHovered = true;
            Invalidate();
            base.OnMouseEnter(e);
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            isHovered = false;
            Invalidate();
            base.OnMouseLeave(e);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            Rectangle bounds = new Rectangle(0, 0, Width - 1, Height - 1);
            Color background = IsSelected
                ? UiTheme.PrimaryBlue
                : (isHovered && Enabled ? Color.FromArgb(242, 249, 253) :
                    (Enabled ? Color.White : UiTheme.DisabledBackground));
            Color foreground = IsSelected ? Color.White : (Enabled ? UiTheme.TextPrimary : UiTheme.DisabledText);
            Color border = IsSelected ? UiTheme.PrimaryBlueDark : UiTheme.Border;

            using (GraphicsPath path = RoundedPanel.CreateRoundedPath(bounds, 7))
            using (SolidBrush fill = new SolidBrush(background))
            using (Pen borderPen = new Pen(border))
            {
                e.Graphics.FillPath(fill, path);
                e.Graphics.DrawPath(borderPen, path);
            }

            int iconSize = 27;
            int iconTop = (Height - iconSize) / 2;
            DrawIcon(e.Graphics, new Rectangle(17, iconTop, iconSize, iconSize), foreground);

            using (Font mainFont = new Font("Segoe UI", 10.5f, FontStyle.Regular))
            using (Font secondaryFont = new Font("Segoe UI", 8.5f, FontStyle.Regular))
            using (SolidBrush textBrush = new SolidBrush(foreground))
            using (StringFormat textFormat = new StringFormat
            {
                Alignment = StringAlignment.Near,
                LineAlignment = StringAlignment.Center,
                Trimming = StringTrimming.EllipsisCharacter
            })
            {
                if (string.IsNullOrEmpty(SecondaryText))
                {
                    e.Graphics.DrawString(
                        MainText,
                        mainFont,
                        textBrush,
                        new RectangleF(60, 0, Width - 72, Height),
                        textFormat);
                }
                else
                {
                    textFormat.LineAlignment = StringAlignment.Near;
                    e.Graphics.DrawString(MainText, mainFont, textBrush, new RectangleF(60, 6, Width - 72, 23), textFormat);
                    e.Graphics.DrawString(SecondaryText, secondaryFont, textBrush, new RectangleF(60, 28, Width - 72, 18), textFormat);
                }
            }

            if (Focused && ShowFocusCues)
                ControlPaint.DrawFocusRectangle(e.Graphics, new Rectangle(4, 4, Width - 9, Height - 9), foreground, background);
        }

        private void DrawIcon(Graphics graphics, Rectangle area, Color color)
        {
            using (Pen pen = new Pen(color, 2.5f))
            using (SolidBrush brush = new SolidBrush(color))
            {
                pen.StartCap = LineCap.Round;
                pen.EndCap = LineCap.Round;

                switch (IconKind)
                {
                    case NavigationIcon.Home:
                        Point[] roof = { new Point(area.Left, area.Top + 14), new Point(area.Left + 16, area.Top), new Point(area.Right, area.Top + 14) };
                        graphics.FillPolygon(brush, roof);
                        graphics.FillRectangle(brush, area.Left + 5, area.Top + 13, 22, 18);
                        using (SolidBrush door = new SolidBrush(IsSelected ? UiTheme.PrimaryBlue : Color.White))
                            graphics.FillRectangle(door, area.Left + 13, area.Top + 20, 7, 11);
                        break;
                    case NavigationIcon.Issues:
                        for (int y = 4; y <= 26; y += 11)
                        {
                            graphics.FillEllipse(brush, area.Left, area.Top + y, 6, 6);
                            graphics.DrawLine(pen, area.Left + 11, area.Top + y + 3, area.Right, area.Top + y + 3);
                        }
                        break;
                    case NavigationIcon.Calendar:
                        graphics.DrawRectangle(pen, area.Left + 2, area.Top + 5, 28, 25);
                        graphics.DrawLine(pen, area.Left + 2, area.Top + 12, area.Left + 30, area.Top + 12);
                        graphics.DrawLine(pen, area.Left + 9, area.Top + 1, area.Left + 9, area.Top + 8);
                        graphics.DrawLine(pen, area.Left + 23, area.Top + 1, area.Left + 23, area.Top + 8);
                        break;
                    case NavigationIcon.Status:
                        graphics.DrawRectangle(pen, area.Left + 5, area.Top + 3, 22, 28);
                        graphics.DrawRectangle(pen, area.Left + 11, area.Top, 10, 6);
                        graphics.DrawLine(pen, area.Left + 11, area.Top + 15, area.Left + 15, area.Top + 19);
                        graphics.DrawLine(pen, area.Left + 15, area.Top + 19, area.Left + 23, area.Top + 11);
                        break;
                    case NavigationIcon.Exit:
                        graphics.DrawRectangle(pen, area.Left + 2, area.Top + 2, 19, 28);
                        graphics.DrawLine(pen, area.Left + 14, area.Top + 16, area.Right, area.Top + 16);
                        graphics.DrawLine(pen, area.Right - 7, area.Top + 9, area.Right, area.Top + 16);
                        graphics.DrawLine(pen, area.Right - 7, area.Top + 23, area.Right, area.Top + 16);
                        break;
                }
            }
        }
    }
}
