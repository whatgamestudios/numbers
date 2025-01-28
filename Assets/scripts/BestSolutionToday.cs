// Copyright (c) Whatgame Studios 2024 - 2025
using UnityEngine;
using System;
using System.Collections;
using System.Numerics;

using Nethereum.Web3;
using Nethereum.Contracts;
using Nethereum.Web3.Accounts;
using Nethereum.Hex.HexTypes;


namespace FourteenNumbers {

    // Connect this script to an empty game object that is on the first scene.
    public class BestSolutionToday : MonoBehaviour {

        // Call BestSolutionToday.Instance from anywhere to get the one and only instance.
        public static BestSolutionToday Instance { get; private set; }

        // The best score so far today.
        public uint BestScore { get; private set; }

        private FourteenNumbersSolutionsService contractService;

        // When the best score was fetched
        public DateTime BestScoreFetched { get; private set; }

        private Coroutine minuteRoutine;
        private bool isRunning = false;

        public void Start() {
            if (Instance == null) {
                Instance = this;
                Debug.Log("Best Solution Today monitor started");

                var web3 = new Web3("https://rpc.immutable.com/");
                var contractAddress = "0xe2E762770156FfE253C49Da6E008b4bECCCf2812";
                contractService = new FourteenNumbersSolutionsService(web3, contractAddress);

                StartTimer();

                DontDestroyOnLoad(gameObject);
            }
            else {
                Destroy(gameObject);
            }
        }

        public void StartTimer() {
            if (!isRunning) {
                minuteRoutine = StartCoroutine(MinuteRoutine());
                isRunning = true;
            }
        }

        public void StopTimer() {
            if (isRunning && minuteRoutine != null) {
                StopCoroutine(minuteRoutine);
                isRunning = false;
            }
        }

        IEnumerator MinuteRoutine() {
            while (true) {
                FetchBestScore();
                yield return new WaitForSeconds(300f);
            }
        }

        private async void FetchBestScore() {
            uint gameDay = Timeline.GameDay();
            SolutionsOutputDTO solution = await contractService.SolutionsQueryAsync(gameDay);
            BigInteger bestScoreBigInt = solution.Points;
            if (bestScoreBigInt < 0 || bestScoreBigInt > uint.MaxValue) {
                Debug.LogError($"Number {bestScoreBigInt} is outside uint range");
                BestScore = 7;
            }
            else {
                BestScore = (uint) solution.Points;
            }
            BestScoreFetched = DateTime.Now;
        }
    }
}