// Copyright (c) Whatgame Studios 2024 - 2025
using UnityEngine;
using System;
using System.Collections;
using System.Numerics;


namespace FourteenNumbers {

    public class BestScoreLoader : MonoBehaviour {

        // The best score so far today.
        public static uint BestScore { get; private set; }

        public static bool LoadedBestScore { get; private set; }

        private FourteenNumbersSolutionsContract fourteenNumbersContracts = new FourteenNumbersSolutionsContract();
        private Coroutine loadRoutine;


        public void OnStart() {
            LoadedBestScore = false;
            BestScore = 0;
        }

        public void OnEnable() {
            StartLoader();
        }

        // Force the best score to be reloaded each time the game screen is entered.
        public void OnDisable()
        {
            LoadedBestScore = false;
        }

        public void StartLoader() {
            // Cache the best points so that they are more quickly available.
            uint statsGameDay = (uint) Stats.GetLastGameDay();
            uint gameDay = (uint) Timeline.GameDay();
            if (statsGameDay == gameDay) {
                BestScore = (uint) Stats.GetBestPointsToday();
                if (BestScore == 210) {
                    LoadedBestScore = true;
                }
            }

            if (!LoadedBestScore) {
                loadRoutine = StartCoroutine(LoadRoutine());
            }
        }

        IEnumerator LoadRoutine() {
            FetchBestScore();
            yield return new WaitForSeconds(0f);
        }

        private async void FetchBestScore() {
            uint gameDay = Timeline.GameDay();
            BestScore = await fourteenNumbersContracts.GetBestScore(gameDay);
            LoadedBestScore = true;
            Stats.SetBestPointsToday((int) BestScore);
        }
    }
}