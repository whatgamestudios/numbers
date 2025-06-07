// Copyright (c) Whatgame Studios 2024 - 2025
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Threading.Tasks;
using System;
using System.Numerics;

namespace FourteenNumbers {

    public class PublishingScreen : MonoBehaviour {
        public TextMeshProUGUI info;
                    
        private const int TIME_PER_DOT = 1000;
        DateTime timeOfLastDot = DateTime.Now;

        FourteenNumbersSolutionsContract contract;

        string status;
        private bool isProcessing = false;
        private bool hasError = false;
        private string errorMessage = "";

        public void Start() {
            AuditLog.Log("Publishing screen");
            status = "Publishing ";
            contract = new FourteenNumbersSolutionsContract();
            timeOfLastDot = DateTime.Now;
            StartPublishProcess();
        }

        private async void StartPublishProcess() {
            if (isProcessing) {
                return;
            }
            isProcessing = true;
            hasError = false;
            errorMessage = "";

            try {
                AuditLog.Log("Publish transaction");
                uint pointsToday = GameState.Instance().PointsEarnedTotal();
                uint gameDay = (uint) Stats.GetLastGameDay();
                (string sol1, string sol2, string sol3) = Stats.GetSolutions();
                bool publishSuccess = false;
                uint retry = 0;
                while (!publishSuccess)
                {
                    publishSuccess = await contract.SubmitBestScore(gameDay, sol1, sol2, sol3);
                    if (!publishSuccess)
                    {
                        uint bestScore = await contract.GetBestScore(gameDay);
                        if (pointsToday <= bestScore)
                        {
                            AuditLog.Log($"Someone published first: points: {pointsToday}, best points: {bestScore}");
                            hasError = true;
                            errorMessage = "Oh no! Someone published before you";
                            break;
                        }
                        retry++;
                        if (retry > 3)
                        {
                            AuditLog.Log("Failed to publish");
                            hasError = true;
                            errorMessage = "Failed to publish. Please try again later";
                            break;                           
                        }
                    }
                }
            }
            catch (Exception ex) {
                hasError = true;
                errorMessage = "Error during publish process. Please try again later";
                AuditLog.Log($"Exception in publish process: {ex.Message}");
            }
            finally {
                isProcessing = false;
            }
        }

        public void Update() {
            if (hasError) {
                info.text = errorMessage;
                return;
            }

            if (isProcessing) {
                DateTime now = DateTime.Now;
                if ((now - timeOfLastDot).TotalMilliseconds > TIME_PER_DOT) {
                    timeOfLastDot = now;
                    status = status + ".";
                }
                info.text = status;
            }
            else {
                info.text = "Published your high score!";
            }
        }
    }
}
