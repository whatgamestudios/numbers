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
                    "Your points: " + pointsToday;
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
                    uint gameDay = (uint) Stats.GetLastGameDay();
                    (string sol1, string sol2, string sol3) = Stats.GetSolutions();




                    await SceneManager.UnloadSceneAsync("GameDoneScene");
                }
            }
            else if (buttonText == "Share") {
                uint gameDay = (uint) Stats.GetLastGameDay();
                uint target = TargetValue.GetTarget(gameDay);
                (string sol1, string sol2, string sol3) = Stats.GetSolutions();
                (int result1, int err1) = (new CalcProcessor()).Calc(sol1);
                (int result2, int err2) = (new CalcProcessor()).Calc(sol2);
                (int result3, int err3) = (new CalcProcessor()).Calc(sol3);
                sol1 = replace(sol1);
                sol2 = replace(sol2);
                sol3 = replace(sol3);
                uint points1 = Points.CalcPoints((uint) result1, target);
                uint points2 = Points.CalcPoints((uint) result2, target);
                uint points3 = Points.CalcPoints((uint) result3, target);
                uint total = points1 + points2 + points3;

                string msg = 
                    "14Numbers - game day " + gameDay + "\n" +
                    format(sol1, (uint) result1, points1) + "\n" +
                    format(sol2, (uint) result2, points2) + "\n" +
                    format(sol3, (uint) result3, points3) + "\n" +
                    "                         " + total;
                Debug.Log("Share: \n" + msg);
                SunShineNativeShare.instance.ShareText("14Numbers", msg);
            }
            else {
                Debug.Log("Unknown button");
            }
        }

        private string replace(string solution) {
            string output = solution.Replace("100", "?");
            output = output.Replace("75", "?");
            output = output.Replace("50", "?");
            output = output.Replace("25", "?");
            output = output.Replace("10", "?");
            output = output.Replace('9', '?');
            output = output.Replace('8', '?');
            output = output.Replace('7', '?');
            output = output.Replace('6', '?');
            output = output.Replace('5', '?');
            output = output.Replace('4', '?');
            output = output.Replace('3', '?');
            output = output.Replace('2', '?');
            output = output.Replace('1', '?');
            output = output.Replace('×', '*');
            output = output.Replace('÷', '/');
            return output;
        }

        private string format(string sol, uint result, uint points) {
            int lenSol = sol.Length;
            string resStr = "" + result;
            int lenRes = resStr.Length;
            string pointsStr = "" + points;
            int lenPoints = pointsStr.Length;

            string output = sol;
            for (uint i = 0; i < (20 - lenSol); i++) {
                output = output + " ";
            }
            for (uint i = 0; i < (4 - lenRes); i++) {
                output = output + " ";
            }
            output = output + resStr;
            for (uint i = 0; i < (5 - lenPoints); i++) {
                output = output + " ";
            }
            output = output + pointsStr;
            return output;
        }
    }
}