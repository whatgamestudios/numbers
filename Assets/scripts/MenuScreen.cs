// Copyright (c) Whatgame Studios 2024 - 2025
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using Immutable.Passport;

namespace FourteenNumbers {

    public class MenuScreen : MonoBehaviour {

        public Button buttonPlay;
        public Button buttonStats;
        public Button buttonBackgrounds;
        public Button buttonSolutions;
        public Button buttonHelp;
        public Button buttonOther;


        public TextMeshProUGUI loggedIn;


        public async void Start() {
            // Check login
            bool isLoggedIn = PassportStore.IsLoggedIn();
            bool recentlyCheckedLogin = PassportStore.WasLoggedInRecently();
            Debug.Log("isloggedIn: " + isLoggedIn + ", recentlyCheckedLogin: " + recentlyCheckedLogin);
            if (isLoggedIn) {
                if (!recentlyCheckedLogin) {
                    if (await Passport.Instance.HasCredentialsSaved()) {
                        // Try to log in using saved credentials
                        bool success = await Passport.Instance.Login(useCachedSession: true);
                        if (success) {
                            PassportStore.SetLoggedInChecked();
                        }
                        else {
                            SceneManager.LoadScene("LoginScene", LoadSceneMode.Single);
                        }
                    }
                    else {
                        SceneManager.LoadScene("LoginScene", LoadSceneMode.Single);
                    }
                }

                loggedIn.text = "Setting up wallet..";
                // Set up provider
                await Passport.Instance.ConnectEvm();
                loggedIn.text = "Setting up wallet...";
                // Set up wallet (includes creating a wallet for new players)
                List<string> accounts = await Passport.Instance.ZkEvmRequestAccounts();
                if (accounts.Count ==0) {
                    loggedIn.text = "Logged In";
                }
                else {
                    //loggedIn.text = "Logged In as\n" + accounts[0];
                    loggedIn.text = "Logged In (" + 
                                    DeepLinkManager.Instance.LoginPath + 
                                    ") as\n" + 
                                    accounts[0];
                    Debug.Log("Account count was: " + accounts.Count);
                }
            }
            else {
                loggedIn.text = "Not Logged In";
            }
        }


        public void OnButtonClick(string buttonText) {
            if (buttonText == "Play") {
                // Only specifying the sceneName or sceneBuildIndex will load the Scene with the Single mode
                SceneManager.LoadScene("GamePlayScene", LoadSceneMode.Single);
            }
            else if (buttonText == "Stats") {
                SceneManager.LoadScene("StatsScene", LoadSceneMode.Single);
            }
            else if (buttonText == "Solutions") {
                SceneManager.LoadScene("SolutionsScene", LoadSceneMode.Single);
            }
            else if (buttonText == "Backgrounds") {
                SceneManager.LoadScene("BackgroundsScene", LoadSceneMode.Single);
            }
            else if (buttonText == "Help") {
                SceneManager.LoadScene("HelpScene", LoadSceneMode.Single);
            }
            else if (buttonText == "Other") {
                SceneManager.LoadScene("OtherMenuScene", LoadSceneMode.Single);
            }
            else {
                Debug.Log("Unknown button");
            }
        }
    }
}
