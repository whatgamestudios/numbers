// Copyright (c) Whatgame Studios 2024 - 2025
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;

namespace FourteenNumbers {

    public class MenuScreen : MonoBehaviour {

        public Button buttonPlay;
        public Button buttonStats;
        public Button buttonBackgrounds;
        public Button buttonSolutions;
        public Button buttonHelp;
        public Button buttonOther;


        public void Start() {
            AuditLog.Log("Menu screen");
        }


        public void OnButtonClick(string buttonText)
        {
            if (buttonText == "Play")
            {
                SceneStack.Instance().PushScene();
                SceneManager.LoadScene("GamePlayScene", LoadSceneMode.Single);
            }
            else if (buttonText == "Stats")
            {
                SceneStack.Instance().PushScene();
                SceneManager.LoadScene("StatsScene", LoadSceneMode.Single);
            }
            else if (buttonText == "Solutions")
            {
                SceneStack.Instance().PushScene();
                SceneManager.LoadScene("SolutionsScene", LoadSceneMode.Single);
            }
            else if (buttonText == "Backgrounds")
            {
                SceneStack.Instance().PushScene();
                SceneManager.LoadScene("BackgroundsScene", LoadSceneMode.Single);
            }
            else if (buttonText == "Help")
            {
                SceneStack.Instance().PushScene();
                SceneManager.LoadScene("HelpScene", LoadSceneMode.Single);
            }
            else if (buttonText == "Other")
            {
                SceneStack.Instance().PushScene();
                SceneManager.LoadScene("OtherMenuScene", LoadSceneMode.Single);
            }
            else
            {
                AuditLog.Log($"Menu: Unknown button {buttonText}");
                return;
            }
        }
    }
}
