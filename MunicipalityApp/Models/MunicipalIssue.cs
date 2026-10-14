using System;
using System.Collections.Generic;

namespace MunicipalityApp.Models
{
    public enum IssueStatus
    {
        Open,
        InProgress,
        Resolved
    }

    [Serializable]
    public class IssueStatusUpdate
    {
        public DateTime DateUpdated { get; set; }
        public IssueStatus Status { get; set; }
        public string Message { get; set; }

        public IssueStatusUpdate()
        {
            DateUpdated = DateTime.Now;
            Status = IssueStatus.Open;
            Message = string.Empty;
        }

        public string StatusDisplay
        {
            get
            {
                return Status == IssueStatus.InProgress ? "In Progress" : Status.ToString();
            }
        }
    }

    [Serializable]
    public class MunicipalIssue
    {
        public string Reference { get; set; }
        public string Location { get; set; }
        public string Category { get; set; }
        public string Description { get; set; }
        public string AttachmentPath { get; set; }
        public DateTime DateReported { get; set; }
        public IssueStatus Status { get; set; }
        public List<IssueStatusUpdate> StatusHistory { get; set; }
        public int FeedbackRating { get; set; }
        public string FeedbackComment { get; set; }
        public DateTime? FeedbackDate { get; set; }

        public MunicipalIssue()
        {
            Reference = string.Empty;
            Location = string.Empty;
            Category = string.Empty;
            Description = string.Empty;
            AttachmentPath = string.Empty;
            DateReported = DateTime.Now;
            Status = IssueStatus.Open;
            StatusHistory = new List<IssueStatusUpdate>();
            FeedbackRating = 0;
            FeedbackComment = string.Empty;
            FeedbackDate = null;
        }

        public string StatusDisplay
        {
            get
            {
                return Status == IssueStatus.InProgress ? "In Progress" : Status.ToString();
            }
        }

        public bool HasFeedback
        {
            get { return FeedbackRating > 0; }
        }
    }
}
