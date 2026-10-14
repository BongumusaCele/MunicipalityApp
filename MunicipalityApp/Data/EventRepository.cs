using System;
using System.Collections.Generic;
using System.Linq;
using MunicipalityApp.Models;

namespace MunicipalityApp.Data
{
    internal sealed class EventRepository
    {
        private const int SearchHistoryLimit = 20;
        private static readonly Lazy<EventRepository> current =
            new Lazy<EventRepository>(() => new EventRepository());

        private readonly SortedDictionary<DateTime, List<MunicipalEvent>> eventsByDate;
        private readonly Dictionary<string, MunicipalEvent> eventsById;
        private readonly HashSet<string> categories;
        private readonly HashSet<DateTime> eventDates;
        private readonly Queue<SearchRecord> recentSearches;
        private readonly Dictionary<string, int> categoryPreferences;
        private readonly Dictionary<string, int> keywordPreferences;

        internal static EventRepository Current { get { return current.Value; } }

        private EventRepository()
        {
            eventsByDate = new SortedDictionary<DateTime, List<MunicipalEvent>>();
            eventsById = new Dictionary<string, MunicipalEvent>(StringComparer.OrdinalIgnoreCase);
            categories = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            eventDates = new HashSet<DateTime>();
            recentSearches = new Queue<SearchRecord>();
            categoryPreferences = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
            keywordPreferences = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
            SeedEvents();
        }

        internal IList<string> GetCategories()
        {
            return categories.OrderBy(value => value).ToList();
        }

        internal IList<DateTime> GetEventDates()
        {
            return eventDates.OrderBy(value => value).ToList();
        }

        internal IList<MunicipalEvent> Search(string text, string category, DateTime? date)
        {
            string query = (text ?? string.Empty).Trim();
            string selectedCategory = string.Equals(category, "All Categories", StringComparison.OrdinalIgnoreCase)
                ? string.Empty : (category ?? string.Empty).Trim();

            RecordSearch(query, selectedCategory, date);

            IEnumerable<MunicipalEvent> source;
            if (date.HasValue)
            {
                List<MunicipalEvent> datedEvents;
                source = eventsByDate.TryGetValue(date.Value.Date, out datedEvents)
                    ? datedEvents : Enumerable.Empty<MunicipalEvent>();
            }
            else
            {
                source = eventsByDate.SelectMany(pair => pair.Value);
            }

            if (!string.IsNullOrWhiteSpace(selectedCategory))
                source = source.Where(item => string.Equals(item.Category, selectedCategory, StringComparison.OrdinalIgnoreCase));

            if (!string.IsNullOrWhiteSpace(query))
                source = source.Where(item => Contains(item.Title, query) || Contains(item.Description, query) ||
                    Contains(item.Venue, query) || Contains(item.Category, query));

            return source.OrderBy(item => item.EventDate).ThenBy(item => item.Title).ToList();
        }

        internal IList<EventRecommendation> GetRecommendations(int maximum)
        {
            return eventsByDate.SelectMany(pair => pair.Value)
                .Where(item => item.EventDate.Date >= DateTime.Today)
                .Select(item => Score(item))
                .Where(item => item.Score > 0)
                .OrderByDescending(item => item.Score)
                .ThenBy(item => item.Event.EventDate)
                .Take(maximum)
                .ToList();
        }

        private EventRecommendation Score(MunicipalEvent item)
        {
            int score = 0;
            string reason = string.Empty;
            int categoryScore;
            if (categoryPreferences.TryGetValue(item.Category, out categoryScore))
            {
                score += categoryScore * 4;
                reason = "Based on your interest in " + item.Category;
            }

            foreach (KeyValuePair<string, int> preference in keywordPreferences)
            {
                if (Contains(item.Title, preference.Key) || Contains(item.Description, preference.Key))
                {
                    score += preference.Value * 2;
                    if (string.IsNullOrEmpty(reason))
                        reason = "Matches your search for “" + preference.Key + "”";
                }
            }

            return new EventRecommendation { Event = item, Score = score, Reason = reason };
        }

