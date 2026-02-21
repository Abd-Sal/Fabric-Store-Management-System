namespace FabricesStoreManagementSystem.HelpTools;

public static class DateRangeHelper
{
    public static (DateTime UtcFrom, DateTime UtcTo) ConvertToUtcRange(
        DateOnly fromDate,
        DateOnly toDate,
        string timezoneId)
    {
        // Get user's timezone
        TimeZoneInfo userTimeZone;
        try
        {
            userTimeZone = TimeZoneInfo.FindSystemTimeZoneById(timezoneId);
        }
        catch
        {
            userTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Arab Standard Time");
        }

        // Convert user's start of day to UTC
        var fromLocalStart = fromDate.ToDateTime(TimeOnly.MinValue); // 00:00:00 local
        var utcFrom = TimeZoneInfo.ConvertTimeToUtc(fromLocalStart, userTimeZone);

        // Convert user's end of day to UTC
        var toLocalEnd = toDate.ToDateTime(TimeOnly.MaxValue); // 23:59:59.999 local
        var utcTo = TimeZoneInfo.ConvertTimeToUtc(toLocalEnd, userTimeZone);

        return (utcFrom, utcTo);
    }

    // Alternative: Use DateTimeOffset for clarity
    public static (DateTimeOffset UtcFrom, DateTimeOffset UtcTo) ConvertToUtcOffsetRange(
        DateOnly fromDate,
        DateOnly toDate,
        string timezoneId)
    {
        TimeZoneInfo userTimeZone = TimeZoneInfo.FindSystemTimeZoneById(timezoneId);

        var fromLocalStart = fromDate.ToDateTime(TimeOnly.MinValue);
        var toLocalEnd = toDate.ToDateTime(TimeOnly.MaxValue);

        var utcFrom = new DateTimeOffset(fromLocalStart, userTimeZone.GetUtcOffset(fromLocalStart)).UtcDateTime;
        var utcTo = new DateTimeOffset(toLocalEnd, userTimeZone.GetUtcOffset(toLocalEnd)).UtcDateTime;

        return (utcFrom, utcTo);
    }
}