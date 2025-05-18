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
            AuditLog.Log("Menu screen");
            loggedIn.text = "Loading";

            bool isLoggedIn = PassportStore.IsLoggedIn();
            if (isLoggedIn) {
                await PassportLogin.Init();
                await PassportLogin.Login();

                // Set up wallet (includes creating a wallet for new players)
                List<string> accounts = await Passport.Instance.ZkEvmRequestAccounts();
                if (accounts.Count ==0) {
                    loggedIn.text = "Logged In";
                }
                else {
                    string account = accounts[0];
                    loggedIn.text = "Logged In (" + 
                                    DeepLinkManager.Instance.LoginPath + 
                                    ") as\n" + 
                                    account;
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
                AuditLog.Log($"Menu: Unknown button {buttonText}");
            }
        }
    }
}
