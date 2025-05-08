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

    public class ClaimScreen : MonoBehaviour {
        public TextMeshProUGUI info;
                    
        private const int TIME_PER_DOT = 1000;
        DateTime timeOfLastDot = DateTime.Now;

        FourteenNumbersClaimContract claimContract;

        string status;
        private bool isProcessing = false;
        private bool prepareComplete = false;
        private bool claimComplete = false;
        private bool hasError = false;
        private string errorMessage = "";
        private BigInteger tokenId = BigInteger.Zero;

        public void Start() {
            AuditLog.Log("Claim screen");
            status = "Preparing to claim";
            claimContract = new FourteenNumbersClaimContract();
            StartClaimProcess();
        }

        private async void StartClaimProcess() {
            if (isProcessing) return;
            isProcessing = true;
            hasError = false;
            errorMessage = "";
            prepareComplete = false;
            claimComplete = false;
            tokenId = BigInteger.Zero;

            try {
                int salt = 1; // TODO fix this
                AuditLog.Log("Prepare for claim transaction");
                bool prepareSuccess = await claimContract.PrepareForClaim(salt);
                if (!prepareSuccess) {
                    throw new Exception("Prepare for claim transaction failed");
                }
                prepareComplete = true;

                // Wait for 5 seconds before claiming
                await Task.Delay(5000);

                AuditLog.Log("Claim transaction");
                var (claimSuccess, claimedTokenId) = await claimContract.Claim(salt);
                if (!claimSuccess) {
                    throw new Exception("Claim transaction failed");
                }
                tokenId = claimedTokenId;
                claimComplete = true;
            }
            catch (Exception ex) {
                hasError = true;
                errorMessage = $"Error: {ex.Message}";
                AuditLog.Log($"Exception in claim process: {ex.Message}");
            }
            finally {
                isProcessing = false;
            }
        }

        public async void OnButtonClick(string buttonText) {
            if (buttonText == "Back") {
                await SceneManager.UnloadSceneAsync("CLaimScene");
            }
            else {
                Debug.Log("Claim screen: Unknown button: " + buttonText);
            }
        }

        public void Update() {
            if (hasError) {
                info.text = status + " " + errorMessage;
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
            else if (prepareComplete && !claimComplete) {
                status = "Preparing to claim complete. Starting claim transaction...";
                info.text = status;
            }
            else if (claimComplete) {
                status = $"Claim process complete! Token ID: {tokenId}";
                info.text = status;
            }
        }
    }
}