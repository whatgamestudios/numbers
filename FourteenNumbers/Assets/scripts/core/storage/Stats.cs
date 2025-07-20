// Copyright (c) Whatgame Studios 2024 - 2025
using UnityEngine;
using System.Collections;

namespace FourteenNumbers
{

    public class Stats
    {
        public const string STATS_SOLUTIONS = "STATS_SOLUTIONS_";
        public const string STATS_SOLUTION1 = "STATS_SOLUTION1";
        public const string STATS_SOLUTION2 = "STATS_SOLUTION2";
        public const string STATS_SOLUTION3 = "STATS_SOLUTION3";
        public const string STATS_FIRST_PLAYED = "STATS_FIRST_PLAYED";
        public const string STATS_LAST_PLAYED = "STATS_LAST_PLAYED";
        public const string STATS_TIMES_PLAYED = "STATS_TIMES_PLAYED";
        public const string STATS_TOTAL_POINTS = "STATS_TOTAL_POINTS";
        public const string STATS_PERFECT_SCORE_DAYS = "STATS_PERFECT_SCORE_DAYS";

        public const string STATS_POINTS_TODAY = "STATS_POINTS_TODAY";

        public const string STATS_DAYS_PLAYED = "STATS_DAYS_PLAYED";
        public const string STATS_DAYS_CLAIMED = "STATS_DAYS_CLAIMED";
        public const string BEST_TODAY = "BEST_TODAY";

        public const string STATS_MOST_RECENT_PUBLISHED_DAY = "STATS_MOST_RECENT_PUBLISHED_DAY";
        public const string STATS_NUM_TIMES_PUBLISHED = "STATS_NUM_TIMES_PUBLISHED";

        public const string STATS_SILVER_STREAK_LENGTH = "STATS_SSTREAK_LEN";
        public const string STATS_SILVER_STREAK_LONGEST_LENGTH = "STATS_SSTREAK_LONGEST_LEN";
        public const string STATS_SILVER_STREAK_LAST_DAY = "STATS_SSTREAK_LAST_DAY";
        public const uint STATS_SILVER_STREAK_THRESHOLD = 70;
        public const uint STATS_SILVER_STREAK_DAYS = 7;

        public const string STATS_GOLD_STREAK_LENGTH = "STATS_GSTREAK_LEN";
        public const string STATS_GOLD_STREAK_LONGEST_LENGTH = "STATS_GSTREAK_LONGEST_LEN";
        public const string STATS_GOLD_STREAK_LAST_DAY = "STATS_GSTREAK_LAST_DAY";
        public const uint STATS_GOLD_STREAK_THRESHOLD = 140;
        public const uint STATS_GOLD_STREAK_DAYS = 14;

        public const string STATS_DIAMOND_STREAK_LENGTH = "STATS_DSTREAK_LEN";
        public const string STATS_DIAMOND_STREAK_LONGEST_LENGTH = "STATS_DSTREAK_LONGEST_LEN";
        public const string STATS_DIAMOND_STREAK_LAST_DAY = "STATS_DSTREAK_LAST_DAY";
        public const uint STATS_DIAMOND_STREAK_THRESHOLD = 160;
        public const uint STATS_DIAMOND_STREAK_DAYS = 21;

        public const string STATS_BDIAMOND_STREAK_LENGTH = "STATS_BDSTREAK_LEN";
        public const string STATS_BDIAMOND_STREAK_LONGEST_LENGTH = "STATS_BDSTREAK_LONGEST_LEN";
        public const string STATS_BDIAMOND_STREAK_LAST_DAY = "STATS_BDSTREAK_LAST_DAY";
        public const uint STATS_BDIAMOND_STREAK_THRESHOLD = 180;
        public const uint STATS_BDIAMOND_STREAK_DAYS = 28;

        public const int NEVER_PLAYED = -1;


        /**
        * Set the background used by all scenes.
        */
        public static void StartNewGameDay()
        {
            PlayerPrefs.SetString(STATS_SOLUTION1, "");
            PlayerPrefs.SetString(STATS_SOLUTION2, "");
            PlayerPrefs.SetString(STATS_SOLUTION3, "");

            PlayerPrefs.SetInt(STATS_POINTS_TODAY, 0);

            PlayerPrefs.Save();
        }


        public static void SetSolution1(uint gameDay, string solution, int points)
        {
            int firstPlayed = PlayerPrefs.GetInt(STATS_FIRST_PLAYED, NEVER_PLAYED);
            if (firstPlayed == NEVER_PLAYED)
            {
                PlayerPrefs.SetInt(STATS_FIRST_PLAYED, (int)gameDay);
            }

            PlayerPrefs.SetInt(STATS_LAST_PLAYED, (int)gameDay);

            PlayerPrefs.SetString(STATS_SOLUTION1, solution);
            setCombinedSolution(gameDay);

            int timesPlayed = PlayerPrefs.GetInt(STATS_TIMES_PLAYED, 0);
            timesPlayed++;
            PlayerPrefs.SetInt(STATS_TIMES_PLAYED, timesPlayed);

            int totalPoints = PlayerPrefs.GetInt(STATS_TOTAL_POINTS, 0);
            totalPoints += points;
            PlayerPrefs.SetInt(STATS_TOTAL_POINTS, totalPoints);

            PlayerPrefs.SetInt(STATS_POINTS_TODAY, points);

            updateStreaks(gameDay, points);
            PlayerPrefs.Save();
        }

