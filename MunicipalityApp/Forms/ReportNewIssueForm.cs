using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using MunicipalityApp.Controls;
using MunicipalityApp.Data;
using MunicipalityApp.Models;

namespace MunicipalityApp.Forms
{
    internal sealed class ReportNewIssueForm : CitizenServicesFormBase
    {
        private const long MaximumAttachmentBytes = 10 * 1024 * 1024;
        private static readonly string[] AllowedExtensions = { ".jpg", ".jpeg", ".png", ".pdf", ".docx" };

        private readonly IssueRepository repository;
        private readonly Panel contentPanel;
        private readonly SecondaryButton backButton;
        private readonly Label titleLabel;
        private readonly Label subtitleLabel;
        private readonly AccentBar accentBar;
        private readonly RoundedPanel formCard;
        private readonly Label locationLabel;
        private readonly WatermarkTextBox locationTextBox;
        private readonly Label categoryLabel;
        private readonly ComboBox categoryComboBox;
        private readonly Label descriptionLabel;
        private readonly RichTextBox descriptionTextBox;
        private readonly Label attachmentLabel;
        private readonly SecondaryButton chooseFileButton;
        private readonly Label supportedTypesLabel;
        private readonly Label selectedFileLabel;
        private readonly Label completionLabel;
        private readonly CompletionProgressBar completionBar;
        private readonly RoundedPanel encouragementPanel;
        private readonly Label encouragementLabel;
        private readonly Label requiredLabel;
        private readonly SecondaryButton clearButton;
        private readonly PrimaryButton submitButton;
        private readonly OpenFileDialog attachmentDialog;
        private bool showingDescriptionWatermark;
        private string selectedAttachmentPath = string.Empty;

        internal ReportNewIssueForm()
            : base(AppPage.ReportIssues)
        {
            repository = IssueRepository.Current;

            contentPanel = new Panel
            {
                BackColor = UiTheme.PageBackground,
                Location = new Point(38, 24),
                Size = new Size(920, 690)
            };

            backButton = new SecondaryButton
            {
                Location = new Point(0, 0),
                Size = new Size(210, 38),
                Text = "←  Back to Reported Issues"
            };
            backButton.Click += BackButton_Click;

            titleLabel = new Label
            {
                AutoSize = true,
                Font = new Font("Segoe UI", 27f, FontStyle.Bold),
                ForeColor = UiTheme.TextPrimary,
                Location = new Point(0, 50),
                Text = "Report New Issue"
            };

            subtitleLabel = new Label
            {
                AutoSize = true,
                Font = new Font("Segoe UI", 13f),
                ForeColor = UiTheme.TextSecondary,
                Location = new Point(2, 105),
                Text = "Tell us about a municipal issue in your area."
            };

            accentBar = new AccentBar { Location = new Point(0, 145), Height = 5 };
            formCard = new RoundedPanel { Location = new Point(0, 169), Size = new Size(920, 500) };

            locationLabel = CreateFieldLabel("Issue Location  *");
            categoryLabel = CreateFieldLabel("Category  *");
            descriptionLabel = CreateFieldLabel("Issue Description  *");
            attachmentLabel = CreateFieldLabel("Supporting Attachment");

            locationTextBox = new WatermarkTextBox
            {
                Font = new Font("Segoe UI", 10.5f),
                WatermarkText = "Enter street address or area",
                BorderStyle = BorderStyle.FixedSingle,
                Height = 40
            };
            locationTextBox.TextChanged += InputChanged;

            categoryComboBox = new ComboBox
            {
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new Font("Segoe UI", 10.5f),
                IntegralHeight = false,
                Height = 40
            };
            categoryComboBox.Items.AddRange(new object[]
            {
                "Select a category",
                "Roads",
                "Water & Sanitation",
                "Waste Removal",
                "Electricity",
                "Public Safety",
                "Parks & Recreation",
                "Other"
            });
            categoryComboBox.SelectedIndex = 0;
            categoryComboBox.SelectedIndexChanged += InputChanged;

            descriptionTextBox = new RichTextBox
            {
                BorderStyle = BorderStyle.FixedSingle,
                Font = new Font("Segoe UI", 10.5f),
                ScrollBars = RichTextBoxScrollBars.Vertical
            };
            descriptionTextBox.Enter += DescriptionTextBox_Enter;
            descriptionTextBox.Leave += DescriptionTextBox_Leave;
            descriptionTextBox.TextChanged += InputChanged;

            chooseFileButton = new SecondaryButton
            {
                Size = new Size(120, 38),
                Text = "Choose File"
            };
            chooseFileButton.Click += ChooseFileButton_Click;

            supportedTypesLabel = new Label
            {
                AutoSize = true,
                Font = new Font("Segoe UI", 9.5f),
                ForeColor = UiTheme.TextSecondary,
                Text = "JPG, PNG, PDF or DOCX (maximum 10 MB)"
            };

            selectedFileLabel = new Label
            {
                AutoEllipsis = true,
                Font = new Font("Segoe UI", 9.5f),
                ForeColor = UiTheme.TextSecondary,
                Text = "No file selected",
                TextAlign = ContentAlignment.MiddleLeft
            };

            completionLabel = new Label
            {
                AutoSize = true,
                Font = new Font("Segoe UI", 10f, FontStyle.Bold),
                ForeColor = UiTheme.TextPrimary,
                Text = "Form completion: 0%"
            };

            completionBar = new CompletionProgressBar { Height = 13, Value = 0 };

            encouragementPanel = new RoundedPanel
            {
                BackColor = Color.FromArgb(237, 251, 237),
                BorderColor = Color.FromArgb(132, 204, 128),
                CornerRadius = 5,
                Height = 48
            };
            encouragementLabel = new Label
            {
                BackColor = Color.Transparent,
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 10f),
                ForeColor = Color.FromArgb(23, 105, 37),
                Padding = new Padding(14, 0, 8, 0),
                Text = "Every report helps us improve Midvaal.",
                TextAlign = ContentAlignment.MiddleLeft
            };
            encouragementPanel.Controls.Add(encouragementLabel);

