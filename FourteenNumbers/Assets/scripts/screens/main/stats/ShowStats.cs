// Copyright (c) Whatgame Studios 2024 - 2025
using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;
using UnityEngine.SceneManagement;

namespace FourteenNumbers {

    public class ShowStats : MonoBehaviour {
        public TextMeshProUGUI pointsAveText;
        public TextMeshProUGUI perfectScoreDaysText;
        public TextMeshProUGUI daysPlayedText;
        public TextMeshProUGUI daysPublishedText;
        public TextMeshProUGUI firstDayPlayedText;
        public TextMeshProUGUI firstDatePlayedText;


        private string help = "" +
            "Streaks are sequential days played.";

        public void Start() {
            AuditLog.Log("Stats screen");

            int firstPlayed;
            int lastPlayed;
            int timesPlayed;
            int timesPublished;
            int totalPoints;
            int perfectScoreDays;

            (firstPlayed, lastPlayed, timesPlayed, timesPublished, totalPoints, perfectScoreDays) = Stats.GetStats();
            AuditLog.Log($"Stats: firstPlayed: {firstPlayed}, lastPlayed: {lastPlayed}, timesPlayed: {timesPlayed}, timesPublished: {timesPublished}, totalPoints: {totalPoints}, perfectScoreDays: {perfectScoreDays}");

            string firstPlayedS;
            string lastPlayedS;

            if (firstPlayed == 0) {
                firstPlayedS = "Never Played";
                lastPlayedS = "Never Played";
            }
            else {
                DateTime firstPlayedDate = Timeline.GetRelativeDate(firstPlayed);
                firstPlayedS = firstPlayedDate.ToString("D");

                DateTime lastPlayedDate = Timeline.GetRelativeDate(lastPlayed);
                lastPlayedS = lastPlayedDate.ToString("D");
            }

            int ave = 0;
            if (timesPlayed != 0) {
                ave = totalPoints / timesPlayed;
            }

            pointsAveText.text = ave.ToString();
            perfectScoreDaysText.text = perfectScoreDays.ToString();
            daysPlayedText.text = timesPlayed.ToString();
            daysPublishedText.text = timesPublished.ToString();
            firstDayPlayedText.text = firstPlayed.ToString();
            firstDatePlayedText.text = firstPlayedS;
        }

        public void OnButtonClick(string buttonText) {
            if (buttonText == "Help") {
                MessagePass.SetMsg(help);
                SceneStack.Instance().PushScene();
                SceneManager.LoadScene("HelpContextScene", LoadSceneMode.Additive);
            }
            else {
                AuditLog.Log($"Show Stats: Unknown button: {buttonText}");
            }
        }

    }
}