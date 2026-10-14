using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace MunicipalityApp.Controls
{
    internal sealed class CitizenServicesIcon : Control
    {
        public CitizenServicesIcon()
        {
            DoubleBuffered = true;
            Size = new Size(92, 92);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

            float scale = System.Math.Min(Width / 100f, Height / 100f);
            using (Pen pen = new Pen(UiTheme.PrimaryBlueDark, 7f * scale))
            using (SolidBrush brush = new SolidBrush(UiTheme.PrimaryBlueDark))
            {
                pen.StartCap = LineCap.Round;
                pen.EndCap = LineCap.Round;

                PointF[] roof =
                {
                    new PointF(50 * scale, 10 * scale),
                    new PointF(92 * scale, 34 * scale),
                    new PointF(8 * scale, 34 * scale)
                };
                e.Graphics.FillPolygon(brush, roof);
                e.Graphics.DrawLine(pen, 10 * scale, 43 * scale, 90 * scale, 43 * scale);

                for (int x = 25; x <= 75; x += 25)
                    e.Graphics.DrawLine(pen, x * scale, 48 * scale, x * scale, 79 * scale);

                e.Graphics.DrawLine(pen, 16 * scale, 84 * scale, 84 * scale, 84 * scale);
                e.Graphics.DrawLine(pen, 9 * scale, 94 * scale, 91 * scale, 94 * scale);
            }
        }
    }
}
