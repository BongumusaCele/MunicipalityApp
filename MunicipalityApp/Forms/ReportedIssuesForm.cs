using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using MunicipalityApp.Controls;
using MunicipalityApp.Data;
using MunicipalityApp.Models;

namespace MunicipalityApp.Forms
{
    internal sealed class ReportedIssuesForm : CitizenServicesFormBase
    {
        private readonly IssueRepository repository;
        private readonly Panel contentPanel;
        private readonly Label titleLabel;
        private readonly Label subtitleLabel;
        private readonly PrimaryButton reportNewButton;
        private readonly AccentBar accentBar;
        private readonly MetricCard totalMetric;
        private readonly MetricCard openMetric;
        private readonly MetricCard progressMetric;
        private readonly MetricCard resolvedMetric;
        private readonly WatermarkTextBox searchTextBox;
        private readonly ComboBox categoryComboBox;
        private readonly DataGridView issuesGrid;
        private List<MunicipalIssue> allIssues;

        internal ReportedIssuesForm()
            : base(AppPage.ReportIssues)
        {
            repository = IssueRepository.Current;
            allIssues = new List<MunicipalIssue>();

            contentPanel = new Panel
            {
                BackColor = UiTheme.PageBackground,
                Location = new Point(44, 35),
                Size = new Size(900, 670)
            };

            titleLabel = new Label
            {
                AutoSize = true,
                Font = new Font("Segoe UI", 29f, FontStyle.Bold),
                ForeColor = UiTheme.TextPrimary,
                Location = new Point(0, 0),
                Text = "Reported Issues"
            };

            subtitleLabel = new Label
            {
                AutoSize = true,
                Font = new Font("Segoe UI", 14f),
                ForeColor = UiTheme.TextSecondary,
                Location = new Point(2, 63),
                Text = "View issues reported to Midvaal Local Municipality"
            };

            reportNewButton = new PrimaryButton
            {
                Text = "+  Report New Issue",
                Size = new Size(210, 48)
            };
            reportNewButton.Click += ReportNewButton_Click;

            accentBar = new AccentBar { Location = new Point(2, 113), Height = 5 };

            totalMetric = CreateMetric("Total Issues", UiTheme.PrimaryBlue);
            openMetric = CreateMetric("Open", UiTheme.Green);
            progressMetric = CreateMetric("In Progress", UiTheme.PrimaryBlueDark);
            resolvedMetric = CreateMetric("Resolved", UiTheme.Yellow);

            searchTextBox = new WatermarkTextBox
            {
                Font = new Font("Segoe UI", 11f),
                WatermarkText = "Search reported issues...",
                Location = new Point(0, 286),
                Height = 42,
                BorderStyle = BorderStyle.FixedSingle
            };
            searchTextBox.TextChanged += FilterChanged;

            categoryComboBox = new ComboBox
            {
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new Font("Segoe UI", 11f),
                IntegralHeight = false,
                Location = new Point(0, 286),
                Height = 42
            };
            categoryComboBox.SelectedIndexChanged += FilterChanged;

            issuesGrid = CreateIssuesGrid();
            issuesGrid.Location = new Point(0, 349);
            issuesGrid.CellDoubleClick += IssuesGrid_CellDoubleClick;
            issuesGrid.SelectionChanged += IssuesGrid_SelectionChanged;

            contentPanel.Controls.Add(titleLabel);
            contentPanel.Controls.Add(subtitleLabel);
            contentPanel.Controls.Add(reportNewButton);
            contentPanel.Controls.Add(accentBar);
            contentPanel.Controls.Add(totalMetric);
            contentPanel.Controls.Add(openMetric);
            contentPanel.Controls.Add(progressMetric);
            contentPanel.Controls.Add(resolvedMetric);
            contentPanel.Controls.Add(searchTextBox);
            contentPanel.Controls.Add(categoryComboBox);
            contentPanel.Controls.Add(issuesGrid);
            ContentHost.Controls.Add(contentPanel);
            ContentHost.Resize += ContentHost_Resize;

            LayoutContent();
            RefreshIssues();
        }

        private static MetricCard CreateMetric(string caption, Color colour)
        {
            return new MetricCard
            {
                Caption = caption,
                AccentColor = colour,
                Size = new Size(190, 105),
                ValueText = "0"
            };
        }

