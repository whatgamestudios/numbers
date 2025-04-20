// Copyright (c) Whatgame Studios 2024 - 2025
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

namespace FourteenNumbers {
    /**
     * Screen shown when play presses on a scene in the scene selector screen, when the 
     * scene is already selected.
     */
    public class SceneDetailScreen : MonoBehaviour {

        public TextMeshProUGUI sceneName;
        public TextMeshProUGUI sceneMetadata;


        public void Start() {
            int option = ScreenBackground.GetBackground();
            SceneInfo sceneInfo = BackgroundsMetadata.GetInfo(option);
            string maxSupply = sceneInfo.maxSupply == 0 ? "Infinite" : sceneInfo.maxSupply.ToString();
            sceneName.text = sceneInfo.name;
            sceneMetadata.text = 
                sceneInfo.series + "\n" +
                sceneInfo.rarity + "\n" +
                maxSupply + "\n" + 
                sceneInfo.artist;
        }

        public async void OnButtonClick(string buttonText) {
            if (buttonText == "Back") {
                await SceneManager.UnloadSceneAsync("SceneDetailScene");
            }
            else {
                Debug.Log("SceneDetailScreen: Unknown button");
            }
        }
    }
}