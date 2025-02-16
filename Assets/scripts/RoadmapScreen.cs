// Copyright (c) Whatgame Studios 2024 - 2025
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;


namespace FourteenNumbers {

    public class RoadmapScreen : MonoBehaviour {

        public void OnButtonClick(string buttonText) {
            if (buttonText == "Roadmap") {
                Application.OpenURL("https://whatgamestudios.com/14numbers/");
            }
            else {
                Debug.Log("Unknown button: " + buttonText);
            }
        }

    }
}