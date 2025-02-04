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

        // When the best score was fetched
        public DateTime BestScoreFetched { get; private set; }

        private FourteenNumbersContract fourteenNumbersContracts = new FourteenNumbersContract();
        private Coroutine minuteRoutine;
        private bool isRunning = false;

        public void StartTimer() {
            if (!isRunning) {
                Debug.Log("Best Solution Today monitor started");
                minuteRoutine = StartCoroutine(MinuteRoutine());
                isRunning = true;
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
                yield return new WaitForSeconds(5f);
            }
        }

        private async void FetchBestScore() {
            uint gameDay = Timeline.GameDay();
            BestScore = await fourteenNumbersContracts.GetBestScore(gameDay);
            Stats.SetBestPointsToday((int) BestScore);
            BestScoreFetched = DateTime.Now;
            Debug.Log("Best Solution Today: " + BestScore);
        }
    }
}