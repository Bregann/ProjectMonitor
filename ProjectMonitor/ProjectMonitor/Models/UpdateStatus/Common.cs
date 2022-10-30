namespace ProjectMonitor.Api.Models.UpdateStatus
{
    public class AddAndUpdateProjectHealthStatusDto
    {
        public string ProjectName { get; set; }
        public double CPUUsage { get; set; }
        public long RAMUsage { get; set; }
        public TimeSpan ProjectUptime { get; set; }
        public TimeSpan ServerUptime { get; set; }
    }

    public class AddAndUpdateSystemDataDto
    {
        public string SystemName { get; set; }
        public TimeSpan SystemUptime { get; set; }
    }
}
