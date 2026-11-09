using ProjectMonitor.Domain.DTOs.Monitoring;

namespace ProjectMonitor.Domain.Helpers
{
    /// <summary>Works out which alerts to send by comparing this check's critical issues with the last one's.</summary>
    public class AlertTracker
    {
        private Dictionary<string, (string Project, string Message)>? _active;

        public IReadOnlyList<AlertMessage> Update(IReadOnlyList<(string Project, HealthIssue Issue)> critical)
        {
            Dictionary<string, (string Project, string Message)> current = critical
                .GroupBy(x => x.Issue.Key)
                .ToDictionary(g => g.Key, g => (g.First().Project, g.First().Issue.Message));

            // First run: remember what's already broken without announcing it
            if (_active is null)
            {
                _active = current;
                return [];
            }

            List<AlertMessage> messages = [];

            foreach ((string key, (string project, string message)) in current.Where(x => !_active.ContainsKey(x.Key)))
            {
                messages.Add(new(key, $"🔴 {project}: {message}"));
            }

            foreach ((string key, (string project, string message)) in _active.Where(x => !current.ContainsKey(x.Key)))
            {
                messages.Add(new(key, $"✅ {project}: resolved: {message}"));
            }

            _active = current;
            return messages;
        }
    }
}
