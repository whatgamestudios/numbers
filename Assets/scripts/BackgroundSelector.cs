// Copyright (c) Whatgame Studios 2024 - 2025
using UnityEngine;
using UnityEngine.UI;
using System.Collections;


namespace FourteenNumbers {

    public class BackgroundSelector : MonoBehaviour {
        public GameObject panelOption1;
        public GameObject panelOption2;
        public GameObject panelOption3;
        //public GameObject panelOption4;
        //public GameObject panelOption5;
        //public GameObject panelOption6;

        public GameObject panel;


        public void Start() {
            int selected = ScreenBackground.GetBackground();
            setSelected(selected);
        }

        public void OnButtonClick(string buttonText) {
            // if (buttonText == "1") {
            //     ScreenBackground.SetBackground(1);
            //     setSelected(1);
            // }
            // else if (buttonText == "2") {
            //     ScreenBackground.SetBackground(2);
            //     setSelected(2);
            // }
            if (buttonText == "free1") {
                ScreenBackground.SetBackground(1);
                setSelected(1);
            }
            else if (buttonText == "free2") {
                ScreenBackground.SetBackground(2);
                setSelected(2);
            }
            // else if (buttonText == "5") {
            //     ScreenBackground.SetBackground(5);
            //     setSelected(5);
            // }
            else if (buttonText == "free3") {
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
            //Image img4 = panelOption4.GetComponent<Image>();
            // Image img5 = panelOption5.GetComponent<Image>();
            //Image img6 = panelOption6.GetComponent<Image>();

            img1.color = UnityEngine.Color.black;
            img2.color = UnityEngine.Color.black;
            img3.color = UnityEngine.Color.black;
            //img4.color = UnityEngine.Color.black;
            // img5.color = UnityEngine.Color.black;
            //img6.color = UnityEngine.Color.black;

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
                // case 4:
                //     img4.color = UnityEngine.Color.red;
                //     break;
                // case 5:
                //     img5.color = UnityEngine.Color.red;
                //     break;
                // case 6:
                //     img6.color = UnityEngine.Color.red;
                //     break;
                default:
                    Debug.Log("Unknown option3");
                    break;
            }
        }

    }
}