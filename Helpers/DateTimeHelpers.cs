using System.Text.RegularExpressions;

namespace Helpers;

public static partial class DateTimeHelpers
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
        var result = $"{timespan.Days}d {timespan.Hours}h {timespan.Minutes}m";
        return MyRegex().Replace(result, "");
    }

    [GeneratedRegex(@"(?<!\d)0([dhm])")]
    private static partial Regex MyRegex();
}