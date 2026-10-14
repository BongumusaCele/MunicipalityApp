using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace MunicipalityApp.Controls
{
    internal sealed class WatermarkTextBox : TextBox
    {
        private const int EmSetCueBanner = 0x1501;
        private string watermarkText = string.Empty;

        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        private static extern IntPtr SendMessage(IntPtr handle, int message, IntPtr parameter, string text);

        public string WatermarkText
        {
            get { return watermarkText; }
            set
            {
                watermarkText = value ?? string.Empty;
                ApplyWatermark();
            }
        }

        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);
            ApplyWatermark();
        }

        private void ApplyWatermark()
        {
            if (IsHandleCreated)
                SendMessage(Handle, EmSetCueBanner, (IntPtr)1, watermarkText);
        }
    }
}
