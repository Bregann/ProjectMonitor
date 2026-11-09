using ProjectMonitor.Domain.DTOs.Configuration;
using ProjectMonitor.Domain.DTOs.Monitoring;
using ProjectMonitor.Domain.Interfaces.Helpers;
using System.Diagnostics;

namespace ProjectMonitor.Domain.Helpers
{
    /// <summary>One HTTP check. The timeout is the HttpClient's own (set from UptimeTimeoutSeconds in Program.cs).</summary>
    public class UptimeProbeHelper(HttpClient httpClient) : IUptimeProbeHelper
    {
        public async Task<ProbeResult> CheckEndpoint(EndpointConfig endpoint)
        {
            Stopwatch stopwatch = Stopwatch.StartNew();

            try
            {
                using HttpResponseMessage response = await httpClient.GetAsync(endpoint.Url, HttpCompletionOption.ResponseHeadersRead);
                int statusCode = (int)response.StatusCode;
                bool isUp = IsUpStatus(statusCode, endpoint.ExpectedStatusCodes);

                return new ProbeResult(isUp, statusCode, (int)stopwatch.ElapsedMilliseconds, isUp ? null : $"HTTP {statusCode}");
            }
            catch (TaskCanceledException)
            {
                return new ProbeResult(false, null, null, $"No response within {httpClient.Timeout.TotalSeconds:0}s");
            }
            catch (HttpRequestException ex)
            {
                return new ProbeResult(false, null, null, ex.InnerException?.Message ?? ex.Message);
            }
        }

        public static bool IsUpStatus(int statusCode, IReadOnlyCollection<int> expectedStatusCodes)
        {
            return expectedStatusCodes.Count > 0 ? expectedStatusCodes.Contains(statusCode) : statusCode < 500;
        }
    }
}
