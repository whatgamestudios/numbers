// Copyright (c) Whatgame Studios 2024 - 2025
using UnityEngine;
using System;
using System.Collections;
using System.Numerics;


namespace FourteenNumbers {

    // Connect this script to an empty game object that is on the first scene.
    public class BestScoreLoader : MonoBehaviour {

        // The best score so far today.
        public uint BestScore { get; private set; }

        public bool LoadedBestScore { get; private set; }

        private FourteenNumbersSolutionsContract fourteenNumbersContracts = new FourteenNumbersSolutionsContract();
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
            AuditLog.Log("Best Solution Today: " + BestScore);
            BestScoreLoaded();
        }

        public virtual void BestScoreLoaded() {

        }
    }
}