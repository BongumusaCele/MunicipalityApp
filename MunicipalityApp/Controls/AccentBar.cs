using System.Drawing;
using System.Windows.Forms;

namespace MunicipalityApp.Controls
{
    internal sealed class AccentBar : Control
    {
        public AccentBar()
        {
            DoubleBuffered = true;
            Height = 5;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            int first = Width / 3;
            int second = Width / 3;
            using (SolidBrush blueBrush = new SolidBrush(UiTheme.PrimaryBlue))
            using (SolidBrush greenBrush = new SolidBrush(UiTheme.Green))
            using (SolidBrush yellowBrush = new SolidBrush(UiTheme.Yellow))
            {
                e.Graphics.FillRectangle(blueBrush, 0, 0, first, Height);
                e.Graphics.FillRectangle(greenBrush, first, 0, second, Height);
                e.Graphics.FillRectangle(yellowBrush, first + second, 0, Width - first - second, Height);
            }
        }
    }
}
