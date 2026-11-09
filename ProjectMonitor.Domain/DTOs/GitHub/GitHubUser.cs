using System.Text.Json.Serialization;

namespace ProjectMonitor.Domain.DTOs.GitHub
{
    public class GitHubUser
    {
        [JsonPropertyName("login")]
        public string Login { get; set; } = "";
    }
}
