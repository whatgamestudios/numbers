// Copyright (c) Whatgame Studios 2024 - 2025
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;

namespace FourteenNumbers {
    public class BackgroundsMetadata {

        public static int OptionToGeneration(int option) {
            return option / 100;
        }
        public static int OptionToType(int option) {
            return option % 100;
        }

        public static (string, UnityEngine.Color, UnityEngine.Color) GetInfo(int option) {
            int generation = OptionToGeneration(option);
            int type = OptionToType(option);

            switch (generation) {
                case 0:
                    return getFreeInfo(type);
                default:
                    Debug.Log("Unknown generation: " + generation);
                    return (null, UnityEngine.Color.white, UnityEngine.Color.black);
            }
        }


        public static (string, UnityEngine.Color, UnityEngine.Color) getFreeInfo(int type) {
            switch (type) {
                case 1:
                    return ("scenes/free/free-type1-coffee", UnityEngine.Color.white, UnityEngine.Color.black);
                case 2:
                    return ("scenes/free/free-type2-dogs", UnityEngine.Color.white, UnityEngine.Color.black);
                case 3:
                    return ("scenes/free/free-type3-koi", UnityEngine.Color.white, UnityEngine.Color.black);
                default:
                    Debug.Log("Unknown free type: " + type);
                    return (null, UnityEngine.Color.white, UnityEngine.Color.black);
            }

        }

        public static int ButtonTextToOption(string buttonText) {
            if (buttonText == "free1") {
                return 1;
            }
            else if (buttonText == "free2") {
                return 2;
            }
            else if (buttonText == "free3") {
                return 3;
            }
            else {
                // Default
                Debug.Log("Unknown button: " + buttonText);
                return 1;
            }

        }
    }
}