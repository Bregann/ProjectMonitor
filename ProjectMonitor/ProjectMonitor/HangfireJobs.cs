using Hangfire;
using ProjectMonitor.Api.Data;

namespace ProjectMonitor.Api
{
    public class HangfireJobs
    {
        public static void SetupHangfireJobs()
        {
            RecurringJob.AddOrUpdate("CheckServiceHealthJob", () => CheckServiceHealthJob(), "*/30 * * * * *");
            RecurringJob.AddOrUpdate("CheckProjectErrors", () => CheckProjectErrors(), "*/15 * * * * *");
            RecurringJob.AddOrUpdate("CheckForEmailsToSend", () => CheckForEmailsToSend(), "*/20 * * * * *");
        }

        public static void CheckServiceHealthJob()
        {
            ErrorMonitoring.CheckProjectAndSystemHealth();
        }

        public static void CheckProjectErrors()
        {
            ErrorMonitoring.CheckForProjectErrors();
            ErrorMonitoring.CheckErrorsAndCompleteIfFixed();
        }

        public static void CheckForEmailsToSend()
        {
            ErrorMonitoring.CheckAndSendOutErrors();
        }
    }
}