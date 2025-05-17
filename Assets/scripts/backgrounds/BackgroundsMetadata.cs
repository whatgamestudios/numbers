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
                case 2:
                    return getGen2Info(type);
                default:
                    Debug.Log("Unknown generation: " + generation);
                    return getDefaultInfo();
            }
        }

        public static int[] GetAllOwnedNftIds() {
            return new int[5] { 200, 201, 202, 203, 204};
//            return new int[9] { 100, 101, 102, 103, 200, 201, 202, 203, 204};
//            return new int[4] { 100, 101, 102, 103};
        }


        private static SceneInfo getDefaultInfo() {
            SceneInfo info = new SceneInfo("scenes/free/free-type1-coffee", UnityEngine.Color.white, UnityEngine.Color.black);
            info.SetMetadata("Coffee Beans", "Free", "Common", 0, "Frendy Adew");
            return info;
        }


        public static SceneInfo getFreeInfo(int type) {
            SceneInfo info;
            switch (type) {
                case 1:
                    info = getDefaultInfo();
                    break;
                case 2:
                    info = new SceneInfo("scenes/free/free-type2-dogs", UnityEngine.Color.white, UnityEngine.Color.black);
                    info.SetMetadata("Happy Dogs", "Free", "Common", 0, "Anna Allakuz");
                    break;
                case 3:
                    info = new SceneInfo("scenes/free/free-type3-koi", UnityEngine.Color.white, UnityEngine.Color.black);
                    info.SetMetadata("Koi", "Free", "Common", 0, "Olga Graholskaya");
                    break;
                default:
                    Debug.Log("BackgroundMetadata: Unknown free type: " + type);
                    info = getDefaultInfo();
                    break;
            }
            return info;
        }

        public static SceneInfo getGen1Info(int type) {
            SceneInfo info;
            switch (type) {
                case 0:
                    info = new SceneInfo("scenes/gen1/gen1-type0-goldenfans", UnityEngine.Color.white, UnityEngine.Color.black);
                    info.SetMetadata("Golden Fans", "Gen1", "Mythical", 10, "Natata");
                    break;
                case 1:
                    info = new SceneInfo("scenes/gen1/gen1-type1-brightcats", UnityEngine.Color.white, UnityEngine.Color.black);
                    info.SetMetadata("Bright Cats", "Gen1", "Legendary", 25, "Burbura");
                    break;
                case 2:
                    info = new SceneInfo("scenes/gen1/gen1-type2-mixedflowers", UnityEngine.Color.black, UnityEngine.Color.black);
                    info.SetMetadata("Fantasy Flowers", "Gen1", "Epic", 100, "Elen Lane");
                    break;
                case 3:
                    info = new SceneInfo("scenes/gen1/gen1-type3-yellowflowers", UnityEngine.Color.black, UnityEngine.Color.black);
                    info.SetMetadata("Yellow Flowers", "Gen1", "Common", 1000, "Unknown");
                    break;
                default:
                    Debug.Log("Unknown gen1 type: " + type);
                    info = getDefaultInfo();
                    break;
            }
            return info;
        }

        public static SceneInfo getGen2Info(int type) {
            SceneInfo info;
            switch (type) {
                case 0:
                    info = new SceneInfo("scenes/gen2/gen2-type0-clocks", UnityEngine.Color.white, UnityEngine.Color.black);
                    info.SetMetadata("Clocks", "Gen2", "Mythical", 10, "Natata");
                    break;
                case 1:
                    info = new SceneInfo("scenes/gen2/gen2-type1-circuit", UnityEngine.Color.white, UnityEngine.Color.black);
                    info.SetMetadata("Circuit1", "Gen2", "Legendary", 25, "Burbura");
                    break;
                case 2:
                    info = new SceneInfo("scenes/gen2/gen2-type2-circuit", UnityEngine.Color.black, UnityEngine.Color.black);
                    info.SetMetadata("Circuit2", "Gen2", "Epic", 100, "Elen Lane");
                    break;
                case 3:
                    info = new SceneInfo("scenes/gen2/gen2-type3-cats", UnityEngine.Color.black, UnityEngine.Color.black);
                    info.SetMetadata("Cats", "Gen2", "Common", 1000, "Unknown");
                    break;
                case 4:
                    info = new SceneInfo("scenes/gen2/gen2-type4-butterflies", UnityEngine.Color.black, UnityEngine.Color.black);
                    info.SetMetadata("Butterflies", "Gen2", "Common", 1000, "Unknown");
                    break;
                default:
                    Debug.Log("Unknown gen1 type: " + type);
                    info = getDefaultInfo();
                    break;
            }
            return info;
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
            else if (buttonText == "gen1_0") {
                return 100;
            }
            else if (buttonText == "gen1_1") {
                return 101;
            }
            else if (buttonText == "gen1_2") {
                return 102;
            }
            else if (buttonText == "gen1_3") {
                return 103;
            }
            else if (buttonText == "gen2_0") {
                return 200;
            }
            else if (buttonText == "gen2_1") {
                return 201;
            }
            else if (buttonText == "gen2_2") {
                return 202;
            }
            else if (buttonText == "gen2_3") {
                return 203;
            }
            else if (buttonText == "gen2_4") {
                return 204;
            }
            else {
                // Default
                AuditLog.Log("Background Meta: Unknown button: " + buttonText);
                return 1;
            }

        }
    }
}