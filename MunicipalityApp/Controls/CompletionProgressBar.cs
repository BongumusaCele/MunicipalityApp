using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace MunicipalityApp.Controls
{
    internal sealed class CompletionProgressBar : Control
    {
        private int value;

        public int Value
        {
            get { return value; }
            set
            {
                this.value = Math.Max(0, Math.Min(100, value));
                Invalidate();
            }
        }

        public CompletionProgressBar()
        {
            DoubleBuffered = true;
            Height = 13;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            Rectangle bounds = new Rectangle(0, 0, Width - 1, Height - 1);

            using (GraphicsPath track = RoundedPanel.CreateRoundedPath(bounds, Height / 2))
            using (SolidBrush trackBrush = new SolidBrush(Color.FromArgb(226, 230, 235)))
            {
                e.Graphics.FillPath(trackBrush, track);
            }

            int fillWidth = (int)Math.Round((Width - 1) * (value / 100d));
            if (fillWidth > 1)
            {
                Rectangle fillBounds = new Rectangle(0, 0, fillWidth, Height - 1);
                using (GraphicsPath fill = RoundedPanel.CreateRoundedPath(fillBounds, Height / 2))
                using (SolidBrush fillBrush = new SolidBrush(UiTheme.PrimaryBlueDark))
                {
                    e.Graphics.FillPath(fillBrush, fill);
                }
            }
        }
    }
}