            requiredLabel = new Label
            {
                AutoSize = true,
                Font = new Font("Segoe UI", 9.5f),
                ForeColor = UiTheme.TextPrimary,
                Text = "*  Required fields"
            };

            clearButton = new SecondaryButton
            {
                Size = new Size(140, 40),
                Text = "Clear Form"
            };
            clearButton.Click += ClearButton_Click;

            submitButton = new PrimaryButton
            {
                Size = new Size(150, 42),
                Text = "Submit Report"
            };
            submitButton.Click += SubmitButton_Click;

            attachmentDialog = new OpenFileDialog
            {
                AddExtension = true,
                CheckFileExists = true,
                CheckPathExists = true,
                Filter = "Supported files|*.jpg;*.jpeg;*.png;*.pdf;*.docx|Images|*.jpg;*.jpeg;*.png|Documents|*.pdf;*.docx",
                Multiselect = false,
                Title = "Choose a supporting attachment"
            };

            formCard.Controls.Add(locationLabel);
            formCard.Controls.Add(locationTextBox);
            formCard.Controls.Add(categoryLabel);
            formCard.Controls.Add(categoryComboBox);
            formCard.Controls.Add(descriptionLabel);
            formCard.Controls.Add(descriptionTextBox);
            formCard.Controls.Add(attachmentLabel);
            formCard.Controls.Add(chooseFileButton);
            formCard.Controls.Add(supportedTypesLabel);
            formCard.Controls.Add(selectedFileLabel);
            formCard.Controls.Add(completionLabel);
            formCard.Controls.Add(completionBar);
            formCard.Controls.Add(encouragementPanel);
            formCard.Controls.Add(requiredLabel);
            formCard.Controls.Add(clearButton);
            formCard.Controls.Add(submitButton);

            contentPanel.Controls.Add(backButton);
            contentPanel.Controls.Add(titleLabel);
            contentPanel.Controls.Add(subtitleLabel);
            contentPanel.Controls.Add(accentBar);
            contentPanel.Controls.Add(formCard);
            ContentHost.Controls.Add(contentPanel);
            ContentHost.Resize += ContentHost_Resize;

            SetDescriptionWatermark();
            LayoutContent();
            UpdateCompletion();
            SetStatus("Ready to submit a new issue.");
        }

        private static Label CreateFieldLabel(string text)
        {
            return new Label
            {
                AutoSize = true,
                Font = new Font("Segoe UI", 10.5f, FontStyle.Bold),
                ForeColor = UiTheme.TextPrimary,
                Text = text
            };
        }

        private void ContentHost_Resize(object sender, EventArgs e)
        {
            LayoutContent();
        }

