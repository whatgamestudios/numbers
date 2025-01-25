using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

namespace FourteenNumbers {

    public class DeepLinkManager1 : MonoBehaviour {

        public static DeepLinkManager1 Instance { get; private set; }

    //    private void Awake() {
        public void Start() {
            if (Instance == null) {
                Instance = this;
                // Register the deep link handler.
                Application.deepLinkActivated += onDeepLinkActivated1;
                Debug.Log("Deep link handler registered");

                if (!string.IsNullOrEmpty(Application.absoluteURL)) {
                    // Cold start and Application.absoluteURL not null so process Deep Link.
                    onDeepLinkActivated1(Application.absoluteURL);
                }
                DontDestroyOnLoad(gameObject);
            }
            else {
                Destroy(gameObject);
            }
        }


        /**
        * Called when a deep link is activated.
        * Processes the URL, loads the specified scene if valid, and extracts additional parameters.
        *
        * @param url The URL received from the deep link.
        */
        private void onDeepLinkActivated1(string url) {
            Debug.Log("Deep link: " + url);

            if (url == WelcomeScreen.RedirectUri) {
                PassportStore.SetLoggedIn(true);
                Debug.Log("Deep link is login");
            }
            else if (url == WelcomeScreen.LogoutUri) {
                PassportStore.SetLoggedIn(false);
                Debug.Log("Deep link is logout");
            }
            else {
                Debug.LogWarning("Unknown deeplink: " + url);
            }

            // No matter what happens, go to the menu scene.
            SceneManager.LoadScene("MenuScene", LoadSceneMode.Single);
        }
    }
}