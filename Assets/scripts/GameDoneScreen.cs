// Copyright (c) Whatgame Studios 2024 - 2025
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;


namespace FourteenNumbers {

    public class GameDoneScreen : MonoBehaviour {
        public Button buttonPublish;
        public Button buttonShare;

        public TextMeshProUGUI info;

        public TextMeshProUGUI publishButtonText;

        bool buttonPublishIsPublish = false;

        public void Start() {
            uint pointsToday = (uint) Stats.GetTotalPointsToday();

            uint bestPoints = 0;
            if (BestSolutionToday.Instance != null) {
                bestPoints = BestSolutionToday.Instance.BestScore;
            }

            if (pointsToday > bestPoints) {
                info.text = "You have the best solution today so far!\n" +
                    "Best so far: " + bestPoints + "\n" +
                    "Your points: " + pointsToday + "\n" +
                    "Publish results?";
                buttonPublishIsPublish = true;
            }
            else {
                info.text = "Best so far today: " + bestPoints + "\n" +
                    "Your points: " + pointsToday + "\n";
                buttonPublishIsPublish = false;
                publishButtonText.text = "Back";
            }
        }

        public void OnButtonClick(string buttonText) {
            if (buttonText == "Publish") {
                if (buttonPublishIsPublish) {
                    // publish
                    buttonPublishIsPublish = false;
                    publishButtonText.text = "Back";
                }
                else {
                    SceneManager.LoadScene("GamePlayScene", LoadSceneMode.Single);
                }

            }
            else {
                Debug.Log("Unknown button");
            }
        }
    }
}