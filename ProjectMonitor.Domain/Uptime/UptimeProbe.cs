using ProjectMonitor.Domain.Configuration;
using System.Diagnostics;

namespace ProjectMonitor.Domain.Uptime
{
    public record ProbeResult(bool IsUp, int? StatusCode, int? ResponseTimeMs, string? Error);

    public interface IUptimeProbe
    {
        Task<ProbeResult> CheckAsync(EndpointOptions endpoint, TimeSpan timeout, CancellationToken cancellationToken);
    }

    public class UptimeProbe(IHttpClientFactory httpClientFactory) : IUptimeProbe
    {
        public const string HttpClientName = "uptime";

        public async Task<ProbeResult> CheckAsync(EndpointOptions endpoint, TimeSpan timeout, CancellationToken cancellationToken)
        {
            using CancellationTokenSource timeoutSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            timeoutSource.CancelAfter(timeout);

            HttpClient client = httpClientFactory.CreateClient(HttpClientName);
            Stopwatch stopwatch = Stopwatch.StartNew();

            try
            {
                using HttpResponseMessage response = await client.GetAsync(endpoint.Url, HttpCompletionOption.ResponseHeadersRead, timeoutSource.Token);
                int statusCode = (int)response.StatusCode;
                bool isUp = IsUpStatus(statusCode, endpoint.ExpectedStatusCodes);

                return new ProbeResult(isUp, statusCode, (int)stopwatch.ElapsedMilliseconds, isUp ? null : $"HTTP {statusCode}");
            }
            catch (OperationCanceledException) when (!cancellationToken.IsCancellationRequested)
            {
                return new ProbeResult(false, null, null, $"No response within {timeout.TotalSeconds:0}s");
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
