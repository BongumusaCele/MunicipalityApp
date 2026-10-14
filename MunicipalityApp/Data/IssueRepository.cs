using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using MunicipalityApp.Models;

namespace MunicipalityApp.Data
{
    internal sealed class IssueRepository
    {
        private static readonly Lazy<IssueRepository> current =
            new Lazy<IssueRepository>(() => new IssueRepository());

        private readonly object syncRoot = new object();
        private readonly string dataDirectory;
        private readonly string attachmentsDirectory;
        private readonly string dataFilePath;
        private List<MunicipalIssue> issues;

        internal static IssueRepository Current
        {
            get { return current.Value; }
        }

        private IssueRepository()
        {
            string configuredDirectory = Environment.GetEnvironmentVariable("MIDVAAL_DATA_DIRECTORY");
            dataDirectory = string.IsNullOrWhiteSpace(configuredDirectory)
                ? Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                    "MidvaalCitizenServices")
                : Path.GetFullPath(configuredDirectory);
            attachmentsDirectory = Path.Combine(dataDirectory, "Attachments");
            dataFilePath = Path.Combine(dataDirectory, "issues.xml");

            Directory.CreateDirectory(dataDirectory);
            Directory.CreateDirectory(attachmentsDirectory);
            issues = LoadIssues();
            NormaliseIssues();

            if (issues.Count == 0 && !File.Exists(dataFilePath))
            {
                issues = CreateSeedIssues();
                NormaliseIssues();
                SaveIssues();
            }
        }

        internal List<MunicipalIssue> GetAll()
        {
            lock (syncRoot)
            {
                return issues
                    .OrderByDescending(issue => issue.DateReported)
                    .Select(CloneIssue)
                    .ToList();
            }
        }

        internal MunicipalIssue AddIssue(
            string location,
            string category,
            string description,
            string attachmentSourcePath)
        {
            lock (syncRoot)
            {
                string reference = GenerateReference();
                string storedAttachment = CopyAttachment(reference, attachmentSourcePath);

                MunicipalIssue issue = new MunicipalIssue
                {
                    Reference = reference,
                    Location = location.Trim(),
                    Category = category.Trim(),
                    Description = description.Trim(),
                    AttachmentPath = storedAttachment,
                    DateReported = DateTime.Now,
                    Status = IssueStatus.Open
                };
                issue.StatusHistory.Add(new IssueStatusUpdate
                {
                    DateUpdated = issue.DateReported,
                    Status = IssueStatus.Open,
                    Message = "Report received by Midvaal Citizen Services."
                });

                issues.Add(issue);

                try
                {
                    SaveIssues();
                }
                catch
                {
                    issues.Remove(issue);
                    if (!string.IsNullOrEmpty(storedAttachment) && File.Exists(storedAttachment))
                        File.Delete(storedAttachment);
                    throw;
                }

                return CloneIssue(issue);
            }
        }

        internal MunicipalIssue UpdateStatus(string reference, IssueStatus status, string message)
        {
            lock (syncRoot)
            {
                MunicipalIssue issue = issues.FirstOrDefault(item =>
                    string.Equals(item.Reference, reference, StringComparison.OrdinalIgnoreCase));

                if (issue == null)
                    throw new InvalidOperationException("The issue reference could not be found.");

                issue.Status = status;
                issue.StatusHistory.Add(new IssueStatusUpdate
                {
                    DateUpdated = DateTime.Now,
                    Status = status,
                    Message = string.IsNullOrWhiteSpace(message)
                        ? "Issue status updated to " + GetStatusDisplay(status) + "."
                        : message.Trim()
                });
                SaveIssues();
                return CloneIssue(issue);
            }
        }

        internal MunicipalIssue SubmitFeedback(string reference, int rating, string feedbackComment)
        {
            lock (syncRoot)
            {
                MunicipalIssue issue = issues.FirstOrDefault(item =>
                    string.Equals(item.Reference, reference, StringComparison.OrdinalIgnoreCase));

                if (issue == null)
                    throw new InvalidOperationException("The issue reference could not be found.");
                if (issue.Status != IssueStatus.Resolved)
                    throw new InvalidOperationException("Feedback is available only after an issue is resolved.");
                if (rating < 1 || rating > 5)
                    throw new ArgumentOutOfRangeException("rating", "Select a rating from 1 to 5.");
                if (issue.HasFeedback)
                    throw new InvalidOperationException("Feedback has already been submitted for this issue.");

                issue.FeedbackRating = rating;
                issue.FeedbackComment = string.IsNullOrWhiteSpace(feedbackComment)
                    ? string.Empty
                    : feedbackComment.Trim();
                issue.FeedbackDate = DateTime.Now;
                SaveIssues();
                return CloneIssue(issue);
            }
        }