        private void LayoutContent()
        {
            const int margin = 38;
            int availableWidth = Math.Max(780, ContentHost.ClientSize.Width - (margin * 2));
            int contentWidth = Math.Min(1080, availableWidth);
            contentPanel.Width = contentWidth;
            contentPanel.Left = Math.Max(margin, (ContentHost.ClientSize.Width - contentWidth) / 2);
            accentBar.Width = contentWidth;
            formCard.Width = contentWidth;

            const int innerMargin = 26;
            const int columnGap = 32;
            int fieldWidth = (contentWidth - (innerMargin * 2) - columnGap) / 2;
            int secondColumnLeft = innerMargin + fieldWidth + columnGap;

            locationLabel.Location = new Point(innerMargin, 22);
            locationTextBox.Location = new Point(innerMargin, 52);
            locationTextBox.Width = fieldWidth;
            categoryLabel.Location = new Point(secondColumnLeft, 22);
            categoryComboBox.Location = new Point(secondColumnLeft, 52);
            categoryComboBox.Width = fieldWidth;

            descriptionLabel.Location = new Point(innerMargin, 108);
            descriptionTextBox.Location = new Point(innerMargin, 137);
            descriptionTextBox.Size = new Size(contentWidth - (innerMargin * 2), 92);

            attachmentLabel.Location = new Point(innerMargin, 245);
            chooseFileButton.Location = new Point(innerMargin, 274);
            supportedTypesLabel.Location = new Point(chooseFileButton.Right + 16, 284);
            selectedFileLabel.Location = new Point(secondColumnLeft, 278);
            selectedFileLabel.Size = new Size(contentWidth - secondColumnLeft - innerMargin, 30);

            completionLabel.Location = new Point(innerMargin, 325);
            completionBar.Location = new Point(innerMargin, 352);
            completionBar.Width = contentWidth - (innerMargin * 2);

            encouragementPanel.Location = new Point(innerMargin, 383);
            encouragementPanel.Width = contentWidth - (innerMargin * 2);

            requiredLabel.Location = new Point(innerMargin, 456);
            submitButton.Location = new Point(contentWidth - innerMargin - submitButton.Width, 444);
            clearButton.Location = new Point(submitButton.Left - 16 - clearButton.Width, 445);
            formCard.Height = 505;
            contentPanel.Height = formCard.Bottom + 8;
        }

        private void InputChanged(object sender, EventArgs e)
        {
            UpdateCompletion();
        }

        private void UpdateCompletion()
        {
            int completion = 0;
            if (!string.IsNullOrWhiteSpace(locationTextBox.Text))
                completion += 30;
            if (categoryComboBox.SelectedIndex > 0)
                completion += 30;
            if (!string.IsNullOrWhiteSpace(GetDescription()))
                completion += 30;
            if (!string.IsNullOrWhiteSpace(selectedAttachmentPath))
                completion += 10;

            completionBar.Value = completion;
            completionLabel.Text = "Form completion: " + completion + "%";

            if (completion == 100)
                encouragementLabel.Text = "Thank you for helping us improve Midvaal!";
            else if (completion >= 90)
                encouragementLabel.Text = "Ready to submit. A supporting attachment is optional.";
            else if (completion >= 60)
                encouragementLabel.Text = "Almost there — add a clear description of the problem.";
            else if (completion > 0)
                encouragementLabel.Text = "Great start — complete the remaining required fields.";
            else
                encouragementLabel.Text = "Every report helps us improve Midvaal.";
        }

        private void DescriptionTextBox_Enter(object sender, EventArgs e)
        {
            if (!showingDescriptionWatermark)
                return;

            showingDescriptionWatermark = false;
            descriptionTextBox.Text = string.Empty;
            descriptionTextBox.ForeColor = UiTheme.TextPrimary;
        }

