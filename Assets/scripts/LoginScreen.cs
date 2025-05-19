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

        public Button loginButton;
        private Coroutine loginCheckRoutine;
        private Coroutine buttonAnimationRoutine;
        private bool isRunning = false;
        private Image buttonImage;
        private int currentButtonFrame = 1;

        private string help = "Sign In to qualify for rewards and enable best score publishing";

        public async Task Start() {
            AuditLog.Log("Login screen");
            await PassportLogin.Init();
            startCoroutine();
            startButtonAnimation();
        }

        public void OnDisable() {
            stopCoroutine();
            stopButtonAnimation();
        }

        private void startButtonAnimation() {
            if (buttonAnimationRoutine == null) {
                // Ensure we have an Image component
                buttonImage = loginButton.GetComponent<Image>();
                if (buttonImage == null) {
                    buttonImage = loginButton.gameObject.AddComponent<Image>();
                }
                
                // Set initial sprite
                string initialResource = "animated_button1";
                Texture2D initialTex = Resources.Load<Texture2D>(initialResource);
                if (initialTex != null) {
                    Rect size = new Rect(0.0f, 0.0f, initialTex.width, initialTex.height);
                    Vector2 pivot = new Vector2(0.0f, 0.0f);
                    Sprite s = Sprite.Create(initialTex, size, pivot);
                    buttonImage.sprite = s;
                } else {
                    AuditLog.Log($"ERROR: Failed to load initial button texture: {initialResource}");
                }

                buttonAnimationRoutine = StartCoroutine(ButtonAnimationRoutine());
            }
        }

        private void stopButtonAnimation() {
            if (buttonAnimationRoutine != null) {
                StopCoroutine(buttonAnimationRoutine);
                buttonAnimationRoutine = null;
            }
        }

        IEnumerator ButtonAnimationRoutine() {
            while (true) {
                // Load and set the next button frame
                string resource = $"buttons/animated_button{currentButtonFrame}";
                Texture2D tex = Resources.Load<Texture2D>(resource);
                if (tex != null) {
                    Rect size = new Rect(0.0f, 0.0f, tex.width, tex.height);
                    Vector2 pivot = new Vector2(0.0f, 0.0f);
                    Sprite s = Sprite.Create(tex, size, pivot);
                    buttonImage.sprite = s;
                } else {
                    AuditLog.Log($"ERROR: Failed to load button texture: {resource}");
                }

                // Increment frame counter and reset if needed
                currentButtonFrame++;
                if (currentButtonFrame > 6) {
                    currentButtonFrame = 1;
                }

                // Wait for 125ms before next frame
                yield return new WaitForSeconds(0.125f);
            }
        }

        public async void OnButtonClick(string buttonText) {
            if (buttonText == "Help") {
                MessagePass.SetMsg(help);
                SceneManager.LoadScene("HelpContextScene", LoadSceneMode.Additive);
            }
            else if (buttonText == "Login") {
                //Debug.Log("LoginPKCE start");
#if (UNITY_ANDROID && !UNITY_EDITOR_WIN) || (UNITY_IPHONE && !UNITY_EDITOR_WIN) || UNITY_STANDALONE_OSX
                await Passport.Instance.LoginPKCE();
#else
                await Passport.Instance.Login();
#endif
                //Debug.Log("LoginPKCE done");
            }
            else if (buttonText == "Skip") {
                DeepLinkManager.Instance.LoginPath = DeepLinkManager.LOGIN_SKIP;
                SceneManager.LoadScene("MenuScene", LoadSceneMode.Single);
            }
            else {
                AuditLog.Log("Login Screen: Unknown button");
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
            AuditLog.Log("CheckLogin: Loggedin: " + loggedIn);
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