        private static DataGridView CreateIssuesGrid()
        {
            DataGridView grid = new DataGridView
            {
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                AllowUserToResizeRows = false,
                AutoGenerateColumns = false,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal,
                ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single,
                ColumnHeadersHeight = 46,
                ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing,
                EnableHeadersVisualStyles = false,
                GridColor = UiTheme.Border,
                MultiSelect = false,
                ReadOnly = true,
                RowHeadersVisible = false,
                RowTemplate = { Height = 47 },
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                ShowCellToolTips = true
            };

            grid.ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle
            {
                BackColor = Color.FromArgb(248, 249, 251),
                ForeColor = UiTheme.TextPrimary,
                Font = new Font("Segoe UI", 10.5f, FontStyle.Bold),
                Alignment = DataGridViewContentAlignment.MiddleLeft,
                SelectionBackColor = Color.FromArgb(248, 249, 251),
                SelectionForeColor = UiTheme.TextPrimary
            };
            grid.DefaultCellStyle = new DataGridViewCellStyle
            {
                BackColor = Color.White,
                ForeColor = UiTheme.TextPrimary,
                Font = new Font("Segoe UI", 10f),
                Padding = new Padding(10, 0, 4, 0),
                SelectionBackColor = Color.FromArgb(226, 243, 252),
                SelectionForeColor = UiTheme.TextPrimary
            };

            grid.Columns.Add(CreateTextColumn("Reference", 16f));
            grid.Columns.Add(CreateTextColumn("Location", 25f));
            grid.Columns.Add(CreateTextColumn("Category", 22f));
            grid.Columns.Add(CreateTextColumn("Date Reported", 19f));
            grid.Columns.Add(CreateTextColumn("Status", 18f));
            return grid;
        }

        private static DataGridViewTextBoxColumn CreateTextColumn(string header, float fillWeight)
        {
            return new DataGridViewTextBoxColumn
            {
                HeaderText = header,
                Name = header.Replace(" ", string.Empty),
                AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill,
                FillWeight = fillWeight,
                SortMode = DataGridViewColumnSortMode.NotSortable
            };
        }

        private void ContentHost_Resize(object sender, EventArgs e)
        {
            LayoutContent();
        }

        private void LayoutContent()
        {
            const int margin = 42;
            int availableWidth = Math.Max(780, ContentHost.ClientSize.Width - (margin * 2));
            int contentWidth = Math.Min(1080, availableWidth);
            contentPanel.Width = contentWidth;
            contentPanel.Left = Math.Max(margin, (ContentHost.ClientSize.Width - contentWidth) / 2);

            reportNewButton.Left = contentWidth - reportNewButton.Width;
            reportNewButton.Top = 16;
            accentBar.Width = contentWidth - 2;

            const int metricGap = 18;
            int metricWidth = (contentWidth - (metricGap * 3)) / 4;
            MetricCard[] metrics = { totalMetric, openMetric, progressMetric, resolvedMetric };
            for (int index = 0; index < metrics.Length; index++)
            {
                metrics[index].Location = new Point(index * (metricWidth + metricGap), 148);
                metrics[index].Size = new Size(metricWidth, 105);
            }

            int filterWidth = Math.Min(300, contentWidth / 3);
            categoryComboBox.Width = filterWidth;
            categoryComboBox.Left = contentWidth - filterWidth;
            searchTextBox.Width = categoryComboBox.Left - 18;
            issuesGrid.Width = contentWidth;
            issuesGrid.Height = Math.Max(285, ContentHost.ClientSize.Height - issuesGrid.Top - 48);
            contentPanel.Height = issuesGrid.Bottom + 4;
        }

        private void RefreshIssues()
        {
            allIssues = repository.GetAll();
            PopulateCategories();
            UpdateMetrics();
            ApplyFilters();
        }

        private void PopulateCategories()
        {
            string selected = categoryComboBox.SelectedItem as string;
            List<string> categories = allIssues
                .Select(issue => issue.Category)
                .Where(category => !string.IsNullOrWhiteSpace(category))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .OrderBy(category => category)
                .ToList();

            categoryComboBox.SelectedIndexChanged -= FilterChanged;
            categoryComboBox.Items.Clear();
            categoryComboBox.Items.Add("All Categories");
            foreach (string category in categories)
                categoryComboBox.Items.Add(category);

            int selectedIndex = !string.IsNullOrEmpty(selected)
                ? categoryComboBox.FindStringExact(selected)
                : 0;
            categoryComboBox.SelectedIndex = selectedIndex >= 0 ? selectedIndex : 0;
            categoryComboBox.SelectedIndexChanged += FilterChanged;
        }

