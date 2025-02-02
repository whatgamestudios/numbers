// Copyright (c) Whatgame Studios 2024 - 2025
using UnityEngine;

namespace FourteenNumbers {

    /**
    * Manage data upgrades.
    */
    public class UpgradeStorage {
        // Storage version keys.
        public const string STORAGE_VERSION = "STORAGE_VERSION";

        public const int STORAGE_VERSION_0 = 0;
        public const int STORAGE_VERSION_1 = 1;


        /**
        * Check the storage version. If the app has been updated and the storage
        * layout has changed, upgrade the storage data.
        */
        public static void UpgradeStorageIfNecessary() {
            int currentStorageVersion = PlayerPrefs.GetInt(STORAGE_VERSION, STORAGE_VERSION_0);

            if (currentStorageVersion == STORAGE_VERSION_1) {
                // Latest version - do nothing.
                return;
            }

            switch (currentStorageVersion) {
                case STORAGE_VERSION_0:
                    // Upgrade from 0 to latest.
                    Debug.Log("Upgrading storage from " + STORAGE_VERSION_0 + " to " + STORAGE_VERSION_1);
                    PlayerPrefs.SetInt(STORAGE_VERSION, STORAGE_VERSION_1);

                    int back = ScreenBackground.GetBackground();
                    if (back == 1 || back == 2 || back == 5) {
                        ScreenBackground.SetBackground(3);
                    }

                    PlayerPrefs.Save();
                    break;
                default:
                    Debug.Log("Unknown storage version: " + currentStorageVersion);
                    break;            
            }
        }
    }
}