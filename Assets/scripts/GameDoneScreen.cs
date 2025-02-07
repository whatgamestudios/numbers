// Copyright (c) Whatgame Studios 2024 - 2025
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Threading.Tasks;


namespace FourteenNumbers {

    public class GameDoneScreen : MonoBehaviour {
        public Button buttonPublish;
        public Button buttonBack;

        public TextMeshProUGUI info;
        public void Awake() {
            buttonPublish.gameObject.SetActive(false);
        }
                    
        public async void Start() {
            uint z = 1;
            try {
                uint pointsToday = (uint) Stats.GetTotalPointsToday();
                z = 2;
                uint gameDay = (uint) Stats.GetLastGameDay();
                z = 3;
                uint bestPoints = (uint) Stats.GetBestPointsToday();
                z = 4;
                if (bestPoints == 0) {
                    z = 5;
                    bestPoints = await (new FourteenNumbersContract()).GetBestScore(gameDay);
                }
                z = 6;

                if (pointsToday > bestPoints) {
                    z = 7;
                    info.text = "You have the best solution today so far!\n" +
                        "Best so far: " + bestPoints + "\n" +
                        "Your points: " + pointsToday;
                    z = 8;
                    buttonPublish.gameObject.SetActive(true);
                    z = 9;
                }
                else {
                    z = 10;
                    info.text = "Best so far today: " + bestPoints + "\n" +
                        "Your points: " + pointsToday + "\n";
                    z = 11;
                    buttonPublish.gameObject.SetActive(false);
                }
                z = 12;
            }
            catch (System.Exception ex) {
                info.text = "Exception(" + z + "): " + ex.Message;
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