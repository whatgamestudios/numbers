using UnityEngine;
using UnityEngine.UI;
using System.Collections;



public class BackgroundSelector : MonoBehaviour {

    public Button buttonOption1;
    public Button buttonOption2;
    public Button buttonOption3;
    public GameObject panelOption1;
    public GameObject panelOption2;
    public GameObject panelOption3;

    public GameObject panel;


    public void Start() {
        int selected = ScreenBackground.GetBackground();
        setSelected(selected);
    }

    public void OnButtonClick(string buttonText) {
        if (buttonText == "1") {
            ScreenBackground.SetBackground(1);
            setSelected(1);
        }
        else if (buttonText == "2") {
            ScreenBackground.SetBackground(2);
            setSelected(2);
        }
        else if (buttonText == "3") {
            ScreenBackground.SetBackground(3);
            setSelected(3);
        }
        else {
            Debug.Log("Unknown button");
        }
        ScreenBackground.SetPanelBackground(panel);
    }

    private void setSelected(int selected) {
        Image img1 = panelOption1.GetComponent<Image>();
        Image img2 = panelOption2.GetComponent<Image>();
        Image img3 = panelOption3.GetComponent<Image>();
        switch (selected) {
            case 1:
                img1.color = UnityEngine.Color.red;
                img2.color = UnityEngine.Color.black;
                img3.color = UnityEngine.Color.black;
                break;
            case 2:
                img1.color = UnityEngine.Color.black;
                img2.color = UnityEngine.Color.red;
                img3.color = UnityEngine.Color.black;
                break;
            default:
                img1.color = UnityEngine.Color.black;
                img2.color = UnityEngine.Color.black;
                img3.color = UnityEngine.Color.red;
                break;
        }
    }

}
