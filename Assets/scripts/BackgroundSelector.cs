// Copyright (c) Whatgame Studios 2024 - 2025
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;

namespace FourteenNumbers {

    public class BackgroundSelector : MonoBehaviour {
        public GameObject panelScreenBackground;

        public GameObject panelFreeType1;
        public GameObject panelFreeType2;
        public GameObject panelFreeType3;

        public GameObject panelOwned;

        private GameObject[] panelOwnedNfts;
        private ScrollRect scrollRect;

        public void Start() {
            int selected = ScreenBackground.GetBackground();
            setSelected(selected);

            // Get the ScrollRect component
            scrollRect = panelOwned.GetComponentInParent<ScrollRect>();
            if (scrollRect == null) {
                Debug.LogError("No ScrollRect found in parent of panelOwned");
                return;
            }

            drawOwnedPanel();
        }

        public void OnButtonClick(string buttonText) {
            int option = BackgroundsMetadata.ButtonTextToOption(buttonText);
            ScreenBackground.SetBackground(option);
            setSelected(option);
            ScreenBackgroundSetter.SetPanelBackground(panelScreenBackground);
        }

        private void setSelected(int selected) {
            // Handle free panels
            Image img1 = panelFreeType1.GetComponent<Image>();
            Image img2 = panelFreeType2.GetComponent<Image>();
            Image img3 = panelFreeType3.GetComponent<Image>();
            
            img1.color = UnityEngine.Color.black;
            img2.color = UnityEngine.Color.black;
            img3.color = UnityEngine.Color.black;

            // Handle owned NFT panels
            if (panelOwnedNfts != null) {
                foreach (GameObject panel in panelOwnedNfts) {
                    if (panel != null) {
                        Image panelImage = panel.GetComponent<Image>();
                        if (panelImage != null) {
                            panelImage.color = UnityEngine.Color.black;
                        }
                    }
                }
            }

            // Set the selected panel to red
            if (selected >= 1 && selected <= 3) {
                switch (selected) {
                    case 1:
                        img1.color = UnityEngine.Color.red;
                        break;
                    case 2:
                        img2.color = UnityEngine.Color.red;
                        break;
                    case 3:
                        img3.color = UnityEngine.Color.red;
                        break;
                }
            } else if (selected >= 101 && selected <= 109) {
                // Find the owned NFT panel with this ID
                if (panelOwnedNfts != null) {
                    foreach (GameObject panel in panelOwnedNfts) {
                        if (panel != null) {
                            Button button = panel.GetComponentInChildren<Button>();
                            if (button != null && button.name == $"gen1_{selected}") {
                                Image panelImage = panel.GetComponent<Image>();
                                if (panelImage != null) {
                                    panelImage.color = UnityEngine.Color.red;
                                }
                                break;
                            }
                        }
                    }
                }
            } else {
                Debug.Log("Unknown option: " + selected);
            }
        }