        private void UpdateMetrics()
        {
            totalMetric.ValueText = allIssues.Count.ToString();
            openMetric.ValueText = allIssues.Count(issue => issue.Status == IssueStatus.Open).ToString();
            progressMetric.ValueText = allIssues.Count(issue => issue.Status == IssueStatus.InProgress).ToString();
            resolvedMetric.ValueText = allIssues.Count(issue => issue.Status == IssueStatus.Resolved).ToString();
        }

        private void FilterChanged(object sender, EventArgs e)
        {
            ApplyFilters();
        }

        private void ApplyFilters()
        {
            string search = searchTextBox.Text.Trim();
            string category = categoryComboBox.SelectedItem as string;

            IEnumerable<MunicipalIssue> filtered = allIssues;
            if (!string.IsNullOrWhiteSpace(search))
            {
                filtered = filtered.Where(issue =>
                    ContainsIgnoreCase(issue.Reference, search) ||
                    ContainsIgnoreCase(issue.Location, search) ||
                    ContainsIgnoreCase(issue.Category, search) ||
                    ContainsIgnoreCase(issue.Description, search) ||
                    ContainsIgnoreCase(issue.StatusDisplay, search));
            }

            if (!string.IsNullOrWhiteSpace(category) && category != "All Categories")
                filtered = filtered.Where(issue => string.Equals(issue.Category, category, StringComparison.OrdinalIgnoreCase));

            List<MunicipalIssue> visibleIssues = filtered.ToList();
            issuesGrid.Rows.Clear();

            foreach (MunicipalIssue issue in visibleIssues)
            {
                int rowIndex = issuesGrid.Rows.Add(
                    issue.Reference,
                    issue.Location,
                    issue.Category,
                    issue.DateReported.ToString("dd MMM yyyy"),
                    issue.StatusDisplay);
                DataGridViewRow row = issuesGrid.Rows[rowIndex];
                row.Tag = issue;
                ApplyStatusColour(row.Cells[4], issue.Status);
            }

            issuesGrid.ClearSelection();
            SetStatus(visibleIssues.Count == 1
                ? "Double-click an issue to view its details.   1 issue"
                : "Double-click an issue to view its details.   " + visibleIssues.Count + " issues");
        }

        private static bool ContainsIgnoreCase(string value, string search)
        {
            return !string.IsNullOrEmpty(value) && value.IndexOf(search, StringComparison.OrdinalIgnoreCase) >= 0;
        }

        private static void ApplyStatusColour(DataGridViewCell cell, IssueStatus status)
        {
            if (status == IssueStatus.Open)
                cell.Style.BackColor = Color.FromArgb(255, 249, 205);
            else if (status == IssueStatus.InProgress)
                cell.Style.BackColor = Color.FromArgb(221, 242, 252);
            else
                cell.Style.BackColor = Color.FromArgb(224, 247, 222);

            cell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
        }

        private void ReportNewButton_Click(object sender, EventArgs e)
        {
            using (ReportNewIssueForm form = new ReportNewIssueForm())
            {
                form.StartPosition = FormStartPosition.CenterParent;
                form.ShowDialog(this);

                if (form.NavigationResult == FormNavigationResult.Exit)
                {
                    CloseWithResult(FormNavigationResult.Exit);
                    return;
                }

                if (form.NavigationResult == FormNavigationResult.Home)
                {
                    CloseWithResult(FormNavigationResult.Home);
                    return;
                }
            }

            RefreshIssues();
        }

        private void IssuesGrid_SelectionChanged(object sender, EventArgs e)
        {
            if (issuesGrid.SelectedRows.Count == 1)
            {
                MunicipalIssue issue = issuesGrid.SelectedRows[0].Tag as MunicipalIssue;
                if (issue != null)
                    SetStatus("Selected " + issue.Reference + ". Double-click to view details.");
            }
        }

        private void IssuesGrid_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0)
                return;

            MunicipalIssue issue = issuesGrid.Rows[e.RowIndex].Tag as MunicipalIssue;
            if (issue == null)
                return;

            using (IssueDetailsForm details = new IssueDetailsForm(issue))
            {
                details.StartPosition = FormStartPosition.CenterParent;
                details.ShowDialog(this);
            }
        }

        protected override void OnLocalEventsRequested()
        {
            CloseWithResult(FormNavigationResult.LocalEvents);
        }
    }
}
