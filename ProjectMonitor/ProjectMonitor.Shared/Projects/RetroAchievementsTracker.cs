using RestSharp;

namespace ProjectMonitor.Shared.Projects
{
    public class ProjectMonitorRetroAchievementsTracker
    {
        public static void SendTotalGamesUpdate(long games)
        {
            try
            {
                if (ProjectMonitorConfig.ApiKey == "")
                {
                    return;
                }

                var client = new RestClient(ProjectMonitorConfig.ApiUrl);
                var request = new RestRequest("/api/RetroAchievementsTracker/UpdateTotalGames", Method.Post);

                request.AddHeader("ProjectApiKey", ProjectMonitorConfig.ApiKey);
                request.AddHeader("EnvMode", ProjectMonitorConfig.Mode);

                request.AddBody(games);
                client.Post(request);
            }
            catch (Exception)
            {
                return;
            }
        }

        public static void SendTotalRegisteredUsersUpdate(long users)
        {
            try
            {
                if (ProjectMonitorConfig.ApiKey == "")
                {
                    return;
                }

                var client = new RestClient(ProjectMonitorConfig.ApiUrl);
                var request = new RestRequest("/api/RetroAchievementsTracker/UpdateTotalRegisteredUsers", Method.Post);

                request.AddHeader("ProjectApiKey", ProjectMonitorConfig.ApiKey);
                request.AddHeader("EnvMode", ProjectMonitorConfig.Mode);

                request.AddBody(users);
                client.Post(request);
            }
            catch (Exception)
            {
                return;
            }
        }

        public static void SendLastGameAndGamesUpdatedCountUpdate(long gameCount)
        {
            try
            {
                if (ProjectMonitorConfig.ApiKey == "")
                {
                    return;
                }

                var client = new RestClient(ProjectMonitorConfig.ApiUrl);
                var request = new RestRequest("/api/RetroAchievementsTracker/UpdateLastGameUpdateTimeAndGameCountUpdated", Method.Post);

                request.AddHeader("ProjectApiKey", ProjectMonitorConfig.ApiKey);
                request.AddHeader("EnvMode", ProjectMonitorConfig.Mode);

                request.AddBody(gameCount);
                client.Post(request);
            }
            catch (Exception)
            {
                return;
            }
        }
    }
}
