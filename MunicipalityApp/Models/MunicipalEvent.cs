using System;

namespace MunicipalityApp.Models
{
    internal sealed class MunicipalEvent
    {
        internal string Id { get; set; }
        internal string Title { get; set; }
        internal string Category { get; set; }
        internal DateTime EventDate { get; set; }
        internal string Venue { get; set; }
        internal string Description { get; set; }
        internal bool IsAnnouncement { get; set; }
    }

    internal sealed class EventRecommendation
    {
        internal MunicipalEvent Event { get; set; }
        internal int Score { get; set; }
        internal string Reason { get; set; }
    }
}
