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
                    
        private const int TIME_PER_CHANGE = 1500;
        private DateTime changeTime;
        private UnityEngine.Color textFaceColour;
        private GameObject floatingImage;
        private TextMeshProUGUI infoText;
        private Image floatingImageComponent;

        private uint state = 0;
        private uint lastImageState = 0;
        private float animationProgress = 0f;
        private const string text1 = "Sign In\nto earn\nTradable\nScenes";

        public void Start() {
            textFaceColour = UnityEngine.Color.black;


            changeTime = DateTime.Now;
            
            // Create and setup the text
            GameObject textObj = new GameObject("test");
            textObj.transform.SetParent(advertPanel.transform, false);
            
            RectTransform textRect = textObj.AddComponent<RectTransform>();
            textRect.anchorMin = new Vector2(0.5f, 0.5f);
            textRect.anchorMax = new Vector2(0.5f, 0.5f);
            textRect.anchoredPosition = Vector2.zero;
            textRect.sizeDelta = new Vector2(700, 400);
            
            infoText = textObj.AddComponent<TextMeshProUGUI>();
            infoText.fontSize = 150;
            infoText.alignment = TextAlignmentOptions.Center;
            infoText.color = textFaceColour;
            
            // Create the floating image object (but don't show it yet)
            floatingImage = new GameObject("FloatingImage");
            floatingImage.transform.SetParent(advertPanel.transform, false);
            
            RectTransform imageRect = floatingImage.AddComponent<RectTransform>();
            imageRect.anchorMin = new Vector2(0.5f, 0.5f);
            imageRect.anchorMax = new Vector2(0.5f, 0.5f);
            imageRect.anchoredPosition = Vector2.zero;
            imageRect.sizeDelta = new Vector2(300, 300);
            
            // Add the Image component once during Start
            floatingImageComponent = floatingImage.AddComponent<Image>();
            
            floatingImage.SetActive(false);
        }

        public void Update() {
            DateTime now = DateTime.Now;
            double msSinceChange = (now - changeTime).TotalMilliseconds;

            if (msSinceChange > TIME_PER_CHANGE) {
                state++;
                changeTime = now;
                animationProgress = 0f;
            }
            switch (state) {
                default:
                case 0:
                    // Set state to 0 if is 4.
                    state = 0;
                    infoText.gameObject.SetActive(true);
                    floatingImage.SetActive(false);                    
                    infoText.text = text1;
                    break;
                case 1:
                case 2:
                case 3:
                case 4:
                    setImage();
                    infoText.gameObject.SetActive(false);
                    floatingImage.SetActive(true);

                    // Calculate animation progress
                    animationProgress = (float)(msSinceChange / TIME_PER_CHANGE);
                    RectTransform imageRect = floatingImage.GetComponent<RectTransform>();
                    float panelWidth = advertPanel.GetComponent<RectTransform>().rect.width;
                    float panelHeight = advertPanel.GetComponent<RectTransform>().rect.height;

                    // Keep the image centered during size changes
                    imageRect.anchoredPosition = Vector2.zero;

                    float endGetLarge = 0.6f;
                    // float endGetSmall = 1.0f;

                    if (animationProgress < endGetLarge) {
                        // Grow from 300 to 500 pixels
                        float size = Mathf.Lerp(300, 600, animationProgress * 4);
                        imageRect.sizeDelta = new Vector2(size, size);
                    }
                    else {//if (animationProgress < endGetSmall) {
                        // Shrink from 400 to 100 pixels
                        float size = Mathf.Lerp(600, 0, (animationProgress - endGetLarge) * 4);
                        imageRect.sizeDelta = new Vector2(size, size);
                    }
                    // else {
                    //     float xPos, yPos;
                    //     switch (state) {
                    //         case 3:
                    //             // Fly diagonally to top left
                    //             xPos = Mathf.Lerp(0, -panelWidth, (animationProgress - endGetSmall) * 2);
                    //             yPos = Mathf.Lerp(0, panelHeight, (animationProgress - endGetSmall) * 2);
                    //             break;
                    //         case 4:
                    //             // Fly diagonally to top right
                    //             xPos = Mathf.Lerp(0, panelWidth, (animationProgress - endGetSmall) * 2);
                    //             yPos = Mathf.Lerp(0, panelHeight, (animationProgress - endGetSmall) * 2);
                    //             break;
                    //         case 5:
                    //             // Fly diagonally to bottom right
                    //             xPos = Mathf.Lerp(0, panelWidth, (animationProgress - endGetSmall) * 2);
                    //             yPos = Mathf.Lerp(0, -panelHeight, (animationProgress - endGetSmall) * 2);
                    //             break;
                    //         default:
                    //             // Fly diagonally to bottom left
                    //             xPos = Mathf.Lerp(0, -panelWidth, (animationProgress - endGetSmall) * 2);
                    //             yPos = Mathf.Lerp(0, -panelHeight, (animationProgress - endGetSmall) * 2);
                    //             break;
                    //     }
                    //     imageRect.anchoredPosition = new Vector2(xPos, yPos);
                    //}
                    break;
            }
        }

        private void setImage() {
            if (state != lastImageState) {
                lastImageState = state;

                uint gen1Id = state + (100 - 1);
                SceneInfo sceneInfo = BackgroundsMetadata.GetInfo((int)gen1Id);
                string resource = "scenes/gen1/gen1-type0-goldenfans";
                if (sceneInfo.resource != null) {
                    resource = sceneInfo.resource;
                }

                // Update the existing Image component
                Texture2D tex = Resources.Load<Texture2D>(resource);
                if (tex != null) {
                    Rect size = new Rect(0.0f, 0.0f, tex.width, tex.height);
                    Vector2 pivot = new Vector2(0.0f, 0.0f);
                    Sprite s = Sprite.Create(tex, size, pivot);
                    floatingImageComponent.sprite = s;
                }
            }
        }
    }
}