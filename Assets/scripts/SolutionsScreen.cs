// Copyright (c) Whatgame Studios 2024 - 2025
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Collections;
using UnityEngine.SceneManagement;
using System.Threading.Tasks;


namespace FourteenNumbers {

    public class SolutionsScreen : MonoBehaviour {
        // Control
        public TextMeshProUGUI gameDayText;
        public TextMeshProUGUI gameDateText;

        public Button buttonUp;
        public Button buttonDown;

        // Output
        public TextMeshProUGUI targetText;

        public TextMeshProUGUI bestInput1Text;
        public TextMeshProUGUI bestCalculated1Text;
        public TextMeshProUGUI bestPoints1Text;
        public TextMeshProUGUI bestInput2Text;
        public TextMeshProUGUI bestCalculated2Text;
        public TextMeshProUGUI bestPoints2Text;
        public TextMeshProUGUI bestInput3Text;
        public TextMeshProUGUI bestCalculated3Text;
        public TextMeshProUGUI bestPoints3Text;
        public TextMeshProUGUI bestPointsTotalText;
        public TextMeshProUGUI bestPlayerText;


        public TextMeshProUGUI playerInput1Text;
        public TextMeshProUGUI playerCalculated1Text;
        public TextMeshProUGUI playerPoints1Text;
        public TextMeshProUGUI playerInput2Text;
        public TextMeshProUGUI playerCalculated2Text;
        public TextMeshProUGUI playerPoints2Text;
        public TextMeshProUGUI playerInput3Text;
        public TextMeshProUGUI playerCalculated3Text;
        public TextMeshProUGUI playerPoints3Text;
        public TextMeshProUGUI playerPointsTotalText;



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
            gameDayDisplaying = gameDay;
            gameDayText.text = "" + gameDay;

            gameDateText.text = Timeline.GetRelativeDateString((int) gameDay);

            uint targetValue = TargetValue.GetTarget(gameDay);
            targetText.text = targetValue.ToString();
            DisplayMyResult(gameDay);

            StartCoroutine(GetResultRoutine());
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
            bestPointsTotalText.text = result.Points.ToString();

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
                bestInput1Text.text = replace(sol1);
                bestInput2Text.text = replace(sol2);
                bestInput3Text.text = replace(sol3);
            }
            else {
                bestInput1Text.text = sol1;
                bestInput2Text.text = sol2;
                bestInput3Text.text = sol3;
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
            bestCalculated1Text.text = res1.ToString();
            bestCalculated2Text.text = res2.ToString();
            bestCalculated3Text.text = res3.ToString();

            bestPoints1Text.text = points1.ToString();
            bestPoints2Text.text = points2.ToString();
            bestPoints3Text.text = points3.ToString();
        }

        void DisplayMyResult(uint gameDay) {

            var combinedSolution = Stats.GetCombinedSolution(gameDay);
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

            playerInput1Text.text = replace(sol1, true);
            playerInput2Text.text = replace(sol2, true);
            playerInput3Text.text = replace(sol3, true);

            uint points1 = 0;
            uint points2 = 0;
            uint points3 = 0;
            CalcProcessor processor = new CalcProcessor();
            uint targetValue = TargetValue.GetTarget(gameDay);
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
            playerCalculated1Text.text = res1.ToString();
            playerCalculated2Text.text = res2.ToString();
            playerCalculated3Text.text = res3.ToString();

            playerPoints1Text.text = points1.ToString();
            playerPoints2Text.text = points2.ToString();
            playerPoints3Text.text = points3.ToString();

            playerPointsTotalText.text = (points1 + points2 + points3).ToString();
        }


        private string replace(string solution, bool symbolsOnly = false) {
            string output = solution;
            if (!symbolsOnly) {
                output = output.Replace("100", "?");
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
            }
            output = output.Replace('*', '×');
            output = output.Replace('/', '÷');
            return output;
        }

    }
}