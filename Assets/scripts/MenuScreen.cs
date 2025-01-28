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
        public Button buttonSettings;
        public Button buttonCredits;
        public Button buttonHelp;

        public TextMeshProUGUI loggedIn;


        public async void Start() {
            bool isLoggedIn = PassportStore.IsLoggedIn();
            if (isLoggedIn) {
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
            else if (buttonText == "Settings") {
                SceneManager.LoadScene("SettingsScene", LoadSceneMode.Single);
            }
            else if (buttonText == "Credits") {
                SceneManager.LoadScene("CreditsScene", LoadSceneMode.Single);
            }
            else if (buttonText == "Help") {
                SceneManager.LoadScene("HelpScene", LoadSceneMode.Single);
            }
            else {
                Debug.Log("Unknown button");
            }
        }
    }
}