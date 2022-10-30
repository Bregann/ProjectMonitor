namespace ProjectMonitor.Api.Helpers
{
    public class TimezoneHelper
    {
        public static DateTime ConvertDateTimeToLocalTime(DateTime dt)
        {
            var sourceTZ = TimeZoneInfo.FindSystemTimeZoneById("UTC");
            var destinazionTZ = TimeZoneInfo.FindSystemTimeZoneById("GMT Standard Time");

            return DateTime.SpecifyKind(TimeZoneInfo.ConvertTime(dt, sourceTZ, destinazionTZ), DateTimeKind.Local);
        }
    }
}
