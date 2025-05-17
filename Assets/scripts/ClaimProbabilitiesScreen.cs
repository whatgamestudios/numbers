// Copyright (c) Whatgame Studios 2024 - 2025
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;
using UnityEngine.SceneManagement;

namespace FourteenNumbers {

    public class ClaimProbabilitiesScreen : MonoBehaviour {
        public GameObject panelOwned;

        private GameObject[] panelAvailableNfts;
        private ScrollRect scrollRect;

        private FourteenNumbersClaimContract fourteenNumbersClaimContracts = new FourteenNumbersClaimContract();

        private Coroutine loadRoutineDaysPlayed;

        public void Start() {
            AuditLog.Log("Claim Probabilities screen");
            // Get the ScrollRect component
            scrollRect = panelOwned.GetComponentInParent<ScrollRect>();
            if (scrollRect == null) {
                Debug.LogError("No ScrollRect found in parent of panelOwned");
                return;
            }

            drawAvailablePanel();
        }

        public void OnEnable() {
            StartLoaders();
        }

        public void OnDisable() {
        }

        public void OnButtonClick(string buttonText) {
            if (buttonText == "Back") {
                SceneManager.LoadScene("BackgroundsScene", LoadSceneMode.Single);
            }
            else {
                AuditLog.Log($"Claim Probabilities Screen: Unknown button: {buttonText}");
            }
        }

        private void drawAvailablePanel() {
            // Get the owned backgrounds
            int[] owned = ScreenBackground.GetOwned();
            
            // Clear existing panels if any
            if (panelAvailableNfts != null) {
                foreach (GameObject panel in panelAvailableNfts) {
                    Destroy(panel);
                }
            }
            
            // Reset scroll position to top
            if (scrollRect != null) {
                scrollRect.normalizedPosition = new Vector2(0, 1);
            }
            
            // Resize panelOwned based on number of NFTs
            RectTransform panelOwnedRect = panelOwned.GetComponent<RectTransform>();
            int topOfScreen = 220 + 20;
            float panelHeight = topOfScreen + (owned.Length * 220) + 20; // 225 initial offset + (220 per panel) + 20 padding at bottom
            panelOwnedRect.sizeDelta = new Vector2(panelOwnedRect.sizeDelta.x, panelHeight);
            
            // Create new array for panels
            panelAvailableNfts = new GameObject[owned.Length];
            
            // Create panels for each owned background
            for (int i = 0; i < owned.Length; i++) {
                SceneInfo sceneInfo = BackgroundsMetadata.GetInfo(owned[i]);
                if (sceneInfo.resource == null) {
                    Debug.Log("No resource found for NFT id: " + owned[i]);
                    continue;
                }

                // Create outer container panel
                GameObject holderContainer = new GameObject();
                holderContainer.transform.SetParent(panelOwned.transform, false);

                // Add RectTransform to outer container
                RectTransform buttonRect = holderContainer.AddComponent<RectTransform>();
                buttonRect.anchorMin = new Vector2(0.5f, 1f);
                buttonRect.anchorMax = new Vector2(0.5f, 1f);
                buttonRect.anchoredPosition = new Vector2(0, -topOfScreen - (i * 220));
                buttonRect.sizeDelta = new Vector2(750, 210); // Width of parent less a bit
 
                // Add Image component for background
                Image buttonImage = holderContainer.AddComponent<Image>();
                buttonImage.color = Color.black;
                buttonImage.sprite = Resources.Load<Sprite>("UI/Sprites/UI_Background");

                // Create button container (left side)
                GameObject imageContainer = new GameObject($"imageContainer_{i}");
                imageContainer.transform.SetParent(holderContainer.transform, false);

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
                                
                // Create text container (right side)
                GameObject textContainer = new GameObject($"TextContainer_{i}");
                textContainer.transform.SetParent(holderContainer.transform, false);

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
                textMesh.text = text;
                
                // Store panel reference
                panelAvailableNfts[i] = holderContainer;
            }
        }

        private void StartLoaders() {
            // Only load the days played once per day.
            // uint statsGameDay = (uint) Stats.GetLastGameDay();
            // uint gameDay = (uint) Timeline.GameDay();
            // if (statsGameDay == gameDay) {
            //     int daysPlayed = Stats.GetDaysPlayed();
            //     int daysClaimed = Stats.GetDaysClaimed();
            //     daysPlayedLoaded(daysPlayed, daysClaimed);
            // }
            // else {
                AuditLog.Log("Scene Screen: Loading days played and days claimed");
                loadRoutineDaysPlayed = StartCoroutine(LoadRoutineDaysPlayed());
            // }
        }

        IEnumerator LoadRoutineDaysPlayed() {
            FetchDaysPlayed();
            yield return new WaitForSeconds(0.1f);
        }

        private async void FetchDaysPlayed() {
            // string account = PassportStore.GetPassportAccount();
            // if (account == null || account == "") {
            //     return;
            // }
            // DaysPlayed = (int) (await fourteenNumbersContracts.GetDaysPlayed(account));
            // Stats.SetDaysPlayed(DaysPlayed);
            // if (DaysClaimed != NOT_SET) {
            //     daysPlayedLoaded(DaysPlayed, DaysClaimed);
            // } 
            // else {
            //     AuditLog.Log("DaysPlayed loaded before DaysClaimed");
            // }
        }

        // private async void FetchDaysClaimed() {
        //     string account = PassportStore.GetPassportAccount();
        //     DaysClaimed = (int) (await fourteenNumbersClaimContracts.GetDaysClaimed(account));
        //     Stats.SetDaysClaimed(DaysClaimed);
        //     if (DaysPlayed != NOT_SET) {
        //         daysPlayedLoaded(DaysPlayed, DaysClaimed);
        //     } 
        //     else {
        //         AuditLog.Log("DaysClaimed loaded before DaysPlayed");
        //     }
        // }

        // public void daysPlayedLoaded(int daysPlayed, int daysClaimed) {
        //     AuditLog.Log("Days played: " + daysPlayed.ToString() + ", days claimed: " + daysClaimed.ToString());
        //     if (daysClaimed > daysPlayed) {
        //         AuditLog.Log("Days claimed greater than days played");
        //     }
        //     int net = daysPlayed - daysClaimed;

        //     if (net >= 30) {
        //         claimButton.interactable = true;
        //     }
        //     else {
        //         int daysBeforeClaim = 30 - net;
        //         claimButtonText.text = daysBeforeClaim.ToString() + " days until claim";
        //         claimButtonText.fontSize = 60;
        //     }
    }
}