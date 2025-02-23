// Copyright (c) Whatgame Studios 2024 - 2025
using UnityEngine;
using System;
using System.Collections;
using System.Numerics;


namespace FourteenNumbers {

    // Connect this script to an empty game object that is on the first scene.
    public class BestSolutionToday : MonoBehaviour {

        // The best score so far today.
        public uint BestScore { get; private set; }

        public bool LoadedBestScore { get; private set; }

        // When the best score was fetched
        public DateTime BestScoreFetched { get; private set; }

        private FourteenNumbersContract fourteenNumbersContracts = new FourteenNumbersContract();
        private Coroutine minuteRoutine;
        private bool isRunning = false;


        public void OnStart() {
            LoadedBestScore = false;
        }

        public void OnEnable() {
            StartTimer();
        }

        public void OnDisable() {
            StopTimer();
        }

        public void StartTimer() {
            if (!isRunning) {
                Debug.Log("Best Solution Today monitor started");
                minuteRoutine = StartCoroutine(MinuteRoutine());
                isRunning = true;

                // Cache the best points so that they are more quickly available.
                uint statsGameDay = (uint) Stats.GetLastGameDay();
                uint gameDay = (uint) Timeline.GameDay();
                if (statsGameDay == gameDay) {
                    BestScore = (uint) Stats.GetBestPointsToday();
                    if (BestScore == 210) {
                        LoadedBestScore = true;
                    }
                }
            }
        }

        public void StopTimer() {
            if (isRunning && minuteRoutine != null) {
                Debug.Log("Best Solution Today monitor stopped");
                StopCoroutine(minuteRoutine);
                isRunning = false;
            }
        }

        IEnumerator MinuteRoutine() {
            while (true) {
                FetchBestScore();
                yield return new WaitForSeconds(60f);
            }
        }

        private async void FetchBestScore() {
            uint gameDay = Timeline.GameDay();
            BestScore = await fourteenNumbersContracts.GetBestScore(gameDay);
            LoadedBestScore = true;
            Stats.SetBestPointsToday((int) BestScore);
            BestScoreFetched = DateTime.Now;
            Debug.Log("Best Solution Today: " + BestScore);

            // The score isn't going to get any better. Save resources and don't keep checking.
            if (BestScore == 210) {
                StopTimer();
            }
        }
    }
}