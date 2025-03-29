// Copyright (c) Whatgame Studios 2024 - 2025
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;

namespace FourteenNumbers {
    public class ScreenBackground {
        public const string BG_OPTION = "OPTION_BACKGROUND";

        public const int BG_DEFAULT = 1;


        /**
        * Set the background used by all scenes.
        */
        public static void SetBackground(int option) {
            PlayerPrefs.SetInt(BG_OPTION, option);
            PlayerPrefs.Save();
        }

        /**
        * Get the background option to be used.
        *
        * @return the background option number to use.
        */
        public static int GetBackground() {
            return PlayerPrefs.GetInt(BG_OPTION, BG_DEFAULT);
        }


        /**
        * Pass in the scene's main panel and set the background image.
        */
        public static void SetPanelBackground(GameObject panel) {
            Image img = panel.GetComponent<Image>();
            if (img == null) {
                Debug.Log("No raw image");
                return;
            }

            int option = PlayerPrefs.GetInt(BG_OPTION, BG_DEFAULT);
            string resourceName;
            UnityEngine.Color faceColour;
            UnityEngine.Color outlineColour;
            (resourceName, faceColour, outlineColour) = getInfo(option);

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



        private static (string, UnityEngine.Color, UnityEngine.Color) getInfo(int option) {
            switch (option) {
                case 1:
                    return ("scenes/free/free-type1-coffee", UnityEngine.Color.white, UnityEngine.Color.black);
                case 2:
                    return ("scenes/free/free-type2-dogs", UnityEngine.Color.white, UnityEngine.Color.black);
                case 3:
                    return ("scenes/free/free-type3-koi", UnityEngine.Color.white, UnityEngine.Color.black);
                default:
                    return ("scenes/free/free-type1-coffee", UnityEngine.Color.black, UnityEngine.Color.white);
            }

        }
    }
}