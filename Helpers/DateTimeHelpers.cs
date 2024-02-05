namespace Helpers;

public static class DateTimeHelpers
{
    public static double GetTimeDifferenceFromNowInSecondsWithSpare(this DateTime dateTime)
    {
        return dateTime.GetTimeDifferenceFromNowInSeconds() + 1;
    }
    
    public static double GetTimeDifferenceFromNowInSeconds(this DateTime dateTime)
    {
        return dateTime.Subtract(DateTime.UtcNow).TotalSeconds;
    }

    public static string GetFormattedDateTime(this DateTime dateTime)
    {
        const string format = "dd.MM HH:mm";
        return dateTime.ToString(format);
    }
    
    public static string GetFormattedTimeDifference(this TimeSpan timespan)
    {
        return $"{Math.Round(timespan.TotalDays)}d {timespan.Hours}h {timespan.Minutes}m";
    }
    
}