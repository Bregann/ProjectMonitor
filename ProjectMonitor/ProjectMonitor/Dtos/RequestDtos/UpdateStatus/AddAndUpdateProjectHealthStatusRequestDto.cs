namespace ProjectMonitor.Api.Dtos.RequestDtos.UpdateStatus
{
    public class AddAndUpdateProjectHealthStatusRequestDto
    {
        public string ProjectName { get; set; }
        public double CPUUsage { get; set; }
        public long RAMUsage { get; set; }
        public TimeSpan ProjectUptime { get; set; }
        public TimeSpan ServerUptime { get; set; }
    }
}