// Copyright (c) Whatgame Studios 2024 - 2025
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;


namespace FourteenNumbers {

    public class BackButtonHandlerOther : MonoBehaviour {
        public Button buttonBack;

        public void OnButtonClick(string buttonText) {
            if (buttonText == "Menu") {
                SceneManager.LoadScene("OtherMenuScene", LoadSceneMode.Single);
            }
            else {
                AuditLog.Log($"OtherBackButtonHandler: Unknown button: {buttonText}");
            }
        }
    }
}