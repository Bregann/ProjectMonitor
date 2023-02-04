namespace ProjectMonitor.Api.Data.Enums
{
    public class ErrorTypes
    {
        public const string SystemDown = "SystemDown";
        public const string ProjectDown = "ProjectDown";
        public const string ProjectCrashed = "ProjectCrashed";
        public const string TwitchIRCError = "TwitchIRCError";
        public const string TwitchPubSubError = "TwitchPubSubError";
        public const string DiscordConnectionError = "DiscordConnectionError";
        public const string TwitchAPIRefreshError = "TwitchAPIRefreshError";
        public const string StreamNotAnnounced = "StreamNotAnnounced";
        public const string DailyPointsNotEnabled = "DailyPointsNotEnabled";
        public const string DiscordLbNotUpdated = "DiscordLbNotUpdated";
        public const string UserHoursNotUpdated = "UserHoursNotUpdated";
        public const string RAGamesNotUpdated = "RAGamesNotUpdated";
        public const string MonzoApiRefreshError = "MonzoApiRefreshError";
        public const string MonzoApiStatusError = "MonzoApiStatusError";
        public const string CatBotDiscordConnectionError = "CatBotDiscordConnectionError";
        public const string LastTweetNotSent = "LastTweetNotSent";
        public const string LastDiscordPostNotSent = "LastDiscordPostNotSent";
    }
}