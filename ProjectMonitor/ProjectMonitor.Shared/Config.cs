namespace ProjectMonitor.Shared
{
    public class ProjectMonitorConfig
    {
#if DEBUG
        public static readonly string ApiUrl = "https://localhost:7187";
#else
        public static readonly string ApiUrl = "https://dashboardapi.bregan.me";
#endif

        public static string Mode { get; private set; } = "";
        public static string ApiKey { get; private set; } = "";

        public static void SetupMonitor(string envMode, string apiKey = "")
        {
            if (envMode != "debug" && envMode != "release")
            {
                throw new ApplicationException("Invalid mode! This should be debug or release");
            }

            Mode = envMode;
            ApiKey = apiKey;
        }
    }
}
