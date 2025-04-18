// Copyright (c) Whatgame Studios 2024 - 2025
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;

namespace FourteenNumbers {
    public class ScreenBackgroundSetter {
        /**
        * Pass in the scene's main panel and set the background image.
        */
        public static void SetPanelBackground(GameObject panel) {
            Image img = panel.GetComponent<Image>();
            if (img == null) {
                Debug.Log("No raw image");
                return;
            }

            int option = ScreenBackground.GetBackground();
            string resourceName;
            UnityEngine.Color faceColour;
            UnityEngine.Color outlineColour;
            string description;
            (resourceName, faceColour, outlineColour, description) = BackgroundsMetadata.GetInfo(option);

            // Set the background image.
            Texture2D tex = Resources.Load<Texture2D>(resourceName);
            if (tex == null) {
                Debug.Log("Resource not found: " + resourceName);
            }
            Rect size = new Rect(0.0f, 0.0f, tex.width, tex.height);
            Vector2 pivot = new Vector2(0.0f, 0.0f);
            Sprite s = Sprite.Create(tex, size, pivot);
            img.sprite = s;

            // Find all tagged objects and set their colour to the contract colour.
            GameObject[] textMeshes = GameObject.FindGameObjectsWithTag("ColMe");
            foreach (GameObject textMeshObj in textMeshes) {
                TextMeshProUGUI textMeshProGUI = textMeshObj.GetComponent<TextMeshProUGUI>();
                textMeshProGUI.color = faceColour;    
                textMeshProGUI.outlineColor = outlineColour;
            }
        }
    }
}