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

            if (headers["ProjectApiKey"].Count == 0)
            {
                return false;
            }

            //Make sure that the key matches
            string apiKey = headers["ProjectApiKey"];

            if (apiKey != Config.Key)
            {
                return false;
            }

            return true;
        }
    }
}
