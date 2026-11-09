
namespace ProjectMonitor.Domain.Helpers
{
    /// <summary>
    /// Decides when a service counts as down: only after <c>failuresBeforeDown</c> failed checks in a row,
    /// so a single slow response doesn't flip the dashboard red (or send an alert).
    /// </summary>
    public class FailureStreakTracker(int failuresBeforeDown)
    {
        private readonly Dictionary<string, (int Failures, DateTime FirstFailureUtc)> _streaks = [];

        /// <summary>Records a check and returns whether the endpoint is down, and since when.</summary>
        public (bool IsDown, DateTime? DownSinceUtc) Record(string key, bool succeeded, DateTime checkedAtUtc)
        {
            if (succeeded)
            {
                _streaks.Remove(key);
                return (false, null);
            }

            (int failures, DateTime firstFailureUtc) = _streaks.TryGetValue(key, out (int, DateTime) existing)
                ? (existing.Item1 + 1, existing.Item2)
                : (1, checkedAtUtc);

            _streaks[key] = (failures, firstFailureUtc);

            bool isDown = failures >= Math.Max(1, failuresBeforeDown);
            return (isDown, isDown ? firstFailureUtc : null);
        }
    }
}
