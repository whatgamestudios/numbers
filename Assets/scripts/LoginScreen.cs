// Copyright (c) Whatgame Studios 2024 - 2025
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using Immutable.Passport;
using System.Threading.Tasks;


namespace FourteenNumbers {

    public class LoginScreen : MonoBehaviour {
        private Coroutine loginCheckRoutine;
        private bool isRunning = false;

        private string help = "Login to qualify for rewards and enable best score publishing";

        public async Task Start() {
            await PassportLogin.Init();
            startCoroutine();
        }

        public void OnDisable() {
            stopCoroutine();
        }

        public async void OnButtonClick(string buttonText) {
            if (buttonText == "Help") {
                MessagePass.SetMsg(help);
                SceneManager.LoadScene("HelpContextScene", LoadSceneMode.Additive);
            }
            else if (buttonText == "Login") {
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
                PassportStore.SetLoggedInChecked();
                DeepLinkManager.Instance.LoginPath = DeepLinkManager.LOGIN_THREAD;
                SceneManager.LoadScene("MenuScene", LoadSceneMode.Single);
                stopCoroutine();
            }
        }
    }
}