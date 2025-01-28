// Copyright (c) Whatgame Studios 2024 - 2025
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using Immutable.Passport;

namespace FourteenNumbers {
    public class WelcomeScreen : MonoBehaviour {
        public static string RedirectUri = "fourteennumbers://callback";
        public static string LogoutUri = "fourteennumbers://logout";
        // public static string RedirectUri = "https://whatgamestudios.com/14numbers/app/callback";
        // public static string LogoutUri = "https://whatgamestudios.com/14numbers/app/logout";

        async void Start() {
            // Passport Client ID
            string clientId = "N5pi7DdS7xCeGFoQKFinU6sEY8f8NPuh";

            string redirectUri = null;
            string logoutUri = null;
            #if (UNITY_ANDROID && !UNITY_EDITOR_WIN) || (UNITY_IPHONE && !UNITY_EDITOR_WIN) || UNITY_STANDALONE_OSX
                    redirectUri = RedirectUri;
                    logoutUri = LogoutUri;
            #endif

            
            // Set the environment to SANDBOX for testing or PRODUCTION for production
            string environment = Immutable.Passport.Model.Environment.PRODUCTION;

            // Initialise Passport
            Passport passport = await Passport.Init(clientId, environment, redirectUri, logoutUri);

            // If the player is already logged in, then skip the login screen
            if (PassportStore.IsLoggedIn() || await Passport.Instance.HasCredentialsSaved()) {
                // Try to log in using saved credentials
                bool success = await passport.Login(useCachedSession: true);
                PassportStore.SetLoggedIn(success);
                if (success) {
                    DeepLinkManager.Instance.LoginPath = DeepLinkManager.WELCOME;
                    SceneManager.LoadScene("MenuScene", LoadSceneMode.Single);
                }
                else {
                    SceneManager.LoadScene("LoginScene", LoadSceneMode.Single);
                }
            }
            else {
                PassportStore.SetLoggedIn(false);
                SceneManager.LoadScene("LoginScene", LoadSceneMode.Single);
            }
        }
    }
}
