using SendGrid;
using SendGrid.Helpers.Mail;

namespace ProjectMonitor.Api.Data
{
    public class Alerts
    {
        public static async Task<bool> SendEmail(object dataToSend, string templateId)
        {
            var client = new SendGridClient(Config.SendGridApiKey);
            var from = new EmailAddress("", "");
            var to = new EmailAddress("", "");

            var msg = MailHelper.CreateSingleTemplateEmail(from, to, templateId, dataToSend);
            var response = await client.SendEmailAsync(msg);

            if (response.IsSuccessStatusCode)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
