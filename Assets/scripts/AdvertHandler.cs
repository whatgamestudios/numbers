// Copyright (c) Whatgame Studios 2024 - 2025
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Threading.Tasks;
using System;

namespace FourteenNumbers {
    public class AdvertHandler : MonoBehaviour {
        public GameObject advertPanel;
                    
        private const int TIME_PER_CHANGE = 2000;
        private DateTime changeTime;
        private UnityEngine.Color textFaceColour;
        private GameObject floatingImage;
        private TextMeshProUGUI infoText;
        private bool hasStartedAnimation = false;

        private uint state = 0;

        private string text1 = "Login to";
        private string text2 = "earn";
        private string text3 = "scenes";

        public void Start() {
            int selected = ScreenBackground.GetBackground();
            SceneInfo sceneInfo = BackgroundsMetadata.GetInfo(selected);
            textFaceColour = sceneInfo.faceColour;


            changeTime = DateTime.Now;
            
            // Create and setup the "Hello There" text
            GameObject textObj = new GameObject("qw");
            textObj.transform.SetParent(advertPanel.transform, false);
            
            RectTransform textRect = textObj.AddComponent<RectTransform>();
            textRect.anchorMin = new Vector2(0.5f, 0.5f);
            textRect.anchorMax = new Vector2(0.5f, 0.5f);
            textRect.anchoredPosition = Vector2.zero;
            textRect.sizeDelta = new Vector2(400, 100);
            
            infoText = textObj.AddComponent<TextMeshProUGUI>();
            infoText.fontSize = 150;
            infoText.alignment = TextAlignmentOptions.Center;
            infoText.color = textFaceColour;
            
            // Create the floating image object (but don't show it yet)
            floatingImage = new GameObject("FloatingImage");
            floatingImage.transform.SetParent(advertPanel.transform, false);
            
            RectTransform imageRect = floatingImage.AddComponent<RectTransform>();
            imageRect.anchorMin = new Vector2(0, 0.5f);
            imageRect.anchorMax = new Vector2(0, 0.5f);
            imageRect.sizeDelta = new Vector2(300, 300);
            
            Image image = floatingImage.AddComponent<Image>();
            Texture2D tex = Resources.Load<Texture2D>("scenes/gen1/gen1-type0-goldenfans");
            if (tex != null) {
                Rect size = new Rect(0.0f, 0.0f, tex.width, tex.height);
                Vector2 pivot = new Vector2(0.0f, 0.0f);
                Sprite s = Sprite.Create(tex, size, pivot);
                image.sprite = s;
            }
            
            floatingImage.SetActive(false);
        }

        public void Update() {
            DateTime now = DateTime.Now;
            double msSinceChange = (now - changeTime).TotalMilliseconds;

            if (msSinceChange > TIME_PER_CHANGE) {
                state++;
                changeTime = now;
            }
            switch (state) {
                case 4:
                case 0:
                    // Set state to 0 if is 4.
                    state = 0;
                    infoText.gameObject.SetActive(true);
                    floatingImage.SetActive(false);                    
                    infoText.text = text1;
                    break;
                case 1:
                    infoText.text = text2;
                    break;
                case 2:
                    infoText.text = text3;
                    break;
                case 3:
                    infoText.gameObject.SetActive(false);
                    floatingImage.SetActive(true);

                    // Animate the image from left to right
                    RectTransform imageRect = floatingImage.GetComponent<RectTransform>();
                    float progress = (float)(msSinceChange / TIME_PER_CHANGE);
                    float panelWidth = advertPanel.GetComponent<RectTransform>().rect.width;
                    float xPos = Mathf.Lerp(-100, panelWidth + 100, progress);
                    imageRect.anchoredPosition = new Vector2(xPos, 0);
                    
                    // Reset animation when it reaches the end
                    if (progress >= 1.0f) {
                        changeTime = DateTime.Now;
                    }

                    break;
                // case 4:
                //     break;
            }
        }
    }
}