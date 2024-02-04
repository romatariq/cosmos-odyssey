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
}