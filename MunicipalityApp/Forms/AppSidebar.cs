using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using MunicipalityApp.Controls;

namespace MunicipalityApp.Forms
{
    internal enum AppPage
    {
        Home,
        ReportIssues,
        LocalEvents,
        ServiceRequestStatus
    }

    internal sealed class AppSidebar : UserControl
    {
        private readonly PictureBox logoPictureBox;
        private readonly Label brandNameLabel;
        private readonly Label brandTypeLabel;
        private readonly NavigationButton homeButton;
        private readonly NavigationButton reportIssuesButton;
        private readonly NavigationButton eventsButton;
        private readonly NavigationButton requestStatusButton;
        private readonly NavigationButton exitButton;

        internal event EventHandler HomeRequested;
        internal event EventHandler ReportIssuesRequested;
        internal event EventHandler LocalEventsRequested;
        internal event EventHandler ExitRequested;
        internal event EventHandler ServiceRequestStatusRequested;

        internal AppSidebar(AppPage selectedPage)
        {
            BackColor = Color.White;
            BorderStyle = BorderStyle.FixedSingle;
            MinimumSize = new Size(240, 500);

            logoPictureBox = new PictureBox
            {
                Location = new Point(28, 17),
                Size = new Size(234, 130),
                SizeMode = PictureBoxSizeMode.Zoom,
                TabStop = false
            };

            brandNameLabel = new Label
            {
                Font = new Font("Segoe UI", 20f, FontStyle.Bold),
                ForeColor = UiTheme.TextPrimary,
                Location = new Point(15, 142),
                Size = new Size(260, 42),
                Text = "MIDVAAL",
                TextAlign = ContentAlignment.MiddleCenter
            };

            brandTypeLabel = new Label
            {
                Font = new Font("Segoe UI", 9f, FontStyle.Bold),
                ForeColor = UiTheme.TextPrimary,
                Location = new Point(15, 178),
                Size = new Size(260, 24),
                Text = "L O C A L   M U N I C I P A L I T Y",
                TextAlign = ContentAlignment.MiddleCenter
            };

            homeButton = CreateButton("Home", string.Empty, NavigationIcon.Home, selectedPage == AppPage.Home);
            reportIssuesButton = CreateButton("Report Issues", string.Empty, NavigationIcon.Issues, selectedPage == AppPage.ReportIssues);
            eventsButton = CreateButton("Local Events", string.Empty, NavigationIcon.Calendar, selectedPage == AppPage.LocalEvents);
            requestStatusButton = CreateButton("Service Request Status", string.Empty, NavigationIcon.Status, selectedPage == AppPage.ServiceRequestStatus);
            exitButton = CreateButton("Exit", string.Empty, NavigationIcon.Exit, false);
            requestStatusButton.Click += delegate { if (ServiceRequestStatusRequested != null) ServiceRequestStatusRequested(this, EventArgs.Empty); };

            homeButton.Click += delegate { if (HomeRequested != null) HomeRequested(this, EventArgs.Empty); };
            reportIssuesButton.Click += delegate { if (ReportIssuesRequested != null) ReportIssuesRequested(this, EventArgs.Empty); };
            eventsButton.Click += delegate { if (LocalEventsRequested != null) LocalEventsRequested(this, EventArgs.Empty); };
            exitButton.Click += delegate { if (ExitRequested != null) ExitRequested(this, EventArgs.Empty); };

            Controls.Add(logoPictureBox);
            Controls.Add(brandNameLabel);
            Controls.Add(brandTypeLabel);
            Controls.Add(homeButton);
            Controls.Add(reportIssuesButton);
            Controls.Add(eventsButton);
            Controls.Add(requestStatusButton);
            Controls.Add(exitButton);

            LoadBrandLogo();
            Resize += delegate { LayoutControls(); };
            LayoutControls();
        }

        private static NavigationButton CreateButton(
            string mainText,
            string secondaryText,
            NavigationIcon icon,
            bool selected)
        {
            return new NavigationButton
            {
                MainText = mainText,
                SecondaryText = secondaryText,
                IconKind = icon,
                IsSelected = selected,
                Height = 52,
                AccessibleName = mainText
            };
        }

        private void LayoutControls()
        {
            const int margin = 14;
            const int gap = 10;
            const int buttonHeight = 52;
            int buttonWidth = Math.Max(180, ClientSize.Width - (margin * 2));

            logoPictureBox.Left = Math.Max(margin, (ClientSize.Width - logoPictureBox.Width) / 2);
            brandNameLabel.Left = Math.Max(0, (ClientSize.Width - brandNameLabel.Width) / 2);
            brandTypeLabel.Left = Math.Max(0, (ClientSize.Width - brandTypeLabel.Width) / 2);

            NavigationButton[] buttons =
            {
                homeButton, reportIssuesButton, eventsButton, requestStatusButton, exitButton
            };

            foreach (NavigationButton button in buttons)
            {
                button.Left = margin;
                button.Width = buttonWidth;
                button.Height = buttonHeight;
            }

            homeButton.Top = brandTypeLabel.Bottom + 18;
            reportIssuesButton.Top = homeButton.Bottom + gap;
            eventsButton.Top = reportIssuesButton.Bottom + gap;
            requestStatusButton.Top = eventsButton.Bottom + gap;
            exitButton.Top = Math.Max(requestStatusButton.Bottom + 28, ClientSize.Height - buttonHeight - 20);
        }

        private void LoadBrandLogo()
        {
            string logoPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets", "midvaal-logo.png");
            if (!File.Exists(logoPath))
                return;

            try
            {
                using (FileStream stream = new FileStream(logoPath, FileMode.Open, FileAccess.Read))
                using (Image source = Image.FromStream(stream))
                {
                    logoPictureBox.Image = new Bitmap(source);
                }
            }
            catch (IOException)
            {
                logoPictureBox.Image = null;
            }
            catch (ArgumentException)
            {
                logoPictureBox.Image = null;
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && logoPictureBox.Image != null)
            {
                logoPictureBox.Image.Dispose();
                logoPictureBox.Image = null;
            }

            base.Dispose(disposing);
        }
    }
}
