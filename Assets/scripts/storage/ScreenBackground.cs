using UnityEngine;
using UnityEngine.UI;
using System.Collections;
public class ScreenBackground {
    public const string BG_OPTION = "OPTION_BACKGROUND";

    public static void SetPanelBackground(GameObject panel) {
        Image img = panel.GetComponent<Image>();
        if (img == null) {
            Debug.Log("No raw image");
            return;
        }

        Texture2D tex;
        Rect size;
        Vector2 pivot = new Vector2(0.0f, 0.0f);
        Sprite s;
        int option = PlayerPrefs.GetInt(BG_OPTION, 1);
        switch (option) {
            case 2:
                tex = Resources.Load<Texture2D>("free-background2");
                Debug.Log("tex null: " + (tex == null));
                Debug.Log("tex: " + tex.ToString());
                size = new Rect(0.0f, 0.0f, tex.width, tex.height);
                s = Sprite.Create(tex, size, pivot);
                img.sprite = s;
                Debug.Log("Setting background 2");
                break;
            case 3:
                tex = Resources.Load<Texture2D>("free-background3");
                size = new Rect(0.0f, 0.0f, tex.width, tex.height);
                s = Sprite.Create(tex, size, pivot);
                img.sprite = s;
                Debug.Log("Setting background 3");
                break;
            default:
                tex = Resources.Load<Texture2D>("free-background1");
                size = new Rect(0.0f, 0.0f, tex.width, tex.height);
                s = Sprite.Create(tex, size, pivot);
                img.sprite = s;
                Debug.Log("Setting background 1");
                break;
        }
    }

    public static void SetBackground(int option) {
        PlayerPrefs.SetInt(BG_OPTION, option);
        PlayerPrefs.Save();
    }

    public static int GetBackground() {
        return PlayerPrefs.GetInt(BG_OPTION, 1);
    }
}