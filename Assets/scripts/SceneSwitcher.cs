using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;



public class SceneSwitcher : MonoBehaviour {

    public Button buttonPlay;
    public Button buttonStats;
    public Button buttonSettings;
    public Button buttonCredits;
    public Button buttonHelp;
    public Button buttonBack;


    public void OnButtonClick(string buttonText) {
        if (buttonText == "Play") {
            // Only specifying the sceneName or sceneBuildIndex will load the Scene with the Single mode
            SceneManager.LoadScene("GamePlayScene", LoadSceneMode.Single);
        }
        else if (buttonText == "Stats") {
            SceneManager.LoadScene("StatsScene", LoadSceneMode.Single);
        }
        else if (buttonText == "Settings") {
            SceneManager.LoadScene("SettingsScene", LoadSceneMode.Single);
        }
        else if (buttonText == "Credits") {
            SceneManager.LoadScene("CreditsScene", LoadSceneMode.Single);
        }
        else if (buttonText == "Help") {
            SceneManager.LoadScene("HelpScene", LoadSceneMode.Single);
        }
        else if (buttonText == "Menu") {
            SceneManager.LoadScene("MenuScene", LoadSceneMode.Single);
        }
        else {
            Debug.Log("Unknown button");
        }
    }
}
