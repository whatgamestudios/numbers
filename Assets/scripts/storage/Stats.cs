// Copyright (c) Whatgame Studios 2024 - 2025
using UnityEngine;
using System.Collections;

namespace FourteenNumbers {

    public class Stats {
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

        public const string BEST_TODAY = "BEST_TODAY";


        public const int NEVER_PLAYED = -1;


        /**
        * Set the background used by all scenes.
        */
        public static void StartNewGameDay() {
            PlayerPrefs.SetString(STATS_SOLUTION1, "");
            PlayerPrefs.SetString(STATS_SOLUTION2, "");
            PlayerPrefs.SetString(STATS_SOLUTION3, "");

            PlayerPrefs.SetInt(STATS_POINTS_TODAY, 0);

            PlayerPrefs.Save();
        }


        public static void SetSolution1(uint gameDay, string solution, int points) {
            int firstPlayed = PlayerPrefs.GetInt(STATS_FIRST_PLAYED, NEVER_PLAYED);
            if (firstPlayed == NEVER_PLAYED) {
                PlayerPrefs.SetInt(STATS_FIRST_PLAYED, (int) gameDay);
            }

            PlayerPrefs.SetInt(STATS_LAST_PLAYED, (int) gameDay);

            PlayerPrefs.SetString(STATS_SOLUTION1, solution);
            setCombinedSolution(gameDay);

            int timesPlayed = PlayerPrefs.GetInt(STATS_TIMES_PLAYED, 0);
            timesPlayed++;
            PlayerPrefs.SetInt(STATS_TIMES_PLAYED, timesPlayed);

            int totalPoints = PlayerPrefs.GetInt(STATS_TOTAL_POINTS, 0);
            totalPoints += points;
            PlayerPrefs.SetInt(STATS_TOTAL_POINTS, totalPoints);

            PlayerPrefs.SetInt(STATS_POINTS_TODAY, points);

            PlayerPrefs.Save();
        }

        public static void SetSolution2(uint gameDay, string solution, int points) {
            PlayerPrefs.SetString(STATS_SOLUTION2, solution);
            setCombinedSolution(gameDay);

            int totalPoints = PlayerPrefs.GetInt(STATS_TOTAL_POINTS, 0);
            totalPoints += points;
            PlayerPrefs.SetInt(STATS_TOTAL_POINTS, totalPoints);

            int totalPointsToday = PlayerPrefs.GetInt(STATS_POINTS_TODAY, 0);
            totalPointsToday += points;
            PlayerPrefs.SetInt(STATS_POINTS_TODAY, totalPointsToday);

            PlayerPrefs.Save();
        }

        public static void SetSolution3(uint gameDay, string solution, int points) {
            PlayerPrefs.SetString(STATS_SOLUTION3, solution);
            setCombinedSolution(gameDay);

            int totalPoints = PlayerPrefs.GetInt(STATS_TOTAL_POINTS, 0);
            totalPoints += points;
            PlayerPrefs.SetInt(STATS_TOTAL_POINTS, totalPoints);

            int totalPointsToday = PlayerPrefs.GetInt(STATS_POINTS_TODAY, 0);
            totalPointsToday += points;
            PlayerPrefs.SetInt(STATS_POINTS_TODAY, totalPointsToday);

            if (totalPointsToday == 210) {
                int perfectScoreDays = PlayerPrefs.GetInt(STATS_PERFECT_SCORE_DAYS, 0);
                perfectScoreDays++;
                PlayerPrefs.SetInt(STATS_PERFECT_SCORE_DAYS, perfectScoreDays);
            }

            PlayerPrefs.Save();
        }


        public static (string, string, string) GetSolutions() {
            return (PlayerPrefs.GetString(STATS_SOLUTION1, ""),
                    PlayerPrefs.GetString(STATS_SOLUTION2, ""),
                    PlayerPrefs.GetString(STATS_SOLUTION3, ""));
        }


        public static (int, int, int, int, int) GetStats() {
            return (PlayerPrefs.GetInt(STATS_FIRST_PLAYED, 0),
            PlayerPrefs.GetInt(STATS_LAST_PLAYED, 0),
            PlayerPrefs.GetInt(STATS_TIMES_PLAYED, 0),
            PlayerPrefs.GetInt(STATS_TOTAL_POINTS, 0),
            PlayerPrefs.GetInt(STATS_PERFECT_SCORE_DAYS, 0));
        }

        public static int GetLastGameDay() {
            return PlayerPrefs.GetInt(STATS_LAST_PLAYED, 0);
        }

        public static int GetNumDaysPlayed() {
            return PlayerPrefs.GetInt(STATS_TIMES_PLAYED, 0);
        }

        // public static int GetTotalPointsToday() {
        //     return PlayerPrefs.GetInt(STATS_POINTS_TODAY, 0);
        // }



        public static void SetBestPointsToday(int points) {
            PlayerPrefs.SetInt(BEST_TODAY, points);
        }

        public static int GetBestPointsToday() {
            return PlayerPrefs.GetInt(BEST_TODAY, 0);
        }

        private static void setCombinedSolution(uint gameDay) {
            string key = STATS_SOLUTIONS + gameDay.ToString();
            (string sol1, string sol2, string sol3) =  GetSolutions();
            string combinedSolution = sol1 + "=" + sol2 + "=" + sol3;
            PlayerPrefs.SetString(key, combinedSolution);
        }

        public static string GetCombinedSolution(uint gameDay) {
            string key = STATS_SOLUTIONS + gameDay.ToString();
            return PlayerPrefs.GetString(key, "==");
        }
    }
}