using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using MunicipalityApp.Data;
using MunicipalityApp.Models;
using MunicipalityApp.Structures;

namespace MunicipalityApp.Forms
{
    internal sealed class ServiceRequestStatusForm : CitizenServicesFormBase
    {
        private ServiceRequestIndex index;
        private readonly TextBox reference = new TextBox { Width = 155, AccessibleName = "Unique request reference" };
        private readonly ComboBox status = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList, Width = 130 };
        private readonly ComboBox order = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList, Width = 165 };
        private readonly DataGridView grid = new DataGridView { Dock = DockStyle.Fill, ReadOnly = true, AutoGenerateColumns = false,
            AllowUserToAddRows = false, AllowUserToDeleteRows = false, SelectionMode = DataGridViewSelectionMode.FullRowSelect,
            MultiSelect = false, RowHeadersVisible = false, BackgroundColor = Color.White };
        private readonly TreeView hierarchy = new TreeView { Dock = DockStyle.Fill, AccessibleName = "Requests grouped by category and status" };
        private readonly TextBox relationships = new TextBox { Dock = DockStyle.Fill, Multiline = true, ReadOnly = true, ScrollBars = ScrollBars.Vertical };
        private readonly Label summary = new Label { Dock = DockStyle.Fill, AutoSize = true };
        internal ServiceRequestStatusForm() : base(AppPage.ServiceRequestStatus)
        {
            Text = "Service Request Status - Midvaal Citizen Services";
            var layout = new TableLayoutPanel { Dock = DockStyle.Fill, Padding = new Padding(20), RowCount = 5, ColumnCount = 1 };
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 65));
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 76));
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 28));
            layout.RowStyles.Add(new RowStyle(SizeType.Percent, 60));
            layout.RowStyles.Add(new RowStyle(SizeType.Percent, 40));
            layout.Controls.Add(new Label { Text = "Service Request Status", Font = new Font("Segoe UI", 25, FontStyle.Bold), Dock = DockStyle.Fill }, 0, 0);
            var toolbar = new FlowLayoutPanel { Dock = DockStyle.Fill, AutoScroll = true };
            toolbar.Controls.Add(new Label { Text = "Reference:", AutoSize = true, Padding = new Padding(0, 5, 0, 0) });
            toolbar.Controls.Add(reference);
            AddButton(toolbar, "Track", Track);
            status.Items.AddRange(new object[] { "All statuses", "Open", "In Progress", "Resolved" }); status.SelectedIndex = 0;
            order.Items.AddRange(new object[] { "Newest first", "Oldest first", "Priority (active first)" }); order.SelectedIndex = 0;
            toolbar.Controls.Add(status); toolbar.Controls.Add(order);
            AddButton(toolbar, "Refresh / clear", delegate { reference.Clear(); Reload(); });
            AddButton(toolbar, "View progress", OpenDetails);
            layout.Controls.Add(toolbar, 0, 1); layout.Controls.Add(summary, 0, 2);
            foreach (string property in new[] { "Reference", "Location", "Category", "StatusDisplay", "DateReported" })
                grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = property, HeaderText = property == "StatusDisplay" ? "Status" : property == "DateReported" ? "Reported" : property, AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill });
            grid.Columns[4].DefaultCellStyle.Format = "dd MMM yyyy HH:mm";
            layout.Controls.Add(grid, 0, 3);
            var tabs = new TabControl { Dock = DockStyle.Fill };
            var grouped = new TabPage("Category and status"); grouped.Controls.Add(hierarchy);
            var connected = new TabPage("Related requests and coordination plan"); connected.Controls.Add(relationships);
            tabs.TabPages.Add(grouped); tabs.TabPages.Add(connected); layout.Controls.Add(tabs, 0, 4);
            ContentHost.Controls.Add(layout);
            status.SelectedIndexChanged += delegate { ShowRequests(); };
            order.SelectedIndexChanged += delegate { ShowRequests(); };
            grid.CellDoubleClick += delegate(object sender, DataGridViewCellEventArgs e) { if (e.RowIndex >= 0) OpenDetails(); };
            grid.SelectionChanged += delegate { ShowConnections(); };
            reference.KeyDown += delegate(object sender, KeyEventArgs e) { if (e.KeyCode == Keys.Enter) { Track(); e.SuppressKeyPress = true; } };
            hierarchy.NodeMouseDoubleClick += delegate(object sender, TreeNodeMouseClickEventArgs e) { if (e.Node.Tag is string) { reference.Text = (string)e.Node.Tag; Track(); } };
            Reload();
        }
        private static void AddButton(Control parent, string text, Action action)
        { var button = new Button { Text = text, AutoSize = true, Height = 28 }; button.Click += delegate { action(); }; parent.Controls.Add(button); }
        private void Reload()
        {
            index = new ServiceRequestIndex(IssueRepository.Current.GetAll());
            hierarchy.Nodes.Clear(); hierarchy.Nodes.Add(BuildNode(index.Hierarchy)); hierarchy.Nodes[0].Expand(); ShowRequests();
        }
        private static System.Windows.Forms.TreeNode BuildNode(Structures.TreeNode<string> source)
        {
            var node = new System.Windows.Forms.TreeNode(source.Value);
            if (source.Children.Count == 0) node.Tag = source.Value;
            foreach (var child in source.Children) node.Nodes.Add(BuildNode(child)); return node;
        }
        private void ShowRequests()
        {
            if (index == null) return;
            var rows = index.Display((string)status.SelectedItem, (string)order.SelectedItem);
            grid.DataSource = rows; summary.Text = rows.Count + " request(s). Select a row and View progress, or track an exact MDV reference.";
            ShowConnections();
        }
        private void Track()
        {
            MunicipalIssue issue;
            if (!index.ByReference.TryGet(reference.Text.Trim(), out issue))
            { summary.Text = "Reference not found. Enter the complete reference, for example MDV-1004."; return; }
            grid.DataSource = new[] { issue }; summary.Text = issue.Reference + " — " + issue.StatusDisplay + ". Select View progress for the saved timeline.";
            ShowConnections();
        }
        private MunicipalIssue Selected { get { return grid.CurrentRow == null ? null : grid.CurrentRow.DataBoundItem as MunicipalIssue; } }
        private void OpenDetails()
        { if (Selected == null) return; using (var details = new IssueDetailsForm(Selected)) details.ShowDialog(this); Reload(); }
        private void ShowConnections()
        {
            if (index == null) return;
            var selected = Selected;
            var related = selected == null ? new System.Collections.Generic.List<string>() : index.Graph.BreadthFirst(selected.Reference);
            var forest = index.Graph.MinimumSpanningForest();
            relationships.Text = "Related component (breadth-first, includes indirect links): " + string.Join(", ", related) + Environment.NewLine + Environment.NewLine +
                "Illustrative coordination plan (minimum spanning forest; all requests):" + Environment.NewLine +
                "Links share an exact location or category. Cost: both = 1, location = 2, category = 3. These are coordination scores, not distance or live dispatch priority." + Environment.NewLine +
                (forest.Count == 0 ? "No shared links. Requests remain independent." : string.Join(Environment.NewLine, forest.Select(e => e.From + " <-> " + e.To + "  cost " + e.Weight))) + Environment.NewLine +
                "Total coordination cost: " + forest.Sum(e => e.Weight) + ". Isolated requests need no link.";
        }
        protected override void OnReportIssuesRequested() { CloseWithResult(FormNavigationResult.ReportIssues); }
    }
}

