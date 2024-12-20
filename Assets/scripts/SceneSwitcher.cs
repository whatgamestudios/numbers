using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;



public class SceneSwitcher : MonoBehaviour {

    public Button buttonPlay;
    public Button buttonStats;
    public Button buttonDebug;

    public TextMeshProUGUI dbg;


    void Start() {
        dbg.text = "start";
    }


    public void OnButtonClick(string buttonText) {
        dbg.text = buttonText;
        if (buttonText == "Play") {
            // Only specifying the sceneName or sceneBuildIndex will load the Scene with the Single mode
            SceneManager.LoadScene("GamePlayScene", LoadSceneMode.Single);
        }
        else if (buttonText == "Stats") {
            Debug.Log("Stats not supported yet");
        }
        else if (buttonText == "Debug") {
            Debug.Log("Debug not supported yet");
        }
        else {
            Debug.Log("Unknown button");
        }
    }


    // void Update() {


    // }
}
