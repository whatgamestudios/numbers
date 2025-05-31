// Copyright (c) Whatgame Studios 2024 - 2025
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;


namespace FourteenNumbers {

    public class BackButtonHandler : MonoBehaviour
    {
        public Button buttonBack;

        public void OnButtonClick(string buttonText)
        {
            if (buttonText == "Menu")
            {
                SceneManager.LoadScene("MenuScene", LoadSceneMode.Single);
            }
            else
            {
                AuditLog.Log($"BackButton: Unknown button: {buttonText}");
            }
        }

        public void Update()
        {
            if (Application.platform == RuntimePlatform.Android)
            {
                if (Input.GetKey(KeyCode.Escape))
                {
                    // Insert Code Here (I.E. Load Scene, Etc)
                    // OR Application.Quit();
                    SceneManager.LoadScene("MenuScene", LoadSceneMode.Single);
                    return;
                }
            }
        }

    }
}