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

        public async void OnButtonClick(string buttonText) {
            if (buttonText == "Publish") {
                if (buttonPublishIsPublish) {
                    // publish
                    uint gameDay = (uint) Stats.GetLastGameDay();
                    (string sol1, string sol2, string sol3) = Stats.GetSolutions();

                    FourteenNumbersContract contract = new FourteenNumbersContract();
                    contract.SubmitBestScore(gameDay, sol1, sol2, sol3);

                    buttonPublishIsPublish = false;
                    publishButtonText.text = "Back";
                }
                else {
                    // back
                    await SceneManager.UnloadSceneAsync("GameDoneScene");
                }

            }
            else {
                Debug.Log("Unknown button");
            }
        }
    }
}