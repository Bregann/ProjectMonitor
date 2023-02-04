namespace ProjectMonitor.Api.Dtos.RequestDtos.UpdateStatus
{
    public class AddAndUpdateSystemDataRequestDto
    {
        public string SystemName { get; set; }
        public TimeSpan SystemUptime { get; set; }
    }
}