using System.Text.Json;

namespace ProjectMonitor.Domain.DTOs.Reports
{
    /// <summary>
    /// Body of POST /api/report. Values can be any JSON scalar:
    /// { "values": { "discordConnected": true, "queueLength": 3, "lastLeaderboardUpdate": "2026-11-09T10:00:00Z" } }
    /// </summary>
    public class ReportRequest
    {
        public Dictionary<string, JsonElement> Values { get; set; } = [];
    }
}
