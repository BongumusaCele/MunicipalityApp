using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace MunicipalityApp.Controls
{
    internal sealed class MetricCard : Control
    {
        private string valueText = "0";
        private string caption = string.Empty;
        private Color accentColor = UiTheme.PrimaryBlue;

        public string ValueText
        {
            get { return valueText; }
            set { valueText = value ?? string.Empty; Invalidate(); }
        }

        public string Caption
        {
            get { return caption; }
            set { caption = value ?? string.Empty; Invalidate(); }
        }

        public Color AccentColor
        {
            get { return accentColor; }
            set { accentColor = value; Invalidate(); }
        }

        public MetricCard()
        {
            DoubleBuffered = true;
            BackColor = Color.White;
            Size = new Size(165, 105);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

            Rectangle bounds = new Rectangle(0, 0, Width - 1, Height - 1);
            using (GraphicsPath path = RoundedPanel.CreateRoundedPath(bounds, 8))
            using (Pen borderPen = new Pen(UiTheme.Border))
            using (SolidBrush backgroundBrush = new SolidBrush(Color.White))
            {
                e.Graphics.FillPath(backgroundBrush, path);
                e.Graphics.DrawPath(borderPen, path);
            }

            using (Font valueFont = new Font("Segoe UI", 20f, FontStyle.Bold))
            using (Font captionFont = new Font("Segoe UI", 11f, FontStyle.Regular))
            using (SolidBrush valueBrush = new SolidBrush(AccentColor))
            using (SolidBrush captionBrush = new SolidBrush(UiTheme.TextPrimary))
            using (StringFormat centered = new StringFormat { Alignment = StringAlignment.Center })
            {
                e.Graphics.DrawString(valueText, valueFont, valueBrush, new RectangleF(0, 17, Width, 38), centered);
                e.Graphics.DrawString(caption, captionFont, captionBrush, new RectangleF(0, 61, Width, 28), centered);
            }
        }
    }
}
