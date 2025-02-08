// Copyright (c) Whatgame Studios 2024 - 2025
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;


namespace FourteenNumbers {

    public class PublishManager : MonoBehaviour {

        public void OnButtonClick(string buttonText) {
            if (buttonText == "Publish") {
                uint gameDay = (uint) Stats.GetLastGameDay();
                (string sol1, string sol2, string sol3) = Stats.GetSolutions();

                FourteenNumbersContract contract = new FourteenNumbersContract();
                contract.SubmitBestScore(gameDay, sol1, sol2, sol3);

                SceneManager.LoadScene("PublishScene", LoadSceneMode.Additive);
            }
            else {
                Debug.Log("Unknown button: " + buttonText);
            }
        }

    }
}