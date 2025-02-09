// Copyright (c) Whatgame Studios 2024 - 2025
using UnityEngine;
using System;

namespace FourteenNumbers {

    /**
    * Game time class to determine which game day it is and how long until the next game day.
    */
    public class Timeline{
        /**
        * Return the number of days since the start of game development. This is
        * to generate a new random seed for each day of game play.
        */
        public static uint GameDay() {
            DateTime now = getTimeNow();
            DateTime dawnOfTime = new DateTime(2024, 12, 1, 0, 0, 0);
            TimeSpan diff = now.Subtract(dawnOfTime);
            return (uint)diff.Days;
        }

        public static DateTime GetRelativeDate(int gameDay) {
            DateTime dawnOfTime = new DateTime(2024, 12, 1, 0, 0, 0);
            return dawnOfTime.AddDays((double) gameDay);
        }

        public static string GetRelativeDateString(int gameDay) {
            DateTime dawnOfTime = new DateTime(2024, 12, 1, 0, 0, 0);
            DateTime relativeDate = dawnOfTime.AddDays((double) gameDay);
            return relativeDate.ToString("dddd MMMM dd, yyyy");
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
            uint gameDay = GameDay();
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

        public static string TimeToNextDayStrShort() {
            TimeSpan diff = TimeToNextDay();
            string format = "{0,2:D2} hours {1,2:D2} minutes";
            return string.Format(format, diff.Hours, diff.Minutes);
        }

        /**
        * Return the time now in the local time zone.
        */
        private static DateTime getTimeNow() {
            return DateTime.Now;
        }

    }
}