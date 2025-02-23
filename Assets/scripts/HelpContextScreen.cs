// Copyright (c) Whatgame Studios 2024 - 2025
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using System;
using System.Collections;


// Pop up an error screen for three seconds
namespace FourteenNumbers {

    public class HelpContextScreen : MonoBehaviour {
        public TextMeshProUGUI helpText;

        public void Start() {
            string msg = MessagePass.GetMsg();
            helpText.text = msg;
        }

        public async void OnButtonClick(string buttonText) {
            await SceneManager.UnloadSceneAsync("HelpContextScene");
        }
    }
}