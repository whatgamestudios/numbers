// Copyright (c) Whatgame Studios 2024 - 2025
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;

namespace FourteenNumbers {

    public class BackButtonHandler : MonoBehaviour
    {
        private InputAction backAction;

        private void Awake()
        {
            // Create and set up the back action
            backAction = new InputAction("Back", binding: "<Keyboard>/escape");
            backAction.performed += OnBackPerformed;
        }

        private void OnEnable()
        {
            backAction.Enable();
        }

        private void OnDisable()
        {
            backAction.Disable();
        }

        private void OnBackPerformed(InputAction.CallbackContext context)
        {
                goBack();
        }

        public void OnButtonClick(string buttonText)
        {
            if (buttonText == "Back")
            {
                goBack();
            }
            else
            {
                AuditLog.Log($"BackButton: Unknown button: {buttonText}");
            }
        }

        private void goBack() {
            var previousScene = SceneStack.Instance().PopScene();
            AuditLog.Log($"Back detected: switching to scene {previousScene}");
            SceneManager.LoadScene(previousScene, LoadSceneMode.Single);
        }
    }
}