using ProjectMonitor.Domain.Enums;
using ProjectMonitor.Domain.Interfaces.Helpers;
using Serilog;
using System.Net.Http.Json;

namespace ProjectMonitor.Domain.Helpers
{
    public class CommsSenderClient(HttpClient httpClient, IEnvironmentalSettingHelper environmentalSettingHelper) : ICommsSenderClient
    {
        // Same as the other projects: CommsSender is reached over the internal Docker network
        private const string BaseAddress = "http://CommsSenderApi:8080";

        private string? ApiKey => environmentalSettingHelper.TryGetEnvironmentalSettingValue(EnvironmentalSettingEnum.CommsSenderApiKey);

        private long? ChatId => long.TryParse(environmentalSettingHelper.TryGetEnvironmentalSettingValue(EnvironmentalSettingEnum.TelegramChatId), out long id) ? id : null;

        public bool IsConfigured => ApiKey is not null && ChatId is not null;

        public async Task<bool> SendTelegramMessage(string messageText)
        {
            if (!IsConfigured)
            {
                return false;
            }

            try
            {
                using HttpRequestMessage request = new(HttpMethod.Post, $"{BaseAddress}/api/Message/SendTelegramMessage")
                {
                    Content = JsonContent.Create(new { ChatId, MessageText = messageText })
                };
                request.Headers.Add("X-CommsSender-ApiKey", ApiKey);

                using HttpResponseMessage response = await httpClient.SendAsync(request);
                response.EnsureSuccessStatusCode();
                return true;
            }
            catch (Exception ex)
            {
                Log.Warning(ex, "[Alerts] Couldn't send Telegram message: {Text}", messageText);
                return false;
            }
        }
    }
}
