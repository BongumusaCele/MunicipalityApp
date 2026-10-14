using MunicipalityApp.Controls;

namespace MunicipalityApp
{
    partial class MainMenuForm
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.TableLayoutPanel shellLayout;
        private System.Windows.Forms.Panel sidebarPanel;
        private System.Windows.Forms.PictureBox logoPictureBox;
        private System.Windows.Forms.Label brandNameLabel;
        private System.Windows.Forms.Label brandTypeLabel;
        private NavigationButton homeButton;
        private NavigationButton reportIssuesButton;
        private NavigationButton eventsButton;
        private NavigationButton statusButton;
        private NavigationButton exitButton;
        private System.Windows.Forms.Panel mainPanel;
        private System.Windows.Forms.Panel mainContentPanel;
        private System.Windows.Forms.Label welcomeLabel;
        private System.Windows.Forms.Label assistanceLabel;
        private AccentBar accentBar;
        private RoundedPanel featureCard;
        private CitizenServicesIcon citizenServicesIcon;
        private System.Windows.Forms.Label serviceTitleLabel;
        private System.Windows.Forms.Label serviceDescriptionLabel;
        private PrimaryButton viewIssuesButton;
        private MetricCard reportedMetric;
        private MetricCard openMetric;
        private MetricCard resolvedMetric;
        private System.Windows.Forms.StatusStrip applicationStatusStrip;
        private System.Windows.Forms.ToolStripStatusLabel statusLabel;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();

            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.shellLayout = new System.Windows.Forms.TableLayoutPanel();
            this.sidebarPanel = new System.Windows.Forms.Panel();
            this.logoPictureBox = new System.Windows.Forms.PictureBox();
            this.brandNameLabel = new System.Windows.Forms.Label();
            this.brandTypeLabel = new System.Windows.Forms.Label();
            this.homeButton = new NavigationButton();
            this.reportIssuesButton = new NavigationButton();
            this.eventsButton = new NavigationButton();
            this.statusButton = new NavigationButton();
            this.exitButton = new NavigationButton();
            this.mainPanel = new System.Windows.Forms.Panel();
            this.mainContentPanel = new System.Windows.Forms.Panel();
            this.welcomeLabel = new System.Windows.Forms.Label();
            this.assistanceLabel = new System.Windows.Forms.Label();
            this.accentBar = new AccentBar();
            this.featureCard = new RoundedPanel();
            this.citizenServicesIcon = new CitizenServicesIcon();
            this.serviceTitleLabel = new System.Windows.Forms.Label();
            this.serviceDescriptionLabel = new System.Windows.Forms.Label();
            this.viewIssuesButton = new PrimaryButton();
            this.reportedMetric = new MetricCard();
            this.openMetric = new MetricCard();
            this.resolvedMetric = new MetricCard();
            this.applicationStatusStrip = new System.Windows.Forms.StatusStrip();
            this.statusLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.shellLayout.SuspendLayout();
            this.sidebarPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.logoPictureBox)).BeginInit();
            this.mainPanel.SuspendLayout();
            this.mainContentPanel.SuspendLayout();
            this.featureCard.SuspendLayout();
            this.applicationStatusStrip.SuspendLayout();
            this.SuspendLayout();

            this.shellLayout.ColumnCount = 2;
            this.shellLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 292F));
            this.shellLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.shellLayout.Controls.Add(this.sidebarPanel, 0, 0);
            this.shellLayout.Controls.Add(this.mainPanel, 1, 0);
            this.shellLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.shellLayout.Location = new System.Drawing.Point(0, 0);
            this.shellLayout.Margin = new System.Windows.Forms.Padding(0);
            this.shellLayout.Name = "shellLayout";
            this.shellLayout.RowCount = 1;
            this.shellLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));

            this.sidebarPanel.BackColor = System.Drawing.Color.White;
            this.sidebarPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.sidebarPanel.Controls.Add(this.logoPictureBox);
            this.sidebarPanel.Controls.Add(this.brandNameLabel);
            this.sidebarPanel.Controls.Add(this.brandTypeLabel);
            this.sidebarPanel.Controls.Add(this.homeButton);
            this.sidebarPanel.Controls.Add(this.reportIssuesButton);
            this.sidebarPanel.Controls.Add(this.eventsButton);
            this.sidebarPanel.Controls.Add(this.statusButton);
            this.sidebarPanel.Controls.Add(this.exitButton);
            this.sidebarPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.sidebarPanel.Name = "sidebarPanel";
            this.sidebarPanel.Resize += new System.EventHandler(this.SidebarPanel_Resize);

            this.logoPictureBox.Location = new System.Drawing.Point(28, 17);
            this.logoPictureBox.Name = "logoPictureBox";
            this.logoPictureBox.Size = new System.Drawing.Size(234, 130);
            this.logoPictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.logoPictureBox.TabStop = false;

            this.brandNameLabel.Font = new System.Drawing.Font("Segoe UI", 20F, System.Drawing.FontStyle.Bold);
            this.brandNameLabel.ForeColor = UiTheme.TextPrimary;
            this.brandNameLabel.Location = new System.Drawing.Point(15, 142);
            this.brandNameLabel.Name = "brandNameLabel";
            this.brandNameLabel.Size = new System.Drawing.Size(260, 42);
            this.brandNameLabel.Text = "MIDVAAL";
            this.brandNameLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;

            this.brandTypeLabel.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.brandTypeLabel.ForeColor = UiTheme.TextPrimary;
            this.brandTypeLabel.Location = new System.Drawing.Point(15, 178);
            this.brandTypeLabel.Name = "brandTypeLabel";
            this.brandTypeLabel.Size = new System.Drawing.Size(260, 24);
            this.brandTypeLabel.Text = "L O C A L   M U N I C I P A L I T Y";
            this.brandTypeLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;

            this.homeButton.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            this.homeButton.IconKind = NavigationIcon.Home;
            this.homeButton.IsSelected = true;
            this.homeButton.Location = new System.Drawing.Point(14, 224);
            this.homeButton.MainText = "Home";
            this.homeButton.Name = "homeButton";
            this.homeButton.Size = new System.Drawing.Size(262, 61);
            this.homeButton.Click += new System.EventHandler(this.HomeButton_Click);

            this.reportIssuesButton.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            this.reportIssuesButton.IconKind = NavigationIcon.Issues;
            this.reportIssuesButton.Location = new System.Drawing.Point(14, 297);
            this.reportIssuesButton.MainText = "Report Issues";
            this.reportIssuesButton.Name = "reportIssuesButton";
            this.reportIssuesButton.Size = new System.Drawing.Size(262, 61);
            this.reportIssuesButton.Click += new System.EventHandler(this.ReportIssuesButton_Click);

            this.eventsButton.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            this.eventsButton.IconKind = NavigationIcon.Calendar;
            this.eventsButton.Location = new System.Drawing.Point(14, 370);
            this.eventsButton.MainText = "Local Events";
            this.eventsButton.SecondaryText = "";
            this.eventsButton.Name = "eventsButton";
            this.eventsButton.Size = new System.Drawing.Size(262, 61);
            this.eventsButton.Click += new System.EventHandler(this.EventsButton_Click);

            this.statusButton.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            this.statusButton.Enabled = false;
            this.statusButton.IconKind = NavigationIcon.Status;
            this.statusButton.Location = new System.Drawing.Point(14, 443);
            this.statusButton.MainText = "Service Request Status";
            this.statusButton.SecondaryText = "Coming Soon";
            this.statusButton.Name = "statusButton";
            this.statusButton.Size = new System.Drawing.Size(262, 61);

            this.exitButton.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            this.exitButton.IconKind = NavigationIcon.Exit;
            this.exitButton.Location = new System.Drawing.Point(14, 663);
            this.exitButton.MainText = "Exit";
            this.exitButton.Name = "exitButton";
            this.exitButton.Size = new System.Drawing.Size(262, 61);
            this.exitButton.Click += new System.EventHandler(this.ExitButton_Click);

            this.mainPanel.AutoScroll = true;
            this.mainPanel.BackColor = UiTheme.PageBackground;
            this.mainPanel.Controls.Add(this.mainContentPanel);
            this.mainPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mainPanel.Name = "mainPanel";
            this.mainPanel.Resize += new System.EventHandler(this.MainPanel_Resize);

            this.mainContentPanel.BackColor = UiTheme.PageBackground;
            this.mainContentPanel.Controls.Add(this.welcomeLabel);
            this.mainContentPanel.Controls.Add(this.assistanceLabel);
            this.mainContentPanel.Controls.Add(this.accentBar);
            this.mainContentPanel.Controls.Add(this.featureCard);
            this.mainContentPanel.Controls.Add(this.reportedMetric);
            this.mainContentPanel.Controls.Add(this.openMetric);
            this.mainContentPanel.Controls.Add(this.resolvedMetric);
            this.mainContentPanel.Location = new System.Drawing.Point(48, 45);
            this.mainContentPanel.Name = "mainContentPanel";
            this.mainContentPanel.Size = new System.Drawing.Size(950, 600);

            this.welcomeLabel.AutoSize = true;
            this.welcomeLabel.Font = new System.Drawing.Font("Segoe UI", 29F, System.Drawing.FontStyle.Bold);
            this.welcomeLabel.ForeColor = UiTheme.TextPrimary;
            this.welcomeLabel.Location = new System.Drawing.Point(-2, 0);
            this.welcomeLabel.Name = "welcomeLabel";
            this.welcomeLabel.Text = "Welcome to Midvaal Citizen Services";

            this.assistanceLabel.AutoSize = true;
            this.assistanceLabel.Font = new System.Drawing.Font("Segoe UI", 15F, System.Drawing.FontStyle.Regular);
            this.assistanceLabel.ForeColor = UiTheme.TextSecondary;
            this.assistanceLabel.Location = new System.Drawing.Point(1, 66);
            this.assistanceLabel.Name = "assistanceLabel";
            this.assistanceLabel.Text = "How can we assist you today?";

            this.accentBar.Location = new System.Drawing.Point(3, 116);
            this.accentBar.Name = "accentBar";
            this.accentBar.Size = new System.Drawing.Size(947, 5);

            this.featureCard.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            this.featureCard.Controls.Add(this.citizenServicesIcon);
            this.featureCard.Controls.Add(this.serviceTitleLabel);
            this.featureCard.Controls.Add(this.serviceDescriptionLabel);
            this.featureCard.Controls.Add(this.viewIssuesButton);
            this.featureCard.Location = new System.Drawing.Point(0, 160);
            this.featureCard.Name = "featureCard";
            this.featureCard.Size = new System.Drawing.Size(950, 174);

            this.citizenServicesIcon.Location = new System.Drawing.Point(35, 35);
            this.citizenServicesIcon.Name = "citizenServicesIcon";

            this.serviceTitleLabel.AutoSize = true;
            this.serviceTitleLabel.Font = new System.Drawing.Font("Segoe UI", 16F, System.Drawing.FontStyle.Bold);
            this.serviceTitleLabel.ForeColor = UiTheme.TextPrimary;
            this.serviceTitleLabel.Location = new System.Drawing.Point(155, 24);
            this.serviceTitleLabel.Name = "serviceTitleLabel";
            this.serviceTitleLabel.Text = "Citizen Services";

            this.serviceDescriptionLabel.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            this.serviceDescriptionLabel.AutoEllipsis = true;
            this.serviceDescriptionLabel.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.serviceDescriptionLabel.ForeColor = UiTheme.TextPrimary;
            this.serviceDescriptionLabel.Location = new System.Drawing.Point(158, 63);
            this.serviceDescriptionLabel.Name = "serviceDescriptionLabel";
            this.serviceDescriptionLabel.Size = new System.Drawing.Size(750, 27);
            this.serviceDescriptionLabel.Text = "Report municipal problems and help us build a better Midvaal.";

            this.viewIssuesButton.Location = new System.Drawing.Point(158, 104);
            this.viewIssuesButton.Name = "viewIssuesButton";
            this.viewIssuesButton.Size = new System.Drawing.Size(205, 46);
            this.viewIssuesButton.Text = "View Reported Issues";
            this.viewIssuesButton.Click += new System.EventHandler(this.ReportIssuesButton_Click);

            this.reportedMetric.AccentColor = UiTheme.PrimaryBlue;
            this.reportedMetric.Caption = "Reported";
            this.reportedMetric.Location = new System.Drawing.Point(0, 367);
            this.reportedMetric.Name = "reportedMetric";
            this.reportedMetric.ValueText = "0";

            this.openMetric.AccentColor = UiTheme.Green;
            this.openMetric.Caption = "Open";
            this.openMetric.Location = new System.Drawing.Point(194, 367);
            this.openMetric.Name = "openMetric";
            this.openMetric.ValueText = "0";

            this.resolvedMetric.AccentColor = UiTheme.Yellow;
            this.resolvedMetric.Caption = "Resolved";
            this.resolvedMetric.Location = new System.Drawing.Point(388, 367);
            this.resolvedMetric.Name = "resolvedMetric";
            this.resolvedMetric.ValueText = "0";

            this.applicationStatusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { this.statusLabel });
            this.applicationStatusStrip.Location = new System.Drawing.Point(0, 748);
            this.applicationStatusStrip.Name = "applicationStatusStrip";
            this.applicationStatusStrip.Size = new System.Drawing.Size(1280, 24);

            this.statusLabel.Font = new System.Drawing.Font("Segoe UI", 9.5F);
            this.statusLabel.Name = "statusLabel";
            this.statusLabel.Text = "Ready";

            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.BackColor = UiTheme.PageBackground;
            this.ClientSize = new System.Drawing.Size(1280, 772);
            this.Controls.Add(this.shellLayout);
            this.Controls.Add(this.applicationStatusStrip);
            this.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.MinimumSize = new System.Drawing.Size(1050, 700);
            this.Name = "MainMenuForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Midvaal Citizen Services";
            this.shellLayout.ResumeLayout(false);
            this.sidebarPanel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.logoPictureBox)).EndInit();
            this.mainPanel.ResumeLayout(false);
            this.mainContentPanel.ResumeLayout(false);
            this.mainContentPanel.PerformLayout();
            this.featureCard.ResumeLayout(false);
            this.featureCard.PerformLayout();
            this.applicationStatusStrip.ResumeLayout(false);
            this.applicationStatusStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();
        }
    }
}
