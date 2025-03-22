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

        private FourteenNumbersContract fourteenNumbersContracts = new FourteenNumbersContract();
        private Coroutine loadRoutine;


        public void OnStart() {
            LoadedBestScore = false;
            BestScore = 0;
        }

        public void OnEnable() {
            StartLoader();
        }

        public void OnDisable() {
        }

        public void StartLoader() {
            // Cache the best points so that they are more quickly available.
            uint statsGameDay = (uint) Stats.GetLastGameDay();
            uint gameDay = (uint) Timeline.GameDay();
            if (statsGameDay == gameDay) {
                BestScore = (uint) Stats.GetBestPointsToday();
                if (BestScore == 210) {
                    LoadedBestScore = true;
                    Debug.Log("Best Solution Today loaded from cache");
                }
            }

            if (!LoadedBestScore) {
                Debug.Log("Loading Best Solution Today");
                loadRoutine = StartCoroutine(LoadRoutine());
            }
        }

        IEnumerator LoadRoutine() {
            FetchBestScore();
            BestScoreLoaded();
            yield return new WaitForSeconds(0f);
        }

        private async void FetchBestScore() {
            uint gameDay = Timeline.GameDay();
            BestScore = await fourteenNumbersContracts.GetBestScore(gameDay);
            LoadedBestScore = true;
            Stats.SetBestPointsToday((int) BestScore);
            Debug.Log("Best Solution Today: " + BestScore);
        }

        public virtual void BestScoreLoaded() {

        }
    }
}