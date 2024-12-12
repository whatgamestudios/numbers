using UnityEngine;

using System;

public class Timeline{

    public static int DaysSinceEpochStart() {
        DateTime now = getTimeNow();
        DateTime dawnOfTime = new DateTime(2024, 12, 1, 12, 0, 0);
        TimeSpan diff = now.Subtract(dawnOfTime);
        return diff.Days;
    }

    public static TimeSpan SecondsToNextDay() {
        DateTime now = getTimeNow();
        DateTime nextDay = new DateTime(now.Year, now.Month, now.Day, 12, 0, 0);
        if (nextDay.CompareTo(now) < 0) {
            nextDay = nextDay.AddDays(1.0);
        }
        return nextDay.Subtract(now);
    }

    public static string TimeToNextDay() {
        TimeSpan diff = SecondsToNextDay();
        string format = "{0,2:D2}:{1,2:D2}:{2,2:D2}";
        return string.Format(format, diff.Hours, diff.Minutes, diff.Seconds);
    }

    private static DateTime getTimeNow() {
        DateTime timeUtc = DateTime.UtcNow;
        string displayName = "AEST";
        string standardName = "AEST"; 
        TimeSpan offset = new TimeSpan(10, 00, 00);
        TimeZoneInfo timezone = TimeZoneInfo.CreateCustomTimeZone(standardName, offset, displayName, standardName);
        return TimeZoneInfo.ConvertTimeFromUtc(timeUtc, timezone);
    }

}
