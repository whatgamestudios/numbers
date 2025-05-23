// Copyright (c) Whatgame Studios 2024 - 2025
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using Immutable.Passport;
using System.Threading.Tasks;

namespace FourteenNumbers {
    /**
     * Screen shown when play presses on a scene in the scene selector screen, when the 
     * scene is already selected.
     */
    public class CheatCodeScreen : MonoBehaviour {

        public TextMeshProUGUI cheatCodeText;

        private string code;


        public void Start() {
            cheatCodeText.text = "";
            code = "";
        }

        public async void OnButtonClick(string buttonText) {
            if (buttonText == "A") {
                code = code + "A";
            }
            else if (buttonText == "B") {
                code = code + "B";
            } 
            else if (buttonText == "C") {
                code = code + "C";
            }
            else if (buttonText == "D") {
                code = code + "D";
            }
            else if (buttonText == "E") {
                code = code + "E";
            }
            else if (buttonText == "F") {
                code = code + "F";
            }
            else if (buttonText == "G") {
                code = code + "G";
            }
            else if (buttonText == "H") {
                code = code + "H";
            }
            else if (buttonText == "I") {
                code = code + "I";
            }
            else if (buttonText == "J") {
                code = code + "J";
            }
            else if (buttonText == "CheatCode") {
                AuditLog.Log("CheatCode: " + code);
                await Task.Run(() => {
                    executeCheatCode(code);
                });
                code = "";
            }
            else {
                AuditLog.Log("CheatCode: Unknown button: " + buttonText);
            }

            cheatCodeText.text = translate(code);
        }


        private string translate(string cheatCode) {
            string output = cheatCode;
            output = output.Replace("A", "§");
            output = output.Replace("B", "¿");
            output = output.Replace("C", "Ø");
            output = output.Replace("D", "ŋ");
            output = output.Replace("E", "Ƅ");
            output = output.Replace('F', 'Ɛ');
            output = output.Replace('G', 'Ƥ');
            output = output.Replace('H', 'Ƨ');
            output = output.Replace('I', 'Ɇ');
            output = output.Replace('J', 'Ɯ');
            return output;
        }

        private async void executeCheatCode(string cheatCode) {
            AuditLog.Log("CheatCode: " + cheatCode);
            if (cheatCode == "AAAA") {
                AuditLog.Log("CheatCode: Logout start");
                await Passport.Instance.Logout();
                PassportStore.SetLoggedIn(false);
                AuditLog.Log("CheatCode: Logout done");
                SceneManager.LoadScene("LoginScene", LoadSceneMode.Single);
            }
            else if (cheatCode == "BBBB") {
                AuditLog.Log("CheatCode: FakeOwnership start");
                ScreenBackground.FakeOwnership = true;
            }
            else if (cheatCode == "CCCC") {
                AuditLog.Log("CheatCode: FakeOwnership stop");
                ScreenBackground.FakeOwnership = false;
            }

        }


    }
}