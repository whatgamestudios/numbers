// Copyright (c) Whatgame Studios 2024 - 2025
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Threading.Tasks;
using System;



namespace FourteenNumbers {

    public class ClaimScreen : MonoBehaviour {
        public TextMeshProUGUI info;
                    
        private const int TIME_PER_DOT = 1000;
        DateTime timeOfLastDot = DateTime.Now;

        FourteenNumbersClaimContract claimContract;

        string status;

        private int state = 0;
        private DateTime start;

        public void Start() {
            AuditLog.Log("Claim screen");
            status = "Preparing to claim";
            claimContract = new FourteenNumbersClaimContract();
        }

        public async void OnButtonClick(string buttonText) {
            if (buttonText == "Back") {
                await SceneManager.UnloadSceneAsync("CLaimScene");
            }
            else {
                Debug.Log("Claim screen: Unknown button: " + buttonText);
            }
        }

        public async Task Update() {
            int salt = 1; // TODO fix this
            switch (state) {
                case 0:
                    AuditLog.Log("Prepare for claim transaction");
                    try {
                        claimContract.PrepareForClaim(salt);
                    } catch (System.Exception ex) {
                        string errorMessage = $"Exception in Prepare for Claim: {ex.Message}\nStack Trace: {ex.StackTrace}";
                        AuditLog.Log(errorMessage);
                        status = status + " Error";
                        state = 4;
                    }
                    state = 1;
                    break;
                case 1:
                    // TOOD should check when transaction done

                    start = DateTime.Now;
                    timeOfLastDot = start;
                    state = 2;
                    break;
                case 2:
                    DateTime now = DateTime.Now;
                    if ((now - timeOfLastDot).TotalMilliseconds > TIME_PER_DOT) {
                        timeOfLastDot = now;
                        status = status + ".";
                    }
                    info.text = status;

                    if ((now - start).TotalMilliseconds > 5000) {
                        state = 3;
                    }
                    break;
                case 3:
                    AuditLog.Log("Claim transaction");
                    status = status + "claim transaction";
                    info.text = status;

                    try {
                        claimContract.Claim(salt);
                    } catch (System.Exception ex) {
                        string errorMessage = $"Exception in Claim: {ex.Message}\nStack Trace: {ex.StackTrace}";
                        AuditLog.Log(errorMessage);
                        status = status + " Error";
                    }
                    state = 4;
                    break;
                case 4:
                    // TOOD should check when transaction done
                    break;
            }
        }
    }
}