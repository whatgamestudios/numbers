// Copyright (c) Whatgame Studios 2024 - 2025
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using Immutable.Passport;


public class LoginSceneButtonHandler : MonoBehaviour {

    public Button buttonLogin;
    public Button buttonSkip;

    private Passport passport;


    public static string RedirectUri = "https://whatgamestudios.com/14numbers/app/callback";
    public static string LogoutRedirectUri = "https://whatgamestudios.com/14numbers/app/logout";

    async void Start() {
        // Passport Client ID
        string clientId = "N5pi7DdS7xCeGFoQKFinU6sEY8f8NPuh";
        
        // Set the environment to SANDBOX for testing or PRODUCTION for production
        string environment = Immutable.Passport.Model.Environment.PRODUCTION;

        // Initialise Passport
        passport = await Passport.Init(clientId, environment, RedirectUri, LogoutRedirectUri);

        // If the player is already logged in, then skip the login screen
        if (PassportStore.IsLoggedIn()) {
            // Try to log in using saved credentials
            bool success = await passport.Login(useCachedSession: true);
            if (!success) {
                SceneManager.LoadScene("MenuScene", LoadSceneMode.Single);
            }
        }
    }


    public void OnButtonClick(string buttonText) {
        if (buttonText == "Login") {
            login();
        }
        else if (buttonText == "Skip") {
            SceneManager.LoadScene("MenuScene", LoadSceneMode.Single);
        }
        else {
            Debug.Log("Unknown button");
        }
    }

    async private void login() {
        // // Check if the player is supposed to be logged in and if there are credentials saved
        // if (PassportStore.IsLoggedIn()) {
        //     // Try to log in using saved credentials
        //     bool success = await Passport.Instance.Login(useCachedSession: true);
        //     Debug.Log("Success: " + success);
        //     if (!success) {
        //         // No saved credentials to re-login the player, login
        //         await passport.LoginPKCE();
        //         Debug.Log("LoginPKCE done1");
        //     }
        //     else {
        //         SceneManager.LoadScene("MenuScene", LoadSceneMode.Single);
        //     }
        // } else {
            // No saved credentials to re-login the player, login
            await passport.LoginPKCE();
            Debug.Log("LoginPKCE done1");
            Debug.Log("Credentials saved2: " + await Passport.Instance.HasCredentialsSaved());

            // if (await Passport.Instance.HasCredentialsSaved()) {
            //     PassportStore.SetLoggedIn(true);
            // }
//        }
    }
}