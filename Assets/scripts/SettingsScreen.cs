// Copyright (c) Whatgame Studios 2024 - 2025
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;


namespace FourteenNumbers {

    public class SettingsScreen : MonoBehaviour {

        public Button LocalTimezoneButton;
        public Button KiribatiTimezoneButton;

        public void Start() {
            AuditLog.Log("Settings screen");
            if (TimezoneStore.UseLocalTimeZone()) {
                LocalTimezoneButton.GetComponent<Image>().sprite = Resources.Load<Sprite>("buttons/radio_selected");
                KiribatiTimezoneButton.GetComponent<Image>().sprite = Resources.Load<Sprite>("buttons/radio-not-selected");
                LocalTimezoneButton.interactable = false;
                KiribatiTimezoneButton.interactable = true;
            }
            else {
                LocalTimezoneButton.GetComponent<Image>().sprite = Resources.Load<Sprite>("buttons/radio-not-selected");
                KiribatiTimezoneButton.GetComponent<Image>().sprite = Resources.Load<Sprite>("buttons/radio_selected");
                LocalTimezoneButton.interactable = true;
                KiribatiTimezoneButton.interactable = false;
            }
        }

        public void OnButtonClick(string buttonText) {
            if (buttonText == "ShareLogs") {
                string msg = AuditLog.GetLogs();
                SunShineNativeShare.instance.ShareText(msg, msg);
            }
            else if (buttonText == "Local" || buttonText == "Kiribati") {
                if (buttonText == "Local") {
                    AuditLog.Log("Settings: Set timezone to local");
                    TimezoneStore.SetTimeZone(true);
                    LocalTimezoneButton.GetComponent<Image>().sprite = Resources.Load<Sprite>("buttons/radio_selected");
                    KiribatiTimezoneButton.GetComponent<Image>().sprite = Resources.Load<Sprite>("buttons/radio-not-selected");
                    LocalTimezoneButton.interactable = false;
                    KiribatiTimezoneButton.interactable = true;
                }
                else {
                    AuditLog.Log("Settings: Set timezone to Kiribati");
                    TimezoneStore.SetTimeZone(false);
                    LocalTimezoneButton.GetComponent<Image>().sprite = Resources.Load<Sprite>("buttons/radio-not-selected");
                    KiribatiTimezoneButton.GetComponent<Image>().sprite = Resources.Load<Sprite>("buttons/radio_selected");
                    KiribatiTimezoneButton.interactable = false;
                    LocalTimezoneButton.interactable = true;
                }
                string now = Timeline.TimeOfDayStr();
                MessagePass.SetErrorMsg("Time of day now\n" + now);
                SceneManager.LoadScene("ErrorScene", LoadSceneMode.Additive);
            }
            else {
                Debug.Log("Settings: Unknown button: " + buttonText);
            }
        }
    }
}