        private void drawOwnedPanel() {
            int[] owned1 = new int[9] { 101, 102, 103, 104, 105, 106, 107, 108, 109 };
//            int[] owned1 = new int[4] { 101, 102, 103, 104};
            ScreenBackground.SetOwned(owned1);

            // Get the owned backgrounds
            int[] owned = ScreenBackground.GetOwned();
            
            // Clear existing panels if any
            if (panelOwnedNfts != null) {
                foreach (GameObject panel in panelOwnedNfts) {
                    Destroy(panel);
                }
            }
            
            // Reset scroll position to top
            if (scrollRect != null) {
                scrollRect.normalizedPosition = new Vector2(0, 1);
            }
            
            // Resize panelOwned based on number of NFTs
            RectTransform panelOwnedRect = panelOwned.GetComponent<RectTransform>();
            float panelHeight = 247 + (owned.Length * 220) + 20; // 247 initial offset + (220 per panel) + 20 padding at bottom
            panelOwnedRect.sizeDelta = new Vector2(panelOwnedRect.sizeDelta.x, panelHeight);
            
            // Create new array for panels
            panelOwnedNfts = new GameObject[owned.Length];
            
            // Create panels for each owned background
            for (int i = 0; i < owned.Length; i++) {
                // Create outer container panel
                GameObject outerContainer = new GameObject($"OuterContainer_{i}");
                outerContainer.transform.SetParent(panelOwned.transform, false);

                // Add RectTransform to outer container
                RectTransform outerRect = outerContainer.AddComponent<RectTransform>();
                outerRect.anchorMin = new Vector2(0.5f, 1f);
                outerRect.anchorMax = new Vector2(0.5f, 1f);
                outerRect.anchoredPosition = new Vector2(0, -247 - (i * 220));
                outerRect.sizeDelta = new Vector2(800, 210); // Full width of parent
 
                // Add Image component for background
                Image outerImage = outerContainer.AddComponent<Image>();
                outerImage.color = Color.black;
                outerImage.sprite = Resources.Load<Sprite>("UI/Sprites/UI_Background");

                // Create button container (left side)
                GameObject buttonContainer = new GameObject($"ButtonContainer_{i}");
                buttonContainer.transform.SetParent(outerContainer.transform, false);

                // Add RectTransform to button container
                RectTransform buttonContainerRect = buttonContainer.AddComponent<RectTransform>();
                buttonContainerRect.anchorMin = new Vector2(0, 0.5f);
                buttonContainerRect.anchorMax = new Vector2(0, 0.5f);
                buttonContainerRect.anchoredPosition = new Vector2(105, 0); // Half of button width
                buttonContainerRect.sizeDelta = new Vector2(210, 210);

                // Create button
                GameObject button = new GameObject($"OwnedButton_{i}");
                button.transform.SetParent(buttonContainer.transform, false);
                
                // Add RectTransform to button
                RectTransform buttonRect = button.AddComponent<RectTransform>();
                buttonRect.anchorMin = new Vector2(0.5f, 0.5f);
                buttonRect.anchorMax = new Vector2(0.5f, 0.5f);
                buttonRect.anchoredPosition = Vector2.zero;
                buttonRect.sizeDelta = new Vector2(190, 190);
                
                // Add Image component to button
                Image buttonImage = button.AddComponent<Image>();

                string resourceName;
                UnityEngine.Color faceColour;
                UnityEngine.Color outlineColour;
                (resourceName, faceColour, outlineColour) = BackgroundsMetadata.GetInfo(owned[i]);
                if (resourceName == null) {
                    Debug.Log("No resource found for NFT id: " + owned[i]);
                    continue;
                }

                Texture2D tex = Resources.Load<Texture2D>(resourceName);
                if (tex == null) {
                    Debug.Log("Resource not found: " + resourceName);
                }
                Rect size = new Rect(0.0f, 0.0f, tex.width, tex.height);
                Vector2 pivot = new Vector2(0.0f, 0.0f);
                Sprite s = Sprite.Create(tex, size, pivot);
                buttonImage.sprite = s;
                
                // Add Button component
                Button buttonComponent = button.AddComponent<Button>();
                button.name = $"gen1_{owned[i]}"; // Set the button name to match the format
                buttonComponent.onClick.AddListener(() => OnButtonClick(button.name));
                
                // Create text container (right side)
                GameObject textContainer = new GameObject($"TextContainer_{i}");
                textContainer.transform.SetParent(outerContainer.transform, false);

                // Add RectTransform to text container
                RectTransform textContainerRect = textContainer.AddComponent<RectTransform>();
                textContainerRect.anchorMin = new Vector2(0, 0);
                textContainerRect.anchorMax = new Vector2(1, 1);
                textContainerRect.offsetMin = new Vector2(220, 0); // Start after button
                textContainerRect.offsetMax = new Vector2(-20, 0); // Padding on right
                
                // Add Text component
                TextMeshProUGUI textMesh = textContainer.AddComponent<TextMeshProUGUI>();
                textMesh.text = resourceName;
                textMesh.fontSize = 36;
                textMesh.alignment = TextAlignmentOptions.Left;
                textMesh.color = Color.white;
                textMesh.textWrappingMode = TextWrappingModes.Normal;
                
                // Store panel reference
                panelOwnedNfts[i] = outerContainer;
            }
        }
    }
}