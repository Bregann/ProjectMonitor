using ProjectMonitor.Domain.DTOs.Reports;

namespace ProjectMonitor.Domain.Interfaces.Api.Reports
{
    public interface IReportService
    {
        /// <summary>Records the values a project sent to POST /api/report and re-evaluates its checks.</summary>
        Task<ReportResultEnum> RecordReport(string? apiKey, ReportRequest request);
    }
}
