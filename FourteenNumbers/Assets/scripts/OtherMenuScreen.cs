// Copyright (c) Whatgame Studios 2024 - 2025
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using Immutable.Passport;

namespace FourteenNumbers {

    public class OtherMenuScreen : MonoBehaviour {

        public Button buttonPassport;
        public Button buttonSocials;
        public Button buttonSettings;
        public Button buttonCredits;
        public Button buttonRoadmap;

        public void Start() {
            AuditLog.Log("Menu (secondary) screen");
        }

        public void OnButtonClick(string buttonText) {
            if (buttonText == "Passport") {
                // Only specifying the sceneName or sceneBuildIndex will load the Scene with the Single mode
                SceneManager.LoadScene("PassportScene", LoadSceneMode.Single);
            }
            else if (buttonText == "Socials") {
                SceneManager.LoadScene("SocialsScene", LoadSceneMode.Single);
            }
            else if (buttonText == "Settings") {
                SceneManager.LoadScene("SettingsScene", LoadSceneMode.Single);
            }
            else if (buttonText == "Credits") {
                SceneManager.LoadScene("CreditsScene", LoadSceneMode.Single);
            }
            else {
                AuditLog.Log("Other Menu: Unknown button" + buttonText);
            }
        }
    }
}