// Copyright (c) Whatgame Studios 2024 - 2025
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;


namespace FourteenNumbers {

    public class SocialsScreen : MonoBehaviour {
    
        public void OnButtonClick(string buttonText) {
            if (buttonText == "Instagram") {
                Application.OpenURL("https://www.instagram.com/14numbers_/");
            }
            else if (buttonText == "Discord") {
                Application.OpenURL("https://discord.gg/V6nbs2grws");
            }
            else if (buttonText == "YouTube") {
                Application.OpenURL("https://www.youtube.com/@14Numbers");
            }
            else if (buttonText == "Twitter") {
                Application.OpenURL("https://x.com/14_numbers");
            }
            else {
                Debug.Log("Unknown button " + buttonText);
            }
        }
    }
}