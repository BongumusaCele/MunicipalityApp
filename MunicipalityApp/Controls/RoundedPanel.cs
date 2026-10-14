using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace MunicipalityApp.Controls
{
    internal class RoundedPanel : Panel
    {
        [DefaultValue(10)]
        public int CornerRadius { get; set; } = 10;

        public Color BorderColor { get; set; } = UiTheme.Border;

        [DefaultValue(1)]
        public int BorderThickness { get; set; } = 1;

        public RoundedPanel()
        {
            DoubleBuffered = true;
            BackColor = Color.White;
            ResizeRedraw = true;
        }

        internal static GraphicsPath CreateRoundedPath(Rectangle bounds, int radius)
        {
            GraphicsPath path = new GraphicsPath();
            int diameter = Math.Max(2, Math.Min(radius * 2, Math.Min(bounds.Width, bounds.Height)));
            Rectangle arc = new Rectangle(bounds.Location, new Size(diameter, diameter));

            path.AddArc(arc, 180, 90);
            arc.X = bounds.Right - diameter;
            path.AddArc(arc, 270, 90);
            arc.Y = bounds.Bottom - diameter;
            path.AddArc(arc, 0, 90);
            arc.X = bounds.Left;
            path.AddArc(arc, 90, 90);
            path.CloseFigure();
            return path;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

            Rectangle bounds = new Rectangle(0, 0, Width - 1, Height - 1);
            using (GraphicsPath path = CreateRoundedPath(bounds, CornerRadius))
            using (Pen pen = new Pen(BorderColor, BorderThickness))
            {
                e.Graphics.DrawPath(pen, path);
            }
        }
    }
}
