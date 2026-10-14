using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace MunicipalityApp.Controls
{
    internal sealed class PrimaryButton : Button
    {
        private bool isHovered;

        public PrimaryButton()
        {
            Cursor = Cursors.Hand;
            FlatStyle = FlatStyle.Flat;
            FlatAppearance.BorderSize = 0;
            Font = new Font("Segoe UI", 11f, FontStyle.Bold);
            ForeColor = Color.White;
            BackColor = UiTheme.PrimaryBlueDark;
            Size = new Size(205, 46);
            SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint |
                     ControlStyles.OptimizedDoubleBuffer, true);
        }

        protected override void OnMouseEnter(System.EventArgs e)
        {
            isHovered = true;
            Invalidate();
            base.OnMouseEnter(e);
        }

        protected override void OnMouseLeave(System.EventArgs e)
        {
            isHovered = false;
            Invalidate();
            base.OnMouseLeave(e);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            Rectangle bounds = new Rectangle(0, 0, Width - 1, Height - 1);
            using (GraphicsPath path = RoundedPanel.CreateRoundedPath(bounds, 6))
            using (SolidBrush brush = new SolidBrush(isHovered ? UiTheme.PrimaryBlue : UiTheme.PrimaryBlueDark))
            {
                e.Graphics.FillPath(brush, path);
            }

            TextRenderer.DrawText(e.Graphics, Text, Font, ClientRectangle, ForeColor,
                TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.SingleLine);
        }
    }
}
