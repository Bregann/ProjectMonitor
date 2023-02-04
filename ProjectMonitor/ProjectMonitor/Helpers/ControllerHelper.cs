namespace ProjectMonitor.Api.Helpers
{
    public class ControllerHelper
    {
        public static bool ValidateApiRequest(IHeaderDictionary headers)
        {
            //Check if the headers are set
            if (headers["EnvMode"].Count == 0)
            {
                return false;
            }

            if (headers["Authorization"].Count == 0)
            {
                return false;
            }

            //Make sure that the key matches
            string apiKey = headers["Authorization"];

            if (apiKey != AppConfig.ApiKey)
            {
                return false;
            }

            return true;
        }
    }
}