        private void RecordSearch(string query, string category, DateTime? date)
        {
            if (string.IsNullOrWhiteSpace(query) && string.IsNullOrWhiteSpace(category) && !date.HasValue)
                return;

            recentSearches.Enqueue(new SearchRecord { Query = query, Category = category, Date = date });
            while (recentSearches.Count > SearchHistoryLimit)
                recentSearches.Dequeue();

            if (!string.IsNullOrWhiteSpace(category))
                Increment(categoryPreferences, category);

            foreach (string word in query.Split(new[] { ' ', ',', '.', ';' }, StringSplitOptions.RemoveEmptyEntries))
            {
                if (word.Length >= 3)
                    Increment(keywordPreferences, word.ToLowerInvariant());
            }
        }

        private static void Increment(Dictionary<string, int> preferences, string key)
        {
            int count;
            preferences.TryGetValue(key, out count);
            preferences[key] = count + 1;
        }

        private void Add(MunicipalEvent item)
        {
            eventsById[item.Id] = item;
            categories.Add(item.Category);
            eventDates.Add(item.EventDate.Date);
            List<MunicipalEvent> items;
            if (!eventsByDate.TryGetValue(item.EventDate.Date, out items))
            {
                items = new List<MunicipalEvent>();
                eventsByDate.Add(item.EventDate.Date, items);
            }
            items.Add(item);
        }

        private void SeedEvents()
        {
            DateTime today = DateTime.Today;
            Add(new MunicipalEvent { Id = "EVT-001", Title = "Community Clean-up Day", Category = "Community", EventDate = today.AddDays(5).AddHours(8), Venue = "Meyerton Civic Centre", Description = "Join neighbours in cleaning public spaces. Bags and gloves will be supplied." });
            Add(new MunicipalEvent { Id = "EVT-002", Title = "Water Supply Maintenance", Category = "Service Notice", EventDate = today.AddDays(2).AddHours(9), Venue = "Sicelo and surrounding areas", Description = "Planned maintenance may interrupt water supply between 09:00 and 15:00.", IsAnnouncement = true });
            Add(new MunicipalEvent { Id = "EVT-003", Title = "Youth Career Workshop", Category = "Education", EventDate = today.AddDays(9).AddHours(10), Venue = "Meyerton Library", Description = "Free CV, interview and career planning workshop for young residents." });
            Add(new MunicipalEvent { Id = "EVT-004", Title = "Farmers and Craft Market", Category = "Markets", EventDate = today.AddDays(12).AddHours(9), Venue = "Henley on Klip Recreation Hall", Description = "Local food, crafts and small-business stalls for the whole family." });
            Add(new MunicipalEvent { Id = "EVT-005", Title = "Electricity Network Upgrade", Category = "Service Notice", EventDate = today.AddDays(16).AddHours(8), Venue = "Golf Park", Description = "A scheduled outage will support upgrades to the local electricity network.", IsAnnouncement = true });
            Add(new MunicipalEvent { Id = "EVT-006", Title = "Senior Wellness Morning", Category = "Health", EventDate = today.AddDays(20).AddHours(9), Venue = "Midvaal Community Hall", Description = "Health screenings, light exercise and municipal support information." });
            Add(new MunicipalEvent { Id = "EVT-007", Title = "Recycling Information Session", Category = "Environment", EventDate = today.AddDays(24).AddHours(17), Venue = "Meyerton Town Hall", Description = "Learn how household recycling works and where to find collection points." });
        }

        private static bool Contains(string value, string search)
        {
            return !string.IsNullOrEmpty(value) && value.IndexOf(search, StringComparison.OrdinalIgnoreCase) >= 0;
        }

        private sealed class SearchRecord
        {
            internal string Query { get; set; }
            internal string Category { get; set; }
            internal DateTime? Date { get; set; }
        }
    }
}
