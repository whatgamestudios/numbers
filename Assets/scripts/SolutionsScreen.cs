// Copyright (c) Whatgame Studios 2024 - 2025
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Collections;
using UnityEngine.SceneManagement;


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

        }
    }
}