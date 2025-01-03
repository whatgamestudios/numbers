// Copyright (c) Whatgame Studios 2024 - 2025
using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;


public class ShowStats : MonoBehaviour {
    public TextMeshProUGUI statsText;

    public void Start() {
        int firstPlayed;
        int lastPlayed;
        int timesPlayed;
        int totalPoints;
        int perfectScoreDays;

        (firstPlayed, lastPlayed, timesPlayed, totalPoints, perfectScoreDays) = Stats.GetStats();

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

        int ave = totalPoints / timesPlayed;

        string stats = 
            "Points Average: " + ave + "\n" +
            "Perfect Score Days: " + perfectScoreDays + "\n" +
            "First Day Played: \n" +
            " " + firstPlayedS + "\n" +
            " Game Day: " + firstPlayed + "\n" +
            "More Recent Day Played: \n" +
            " " + lastPlayedS + "\n" +
            " Game Day: " + lastPlayed + "\n" +
            "Number of times played: " + timesPlayed;

        statsText.text = stats;
    }
}
