using UnityEngine;

using System;

/**
 * Game time class to determine which game day it is and how long until the next game day.
 */
public class Timeline{
    /**
     * Return the number of days since the start of game development. This is
     * to generate a new random seed for each day of game play.
     */
    public static int DaysSinceEpochStart() {
        DateTime now = getTimeNow();
        DateTime dawnOfTime = new DateTime(2024, 12, 1, 0, 0, 0);
        TimeSpan diff = now.Subtract(dawnOfTime);
        return diff.Days;
    }

    /**
     * Return the time until the next day of game play.
     */
    public static TimeSpan TimeToNextDay() {
        DateTime now = getTimeNow();
        DateTime nextDay = new DateTime(now.Year, now.Month, now.Day, 23, 59, 59);
        return nextDay.Subtract(now);
    }

    public static string GameDayStr() {
        int gameDay = DaysSinceEpochStart();
        return gameDay.ToString();
    }

    /**
     * Return the hours, minutes, and seconds until the next day.
     */
    public static string TimeToNextDayStr() {
        TimeSpan diff = TimeToNextDay();
        string format = "{0,2:D2}:{1,2:D2}:{2,2:D2}";
        return string.Format(format, diff.Hours, diff.Minutes, diff.Seconds);
    }

    /**
     * Return the time now in the local time zone.
     */
    private static DateTime getTimeNow() {
        return DateTime.Now;
    }

}
