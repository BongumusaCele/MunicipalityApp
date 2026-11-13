using System;
using System.Collections.Generic;
using System.Linq;
using MunicipalityApp.Models;
using MunicipalityApp.Structures;

namespace MunicipalityApp.Data
{
    internal sealed class ServiceRequestIndex
    {
        internal readonly AvlTree<string, MunicipalIssue> ByReference = new AvlTree<string, MunicipalIssue>(StringComparer.OrdinalIgnoreCase);
        internal readonly SearchTree<int, List<MunicipalIssue>> ByStatus = new SearchTree<int, List<MunicipalIssue>>();
        internal readonly RedBlackTree<string, MunicipalIssue> ByDate = new RedBlackTree<string, MunicipalIssue>(StringComparer.Ordinal);
        internal readonly TreeNode<string> Hierarchy = new TreeNode<string>("All service requests");
        internal readonly RequestGraph Graph = new RequestGraph();
        private readonly List<MunicipalIssue> issues;
        internal ServiceRequestIndex(List<MunicipalIssue> snapshot)
        {
            issues = snapshot;
            foreach (IssueStatus status in Enum.GetValues(typeof(IssueStatus)))
                ByStatus.Put((int)status, new List<MunicipalIssue>());
            foreach (var issue in issues)
            {
                ByReference.Put(issue.Reference, issue);
                ByDate.Put(issue.DateReported.Ticks.ToString("D19") + ":" + issue.Reference, issue);
                List<MunicipalIssue> bucket; ByStatus.TryGet((int)issue.Status, out bucket); bucket.Add(issue);
                var category = Hierarchy.Children.FirstOrDefault(n => n.Value == issue.Category) ?? Hierarchy.Add(issue.Category);
                var state = category.Children.FirstOrDefault(n => n.Value == issue.StatusDisplay) ?? category.Add(issue.StatusDisplay);
                state.Add(issue.Reference);
                Graph.AddVertex(issue.Reference);
            }
            for (int i = 0; i < issues.Count; i++)
                for (int j = i + 1; j < issues.Count; j++)
                {
                    bool location = string.Equals(issues[i].Location, issues[j].Location, StringComparison.OrdinalIgnoreCase);
                    bool category = string.Equals(issues[i].Category, issues[j].Category, StringComparison.OrdinalIgnoreCase);
                    if (location || category) Graph.Connect(issues[i].Reference, issues[j].Reference, location && category ? 1 : location ? 2 : 3);
                }
        }
        internal List<MunicipalIssue> Display(string status, string order)
        {
            IEnumerable<MunicipalIssue> source = ByDate.Ordered();
            if (status != "All statuses")
            {
                List<MunicipalIssue> bucket;
                IssueStatus parsed = status == "In Progress" ? IssueStatus.InProgress : (IssueStatus)Enum.Parse(typeof(IssueStatus), status);
                ByStatus.TryGet((int)parsed, out bucket); var references = new HashSet<string>(bucket.Select(i => i.Reference), StringComparer.OrdinalIgnoreCase);
                source = source.Where(i => references.Contains(i.Reference));
            }
            if (order == "Priority (active first)")
            {
                var heap = new MinHeap<MunicipalIssue>((a, b) => {
                    int c = Priority(a).CompareTo(Priority(b));
                    if (c == 0) c = a.DateReported.CompareTo(b.DateReported);
                    return c != 0 ? c : StringComparer.Ordinal.Compare(a.Reference, b.Reference);
                });
                foreach (var item in source) heap.Push(item);
                var result = new List<MunicipalIssue>(); while (heap.Count > 0) result.Add(heap.Pop()); return result;
            }
            return order == "Newest first" ? source.Reverse().ToList() : source.ToList();
        }
        private static int Priority(MunicipalIssue i)
        { return i.Status == IssueStatus.Resolved ? 2 : i.Status == IssueStatus.Open ? 0 : 1; }
    }
}
