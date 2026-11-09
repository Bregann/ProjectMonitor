using Hangfire.Dashboard;

namespace ProjectMonitor.Core
{
    /// <summary>The Hangfire dashboard uses the same login as the rest of the site.</summary>
    public class HangfireDashboardAuthFilter : IDashboardAuthorizationFilter
    {
        public bool Authorize(DashboardContext context)
        {
            return context.GetHttpContext().User.Identity?.IsAuthenticated == true;
        }
    }
}
