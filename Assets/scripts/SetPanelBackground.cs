// Copyright (c)  Whatgame Studios 2024 - 2025
using UnityEngine;
using UnityEngine.UI;
using System.Collections;


namespace FourteenNumbers {
    public class SetPanelBackground : MonoBehaviour {
        public GameObject panel;

        public void Start() {
            ScreenBackgroundSetter.SetPanelBackground(panel);
        }
    }
}