        private List<MunicipalIssue> LoadIssues()
        {
            if (!File.Exists(dataFilePath))
                return new List<MunicipalIssue>();

            try
            {
                XmlSerializer serializer = new XmlSerializer(typeof(List<MunicipalIssue>));
                using (FileStream stream = new FileStream(dataFilePath, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    List<MunicipalIssue> loaded = serializer.Deserialize(stream) as List<MunicipalIssue>;
                    return loaded ?? new List<MunicipalIssue>();
                }
            }
            catch (InvalidOperationException)
            {
                string backupPath = dataFilePath + ".corrupt-" + DateTime.Now.ToString("yyyyMMddHHmmss");
                File.Copy(dataFilePath, backupPath, true);
                return CreateSeedIssues();
            }
            catch (IOException)
            {
                return new List<MunicipalIssue>();
            }
        }

        private void SaveIssues()
        {
            string temporaryPath = dataFilePath + ".tmp";
            XmlSerializer serializer = new XmlSerializer(typeof(List<MunicipalIssue>));

            using (FileStream stream = new FileStream(temporaryPath, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                serializer.Serialize(stream, issues);
            }

            File.Copy(temporaryPath, dataFilePath, true);
            File.Delete(temporaryPath);
        }

        private void NormaliseIssues()
        {
            foreach (MunicipalIssue issue in issues)
            {
                if (issue.StatusHistory == null)
                    issue.StatusHistory = new List<IssueStatusUpdate>();

                if (issue.StatusHistory.Count == 0)
                {
                    issue.StatusHistory.Add(new IssueStatusUpdate
                    {
                        DateUpdated = issue.DateReported,
                        Status = IssueStatus.Open,
                        Message = "Report received by Midvaal Citizen Services."
                    });

                    if (issue.Status == IssueStatus.InProgress || issue.Status == IssueStatus.Resolved)
                    {
                        issue.StatusHistory.Add(new IssueStatusUpdate
                        {
                            DateUpdated = issue.DateReported.AddDays(1),
                            Status = IssueStatus.InProgress,
                            Message = "The relevant municipal team is attending to the issue."
                        });
                    }

                    if (issue.Status == IssueStatus.Resolved)
                    {
                        issue.StatusHistory.Add(new IssueStatusUpdate
                        {
                            DateUpdated = issue.DateReported.AddDays(2),
                            Status = IssueStatus.Resolved,
                            Message = "The municipal service request has been completed."
                        });
                    }
                }

                if (issue.FeedbackComment == null)
                    issue.FeedbackComment = string.Empty;
            }
        }

        private string GenerateReference()
        {
            int highestNumber = 1000;

            foreach (MunicipalIssue issue in issues)
            {
                if (string.IsNullOrWhiteSpace(issue.Reference) || !issue.Reference.StartsWith("MDV-"))
                    continue;

                int parsedNumber;
                if (int.TryParse(issue.Reference.Substring(4), out parsedNumber))
                    highestNumber = Math.Max(highestNumber, parsedNumber);
            }

            return "MDV-" + (highestNumber + 1).ToString("0000");
        }

        private string CopyAttachment(string reference, string sourcePath)
        {
            if (string.IsNullOrWhiteSpace(sourcePath))
                return string.Empty;

            string destinationDirectory = Path.Combine(attachmentsDirectory, reference);
            Directory.CreateDirectory(destinationDirectory);
            string destinationPath = Path.Combine(destinationDirectory, Path.GetFileName(sourcePath));
            File.Copy(sourcePath, destinationPath, true);
            return destinationPath;
        }

        private static MunicipalIssue CloneIssue(MunicipalIssue source)
        {
            MunicipalIssue clone = new MunicipalIssue
            {
                Reference = source.Reference,
                Location = source.Location,
                Category = source.Category,
                Description = source.Description,
                AttachmentPath = source.AttachmentPath,
                DateReported = source.DateReported,
                Status = source.Status,
                FeedbackRating = source.FeedbackRating,
                FeedbackComment = source.FeedbackComment,
                FeedbackDate = source.FeedbackDate
            };

            clone.StatusHistory = source.StatusHistory
                .Select(update => new IssueStatusUpdate
                {
                    DateUpdated = update.DateUpdated,
                    Status = update.Status,
                    Message = update.Message
                })
                .ToList();
            return clone;
        }

        private static string GetStatusDisplay(IssueStatus status)
        {
            return status == IssueStatus.InProgress ? "In Progress" : status.ToString();
        }

        private static List<MunicipalIssue> CreateSeedIssues()
        {
            return new List<MunicipalIssue>
            {
                new MunicipalIssue
                {
                    Reference = "MDV-1004", Location = "Meyerton CBD", Category = "Roads",
                    Description = "A large pothole is obstructing traffic and may damage vehicles.",
                    DateReported = new DateTime(2026, 9, 1), Status = IssueStatus.Open
                },
                new MunicipalIssue
                {
                    Reference = "MDV-1003", Location = "Sicelo", Category = "Water & Sanitation",
                    Description = "A leaking municipal water pipe is causing water to collect beside the road.",
                    DateReported = new DateTime(2026, 8, 29), Status = IssueStatus.InProgress
                },
                new MunicipalIssue
                {
                    Reference = "MDV-1002", Location = "Henley on Klip", Category = "Waste Removal",
                    Description = "A scheduled refuse collection was missed.",
                    DateReported = new DateTime(2026, 8, 27), Status = IssueStatus.Resolved
                },
                new MunicipalIssue
                {
                    Reference = "MDV-1001", Location = "Golf Park", Category = "Electricity",
                    Description = "A street light is not working and the area is dark at night.",
                    DateReported = new DateTime(2026, 8, 25), Status = IssueStatus.Open
                }
            };
        }
    }
}
