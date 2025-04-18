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

        public static (string, UnityEngine.Color, UnityEngine.Color, string) GetInfo(int option) {
            int generation = OptionToGeneration(option);
            int type = OptionToType(option);

            switch (generation) {
                case 0:
                    return getFreeInfo(type);
                case 1:
                    return getGen1Info(type);
                default:
                    Debug.Log("Unknown generation: " + generation);
                    return (null, UnityEngine.Color.white, UnityEngine.Color.black, "");
            }
        }


        public static (string, UnityEngine.Color, UnityEngine.Color, string) getFreeInfo(int type) {
            switch (type) {
                case 1:
                    return ("scenes/free/free-type1-coffee", UnityEngine.Color.white, UnityEngine.Color.black, null);
                case 2:
                    return ("scenes/free/free-type2-dogs", UnityEngine.Color.white, UnityEngine.Color.black, null);
                case 3:
                    return ("scenes/free/free-type3-koi", UnityEngine.Color.white, UnityEngine.Color.black, null);
                default:
                    Debug.Log("Unknown free type: " + type);
                    return (null, UnityEngine.Color.white, UnityEngine.Color.black, null);
            }
        }

        public static (string, UnityEngine.Color, UnityEngine.Color, string) getGen1Info(int type) {
            string description;
            switch (type) {
                case 0:
                    description = "Golden Fans\n" + 
                        "Serier: Gen1\n" +
                        "Rarity: Mythical\n" + 
                        "Max Supply: 10\n" + 
                        "Artist: Natata";
                    return ("scenes/gen1/gen1-type0-goldenfans", UnityEngine.Color.white, UnityEngine.Color.black, description);
                case 1:
                    description = "Golden Fans\n" + 
                        "Serier: Gen1\n" +
                        "Rarity: Mythical\n" + 
                        "Artist: Natata";
                    return ("scenes/gen1/gen1-type1-brightcats", UnityEngine.Color.white, UnityEngine.Color.black, description);
                case 2:
                    description = "Golden Fans\n" + 
                        "Serier: Gen1\n" +
                        "Artist: Natata";
                    return ("scenes/gen1/gen1-type2-mixedflowers", UnityEngine.Color.black, UnityEngine.Color.black, description);
                case 3:
                    description = "Golden Fans\n" + 
                        "Artist: Natata";
                    return ("scenes/gen1/gen1-type3-yellowflowers", UnityEngine.Color.black, UnityEngine.Color.black, description);
                default:
                    Debug.Log("Unknown free type: " + type);
                    return (null, UnityEngine.Color.white, UnityEngine.Color.black, "");
            }
        }
        public static (string, UnityEngine.Color, UnityEngine.Color, string) getGen2MaybeInfo(int type) {
            string description = "";
            switch (type) {
                case 1:
                case 2:
                case 3:
                    return ("scenes/gen1/gen1-type3-wetdogs", UnityEngine.Color.black, UnityEngine.Color.black, description);
                case 4:
                    return ("scenes/gen1/gen1-type4-honeycomb", UnityEngine.Color.black, UnityEngine.Color.red, description);
                case 5:
                case 6:
                    return ("scenes/gen1/gen1-type6-chineselandscape", UnityEngine.Color.black, UnityEngine.Color.black, description);
                case 7:
                case 8:
                    return ("scenes/gen1/gen1-type8-witch", UnityEngine.Color.white, UnityEngine.Color.black, description);
                case 9:
                    return ("scenes/gen1/gen1-type9-blackcat", UnityEngine.Color.white, UnityEngine.Color.black, description);
                default:
                    Debug.Log("Unknown free type: " + type);
                    return (null, UnityEngine.Color.white, UnityEngine.Color.black, description);
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