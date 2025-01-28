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

        private Coroutine loginCheckRoutine;
        private bool isRunning = false;

        public void Start() {
            startCoroutine();
        }

        public void OnDisable() {
            stopCoroutine();
        }

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
                DeepLinkManager.Instance.LoginPath = DeepLinkManager.LOGIN_SKIP;
                SceneManager.LoadScene("MenuScene", LoadSceneMode.Single);
            }
            else {
                Debug.Log("Unknown button");
            }
        }

        private void startCoroutine() {
            if (!isRunning) {
                loginCheckRoutine = StartCoroutine(LoginCheckRoutine());
                isRunning = true;
            }
        }

        public void stopCoroutine() {
            if (isRunning && loginCheckRoutine != null) {
                StopCoroutine(loginCheckRoutine);
                isRunning = false;
            }
        }

        IEnumerator LoginCheckRoutine() {
            while (true) {
                CheckLogin();
                yield return new WaitForSeconds(1f);
            }
        }

        private async void CheckLogin() {
            bool loggedIn = await Passport.Instance.HasCredentialsSaved();
            Debug.Log("CheckLogin: Loggedin: " + loggedIn);
            if (loggedIn) {
                PassportStore.SetLoggedIn(true);
                DeepLinkManager.Instance.LoginPath = DeepLinkManager.LOGIN_THREAD;
                SceneManager.LoadScene("MenuScene", LoadSceneMode.Single);
                stopCoroutine();
            }
        }
    }
}