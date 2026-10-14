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
    internal sealed class LocalEventsForm : CitizenServicesFormBase
    {
        private readonly EventRepository repository;
        private readonly Panel contentPanel;
        private readonly WatermarkTextBox searchTextBox;
        private readonly ComboBox categoryComboBox;
        private readonly DateTimePicker datePicker;
        private readonly CheckBox useDateCheckBox;
        private readonly DataGridView eventsGrid;
        private readonly FlowLayoutPanel recommendationsPanel;
        private readonly Label resultsLabel;

        internal LocalEventsForm()
            : base(AppPage.LocalEvents)
        {
            repository = EventRepository.Current;
            Text = "Local Events and Announcements - Midvaal Citizen Services";

            contentPanel = new Panel { BackColor = UiTheme.PageBackground, Location = new Point(44, 30), Size = new Size(900, 900) };
            Label title = new Label { AutoSize = true, Font = new Font("Segoe UI", 27f, FontStyle.Bold), ForeColor = UiTheme.TextPrimary, Location = new Point(0, 0), Text = "Local Events & Announcements" };
            Label subtitle = new Label { AutoSize = true, Font = new Font("Segoe UI", 11f), ForeColor = UiTheme.TextSecondary, Location = new Point(2, 57), Text = "Discover what is happening across Midvaal." };
            AccentBar accent = new AccentBar { Location = new Point(2, 92), Size = new Size(898, 5) };

            RoundedPanel filterCard = new RoundedPanel { Location = new Point(0, 120), Size = new Size(900, 112) };
            searchTextBox = new WatermarkTextBox { Location = new Point(22, 24), Size = new Size(285, 34), Font = new Font("Segoe UI", 10f), WatermarkText = "Search title, venue or keyword..." };
            categoryComboBox = new ComboBox { Location = new Point(323, 24), Size = new Size(190, 34), DropDownStyle = ComboBoxStyle.DropDownList, Font = new Font("Segoe UI", 10f) };
            categoryComboBox.Items.Add("All Categories");
            categoryComboBox.Items.AddRange(repository.GetCategories().Cast<object>().ToArray());
            categoryComboBox.SelectedIndex = 0;
            useDateCheckBox = new CheckBox { Location = new Point(530, 28), AutoSize = true, Font = new Font("Segoe UI", 9.5f), Text = "On date" };
            datePicker = new DateTimePicker { Location = new Point(602, 24), Size = new Size(178, 34), Format = DateTimePickerFormat.Short, Font = new Font("Segoe UI", 10f), Enabled = false, MinDate = DateTime.Today };
            SecondaryButton clearButton = new SecondaryButton { Location = new Point(796, 20), Size = new Size(82, 40), Text = "Clear" };
            Label filterHint = new Label { Location = new Point(22, 72), AutoSize = true, ForeColor = UiTheme.TextSecondary, Text = "Results update as you type. Select a category or exact event date to narrow the list." };
            filterCard.Controls.Add(searchTextBox); filterCard.Controls.Add(categoryComboBox); filterCard.Controls.Add(useDateCheckBox);
            filterCard.Controls.Add(datePicker); filterCard.Controls.Add(clearButton); filterCard.Controls.Add(filterHint);

            resultsLabel = new Label { AutoSize = true, Font = new Font("Segoe UI", 14f, FontStyle.Bold), ForeColor = UiTheme.TextPrimary, Location = new Point(0, 254), Text = "Upcoming events" };
            eventsGrid = CreateGrid();
            eventsGrid.Location = new Point(0, 292); eventsGrid.Size = new Size(900, 280);

            Label recommendedTitle = new Label { AutoSize = true, Font = new Font("Segoe UI", 14f, FontStyle.Bold), ForeColor = UiTheme.TextPrimary, Location = new Point(0, 598), Text = "Recommended for you" };
            Label recommendedHint = new Label { AutoSize = true, Font = new Font("Segoe UI", 9.5f), ForeColor = UiTheme.TextSecondary, Location = new Point(0, 629), Text = "Suggestions adapt to the categories and words you search for." };
            recommendationsPanel = new FlowLayoutPanel { Location = new Point(0, 660), Size = new Size(900, 165), AutoScroll = true, WrapContents = false, FlowDirection = FlowDirection.LeftToRight };

            contentPanel.Controls.Add(title); contentPanel.Controls.Add(subtitle); contentPanel.Controls.Add(accent); contentPanel.Controls.Add(filterCard);
            contentPanel.Controls.Add(resultsLabel); contentPanel.Controls.Add(eventsGrid); contentPanel.Controls.Add(recommendedTitle);
            contentPanel.Controls.Add(recommendedHint); contentPanel.Controls.Add(recommendationsPanel);
            ContentHost.Controls.Add(contentPanel);
            ContentHost.Resize += delegate { LayoutContent(); };

            searchTextBox.TextChanged += FilterChanged;
            categoryComboBox.SelectedIndexChanged += FilterChanged;
            datePicker.ValueChanged += FilterChanged;
            useDateCheckBox.CheckedChanged += delegate { datePicker.Enabled = useDateCheckBox.Checked; RefreshEvents(); };
            clearButton.Click += delegate { searchTextBox.Clear(); categoryComboBox.SelectedIndex = 0; useDateCheckBox.Checked = false; RefreshEvents(); };
            eventsGrid.CellDoubleClick += ShowEventDetails;

            LayoutContent();
            RefreshEvents();
        }

        private static DataGridView CreateGrid()
        {
            DataGridView grid = new DataGridView
            {
                AllowUserToAddRows = false, AllowUserToDeleteRows = false, AllowUserToResizeRows = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill, BackgroundColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle, CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal,
                ColumnHeadersHeight = 42, ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing,
                EnableHeadersVisualStyles = false, MultiSelect = false, ReadOnly = true, RowHeadersVisible = false,
                RowTemplate = { Height = 43 }, SelectionMode = DataGridViewSelectionMode.FullRowSelect
            };
            grid.ColumnHeadersDefaultCellStyle.BackColor = UiTheme.PrimaryBlueDark;
            grid.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            grid.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10f, FontStyle.Bold);
            grid.DefaultCellStyle.Font = new Font("Segoe UI", 9.5f);
            grid.DefaultCellStyle.SelectionBackColor = Color.FromArgb(220, 242, 252);
            grid.DefaultCellStyle.SelectionForeColor = UiTheme.TextPrimary;
            grid.Columns.Add(Column("Date", 18)); grid.Columns.Add(Column("Event / Announcement", 29));
            grid.Columns.Add(Column("Category", 18)); grid.Columns.Add(Column("Venue", 27)); grid.Columns.Add(Column("Type", 12));
            return grid;
        }

        private static DataGridViewTextBoxColumn Column(string title, float weight)
        {
            return new DataGridViewTextBoxColumn { HeaderText = title, FillWeight = weight, SortMode = DataGridViewColumnSortMode.NotSortable };
        }

        private void FilterChanged(object sender, EventArgs e) { RefreshEvents(); }

        private void RefreshEvents()
        {
            DateTime? date = useDateCheckBox.Checked ? (DateTime?)datePicker.Value.Date : null;
            IList<MunicipalEvent> items = repository.Search(searchTextBox.Text, categoryComboBox.SelectedItem as string, date);
            eventsGrid.Rows.Clear();
            foreach (MunicipalEvent item in items)
            {
                int index = eventsGrid.Rows.Add(item.EventDate.ToString("ddd, dd MMM yyyy HH:mm"), item.Title, item.Category, item.Venue, item.IsAnnouncement ? "Notice" : "Event");
                eventsGrid.Rows[index].Tag = item;
                if (item.IsAnnouncement) eventsGrid.Rows[index].Cells[4].Style.BackColor = Color.FromArgb(255, 249, 205);
            }
            resultsLabel.Text = "Upcoming events (" + items.Count + ")";
            SetStatus(items.Count + (items.Count == 1 ? " result" : " results") + ". Double-click an item for details.");
            RefreshRecommendations();
        }

        private void RefreshRecommendations()
        {
            recommendationsPanel.Controls.Clear();
            IList<EventRecommendation> recommendations = repository.GetRecommendations(4);
            if (recommendations.Count == 0)
            {
                recommendationsPanel.Controls.Add(new Label { AutoSize = false, Size = new Size(860, 60), Padding = new Padding(4, 16, 0, 0), ForeColor = UiTheme.TextSecondary, Text = "Search for an event or select a category to receive personalised suggestions." });
                return;
            }
            foreach (EventRecommendation recommendation in recommendations)
            {
                RoundedPanel card = new RoundedPanel { Size = new Size(270, 132), Margin = new Padding(0, 4, 14, 4), Cursor = Cursors.Hand };
                Label eventTitle = new Label { Location = new Point(15, 13), Size = new Size(240, 43), Font = new Font("Segoe UI", 10.5f, FontStyle.Bold), ForeColor = UiTheme.TextPrimary, Text = recommendation.Event.Title };
                Label detail = new Label { Location = new Point(15, 59), Size = new Size(240, 23), ForeColor = UiTheme.TextSecondary, Text = recommendation.Event.EventDate.ToString("dd MMM") + "  •  " + recommendation.Event.Category };
                Label reason = new Label { Location = new Point(15, 88), Size = new Size(240, 30), Font = new Font("Segoe UI", 8.5f, FontStyle.Italic), ForeColor = UiTheme.PrimaryBlueDark, Text = recommendation.Reason };
                card.Tag = recommendation.Event; eventTitle.Tag = recommendation.Event; detail.Tag = recommendation.Event; reason.Tag = recommendation.Event;
                card.Click += RecommendationClicked; eventTitle.Click += RecommendationClicked; detail.Click += RecommendationClicked; reason.Click += RecommendationClicked;
                card.Controls.Add(eventTitle); card.Controls.Add(detail); card.Controls.Add(reason); recommendationsPanel.Controls.Add(card);
            }
        }

        private void RecommendationClicked(object sender, EventArgs e)
        {
            Control control = sender as Control;
            ShowDetails(control == null ? null : control.Tag as MunicipalEvent);
        }

        private void ShowEventDetails(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0) ShowDetails(eventsGrid.Rows[e.RowIndex].Tag as MunicipalEvent);
        }

        private void ShowDetails(MunicipalEvent item)
        {
            if (item == null) return;
            MessageBox.Show(this, item.Title + Environment.NewLine + Environment.NewLine +
                item.EventDate.ToString("dddd, dd MMMM yyyy 'at' HH:mm") + Environment.NewLine + item.Venue + Environment.NewLine +
                item.Category + Environment.NewLine + Environment.NewLine + item.Description,
                item.IsAnnouncement ? "Municipal announcement" : "Local event", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void LayoutContent()
        {
            int width = Math.Min(1000, Math.Max(720, ContentHost.ClientSize.Width - 88));
            contentPanel.Width = width; contentPanel.Left = Math.Max(44, (ContentHost.ClientSize.Width - width) / 2);
            foreach (Control control in contentPanel.Controls)
            {
                if (control is AccentBar || control == eventsGrid || control == recommendationsPanel || control is RoundedPanel && control.Top == 120)
                    control.Width = width;
            }
        }

        protected override void OnReportIssuesRequested() { CloseWithResult(FormNavigationResult.ReportIssues); }
    }
}
