namespace ProjectMonitor.Domain.Enums
{
    public enum ValueConditionEnum
    {
        IsTrue,
        IsFalse,
        EqualTo,
        NotEqualTo,
        GreaterThan,
        LessThan,

        /// <summary>The value is a timestamp no older than ExpectedValue minutes (e.g. "lastLeaderboardUpdate").</summary>
        WithinMinutes
    }
}
