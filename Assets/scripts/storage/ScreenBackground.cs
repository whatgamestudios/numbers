// Copyright (c) Whatgame Studios 2024 - 2025
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;

namespace FourteenNumbers {
    public class ScreenBackground {
        public const string BG_OPTION = "OPTION_BACKGROUND";


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
            return PlayerPrefs.GetInt(BG_OPTION, 3);
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

            int option = PlayerPrefs.GetInt(BG_OPTION, 1);
            string resourceName;
            UnityEngine.Color faceColour;
            UnityEngine.Color outlineColour;
            (resourceName, faceColour, outlineColour) = getInfo(option);

            // Set the background image.
            Texture2D tex = Resources.Load<Texture2D>(resourceName);
            // Debug.Log("tex null: " + (tex == null));
            // Debug.Log("tex: " + tex.ToString());
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
                case 2:
                    return ("free-background2", UnityEngine.Color.black, UnityEngine.Color.white);
                case 3:
                    return ("free-background3", UnityEngine.Color.white, UnityEngine.Color.black);
                case 4:
                    return ("free-background4", UnityEngine.Color.black, UnityEngine.Color.white);
                case 5:
                    return ("free-background5", UnityEngine.Color.black, UnityEngine.Color.white);
                case 6:
                    return ("free-background6", UnityEngine.Color.white, UnityEngine.Color.black);
                default:
                    return ("free-background1", UnityEngine.Color.black, UnityEngine.Color.white);
            }

        }
    }
}