        public static void SetSolution2(uint gameDay, string solution, int points)
        {
            PlayerPrefs.SetString(STATS_SOLUTION2, solution);
            setCombinedSolution(gameDay);

            int totalPoints = PlayerPrefs.GetInt(STATS_TOTAL_POINTS, 0);
            totalPoints += points;
            PlayerPrefs.SetInt(STATS_TOTAL_POINTS, totalPoints);

            int totalPointsToday = PlayerPrefs.GetInt(STATS_POINTS_TODAY, 0);
            totalPointsToday += points;
            PlayerPrefs.SetInt(STATS_POINTS_TODAY, totalPointsToday);
            updateStreaks(gameDay, totalPointsToday);
            PlayerPrefs.Save();
        }

        public static void SetSolution3(uint gameDay, string solution, int points)
        {
            PlayerPrefs.SetString(STATS_SOLUTION3, solution);
            setCombinedSolution(gameDay);

            int totalPoints = PlayerPrefs.GetInt(STATS_TOTAL_POINTS, 0);
            totalPoints += points;
            PlayerPrefs.SetInt(STATS_TOTAL_POINTS, totalPoints);

            int totalPointsToday = PlayerPrefs.GetInt(STATS_POINTS_TODAY, 0);
            totalPointsToday += points;
            PlayerPrefs.SetInt(STATS_POINTS_TODAY, totalPointsToday);
            updateStreaks(gameDay, totalPointsToday, true);

            if (totalPointsToday == 210)
            {
                int perfectScoreDays = PlayerPrefs.GetInt(STATS_PERFECT_SCORE_DAYS, 0);
                perfectScoreDays++;
                PlayerPrefs.SetInt(STATS_PERFECT_SCORE_DAYS, perfectScoreDays);
            }

            PlayerPrefs.Save();
        }


        public static (string, string, string) GetSolutions()
        {
            return (PlayerPrefs.GetString(STATS_SOLUTION1, ""),
                    PlayerPrefs.GetString(STATS_SOLUTION2, ""),
                    PlayerPrefs.GetString(STATS_SOLUTION3, ""));
        }


        public static (int, int, int, int, int) GetStats()
        {
            return (PlayerPrefs.GetInt(STATS_FIRST_PLAYED, 0),
            PlayerPrefs.GetInt(STATS_LAST_PLAYED, 0),
            PlayerPrefs.GetInt(STATS_TIMES_PLAYED, 0),
            PlayerPrefs.GetInt(STATS_TOTAL_POINTS, 0),
            PlayerPrefs.GetInt(STATS_PERFECT_SCORE_DAYS, 0));
        }

        public static int GetLastGameDay()
        {
            return PlayerPrefs.GetInt(STATS_LAST_PLAYED, 0);
        }

        public static int GetNumDaysPlayed()
        {
            return PlayerPrefs.GetInt(STATS_TIMES_PLAYED, 0);
        }

        // public static int GetTotalPointsToday() {
        //     return PlayerPrefs.GetInt(STATS_POINTS_TODAY, 0);
        // }



        public static void SetBestPointsToday(int points)
        {
            PlayerPrefs.SetInt(BEST_TODAY, points);
        }

        public static int GetBestPointsToday()
        {
            return PlayerPrefs.GetInt(BEST_TODAY, 0);
        }

        public static void SetDaysPlayed(int daysPlayed)
        {
            PlayerPrefs.SetInt(STATS_DAYS_PLAYED, daysPlayed);
        }

        public static int GetDaysPlayed()
        {
            return PlayerPrefs.GetInt(STATS_DAYS_PLAYED, 0);
        }

        public static void SetDaysClaimed(int daysPlayed)
        {
            PlayerPrefs.SetInt(STATS_DAYS_CLAIMED, daysPlayed);
        }

        public static int GetDaysClaimed()
        {
            return PlayerPrefs.GetInt(STATS_DAYS_CLAIMED, 0);
        }

        private static void setCombinedSolution(uint gameDay)
        {
            string key = STATS_SOLUTIONS + gameDay.ToString();
            (string sol1, string sol2, string sol3) = GetSolutions();
            string combinedSolution = sol1 + "=" + sol2 + "=" + sol3;
            PlayerPrefs.SetString(key, combinedSolution);
        }

        public static string GetCombinedSolution(uint gameDay)
        {
            string key = STATS_SOLUTIONS + gameDay.ToString();
            return PlayerPrefs.GetString(key, "==");
        }

