using Application.Services.IServices;
using System.Text.Json;

namespace Application.Services
{
    public class JiraService : IJiraService
    {
        private readonly HttpClient _httpClient;

        public JiraService(IHttpClientFactory factory)
        {
            _httpClient = factory.CreateClient("Jira");

        }

        public async Task<object> GetSprintIssuesAsync(int boardId, int sprintId)
        {
            var response = await _httpClient.GetAsync($"rest/agile/1.0/sprint/{sprintId}/issue");
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();

            using var doc = JsonDocument.Parse(json);
            var issues = doc.RootElement.GetProperty("issues");

            var today = DateOnly.FromDateTime(DateTime.UtcNow);
            var events = issues.EnumerateArray()
                               .Select(issue => BuildEvent(issue, today))
                               .ToList();

            return new { boardId, sprintId, events };
        }

        private static string? ParseAssignee(JsonElement fields)
        {
            if (fields.TryGetProperty("assignee", out var aEl) &&
                aEl.ValueKind == JsonValueKind.Object &&
                aEl.TryGetProperty("displayName", out var dn))
            {
                return dn.GetString();
            }
            return null;
        }

        private static (string Start, string End) ParseDates(JsonElement fields)
        {
            DateOnly? start = null, end = null;

            // Prefer custom Start Date field
            if (fields.TryGetProperty("customfield_10015", out var sEl) &&
                DateTime.TryParse(sEl.GetString(), out var sDate))
            {
                start = DateOnly.FromDateTime(sDate);
            }

            // End date
            if (fields.TryGetProperty("duedate", out var dEl) &&
                DateTime.TryParse(dEl.GetString(), out var due))
            {
                end = DateOnly.FromDateTime(due);
            }

            // Fallbacks
            if (start is null && end is not null) start = end;
            if (end is null && start is not null) end = start;

            return (
                start!.Value.ToDateTime(TimeOnly.MinValue).ToString("yyyy-MM-dd"),
                end!.Value.AddDays(1).ToDateTime(TimeOnly.MinValue).ToString("yyyy-MM-dd")
            );
        }

        private static string BuildEventClass(string statusCat, bool hasDeps, bool isLate)
        {
            if (hasDeps) return "evt-dep";
            if (isLate) return "evt-late";
            if (statusCat == "In Progress") return "evt-progress";
            return "evt-normal";
        }

        private static object BuildEvent(JsonElement issue, DateOnly today)
        {
            var key = issue.GetProperty("key").GetString();
            var fields = issue.GetProperty("fields");
            var summary = fields.GetProperty("summary").GetString();

            var assignee = ParseAssignee(fields);
            var (fcStart, fcEnd) = ParseDates(fields);

            var statusObj = fields.GetProperty("status");
            var status = statusObj.GetProperty("name").GetString();
            var statusCat = statusObj.GetProperty("statusCategory").GetProperty("name").GetString();

            var hasDeps = fields.TryGetProperty("issuelinks", out var links) && links.GetArrayLength() > 0;

            // Parse end for late check
            DateTime.TryParse(fcEnd, out var endDt);
            var isLate = DateOnly.FromDateTime(endDt.AddDays(-1)) < today && statusCat != "Done";

            var cls = BuildEventClass(statusCat, hasDeps, isLate);

            return new
            {
                id = key,
                title = summary,
                start = fcStart,
                end = fcEnd,
                className = new[] { cls },
                extendedProps = new
                {
                    status,
                    assignee,
                    hasDependency = hasDeps,
                    late = isLate
                }
            };
        }


    }
}
