namespace ProjectMonitor.Domain.Enums
{
    /// <summary>
    /// Critical = something is broken now. Warning = needs attention soon. Info = worth knowing, no action needed yet.
    /// </summary>
    public enum HealthLevel
    {
        Healthy = 0,
        Info = 1,
        Warning = 2,
        Critical = 3
    }
}
