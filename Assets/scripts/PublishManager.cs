// Copyright (c) Whatgame Studios 2024 - 2025
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System;

namespace FourteenNumbers {

    public class PublishManager : MonoBehaviour {
        public GameObject panelPublish;

        public void OnButtonClick(string buttonText) {
            if (buttonText == "Publish") {
                AuditLog.Log("Publish");
                panelPublish.SetActive(false);
                if (PassportStore.IsLoggedIn()) {
                    SceneManager.LoadScene("PublishScene", LoadSceneMode.Additive);
                }
                else {
                    SceneManager.LoadScene("LoginScene", LoadSceneMode.Single);
                }
            }
            else {
                AuditLog.Log("PublishManager: Unknown button: " + buttonText);
            }
        }

    }
}