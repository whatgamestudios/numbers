// Copyright (c) Whatgame Studios 2024 - 2025
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;


namespace FourteenNumbers {

    public class GameDoneScreen : MonoBehaviour {
        public Button buttonPublish;
        public Button buttonBack;

        public TextMeshProUGUI info;

        public void Start() {
            uint pointsToday = (uint) Stats.GetTotalPointsToday();

            uint bestPoints = 0;
            if (BestSolutionToday.Instance != null) {
                bestPoints = BestSolutionToday.Instance.BestScore;
            }

            if (pointsToday > bestPoints) {
                info.text = "You have the best solution today so far!\n" +
                    "Best so far: " + bestPoints + "\n" +
                    "Your points: " + pointsToday;
                buttonPublish.gameObject.SetActive(true);
            }
            else {
                info.text = "Best so far today: " + bestPoints + "\n" +
                    "Your points: " + pointsToday + "\n";
                buttonPublish.gameObject.SetActive(false);
            }
        }

        public async void OnButtonClick(string buttonText) {
            if (buttonText == "Publish") {
                uint gameDay = (uint) Stats.GetLastGameDay();
                (string sol1, string sol2, string sol3) = Stats.GetSolutions();

                FourteenNumbersContract contract = new FourteenNumbersContract();
                contract.SubmitBestScore(gameDay, sol1, sol2, sol3);
                buttonPublish.gameObject.SetActive(false);
            }
            else if (buttonText == "Back") {
                uint gameDay = (uint) Stats.GetLastGameDay();
                (string sol1, string sol2, string sol3) = Stats.GetSolutions();

                await SceneManager.UnloadSceneAsync("GameDoneScene");
            }
            else {
                Debug.Log("Unknown button");
            }
        }
    }
}