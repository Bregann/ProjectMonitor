using Newtonsoft.Json;
using RestSharp;

namespace ProjectMonitor.Shared.Projects
{
    public class ProjectMonitorFinanceManager
    {
        public static void SendLastTransactionUpdate()
        {
            try
            {
                if (ProjectMonitorConfig.ApiKey == "")
                {
                    return;
                }

                var client = new RestClient(ProjectMonitorConfig.ApiUrl);
                var request = new RestRequest("/api/FinanceManager/UpdateLastTransactionUpdate", Method.Post);

                request.AddHeader("ProjectApiKey", ProjectMonitorConfig.ApiKey);
                request.AddHeader("EnvMode", ProjectMonitorConfig.Mode);
                client.Post(request);

            }
            catch (Exception)
            {
                return;
            }

        }

        public static void SendLastAPIRefreshUpdate()
        {
            try
            {
                if (ProjectMonitorConfig.ApiKey == "")
                {
                    return;
                }

                var client = new RestClient(ProjectMonitorConfig.ApiUrl);
                var request = new RestRequest("/api/FinanceManager/UpdateLastAPIRefresh", Method.Post);

                request.AddHeader("ProjectApiKey", ProjectMonitorConfig.ApiKey);
                request.AddHeader("EnvMode", ProjectMonitorConfig.Mode);
                client.Post(request);
            }
            catch (Exception)
            {
                return;
            }
        }

        public static void SendLastAPIRefreshStatusCodeUpdate(string statusCode)
        {
            try
            {
                if (ProjectMonitorConfig.ApiKey == "")
                {
                    return;
                }

                var client = new RestClient(ProjectMonitorConfig.ApiUrl);
                var request = new RestRequest("/api/FinanceManager/UpdateLastAPIRefreshStatusCode", Method.Post);

                request.AddHeader("ProjectApiKey", ProjectMonitorConfig.ApiKey);
                request.AddHeader("EnvMode", ProjectMonitorConfig.Mode);

                request.AddBody(JsonConvert.SerializeObject(statusCode));
                client.Post(request);
            }
            catch (Exception)
            {
                return;
            }
        }
    }
}
