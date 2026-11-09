namespace ProjectMonitor.Domain.Interfaces.Helpers
{
    public interface ICommsSenderClient
    {
        /// <summary>Sends a Telegram message to the configured chat. Returns false (and logs) if it couldn't.</summary>
        Task<bool> SendTelegramMessage(string messageText);

        bool IsConfigured { get; }
    }
}
