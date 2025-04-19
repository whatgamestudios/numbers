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
            // Get the ScrollRect component
            scrollRect = panelOwned.GetComponentInParent<ScrollRect>();
            if (scrollRect == null) {
                Debug.LogError("No ScrollRect found in parent of panelOwned");
                return;
            }

            drawOwnedPanel();

            int selected = ScreenBackground.GetBackground();
            setSelected(selected);
        }

        public void OnButtonClick(string buttonText) {
            int option = BackgroundsMetadata.ButtonTextToOption(buttonText);
            ScreenBackground.SetBackground(option);
            setSelected(option);
            ScreenBackgroundSetter.SetPanelBackground(panelScreenBackground);
        }

        private void setSelected(int selected) {
            Debug.Log("Background Selector: select: " + selected);

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
            } else if (selected >= 100 && selected <= 103) {
                // Find the owned NFT panel with this ID
                if (panelOwnedNfts != null) {
                    foreach (GameObject panel in panelOwnedNfts) {
                        if (panel != null) {
                            Button button = panel.GetComponentInChildren<Button>();
                            if (button != null && button.name == $"button_Gen1_{selected}") {
                                Image panelImage = panel.GetComponent<Image>();
                                if (panelImage != null) {
                                    panelImage.color = UnityEngine.Color.red;
                                }
                                else {
                                    Debug.Log("Background Selector: Panel image is null");
                                }
                                break;
                            }
                            else {
                                if (button == null) {
                                    Debug.Log("Background Selector: Button is null");
                                }
                                else {
                                    // This is expected - no need to log
                                    // Debug.Log("Background Selector: Incorrect button name: " + button.name);
                                }
                            }
                        }
                        else {
                            Debug.Log("Background Selector: Panel is null");
                        }
                    }
                }
            } else {
                Debug.Log("Background Selector: Unknown option: " + selected);
            }
        }

        private void drawOwnedPanel() {
//            int[] owned1 = new int[9] { 101, 102, 103, 104, 105, 106, 107, 108, 109 };
            int[] owned1 = new int[4] { 100, 101, 102, 103};
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
                SceneInfo sceneInfo = BackgroundsMetadata.GetInfo(owned[i]);
                if (sceneInfo.resource == null) {
                    Debug.Log("No resource found for NFT id: " + owned[i]);
                    continue;
                }
                string buttonId = "button_" + sceneInfo.series + "_" + owned[i];

                // Create outer container panel
                GameObject buttonContainer = new GameObject(buttonId);
                buttonContainer.transform.SetParent(panelOwned.transform, false);

                // Add RectTransform to outer container
                RectTransform buttonRect = buttonContainer.AddComponent<RectTransform>();
                buttonRect.anchorMin = new Vector2(0.5f, 1f);
                buttonRect.anchorMax = new Vector2(0.5f, 1f);
                buttonRect.anchoredPosition = new Vector2(0, -247 - (i * 220));
                buttonRect.sizeDelta = new Vector2(750, 210); // Width of parent less a bit
 
                // Add Image component for background
                Image buttonImage = buttonContainer.AddComponent<Image>();
                buttonImage.color = Color.black;
                buttonImage.sprite = Resources.Load<Sprite>("UI/Sprites/UI_Background");

                // Create button container (left side)
                GameObject imageContainer = new GameObject($"imageContainer_{i}");
                imageContainer.transform.SetParent(buttonContainer.transform, false);

                // Add RectTransform to button container
                RectTransform imageContainerRect = imageContainer.AddComponent<RectTransform>();
                imageContainerRect.anchorMin = new Vector2(0, 0.5f);
                imageContainerRect.anchorMax = new Vector2(0, 0.5f);
                imageContainerRect.anchoredPosition = new Vector2(105, 0); // Half of button width
                imageContainerRect.sizeDelta = new Vector2(210, 210);

                // Create button
                GameObject button = new GameObject($"OwnedButton_{i}");
                button.transform.SetParent(imageContainer.transform, false);
                
                // Add RectTransform to button
                RectTransform imageRect = button.AddComponent<RectTransform>();
                imageRect.anchorMin = new Vector2(0.5f, 0.5f);
                imageRect.anchorMax = new Vector2(0.5f, 0.5f);
                imageRect.anchoredPosition = Vector2.zero;
                imageRect.sizeDelta = new Vector2(190, 190);
                
                // Add Image component to button
                Image imageImage = button.AddComponent<Image>();

                Texture2D tex = Resources.Load<Texture2D>(sceneInfo.resource);
                if (tex == null) {
                    Debug.Log("Resource not found: " + sceneInfo.resource);
                }
                Rect size = new Rect(0.0f, 0.0f, tex.width, tex.height);
                Vector2 pivot = new Vector2(0.0f, 0.0f);
                Sprite s = Sprite.Create(tex, size, pivot);
                imageImage.sprite = s;
                
                // Add Button component
                Button buttonComponent = buttonContainer.AddComponent<Button>();
                
                button.name = $"gen1_{owned[i]}"; // Set the button name to match the format
                buttonComponent.onClick.AddListener(() => OnButtonClick(button.name));
                
                // Create text container (right side)
                GameObject textContainer = new GameObject($"TextContainer_{i}");
                textContainer.transform.SetParent(buttonContainer.transform, false);

                // Add RectTransform to text container
                RectTransform textContainerRect = textContainer.AddComponent<RectTransform>();
                textContainerRect.anchorMin = new Vector2(0, 0);
                textContainerRect.anchorMax = new Vector2(1, 1);
                textContainerRect.offsetMin = new Vector2(220, 0); // Start after button
                textContainerRect.offsetMax = new Vector2(-20, 0); // Padding on right

                // Add TextMeshProUGUI component
                TextMeshProUGUI textMesh = textContainer.AddComponent<TextMeshProUGUI>();
                textMesh.fontSize = 40;
                textMesh.alignment = TextAlignmentOptions.Left;
                textMesh.color = Color.white;
                textMesh.textWrappingMode = TextWrappingModes.Normal;
                string text = sceneInfo.name + "\n" +
                                sceneInfo.series + " " + sceneInfo.rarity + "\n" +
                                sceneInfo.artist;
                if (sceneInfo.artist == "Unknown") {
                    text = text + " Artist";
                }
                textMesh.text = sceneInfo.name + "\n" +
                                sceneInfo.series + " " + sceneInfo.rarity + "\n" +
                                sceneInfo.artist;

                // Add VerticalLayoutGroup to text container
                // VerticalLayoutGroup verticalLayout = textContainer.AddComponent<VerticalLayoutGroup>();
                // verticalLayout.spacing = 10;
                // verticalLayout.padding = new RectOffset(10, 10, 10, 10);
                // verticalLayout.childAlignment = TextAnchor.UpperLeft;
                // verticalLayout.childForceExpandWidth = true;
                // verticalLayout.childForceExpandHeight = false;

                // // Create three rows
                // for (int row = 0; row < 3; row++) {
                //     GameObject rowContainer = new GameObject($"Row_{row}");
                //     rowContainer.transform.SetParent(textContainer.transform, false);

                //     // Add RectTransform to row
                //     RectTransform rowRect = rowContainer.AddComponent<RectTransform>();
                //     rowRect.sizeDelta = new Vector2(0, 50); // Fixed height for each row

                //     // Add HorizontalLayoutGroup to row
                //     HorizontalLayoutGroup horizontalLayout = rowContainer.AddComponent<HorizontalLayoutGroup>();
                //     horizontalLayout.spacing = 10;
                //     horizontalLayout.padding = new RectOffset(0, 0, 0, 0);
                //     horizontalLayout.childAlignment = TextAnchor.MiddleLeft;
                //     horizontalLayout.childForceExpandWidth = row == 0; // Only expand width for first row
                //     horizontalLayout.childForceExpandHeight = true;

                //     // Create two columns in each row
                //     for (int col = 0; col < 2; col++) {
                //         GameObject textBox = new GameObject($"TextBox_{row}_{col}");
                //         textBox.transform.SetParent(rowContainer.transform, false);

                //         // Add RectTransform to text box
                //         RectTransform textBoxRect = textBox.AddComponent<RectTransform>();
                //         if (row > 0) { // Fixed width for bottom two rows
                //             textBoxRect.sizeDelta = new Vector2(200, 0); // Fixed width of 200 pixels
                //         } else {
                //             textBoxRect.sizeDelta = new Vector2(0, 0); // Flexible width for first row
                //         }

                //         // Add TextMeshProUGUI component
                //         TextMeshProUGUI textMesh = textBox.AddComponent<TextMeshProUGUI>();
                //         textMesh.fontSize = 40;
                //         textMesh.alignment = TextAlignmentOptions.Left;
                //         textMesh.color = Color.white;
                //         textMesh.textWrappingMode = TextWrappingModes.Normal;

                //         // Set different text for each box based on row and column
                //         switch (row) {
                //             case 0:
                //                 textMesh.text = col == 0 ? sceneInfo.name : "";
                //                 break;
                //             case 1:
                //                 textMesh.text = col == 0 ? sceneInfo.series : sceneInfo.rarity;
                //                 break;
                //             case 2:
                //                 textMesh.text = col == 0 ? $"Supply: {sceneInfo.maxSupply}" : $"Artist: {sceneInfo.artist}";
                //                 if (col == 0) {
                //                     textMesh.fontSize = 30;
                //                 }
                //                 break;
                //         }
                //     }
                //}
                
                // Store panel reference
                panelOwnedNfts[i] = buttonContainer;
            }
        }
    }
}