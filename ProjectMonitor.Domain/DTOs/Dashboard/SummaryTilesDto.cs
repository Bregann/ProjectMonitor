namespace ProjectMonitor.Domain.DTOs.Dashboard
{
    public class SummaryTilesDto
    {
        public int TotalProjects { get; set; }
        public int HealthyProjects { get; set; }
        public int WarningProjects { get; set; }
        public int CriticalProjects { get; set; }

        public int ServicesTotal { get; set; }
        public int ServicesUp { get; set; }
        public int ServicesDown { get; set; }
        public double? AverageUptime24h { get; set; }

        public int CriticalVulnerabilities { get; set; }
        public int HighVulnerabilities { get; set; }
        public int OtherVulnerabilities { get; set; }
        public int ReposWithoutAlertAccess { get; set; }

        public int RunningWorkflows { get; set; }
        public int FailingWorkflows { get; set; }
        public int UpdatePullRequests { get; set; }
    }
}
