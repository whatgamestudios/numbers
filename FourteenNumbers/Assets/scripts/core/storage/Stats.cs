// Copyright (c) Whatgame Studios 2024 - 2025
using UnityEngine;
using System.Collections;

namespace FourteenNumbers
{

    public class Stats
    {
        // Solutions in format <equation1>=<equation2>=<equation3>=
        // Equals signs are only if the equation has been finalised.
        public const string STATS_SOLUTION = "STATS_SOLUTION";
        public const string STATS_POINTS_TODAY = "STATS_POINTS_TODAY";

        public const string STATS_SOLUTIONS = "STATS_SOLUTIONS_";
        public const string STATS_SCORES = "STATS_SCORES_";

        public const string STATS_FIRST_PLAYED = "STATS_FIRST_PLAYED";
        public const string STATS_LAST_PLAYED = "STATS_LAST_PLAYED";
        public const string STATS_TIMES_PLAYED = "STATS_TIMES_PLAYED";
        public const string STATS_TOTAL_POINTS_ALL_TIME = "STATS_TOTAL_POINTS";
        public const string STATS_PERFECT_SCORE_DAYS = "STATS_PERFECT_SCORE_DAYS";

        public const string STATS_MOST_RECENT_PUBLISHED_DAY = "STATS_MOST_RECENT_PUBLISHED_DAY";
        public const string STATS_NUM_TIMES_PUBLISHED = "STATS_NUM_TIMES_PUBLISHED";
        public const string STATS_PUBLISHED_SCORE = "STATS_PUBLISHED_SCORE";

        public const int NEVER_PLAYED = -1;


        /**
        * Set the background used by all scenes.
        */
        public static void StartNewGameDay()
        {
            PlayerPrefs.SetString(STATS_SOLUTION, "");
            PlayerPrefs.SetInt(STATS_POINTS_TODAY, 0);
            PlayerPrefs.Save();
        }


        public static void SetSolution(uint gameDay, string solution, uint points)
        {
            PlayerPrefs.SetString(STATS_SOLUTION, solution);
            PlayerPrefs.SetInt(STATS_POINTS_TODAY, (int) points);

            uint currentBestScore = GetBestScoreForDay(gameDay);
            // Don't have <= because want to handle a zero point second or third solution.
            if (points >= currentBestScore && currentBestScore != 210) 
            {
                setBestSolutionToday(gameDay, points, solution);

                uint totalPoints = (uint) PlayerPrefs.GetInt(STATS_TOTAL_POINTS_ALL_TIME, 0);
                totalPoints += points;
                totalPoints -= currentBestScore;
                PlayerPrefs.SetInt(STATS_TOTAL_POINTS_ALL_TIME, (int) totalPoints);

                if (points == 210)
                {
                    int perfectScoreDays = PlayerPrefs.GetInt(STATS_PERFECT_SCORE_DAYS, 0);
                    perfectScoreDays++;
                    PlayerPrefs.SetInt(STATS_PERFECT_SCORE_DAYS, perfectScoreDays);
                }
            }


            if (GetLastGameDay() != gameDay) 
            {
                int firstPlayed = PlayerPrefs.GetInt(STATS_FIRST_PLAYED, NEVER_PLAYED);
                if (firstPlayed == NEVER_PLAYED)
                {
                    PlayerPrefs.SetInt(STATS_FIRST_PLAYED, (int)gameDay);
                }
                PlayerPrefs.SetInt(STATS_LAST_PLAYED, (int)gameDay);

                int timesPlayed = PlayerPrefs.GetInt(STATS_TIMES_PLAYED, 0);
                timesPlayed++;
                PlayerPrefs.SetInt(STATS_TIMES_PLAYED, timesPlayed);
            }

            PlayerPrefs.Save();
        }

        public static string GetSolutions()
        {
            return PlayerPrefs.GetString(STATS_SOLUTION, "");
        }

        public static (string, bool, string, bool, string, bool) GetAllSolutions()
        {
            return SolutionResolver.Resolve(PlayerPrefs.GetString(STATS_SOLUTION, ""));
        }

        public static (string, string, string) GetAllCompleteSolutions()
        {
            return SolutionResolver.ResolveComplete(PlayerPrefs.GetString(STATS_SOLUTION, ""));
        }

        public static (int, int, int, int, int, int) GetStats()
        {
            return (PlayerPrefs.GetInt(STATS_FIRST_PLAYED, 0),
            PlayerPrefs.GetInt(STATS_LAST_PLAYED, 0),
            PlayerPrefs.GetInt(STATS_TIMES_PLAYED, 0),
            GetNumTimesPublished(),
            PlayerPrefs.GetInt(STATS_TOTAL_POINTS_ALL_TIME, 0),
            PlayerPrefs.GetInt(STATS_PERFECT_SCORE_DAYS, 0));
        }

        public static uint GetLastGameDay()
        {
            return (uint) PlayerPrefs.GetInt(STATS_LAST_PLAYED, 0);
        }


        private static void setBestSolutionToday(uint gameDay, uint score, string solution)
        {
            string scoreKey = STATS_SCORES + gameDay.ToString();
            PlayerPrefs.SetInt(scoreKey, (int) score);
            string key = STATS_SOLUTIONS + gameDay.ToString();
            PlayerPrefs.SetString(key, solution);
        }


        public static uint GetBestScoreForDay(uint gameDay) {
            string scoreKey = STATS_SCORES + gameDay.ToString();
            return (uint) PlayerPrefs.GetInt(scoreKey, 0);
        }

        public static string GetCombinedSolution(uint gameDay)
        {
            string key = STATS_SOLUTIONS + gameDay.ToString();
            string solutions = PlayerPrefs.GetString(key, "");
            if (solutions == "") 
            {
                return "===";
            }
            if (UpgradeStorage.GetStorageV3UpgradeDay() >= gameDay) 
            {
                return solutions;
            }
            else 
            {
                return solutions + "=";
            }
        }

         public static void SetPublished(uint score)
        {
            uint gameDay = GetLastGameDay();
            if (gameDay != GetMostRecentDayPublished())
            {
                PlayerPrefs.SetInt(STATS_MOST_RECENT_PUBLISHED_DAY, (int) gameDay);
                int timesPlayed = GetNumTimesPublished();
                PlayerPrefs.SetInt(STATS_NUM_TIMES_PUBLISHED, timesPlayed + 1);
            }
            PlayerPrefs.SetInt(STATS_PUBLISHED_SCORE, (int) score);
        }

        public static uint GetMostRecentDayPublished() {
            return (uint) PlayerPrefs.GetInt(STATS_MOST_RECENT_PUBLISHED_DAY, 0);
        }

        public static (bool, uint) HasPublishedToday()
        {
            return (GetLastGameDay() == GetMostRecentDayPublished(), 
                    (uint) PlayerPrefs.GetInt(STATS_PUBLISHED_SCORE, 0));
        }

        public static int GetNumTimesPublished()
        {
            return PlayerPrefs.GetInt(STATS_NUM_TIMES_PUBLISHED, 0);
        }
    }    
}