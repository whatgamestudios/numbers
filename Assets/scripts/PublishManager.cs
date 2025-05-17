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
                SceneManager.LoadScene("PublishScene", LoadSceneMode.Additive);
            }
            else {
                AuditLog.Log("PublishManager: Unknown button: " + buttonText);
            }
        }

    }
}