        private void DescriptionTextBox_Leave(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(descriptionTextBox.Text))
                SetDescriptionWatermark();
        }

        private void SetDescriptionWatermark()
        {
            showingDescriptionWatermark = true;
            descriptionTextBox.ForeColor = UiTheme.DisabledText;
            descriptionTextBox.Text = "Describe the issue in detail...";
        }

        private string GetDescription()
        {
            return showingDescriptionWatermark ? string.Empty : descriptionTextBox.Text.Trim();
        }

        private void ChooseFileButton_Click(object sender, EventArgs e)
        {
            if (attachmentDialog.ShowDialog(this) != DialogResult.OK)
                return;

            FileInfo file = new FileInfo(attachmentDialog.FileName);
            string extension = file.Extension.ToLowerInvariant();

            if (!AllowedExtensions.Contains(extension))
            {
                MessageBox.Show(this, "Choose a JPG, PNG, PDF or DOCX file.", "Unsupported attachment",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (file.Length > MaximumAttachmentBytes)
            {
                MessageBox.Show(this, "The attachment must be 10 MB or smaller.", "Attachment too large",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            selectedAttachmentPath = file.FullName;
            selectedFileLabel.Text = file.Name;
            selectedFileLabel.ForeColor = UiTheme.TextPrimary;
            UpdateCompletion();
            SetStatus("Attachment selected: " + file.Name);
        }

        private void SubmitButton_Click(object sender, EventArgs e)
        {
            string validationMessage;
            Control invalidControl;
            if (!ValidateForm(out validationMessage, out invalidControl))
            {
                MessageBox.Show(this, validationMessage, "Required information",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                invalidControl.Focus();
                return;
            }

            try
            {
                MunicipalIssue issue = repository.AddIssue(
                    locationTextBox.Text,
                    categoryComboBox.SelectedItem.ToString(),
                    GetDescription(),
                    selectedAttachmentPath);

                MessageBox.Show(
                    this,
                    "Thank you. Your issue was submitted successfully.\n\nReference: " + issue.Reference,
                    "Report submitted",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                CloseWithResult(FormNavigationResult.Back);
            }
            catch (Exception exception) when (
                exception is IOException ||
                exception is UnauthorizedAccessException ||
                exception is InvalidOperationException)
            {
                MessageBox.Show(
                    this,
                    "The report could not be saved. Please check the attachment and try again.\n\n" + exception.Message,
                    "Submission error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private bool ValidateForm(out string message, out Control invalidControl)
        {
            if (string.IsNullOrWhiteSpace(locationTextBox.Text))
            {
                message = "Enter the location of the issue.";
                invalidControl = locationTextBox;
                return false;
            }

            if (categoryComboBox.SelectedIndex <= 0)
            {
                message = "Select an issue category.";
                invalidControl = categoryComboBox;
                return false;
            }

            if (string.IsNullOrWhiteSpace(GetDescription()))
            {
                message = "Describe the municipal issue.";
                invalidControl = descriptionTextBox;
                return false;
            }

            if (!string.IsNullOrWhiteSpace(selectedAttachmentPath) && !File.Exists(selectedAttachmentPath))
            {
                message = "The selected attachment no longer exists. Choose the file again.";
                invalidControl = chooseFileButton;
                return false;
            }

            message = string.Empty;
            invalidControl = submitButton;
            return true;
        }

        private void ClearButton_Click(object sender, EventArgs e)
        {
            if (!HasUserInput() || MessageBox.Show(
                    this,
                    "Clear all information entered on this form?",
                    "Clear form",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question,
                    MessageBoxDefaultButton.Button2) == DialogResult.Yes)
            {
                ClearForm();
            }
        }

        private void ClearForm()
        {
            locationTextBox.Clear();
            categoryComboBox.SelectedIndex = 0;
            descriptionTextBox.Clear();
            SetDescriptionWatermark();
            selectedAttachmentPath = string.Empty;
            selectedFileLabel.Text = "No file selected";
            selectedFileLabel.ForeColor = UiTheme.TextSecondary;
            UpdateCompletion();
            locationTextBox.Focus();
            SetStatus("Form cleared.");
        }

        private bool HasUserInput()
        {
            return !string.IsNullOrWhiteSpace(locationTextBox.Text) ||
                   categoryComboBox.SelectedIndex > 0 ||
                   !string.IsNullOrWhiteSpace(GetDescription()) ||
                   !string.IsNullOrWhiteSpace(selectedAttachmentPath);
        }

        private bool ConfirmDiscard()
        {
            return !HasUserInput() || MessageBox.Show(
                this,
                "Discard the information entered on this form?",
                "Discard report",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question,
                MessageBoxDefaultButton.Button2) == DialogResult.Yes;
        }

        private void BackButton_Click(object sender, EventArgs e)
        {
            if (ConfirmDiscard())
                CloseWithResult(FormNavigationResult.Back);
        }

        protected override void OnHomeRequested()
        {
            if (ConfirmDiscard())
                CloseWithResult(FormNavigationResult.Home);
        }

        protected override void OnLocalEventsRequested()
        {
            if (ConfirmDiscard())
                CloseWithResult(FormNavigationResult.LocalEvents);
        }
    }
}
