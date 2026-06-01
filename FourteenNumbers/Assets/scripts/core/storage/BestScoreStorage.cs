// Copyright (c) Whatgame Studios 2024 - 2025
using UnityEngine;
using System.Collections;

namespace FourteenNumbers
{

    public class BestScoreStorage
    {
        public const string BEST_TODAY = "BEST_TODAY";

        public static void SetBestPointsToday(int points)
        {
            PlayerPrefs.SetInt(BEST_TODAY, points);
        }

        public static int GetBestPointsToday()
        {
            return PlayerPrefs.GetInt(BEST_TODAY, 0);
        }
   }    
}