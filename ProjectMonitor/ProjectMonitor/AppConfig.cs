using ProjectMonitor.Api.Database.Context;
using Serilog;

namespace ProjectMonitor.Api
{
    public class AppConfig
    {
        public static readonly string ConnectionString = Environment.GetEnvironmentVariable("PMConnString");

        public static string ToEmailAddress { get; private set; } = "";
        public static string ToEmailAddressName { get; private set; } = "";
        public static string FromEmailAddress { get; private set; } = "";
        public static string FromEmailAddressName { get; private set; } = "";
        public static string ApiKey { get; private set; } = "";
        public static string PMErrorsTemplateId { get; private set; } = "";
        public static string PMErrorsResolvedTemplateId { get; private set; } = "";
        public static string HFConnectionString { get; private set; } = "";
        public static int ChatId { get; private set; } = 0;
        public static string MMSApiKey { get; private set; } = "";

        public static void LoadConfig()
        {
            using(var context = new DatabaseContext())
            {
                var config = context.Config.First();

                ToEmailAddress = config.ToEmailAddress;
                ToEmailAddressName = config.ToEmailAddressName;
                FromEmailAddress = config.FromEmailAddress;
                FromEmailAddressName = config.FromEmailAddressName;
                ApiKey = config.ApiKey;
                PMErrorsTemplateId = config.PMErrorsTemplateId;
                PMErrorsResolvedTemplateId = config.PMErrorsResolvedTemplateId;
                HFConnectionString = config.HFConnectionString;
                ChatId = config.ChatId;
                MMSApiKey = config.MMSApiKey;
                Log.Information("Config loaded from database");
            }
        }
    }
}