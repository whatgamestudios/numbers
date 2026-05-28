// Copyright (c) Whatgame Studios 2024 - 2025
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;


namespace FourteenNumbers {

    public class SettingsScreen : MonoBehaviour {

        public void Start() {
            AuditLog.Log("Settings screen");
        }

        public void OnButtonClick(string buttonText) {
            if (buttonText == "ShareLogs") {
                string msg = AuditLog.GetLogs();
                SunShineNativeShare.instance.ShareText(msg, msg);
            }
            else if (buttonText == "Delete") {
                SceneStack.Instance().PushScene();
                SceneManager.LoadScene("DeleteScene", LoadSceneMode.Single);
            }
            else
            {
                AuditLog.Log("Settings: Unknown button: " + buttonText);
            }
        }
    }
}