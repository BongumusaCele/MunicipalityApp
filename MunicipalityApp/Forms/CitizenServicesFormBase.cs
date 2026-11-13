using System;
using System.Drawing;
using System.Windows.Forms;

namespace MunicipalityApp.Forms
{
    internal enum FormNavigationResult
    {
        None,
        Home,
        Back,
        ReportIssues,
        LocalEvents,
        ServiceRequestStatus,
        Exit
    }

    internal abstract class CitizenServicesFormBase : Form
    {
        private bool allowClose;
        private readonly ToolStripStatusLabel statusLabel;

        protected Panel ContentHost { get; private set; }
        protected AppSidebar Sidebar { get; private set; }
        internal FormNavigationResult NavigationResult { get; private set; }

        protected CitizenServicesFormBase(AppPage selectedPage)
        {
            AutoScaleDimensions = new SizeF(96f, 96f);
            AutoScaleMode = AutoScaleMode.Dpi;
            BackColor = UiTheme.PageBackground;
            ClientSize = new Size(1280, 772);
            Font = new Font("Segoe UI", 9f);
            MinimumSize = new Size(1050, 700);
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Midvaal Citizen Services";

            TableLayoutPanel shell = new TableLayoutPanel
            {
                ColumnCount = 2,
                RowCount = 1,
                Dock = DockStyle.Fill,
                Margin = Padding.Empty
            };
            shell.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 292f));
            shell.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100f));
            shell.RowStyles.Add(new RowStyle(SizeType.Percent, 100f));

            Sidebar = new AppSidebar(selectedPage) { Dock = DockStyle.Fill };
            ContentHost = new Panel
            {
                AutoScroll = true,
                BackColor = UiTheme.PageBackground,
                Dock = DockStyle.Fill
            };
            shell.Controls.Add(Sidebar, 0, 0);
            shell.Controls.Add(ContentHost, 1, 0);

            StatusStrip statusStrip = new StatusStrip();
            statusLabel = new ToolStripStatusLabel
            {
                Font = new Font("Segoe UI", 9.5f),
                Text = "Ready",
                Spring = false
            };
            statusStrip.Items.Add(statusLabel);

            Controls.Add(shell);
            Controls.Add(statusStrip);

            Sidebar.HomeRequested += delegate { OnHomeRequested(); };
            Sidebar.ReportIssuesRequested += delegate { OnReportIssuesRequested(); };
            Sidebar.LocalEventsRequested += delegate { OnLocalEventsRequested(); };
            Sidebar.ServiceRequestStatusRequested += delegate { OnServiceRequestStatusRequested(); };
            Sidebar.ExitRequested += delegate { RequestApplicationExit(); };
        }

        protected void SetStatus(string text)
        {
            statusLabel.Text = text;
        }

        protected virtual void OnHomeRequested()
        {
            CloseWithResult(FormNavigationResult.Home);
        }

        protected virtual void OnReportIssuesRequested()
        {
        }

        protected virtual void OnServiceRequestStatusRequested()
        {
            CloseWithResult(FormNavigationResult.ServiceRequestStatus);
        }

        protected virtual void OnLocalEventsRequested()
        {
            CloseWithResult(FormNavigationResult.LocalEvents);
        }

        protected void CloseWithResult(FormNavigationResult result)
        {
            NavigationResult = result;
            allowClose = true;
            Close();
        }

        private void RequestApplicationExit()
        {
            DialogResult answer = MessageBox.Show(
                this,
                "Are you sure you want to exit Midvaal Citizen Services?",
                "Confirm exit",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question,
                MessageBoxDefaultButton.Button2);

            if (answer == DialogResult.Yes)
                CloseWithResult(FormNavigationResult.Exit);
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (!allowClose && e.CloseReason == CloseReason.UserClosing)
            {
                DialogResult answer = MessageBox.Show(
                    this,
                    "Are you sure you want to exit Midvaal Citizen Services?",
                    "Confirm exit",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question,
                    MessageBoxDefaultButton.Button2);

                if (answer != DialogResult.Yes)
                {
                    e.Cancel = true;
                    return;
                }

                NavigationResult = FormNavigationResult.Exit;
                allowClose = true;
            }

            base.OnFormClosing(e);
        }
    }
}
