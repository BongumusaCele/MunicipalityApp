using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using MunicipalityApp.Controls;
using MunicipalityApp.Data;
using MunicipalityApp.Models;

namespace MunicipalityApp.Forms
{
    internal sealed class IssueDetailsForm : Form
    {
        private readonly IssueRepository repository;
        private MunicipalIssue issue;
        private readonly GroupBox feedbackGroup;
        private ComboBox ratingComboBox;
        private TextBox feedbackTextBox;

        internal IssueDetailsForm(MunicipalIssue selectedIssue)
        {
            repository = IssueRepository.Current;
            issue = selectedIssue;
            AutoScaleMode = AutoScaleMode.Dpi;
            BackColor = UiTheme.PageBackground;
            ClientSize = new Size(720, 680);
            Font = new Font("Segoe UI", 10f);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            ShowInTaskbar = false;
            Text = "Issue Details - " + issue.Reference;

            Label heading = new Label
            {
                AutoSize = true,
                Font = new Font("Segoe UI", 22f, FontStyle.Bold),
                ForeColor = UiTheme.TextPrimary,
                Location = new Point(30, 20),
                Text = issue.Reference
            };

            Label summary = new Label
            {
                Font = new Font("Segoe UI", 10.5f),
                ForeColor = UiTheme.TextPrimary,
                Location = new Point(33, 72),
                Size = new Size(654, 90),
                Text = "Location: " + issue.Location + Environment.NewLine +
                       "Category: " + issue.Category + Environment.NewLine +
                       "Date reported: " + issue.DateReported.ToString("dd MMMM yyyy, HH:mm") + Environment.NewLine +
                       "Current status: " + issue.StatusDisplay
            };

            Label progressHeading = new Label
            {
                AutoSize = true,
                Font = new Font("Segoe UI", 11f, FontStyle.Bold),
                Location = new Point(33, 166),
                Text = "Progress updates"
            };

            ListView progressList = new ListView
            {
                FullRowSelect = true,
                GridLines = true,
                HeaderStyle = ColumnHeaderStyle.Nonclickable,
                Location = new Point(36, 194),
                MultiSelect = false,
                Size = new Size(648, 125),
                View = View.Details
            };
            progressList.Columns.Add("Date", 135);
            progressList.Columns.Add("Status", 100);
            progressList.Columns.Add("Update", 390);

            foreach (IssueStatusUpdate update in issue.StatusHistory.OrderBy(item => item.DateUpdated))
            {
                ListViewItem row = new ListViewItem(update.DateUpdated.ToString("dd MMM yyyy"));
                row.SubItems.Add(update.StatusDisplay);
                row.SubItems.Add(update.Message);
                progressList.Items.Add(row);
            }

            Label descriptionHeading = new Label
            {
                AutoSize = true,
                Font = new Font("Segoe UI", 11f, FontStyle.Bold),
                Location = new Point(33, 333),
                Text = "Description"
            };

            TextBox description = new TextBox
            {
                BackColor = Color.White,
                Font = new Font("Segoe UI", 10f),
                Location = new Point(36, 361),
                Multiline = true,
                ReadOnly = true,
                ScrollBars = ScrollBars.Vertical,
                Size = new Size(648, 91),
                Text = issue.Description
            };

            feedbackGroup = new GroupBox
            {
                Font = new Font("Segoe UI", 10f, FontStyle.Bold),
                Location = new Point(36, 468),
                Size = new Size(648, 128),
                Text = "Resident feedback"
            };
            BuildFeedbackSection();

            SecondaryButton closeButton = new SecondaryButton
            {
                DialogResult = DialogResult.OK,
                Location = new Point(574, 617),
                Size = new Size(110, 40),
                Text = "Close"
            };

            Controls.Add(heading);
            Controls.Add(summary);
            Controls.Add(progressHeading);
            Controls.Add(progressList);
            Controls.Add(descriptionHeading);
            Controls.Add(description);
            Controls.Add(feedbackGroup);
            Controls.Add(closeButton);
            AcceptButton = closeButton;
            CancelButton = closeButton;

            if (!string.IsNullOrWhiteSpace(issue.AttachmentPath))
            {
                SecondaryButton attachmentButton = new SecondaryButton
                {
                    Location = new Point(36, 617),
                    Size = new Size(175, 40),
                    Text = "Open Attachment"
                };
                attachmentButton.Click += OpenAttachment_Click;
                Controls.Add(attachmentButton);
            }
        }

