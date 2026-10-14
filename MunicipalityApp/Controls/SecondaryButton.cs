using System.Drawing;
using System.Windows.Forms;

namespace MunicipalityApp.Controls
{
    internal sealed class SecondaryButton : Button
    {
        public SecondaryButton()
        {
            BackColor = Color.White;
            Cursor = Cursors.Hand;
            FlatStyle = FlatStyle.Flat;
            FlatAppearance.BorderColor = UiTheme.Border;
            FlatAppearance.BorderSize = 1;
            FlatAppearance.MouseOverBackColor = Color.FromArgb(245, 248, 251);
            Font = new Font("Segoe UI", 10f, FontStyle.Regular);
            ForeColor = UiTheme.TextPrimary;
            Height = 40;
            UseVisualStyleBackColor = false;
        }
    }
}
