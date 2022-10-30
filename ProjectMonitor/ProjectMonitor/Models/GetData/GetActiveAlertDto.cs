namespace ProjectMonitor.Api.Models.GetData
{
    public class GetActiveErrorsDto
    {
        public int ErrorId { get; set; }
        public string ProjectName { get; set; }
        public string ErrorDescription { get; set; }
        public string DateStarted { get; set; }
    }
}
