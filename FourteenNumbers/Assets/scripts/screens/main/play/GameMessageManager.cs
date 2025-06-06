// Copyright (c) Whatgame Studios 2024 - 2025
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;


namespace FourteenNumbers {

    public class GameMessageManager : MonoBehaviour
    {
        public TextMeshProUGUI helpTextMesh;

        string helpScreenMessage;

        private bool gameDone = false;


        public void Update()
        {
            GameState gameState = GameState.Instance();
            bool done = gameState.IsPlayerStateDone();
            uint pointsToday = gameState.PointsEarnedTotal();
            if (done && !gameDone) {
                setEndResult(pointsToday);
                gameDone = true;
            }

            string text = "";

            if (done) {
                text = helpScreenMessage + "\n";
                if (BestScoreLoader.LoadedBestScore) {
                    if (pointsToday > BestScoreLoader.BestScore) {
                        text = text + "New high score!\n";
                    }
                    else {
                        text = text + "Best score so far today is " + BestScoreLoader.BestScore + "\n";
                    }
                }
                text = text + "Next game in " + Timeline.TimeToNextDayStrShort();
            }
            else {
                text = "Find three solutions for the target number\n";
                if (pointsToday < Stats.STATS_SILVER_STREAK_THRESHOLD) {
                    uint diff = Stats.STATS_SILVER_STREAK_THRESHOLD - pointsToday;
                    text = text + diff + " points to extend silver streak";
                }
                else if (pointsToday < Stats.STATS_GOLD_STREAK_THRESHOLD) {
                    uint diff = Stats.STATS_GOLD_STREAK_THRESHOLD - pointsToday;
                    text = text + diff + " points to extend gold streak";
                }
                else if (pointsToday < Stats.STATS_DIAMOND_STREAK_THRESHOLD) {
                    uint diff = Stats.STATS_DIAMOND_STREAK_THRESHOLD - pointsToday;
                    text = text + diff + " points to extend diamond streak";
                }
                else if (pointsToday < Stats.STATS_BDIAMOND_STREAK_THRESHOLD) {
                    uint diff = Stats.STATS_BDIAMOND_STREAK_THRESHOLD - pointsToday;
                    text = text + diff + " points to extend blue diamond streak";
                }
            }

            helpTextMesh.text = text;
        }

        private void setEndResult(uint pointsEarnedTotal) {
            if (pointsEarnedTotal < 70) {
                helpScreenMessage = "Practice Makes Perfect.";
            }
            else if (pointsEarnedTotal < 120) {
                helpScreenMessage = "Good work!";
            }
            else if (pointsEarnedTotal < 140) {
                helpScreenMessage = "Well done!";
            }
            else if (pointsEarnedTotal < 150) {
                helpScreenMessage = "Very well done!";
            }
            else if (pointsEarnedTotal < 160) {
                helpScreenMessage = "Awesome day!";
            }
            else if (pointsEarnedTotal < 170) {
                helpScreenMessage = "So close...";
            }
            else if (pointsEarnedTotal < 210) {
                helpScreenMessage = "You are exceptional!";
            }
            else if (pointsEarnedTotal == 210) {
                helpScreenMessage = "Perfect Score Day!!!";
            }
            else {
                helpScreenMessage = "Well done";
            }
        }

    }
}