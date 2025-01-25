// Copyright (c) Whatgame Studios 2024 - 2025
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using Immutable.Passport;


namespace FourteenNumbers {

    public class LoginSceneButtonHandler : MonoBehaviour {

        public Button buttonLogin;
        public Button buttonSkip;


        public async void OnButtonClick(string buttonText) {
            if (buttonText == "Login") {
                Debug.Log("LoginPKCE start");
#if (UNITY_ANDROID && !UNITY_EDITOR_WIN) || (UNITY_IPHONE && !UNITY_EDITOR_WIN) || UNITY_STANDALONE_OSX
                await Passport.Instance.LoginPKCE();
#else
                await Passport.Instance.Login();
#endif
                Debug.Log("LoginPKCE done");
            }
            else if (buttonText == "Skip") {
                SceneManager.LoadScene("MenuScene", LoadSceneMode.Single);
            }
            else {
                Debug.Log("Unknown button");
            }
        }
    }
}