namespace ProjectMonitor.Shared.Objects
{
    public class AddAndUpdateProjectHealthStatusDto
    {
        public string ProjectName { get; set; }
        public TimeSpan ProjectUptime { get; set; }
        public double CPUUsage { get; set; }
        public long RAMUsage { get; set; }
    }

    public class AddAndUpdateSystemDataDto
    {
        public string SystemName { get; set; }
        public TimeSpan SystemUptime { get; set; }
    }
}