        private void BuildFeedbackSection()
        {
            feedbackGroup.Controls.Clear();

            if (issue.Status != IssueStatus.Resolved)
            {
                feedbackGroup.Controls.Add(new Label
                {
                    Dock = DockStyle.Fill,
                    Font = new Font("Segoe UI", 10f),
                    ForeColor = UiTheme.TextSecondary,
                    Padding = new Padding(12, 5, 12, 5),
                    Text = "Feedback becomes available when the municipality marks this issue as Resolved.",
                    TextAlign = ContentAlignment.MiddleLeft
                });
                return;
            }

            if (issue.HasFeedback)
            {
                string feedbackDate = issue.FeedbackDate.HasValue
                    ? issue.FeedbackDate.Value.ToString("dd MMMM yyyy")
                    : "Recorded";
                string comment = string.IsNullOrWhiteSpace(issue.FeedbackComment)
                    ? "No written comment supplied."
                    : issue.FeedbackComment;

                feedbackGroup.Controls.Add(new Label
                {
                    Dock = DockStyle.Fill,
                    Font = new Font("Segoe UI", 9.5f),
                    ForeColor = Color.FromArgb(23, 105, 37),
                    Padding = new Padding(12, 7, 12, 5),
                    Text = "Thank you. Feedback submitted on " + feedbackDate + "." + Environment.NewLine +
                           "Rating: " + issue.FeedbackRating + " out of 5" + Environment.NewLine +
                           "Comment: " + comment,
                    TextAlign = ContentAlignment.MiddleLeft
                });
                return;
            }

            Label ratingLabel = new Label
            {
                AutoSize = true,
                Font = new Font("Segoe UI", 9.5f),
                Location = new Point(15, 30),
                Text = "Rating"
            };
            ratingComboBox = new ComboBox
            {
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new Font("Segoe UI", 9.5f),
                Location = new Point(18, 55),
                Size = new Size(110, 31)
            };
            ratingComboBox.Items.AddRange(new object[] { "Select", "1 - Poor", "2 - Fair", "3 - Good", "4 - Very good", "5 - Excellent" });
            ratingComboBox.SelectedIndex = 0;

            Label commentLabel = new Label
            {
                AutoSize = true,
                Font = new Font("Segoe UI", 9.5f),
                Location = new Point(145, 30),
                Text = "Optional comment"
            };
            feedbackTextBox = new TextBox
            {
                Font = new Font("Segoe UI", 9.5f),
                Location = new Point(148, 55),
                MaxLength = 300,
                Size = new Size(325, 31)
            };

            PrimaryButton submitFeedbackButton = new PrimaryButton
            {
                Location = new Point(491, 51),
                Size = new Size(140, 39),
                Text = "Submit Feedback"
            };
            submitFeedbackButton.Click += SubmitFeedbackButton_Click;

            feedbackGroup.Controls.Add(ratingLabel);
            feedbackGroup.Controls.Add(ratingComboBox);
            feedbackGroup.Controls.Add(commentLabel);
            feedbackGroup.Controls.Add(feedbackTextBox);
            feedbackGroup.Controls.Add(submitFeedbackButton);
        }

        private void SubmitFeedbackButton_Click(object sender, EventArgs e)
        {
            if (ratingComboBox.SelectedIndex <= 0)
            {
                MessageBox.Show(this, "Select a rating from 1 to 5.", "Rating required",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                ratingComboBox.Focus();
                return;
            }

            try
            {
                issue = repository.SubmitFeedback(issue.Reference, ratingComboBox.SelectedIndex, feedbackTextBox.Text);
                BuildFeedbackSection();
                MessageBox.Show(this, "Thank you. Your feedback has been recorded.", "Feedback submitted",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception exception) when (
                exception is IOException ||
                exception is UnauthorizedAccessException ||
                exception is InvalidOperationException ||
                exception is ArgumentOutOfRangeException)
            {
                MessageBox.Show(this, exception.Message, "Feedback could not be saved",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void OpenAttachment_Click(object sender, EventArgs e)
        {
            if (!File.Exists(issue.AttachmentPath))
            {
                MessageBox.Show(this, "The attachment file could not be found.", "Attachment unavailable",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                Process.Start(new ProcessStartInfo(issue.AttachmentPath) { UseShellExecute = true });
            }
            catch (Exception exception) when (exception is InvalidOperationException || exception is System.ComponentModel.Win32Exception)
            {
                MessageBox.Show(this, "Windows could not open this attachment.", "Attachment error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
