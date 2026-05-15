// Copyright (c) Whatgame Studios 2024 - 2025
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System;
using TMPro;

namespace FourteenNumbers {

    // Manage the publish button
    public class PublishManager : MonoBehaviour
    {
        public GameObject panelPublish;

        public void Start()
        {
            panelPublish.SetActive(false);
        }

        public void OnButtonClick(string buttonText)
        {
            if (buttonText == "Publish")
            {
                AuditLog.Log("Publish");
                panelPublish.SetActive(false);
                SceneStack.Instance().PushScene();
                SceneManager.LoadScene("PublishScene", LoadSceneMode.Additive);
            }
            else
            {
                AuditLog.Log("PublishManager: Unknown button: " + buttonText);
            }
        }

        public void Update()
        {
            GameState gameState = GameState.Instance();
            uint pointsToday = gameState.PointsEarnedTotal();
            if (!BestScoreLoader.LoadedBestScore ||
                Stats.HasPublishedToday() ||
                pointsToday < BestScoreLoader.BestScore)
            {
                panelPublish.SetActive(false);
                return;
            }

            panelPublish.SetActive(true);
        }
    }
}