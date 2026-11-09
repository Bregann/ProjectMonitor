using MudBlazor;
using ProjectMonitor.Domain.Health;
using ProjectMonitor.Domain.State;

namespace ProjectMonitor.Core.Components.Shared
{
    public static class Format
    {
        private static readonly TimeZoneInfo UkTime = FindUkTimeZone();

        public static string Ago(DateTime? utc, DateTime nowUtc)
        {
            if (utc is null)
            {
                return "never";
            }

            TimeSpan age = nowUtc - utc.Value;

            return age.TotalSeconds switch
            {
                < 45 => "just now",
                < 3600 => $"{Math.Max(1, (int)age.TotalMinutes)} min ago",
                < 86400 => $"{(int)age.TotalHours} h ago",
                < 86400 * 30 => $"{(int)age.TotalDays} d ago",
                _ => Local(utc.Value).ToString("d MMM yyyy"),
            };
        }

        public static string Duration(TimeSpan span)
        {
            return span.TotalSeconds switch
            {
                < 60 => $"{(int)span.TotalSeconds}s",
                < 3600 => $"{(int)span.TotalMinutes}m",
                < 86400 => $"{(int)span.TotalHours}h {span.Minutes}m",
                _ => $"{(int)span.TotalDays}d {span.Hours}h",
            };
        }

        public static DateTime Local(DateTime utc)
        {
            return TimeZoneInfo.ConvertTimeFromUtc(DateTime.SpecifyKind(utc, DateTimeKind.Utc), UkTime);
        }

        public static string LocalText(DateTime? utc)
        {
            return utc is null ? "" : Local(utc.Value).ToString("ddd d MMM, HH:mm");
        }

        public static string Percent(double? value)
        {
            return value is null ? "–" : value.Value >= 99.995 ? "100%" : $"{value.Value:0.##}%";
        }

        public static Color HealthColor(HealthLevel level)
        {
            return level switch
            {
                HealthLevel.Critical => Color.Error,
                HealthLevel.Warning => Color.Warning,
                _ => Color.Success,
            };
        }

        public static string HealthText(HealthLevel level)
        {
            return level switch
            {
                HealthLevel.Critical => "Critical",
                HealthLevel.Warning => "Warning",
                _ => "Healthy",
            };
        }

        public static string HealthIcon(HealthLevel level)
        {
            return level switch
            {
                HealthLevel.Critical => Icons.Material.Filled.Error,
                HealthLevel.Warning => Icons.Material.Filled.Warning,
                _ => Icons.Material.Filled.CheckCircle,
            };
        }

        /// <summary>Icon for an individual issue: info notes keep their own "i" icon, unlike the project's overall status.</summary>
        public static string IssueIcon(HealthLevel level)
        {
            return level == HealthLevel.Info ? Icons.Material.Filled.Info : HealthIcon(level);
        }

        public static Color IssueColor(HealthLevel level)
        {
            return level == HealthLevel.Info ? Color.Info : HealthColor(level);
        }

        public static Color SeverityColor(string severity)
        {
            return severity switch
            {
                "critical" => Color.Error,
                "high" => Color.Warning,
                "medium" => Color.Info,
                _ => Color.Default,
            };
        }

        public static (string Icon, Color Color, string Text) RunStatus(WorkflowRunInfo run)
        {
            if (run.IsActive)
            {
                return (Icons.Material.Filled.Autorenew, Color.Info, run.Status == "queued" ? "Queued" : "Running");
            }

            if (run.IsSuccess)
            {
                return (Icons.Material.Filled.CheckCircle, Color.Success, "Passed");
            }

            if (run.IsFailure)
            {
                return (Icons.Material.Filled.Cancel, Color.Error, "Failed");
            }

            return (Icons.Material.Filled.RemoveCircleOutline, Color.Default, run.Conclusion is null ? run.Status : char.ToUpperInvariant(run.Conclusion[0]) + run.Conclusion[1..].Replace('_', ' '));
        }

        private static TimeZoneInfo FindUkTimeZone()
        {
            try
            {
                return TimeZoneInfo.FindSystemTimeZoneById("Europe/London");
            }
            catch (TimeZoneNotFoundException)
            {
                return TimeZoneInfo.Utc;
            }
        }
    }
}
