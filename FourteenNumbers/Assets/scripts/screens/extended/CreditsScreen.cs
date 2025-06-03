// Copyright (c) Whatgame Studios 2024 - 2025
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;


namespace FourteenNumbers {

    public class CreditsScreen : MonoBehaviour {

        public void Start() {
            AuditLog.Log("Credits screen");
        }

        public void OnButtonClick(string buttonText) {
            if (buttonText == "Website") {
                Application.OpenURL("https://whatgamestudios.com/14numbers");
            }
            else if (buttonText == "Privacy") {
                Application.OpenURL("https://whatgamestudios.com/14numbers/privacy-policy/");
            }

            else
            {
                AuditLog.Log("Credits screen: Unknown button: " + buttonText);
            }
        }

    }
}