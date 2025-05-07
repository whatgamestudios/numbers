// Copyright (c) Whatgame Studios 2024 - 2025
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Threading.Tasks;
using System;



namespace FourteenNumbers {

    public class PublishingScreen : MonoBehaviour {
        public TextMeshProUGUI info;
                    
        private const int TIME_PER_DOT = 1000;
        DateTime timeOfLastDot = DateTime.Now;

        string status;

        public void Start() {
            AuditLog.Log("Publishing screen");
            status = "Publishing";
        }

        public async void OnButtonClick(string buttonText) {
            if (buttonText == "Back") {
                await SceneManager.UnloadSceneAsync("PublishScene");
            }
            else {
                Debug.Log("Unknown button: " + buttonText);
            }
        }

        public void Update() {
            if (FourteenNumbersContract.LastTransactionStatus == 0) {
                DateTime now = DateTime.Now;
                if ((now - timeOfLastDot).TotalMilliseconds > TIME_PER_DOT) {
                    timeOfLastDot = now;
                    status = status + ".";
                }
                info.text = status;
            }
            else if (FourteenNumbersSolutionsContract.LastTransactionStatus == FourteenNumbersSolutionsContract.TransactionStatus.Success) {
                info.text = "Published your high score!";
            }
            else {
                info.text = "Failed to publish your score";
            }
        }
    }
}