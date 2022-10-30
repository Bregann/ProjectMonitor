using RestSharp;

namespace ProjectMonitor.Shared.Projects
{
    public class ProjectMonitorCatBot
    {
        public static void SendDiscordConnectionStateUpdate(bool state)
        {
            try
            {
                if (ProjectMonitorConfig.ApiKey == "")
                {
                    return;
                }

                var client = new RestClient(ProjectMonitorConfig.ApiUrl);
                var request = new RestRequest("/api/CatBot/UpdateDiscordConnectionState", Method.Post);

                request.AddHeader("ProjectApiKey", ProjectMonitorConfig.ApiKey);
                request.AddHeader("EnvMode", ProjectMonitorConfig.Mode);
                request.AddBody(state);
                client.Post(request);
            }
            catch (Exception)
            {
                return;
            }

        }

        public static void SendLastTweetTimeUpdate()
        {
            try
            {
                if (ProjectMonitorConfig.ApiKey == "")
                {
                    return;
                }

                var client = new RestClient(ProjectMonitorConfig.ApiUrl);
                var request = new RestRequest("/api/CatBot/UpdateLastTweetTime", Method.Post);

                request.AddHeader("ProjectApiKey", ProjectMonitorConfig.ApiKey);
                request.AddHeader("EnvMode", ProjectMonitorConfig.Mode);
                client.Post(request);
            }
            catch (Exception)
            {
                return;
            }
        }

        public static void SendLastDiscordPostTimeUpdate()
        {
            try
            {
                if (ProjectMonitorConfig.ApiKey == "")
                {
                    return;
                }

                var client = new RestClient(ProjectMonitorConfig.ApiUrl);
                var request = new RestRequest("/api/CatBot/UpdateLastDiscordPostTime", Method.Post);

                request.AddHeader("ProjectApiKey", ProjectMonitorConfig.ApiKey);
                request.AddHeader("EnvMode", ProjectMonitorConfig.Mode);
                client.Post(request);
            }
            catch (Exception)
            {
                return;
            }
        }
    }
}
