// Copyright (c) Whatgame Studios 2024 - 2025
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using Immutable.Passport;

namespace FourteenNumbers {

    public class PassportScreen : MonoBehaviour {

        public TextMeshProUGUI TransactionsButtonText;
        public TextMeshProUGUI AccountButtonText;
        private string account = "Passport account number not loaded";

        public async void Start() {
            AuditLog.Log("Passport screen");

            bool isLoggedIn = PassportStore.IsLoggedIn();
            if (isLoggedIn) {
                await PassportLogin.Init();
                await PassportLogin.Login();

                // Set up wallet (includes creating a wallet for new players)
                List<string> accounts = await Passport.Instance.ZkEvmRequestAccounts();
                if (accounts.Count ==0) {
                    AuditLog.Log("Passport: No account available");
                }
                else {
                    account = accounts[0];
                }
            }
            else {
                TransactionsButtonText.text = "Login";
                AccountButtonText.text = "Login";
            }
        }

        public void OnButtonClick(string buttonText) {
            bool isLoggedIn = PassportStore.IsLoggedIn();
            if (buttonText == "Passport") {
                string url = "https://play.immutable.com/collection/zkEvm/0x29c3a209d8423f9a53bf8ad39bbb85087a2a938b/";
                Application.OpenURL(url);
            }
            else if (buttonText == "Account") {
                if (isLoggedIn) {
                    string msg = account;
                    SunShineNativeShare.instance.ShareText(msg, msg);
                }
                else {
                    SceneManager.LoadScene("LoginScene", LoadSceneMode.Single);
                }
            }
            else if (buttonText == "Transactions") {
                if (isLoggedIn) {
                    string url = "https://explorer.immutable.com/address/" + account + "?tab=internal_txns";
                    Application.OpenURL(url);
                }
                else {
                    SceneManager.LoadScene("LoginScene", LoadSceneMode.Single);
                }
            }
            else if (buttonText == "Warning") {
                Application.OpenURL("https://whatgamestudios.com/14numbers/investor-warning/");
            }
            else {
                AuditLog.Log("Passport: Unknown button: " + buttonText);
            }
        }

    }
}