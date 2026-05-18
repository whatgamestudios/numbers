// Copyright (c) Whatgame Studios 2024 - 2025
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FourteenNumbers {
    public class WelcomeScreen : MonoBehaviour
    {
        public void Start()
        {
            AuditLog.Log("Welcome screen");
            SceneManager.LoadScene("MenuScene", LoadSceneMode.Single);
        }
    }
}
