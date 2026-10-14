using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using MunicipalityApp.Controls;
using MunicipalityApp.Data;
using MunicipalityApp.Forms;
using MunicipalityApp.Models;

namespace MunicipalityApp
{
    public partial class MainMenuForm : Form
    {
        private bool canClose;

        public MainMenuForm()
        {
            InitializeComponent();
            LoadBrandLogo();
            LayoutMainContent();
            LayoutSidebar();
            RefreshDashboardMetrics();
        }

        private void LoadBrandLogo()
        {
            string logoPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets", "midvaal-logo.png");

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
                statusLabel.Text = "Ready - logo asset could not be loaded";
            }
            catch (ArgumentException)
            {
                statusLabel.Text = "Ready - logo asset is invalid";
            }
        }

        private void MainPanel_Resize(object sender, EventArgs e)
        {
            LayoutMainContent();
        }

        private void SidebarPanel_Resize(object sender, EventArgs e)
        {
            LayoutSidebar();
        }

        private void LayoutSidebar()
        {
            if (sidebarPanel == null || exitButton == null)
                return;

            const int sideMargin = 14;
            const int buttonHeight = 52;
            const int buttonGap = 10;

            int buttonWidth = Math.Max(180, sidebarPanel.ClientSize.Width - (sideMargin * 2));
            NavigationButton[] buttons =
            {
                homeButton,
                reportIssuesButton,
                eventsButton,
                statusButton,
                exitButton
            };

            foreach (NavigationButton button in buttons)
            {
                button.Left = sideMargin;
                button.Width = buttonWidth;
                button.Height = buttonHeight;
            }

            homeButton.Top = brandTypeLabel.Bottom + 18;
            reportIssuesButton.Top = homeButton.Bottom + buttonGap;
            eventsButton.Top = reportIssuesButton.Bottom + buttonGap;
            statusButton.Top = eventsButton.Bottom + buttonGap;

            int preferredTop = sidebarPanel.ClientSize.Height - exitButton.Height - 20;
            exitButton.Top = Math.Max(statusButton.Bottom + 28, preferredTop);
        }

        private void LayoutMainContent()
        {
            if (mainPanel == null || mainContentPanel == null)
                return;

            const int horizontalMargin = 48;
            int availableWidth = Math.Max(720, mainPanel.ClientSize.Width - (horizontalMargin * 2));
            int contentWidth = Math.Min(1080, availableWidth);

            mainContentPanel.Width = contentWidth;
            mainContentPanel.Left = Math.Max(horizontalMargin, (mainPanel.ClientSize.Width - contentWidth) / 2);
            featureCard.Width = contentWidth;
            accentBar.Width = contentWidth;

            int cardWidth = Math.Min(174, (contentWidth - 40) / 3);
            reportedMetric.Width = cardWidth;
            openMetric.Width = cardWidth;
            resolvedMetric.Width = cardWidth;
            openMetric.Left = reportedMetric.Right + 20;
            resolvedMetric.Left = openMetric.Right + 20;
        }

        private void HomeButton_Click(object sender, EventArgs e)
        {
            statusLabel.Text = "Ready";
            mainPanel.VerticalScroll.Value = 0;
        }

        private void ReportIssuesButton_Click(object sender, EventArgs e)
        {
            statusLabel.Text = "Report Issues selected";
            OpenFeature(FormNavigationResult.ReportIssues);
        }

        private void EventsButton_Click(object sender, EventArgs e)
        {
            statusLabel.Text = "Local Events selected";
            OpenFeature(FormNavigationResult.LocalEvents);
        }

        private void OpenFeature(FormNavigationResult destination)
        {
            Hide();
            while (destination == FormNavigationResult.ReportIssues || destination == FormNavigationResult.LocalEvents)
            {
                if (destination == FormNavigationResult.ReportIssues)
                {
                    using (ReportedIssuesForm form = new ReportedIssuesForm())
                    {
                        form.ShowDialog();
                        destination = form.NavigationResult;
                    }
                }
                else
                {
                    using (LocalEventsForm form = new LocalEventsForm())
                    {
                        form.ShowDialog();
                        destination = form.NavigationResult;
                    }
                }
            }

            if (destination == FormNavigationResult.Exit)
            {
                canClose = true;
                Close();
                return;
            }

            Show();
            RefreshDashboardMetrics();
            statusLabel.Text = "Ready";
        }

        private void RefreshDashboardMetrics()
        {
            System.Collections.Generic.List<MunicipalIssue> issues = IssueRepository.Current.GetAll();
            reportedMetric.ValueText = issues.Count.ToString();
            openMetric.ValueText = issues.Count(issue => issue.Status == IssueStatus.Open).ToString();
            resolvedMetric.ValueText = issues.Count(issue => issue.Status == IssueStatus.Resolved).ToString();
        }

        protected override void OnActivated(EventArgs e)
        {
            base.OnActivated(e);
            RefreshDashboardMetrics();
        }

        private void ExitButton_Click(object sender, EventArgs e)
        {
            Close();
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (!canClose && e.CloseReason == CloseReason.UserClosing)
            {
                DialogResult result = MessageBox.Show(
                    this,
                    "Are you sure you want to exit Midvaal Citizen Services?",
                    "Confirm exit",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question,
                    MessageBoxDefaultButton.Button2);

                if (result != DialogResult.Yes)
                {
                    e.Cancel = true;
                    return;
                }

                canClose = true;
            }

            base.OnFormClosing(e);
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            if (logoPictureBox.Image != null)
                logoPictureBox.Image.Dispose();

            base.OnFormClosed(e);
        }
    }
}
