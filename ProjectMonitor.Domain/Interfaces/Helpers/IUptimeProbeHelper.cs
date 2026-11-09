using ProjectMonitor.Domain.DTOs.Configuration;
using ProjectMonitor.Domain.DTOs.Monitoring;

namespace ProjectMonitor.Domain.Interfaces.Helpers
{
    public interface IUptimeProbeHelper
    {
        Task<ProbeResult> CheckEndpoint(EndpointConfig endpoint);
    }
}
