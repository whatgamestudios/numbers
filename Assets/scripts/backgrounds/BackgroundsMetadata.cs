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

        public static SceneInfo GetInfo(int option) {
            int generation = OptionToGeneration(option);
            int type = OptionToType(option);

            switch (generation) {
                case 0:
                    return getFreeInfo(type);
                case 1:
                    return getGen1Info(type);
                default:
                    Debug.Log("Unknown generation: " + generation);
                    return new SceneInfo("scenes/free/free-type1-coffee", UnityEngine.Color.white, UnityEngine.Color.black);
            }
        }


        public static SceneInfo getFreeInfo(int type) {
            switch (type) {
                case 1:
                    return new SceneInfo("scenes/free/free-type1-coffee", UnityEngine.Color.white, UnityEngine.Color.black);
                case 2:
                    return new SceneInfo("scenes/free/free-type2-dogs", UnityEngine.Color.white, UnityEngine.Color.black);
                case 3:
                    return new SceneInfo("scenes/free/free-type3-koi", UnityEngine.Color.white, UnityEngine.Color.black);
                default:
                    Debug.Log("BackgroundMetadata: Unknown free type: " + type);
                    return new SceneInfo("scenes/free/free-type1-coffee", UnityEngine.Color.white, UnityEngine.Color.black);
            }
        }

        public static SceneInfo getGen1Info(int type) {
            SceneInfo info;
            switch (type) {
                case 0:
                    info = new SceneInfo("scenes/gen1/gen1-type0-goldenfans", UnityEngine.Color.white, UnityEngine.Color.black);
                    info.SetMetadata("Golden Fans", "Gen1", "Mythical", 10, "Natata");
                    return info;
                case 1:
                    info = new SceneInfo("scenes/gen1/gen1-type1-brightcats", UnityEngine.Color.white, UnityEngine.Color.black);
                    info.SetMetadata("Bright Cats", "Gen1", "Legendary", 25, "Burbura");
                    return info;
                case 2:
                    info = new SceneInfo("scenes/gen1/gen1-type2-mixedflowers", UnityEngine.Color.black, UnityEngine.Color.black);
                    info.SetMetadata("Fantasy Flowers", "Gen1", "Epic", 100, "Elen Lane");
                    return info;
                case 3:
                    info = new SceneInfo("scenes/gen1/gen1-type3-yellowflowers", UnityEngine.Color.black, UnityEngine.Color.black);
                    info.SetMetadata("Yellow Flowers", "Gen1", "Common", 1000, "Unknown");
                    return info;
                default:
                    Debug.Log("Unknown gen1 type: " + type);
                    return new SceneInfo("scenes/free/free-type1-coffee", UnityEngine.Color.white, UnityEngine.Color.black);
            }
        }
        public static SceneInfo getGen2MaybeInfo(int type) {
            switch (type) {
                case 1:
                case 2:
                case 3:
                    return new SceneInfo("scenes/gen2/gen2-type3-wetdogs", UnityEngine.Color.black, UnityEngine.Color.black);
                case 4:
                    return new SceneInfo("scenes/gen2/gen2-type4-honeycomb", UnityEngine.Color.black, UnityEngine.Color.red);
                case 5:
                case 6:
                    return new SceneInfo("scenes/gen2/gen2-type6-chineselandscape", UnityEngine.Color.black, UnityEngine.Color.black);
                case 7:
                case 8:
                    return new SceneInfo("scenes/gen2/gen2-type8-witch", UnityEngine.Color.white, UnityEngine.Color.black);
                case 9:
                    return new SceneInfo("scenes/gen2/gen2-type9-blackcat", UnityEngine.Color.white, UnityEngine.Color.black);
                default:
                    Debug.Log("Unknown gen2 type: " + type);
                    return new SceneInfo("scenes/free/free-type1-coffee", UnityEngine.Color.white, UnityEngine.Color.black);
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
            else if (buttonText == "gen1_100") {
                return 100;
            }
            else if (buttonText == "gen1_101") {
                return 101;
            }
            else if (buttonText == "gen1_102") {
                return 102;
            }
            else if (buttonText == "gen1_103") {
                return 103;
            }
            // else if (buttonText == "gen1_105") {
            //     return 105;
            // }
            // else if (buttonText == "gen1_106") {
            //     return 106;
            // }
            // else if (buttonText == "gen1_107") {
            //     return 107;
            // }
            // else if (buttonText == "gen1_108") {
            //     return 108;
            // }
            // else if (buttonText == "gen1_109") {
            //     return 109;
            // }
            else {
                // Default
                Debug.Log("Unknown button: " + buttonText);
                return 1;
            }

        }
    }
}