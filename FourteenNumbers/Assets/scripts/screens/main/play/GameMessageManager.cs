// Copyright (c) Whatgame Studios 2024 - 2025
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System;
using TMPro;

namespace FourteenNumbers {

    public class GameMessageManager : MonoBehaviour
    {
        public TextMeshProUGUI helpTextMesh;

        public GameObject helpPanel;


        string helpScreenMessage;


        public void Update()
        {
            GameState gameState = GameState.Instance();
            bool done = gameState.IsPlayerStateDone();
            uint pointsToday = gameState.PointsEarnedTotal();

            string text = "";

            if (done) {
                helpPanel.SetActive(true);
                setEndResult(pointsToday);
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
                if (Stats.GetNumTimesPublished() != 0)
                {
                    helpPanel.SetActive(false);                
                }
                text = "Find three solutions for the target number\n";
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