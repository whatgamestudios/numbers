// Copyright (c) Whatgame Studios 2024 - 2025
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Collections;
using UnityEngine.SceneManagement;
using System.Threading.Tasks;


namespace FourteenNumbers {

    public class SolutionsScreen : BestSolutionToday {
        // Control
        public TextMeshProUGUI gameDayText;
        public TextMeshProUGUI gameDateText;

        public Button buttonUp;
        public Button buttonDown;

        // Output
        public TextMeshProUGUI targetText;

        public TextMeshProUGUI input1Text;
        public TextMeshProUGUI calculated1Text;
        public TextMeshProUGUI points1Text;
        public TextMeshProUGUI input2Text;
        public TextMeshProUGUI calculated2Text;
        public TextMeshProUGUI points2Text;
        public TextMeshProUGUI input3Text;
        public TextMeshProUGUI calculated3Text;
        public TextMeshProUGUI points3Text;
        public TextMeshProUGUI pointsTotalText;
        public TextMeshProUGUI bestPlayerText;


        private uint gameDayToday = 0;

        private uint gameDayDisplaying = 0;

        public void Start() {
            Debug.Log("Solutions Screen Start");
            uint gameDay = (uint) Timeline.GameDay();
            gameDayToday = gameDay;
            show(gameDay);
            buttonUp.interactable = false;
        }

        public void OnButtonClick(string buttonText) {
            if (buttonText == "Up") {
                uint newDay = gameDayDisplaying + 1;
                buttonDown.interactable = true;
                if (newDay >= gameDayToday) {
                    buttonUp.interactable = false;
                }
                show(newDay);
            }
            else if (buttonText == "Down") {
                uint newDay = gameDayDisplaying - 1;
                buttonUp.interactable = true;
                if (newDay == 0) {
                    buttonDown.interactable = false;
                }
                show(newDay);
            }
            else {
                Debug.Log("Unknown button: " + buttonText);
            }
        }


        public void show(uint gameDay) {
            // if (gameDay == gameDayToday) {
            //     startTimer();
            // }
            // else {
                StopTimer();
            // }

            gameDayDisplaying = gameDay;
            gameDayText.text = "" + gameDay;

            gameDateText.text = Timeline.GetRelativeDateString((int) gameDay);

            uint targetValue = TargetValue.GetTarget(gameDay);
            targetText.text = targetValue.ToString();

            StartCoroutine(GetResultRoutine());

            // FourteenNumbersContract fourteenNumbersContracts = new FourteenNumbersContract();
            // SolutionsOutputDTO result = await fourteenNumbersContracts.GetSolution(gameDay);
            // bestPlayerText.text = result.Player;
            // pointsTotalText.text = result.Points.ToString();
            //CombinedSolution
        }


        IEnumerator GetResultRoutine() {
            GetResult();
            yield return new WaitForSeconds(0f);
        }
        async void GetResult() {
            FourteenNumbersContract fourteenNumbersContracts = new FourteenNumbersContract();
            SolutionsOutputDTO result = await fourteenNumbersContracts.GetSolution(gameDayDisplaying);

            string player = result.Player;
            bestPlayerText.text = player.Substring(0,6) + "...." + player.Substring(player.Length - 4, 4);
            pointsTotalText.text = result.Points.ToString();

            byte[] combinedSolutionBytes = result.CombinedSolution;
            var combinedSolution = System.Text.Encoding.Default.GetString(combinedSolutionBytes);
            string sol1 = "";
            string sol2 = "";
            string sol3 = "";
            if (combinedSolution.Length != 0) {
                int indexOfEquals = combinedSolution.IndexOf('=');
                sol1 = combinedSolution.Substring(0, indexOfEquals);
                combinedSolution = combinedSolution.Substring(indexOfEquals+1);
                indexOfEquals = combinedSolution.IndexOf('=');
                sol2 = combinedSolution.Substring(0, indexOfEquals);
                sol3 = combinedSolution.Substring(indexOfEquals+1);
            }

            if (gameDayDisplaying == gameDayToday) {
                input1Text.text = replace(sol1);
                input2Text.text = replace(sol2);
                input3Text.text = replace(sol3);
            }
            else {
                input1Text.text = sol1;
                input2Text.text = sol2;
                input3Text.text = sol3;
            }

            uint points1 = 0;
            uint points2 = 0;
            uint points3 = 0;
            CalcProcessor processor = new CalcProcessor();
            uint targetValue = TargetValue.GetTarget(gameDayDisplaying);
            int errorCode;
            int res1 = 0;
            int res2 = 0;
            int res3 = 0;
            if (sol1.Length != 0) {
                (res1, errorCode) = processor.Calc(sol1);
                if (errorCode == CalcProcessor.ERR_NO_ERROR) {
                    points1 = Points.CalcPoints((uint) res1, targetValue);
                }

            }
            if (sol2.Length != 0) {
                (res2, errorCode) = processor.Calc(sol2);
                if (errorCode == CalcProcessor.ERR_NO_ERROR) {
                    points2 = Points.CalcPoints((uint) res2, targetValue);
                }
            }
            if (sol3.Length != 0) {
                (res3, errorCode) = processor.Calc(sol3);
                if (errorCode == CalcProcessor.ERR_NO_ERROR) {
                    points3 = Points.CalcPoints((uint) res3, targetValue);
                }
            }
            calculated1Text.text = res1.ToString();
            calculated2Text.text = res2.ToString();
            calculated3Text.text = res3.ToString();

            points1Text.text = points1.ToString();
            points2Text.text = points2.ToString();
            points3Text.text = points3.ToString();
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

    }
}