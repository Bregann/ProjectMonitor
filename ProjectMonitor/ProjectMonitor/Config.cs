namespace ProjectMonitor.Api
{
    public class Config
    {
        public static readonly string Key = "";
        public static readonly string SendGridApiKey = "";
        public static readonly string PMErrorsTemplateId = "";
        public static readonly string PMErrorsResolvedTemplateId = "";

#if DEBUG
        public static readonly string DbHost = "";
        public static readonly string DbUsername = "";
        public static readonly string DbPassword = "";

#else
        public static readonly string DbHost = "";
        public static readonly string DbUsername = "";
        public static readonly string DbPassword = "";
#endif
    }
}