        private static void updateStreaks(uint gameDay, int pointsToday, bool finalSolutionForToday = false)
        {
            updateStreak(gameDay, pointsToday, finalSolutionForToday,
                STATS_SILVER_STREAK_THRESHOLD, STATS_SILVER_STREAK_LAST_DAY,
                STATS_SILVER_STREAK_LENGTH, STATS_SILVER_STREAK_LONGEST_LENGTH);
            updateStreak(gameDay, pointsToday, finalSolutionForToday,
                STATS_GOLD_STREAK_THRESHOLD, STATS_GOLD_STREAK_LAST_DAY,
                STATS_GOLD_STREAK_LENGTH, STATS_GOLD_STREAK_LONGEST_LENGTH);
            updateStreak(gameDay, pointsToday, finalSolutionForToday,
                STATS_DIAMOND_STREAK_THRESHOLD, STATS_DIAMOND_STREAK_LAST_DAY,
                STATS_DIAMOND_STREAK_LENGTH, STATS_DIAMOND_STREAK_LONGEST_LENGTH);
            updateStreak(gameDay, pointsToday, finalSolutionForToday,
                STATS_BDIAMOND_STREAK_THRESHOLD, STATS_BDIAMOND_STREAK_LAST_DAY,
                STATS_BDIAMOND_STREAK_LENGTH, STATS_BDIAMOND_STREAK_LONGEST_LENGTH);
        }
        private static void updateStreak(uint gameDay, int pointsToday, bool finalSolutionForToday,
            uint threshold, string lastDayKey, string lenKey, string longestLenKey)
        {
            if (pointsToday >= (int)threshold)
            {
                int lastDay = PlayerPrefs.GetInt(lastDayKey, 0);
                if (lastDay != gameDay)
                {
                    // Need to update stats for today
                    PlayerPrefs.SetInt(lastDayKey, (int)gameDay);
                    int streakLen;
                    if (lastDay == gameDay - 1)
                    {
                        // Extension of streak
                        streakLen = PlayerPrefs.GetInt(lenKey, 0);
                        streakLen++;
                    }
                    else
                    {
                        // Start of new streak
                        streakLen = 1;
                    }
                    PlayerPrefs.SetInt(lenKey, streakLen);
                    int longestStreakLen = PlayerPrefs.GetInt(longestLenKey, 0);
                    if (streakLen > longestStreakLen)
                    {
                        PlayerPrefs.SetInt(longestLenKey, streakLen);
                    }
                }
            }
            else
            {
                // Less than threshold
                if (finalSolutionForToday)
                {
                    // If this is the third solution for today, and the points scored is
                    // less than the threshold for this type of streak, then any existing 
                    // streak is ended, and the streak length is now 0.
                    PlayerPrefs.SetInt(lenKey, 0);
                }
            }
        }

        public static (int, int, int, int) GetStreaksLengths()
        {
            int silverLen = PlayerPrefs.GetInt(STATS_SILVER_STREAK_LENGTH, 0);
            int goldLen = PlayerPrefs.GetInt(STATS_GOLD_STREAK_LENGTH, 0);
            int diamondLen = PlayerPrefs.GetInt(STATS_DIAMOND_STREAK_LENGTH, 0);
            int bdiamondLen = PlayerPrefs.GetInt(STATS_BDIAMOND_STREAK_LENGTH, 0);
            return (silverLen, goldLen, diamondLen, bdiamondLen);
        }

        public static (int, int, int, int) GetLongestStreaksLengths()
        {
            int silverLen = PlayerPrefs.GetInt(STATS_SILVER_STREAK_LONGEST_LENGTH, 0);
            int goldLen = PlayerPrefs.GetInt(STATS_GOLD_STREAK_LONGEST_LENGTH, 0);
            int diamondLen = PlayerPrefs.GetInt(STATS_DIAMOND_STREAK_LONGEST_LENGTH, 0);
            int bdiamondLen = PlayerPrefs.GetInt(STATS_BDIAMOND_STREAK_LONGEST_LENGTH, 0);
            return (silverLen, goldLen, diamondLen, bdiamondLen);
        }

        public static void SetPublished()
        {
            int gameDay = GetLastGameDay();
            if (gameDay != GetMostRecentDayPublished())
            {
                PlayerPrefs.SetInt(STATS_MOST_RECENT_PUBLISHED_DAY, gameDay);
                int timesPlayed = GetNumTimesPublished();
                PlayerPrefs.SetInt(STATS_NUM_TIMES_PUBLISHED, timesPlayed + 1);
            }
        }

        public static int GetMostRecentDayPublished() {
            return PlayerPrefs.GetInt(STATS_MOST_RECENT_PUBLISHED_DAY, 0);
        }

        public static bool HasPublishedToday()
        {
            return GetLastGameDay() == GetMostRecentDayPublished();
        }

        public static int GetNumTimesPublished()
        {
            return PlayerPrefs.GetInt(STATS_NUM_TIMES_PUBLISHED, 0);
        }
    }    
}