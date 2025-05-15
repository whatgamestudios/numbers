// Copyright (c) Whatgame Studios 2024 - 2025
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using System;

namespace FourteenNumbers {

    public class PublishManager : MonoBehaviour {
        public GameObject panelPublish;

        public void OnButtonClick(string buttonText) {
            if (buttonText == "Publish") {
                AuditLog.Log("Publish");
                try {
                    panelPublish.SetActive(false);

                    uint gameDay = (uint) Stats.GetLastGameDay();
                    (string sol1, string sol2, string sol3) = Stats.GetSolutions();

                    FourteenNumbersSolutionsContract contract = new FourteenNumbersSolutionsContract();
                    contract.SubmitBestScore(gameDay, sol1, sol2, sol3);

                    SceneManager.LoadScene("PublishScene", LoadSceneMode.Additive);
                }
                catch (Exception ex) {
                    string errorMessage = $"Exception in Publish: {ex.Message}\nStack Trace: {ex.StackTrace}";
                    AuditLog.Log(errorMessage);
                }
            }
            else {
                Debug.Log("Unknown button: " + buttonText);
            }
        }

    }
}