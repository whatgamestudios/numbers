// Copyright (c) Whatgame Studios 2024 - 2025
using UnityEngine;

namespace FourteenNumbers {

    /**
    * Manage data upgrades.
    */
    public class UpgradeStorage {
        // Storage version keys.
        public const string STORAGE_VERSION = "STORAGE_VERSION";

        // Old storage variables
        public const string STATS_SOLUTION1 = "STATS_SOLUTION1";
        public const string STATS_SOLUTION2 = "STATS_SOLUTION2";
        public const string STATS_SOLUTION3 = "STATS_SOLUTION3";

        public const int STORAGE_VERSION_0 = 0;
        public const int STORAGE_VERSION_1 = 1;
        public const int STORAGE_VERSION_2 = 2;


        /**
        * Check the storage version. If the app has been updated and the storage
        * layout has changed, upgrade the storage data.
        */
        public static void UpgradeStorageIfNecessary() {
            int currentStorageVersion = PlayerPrefs.GetInt(STORAGE_VERSION, STORAGE_VERSION_0);

            if (currentStorageVersion == STORAGE_VERSION_2) {
                // Latest version - do nothing.
                return;
            }

            if (currentStorageVersion == STORAGE_VERSION_0) 
            {
                AuditLog.Log("Upgrading storage from " + STORAGE_VERSION_0 + " to " + STORAGE_VERSION_1);
                SceneStore.SetBackground(SceneStore.BG_DEFAULT);
            }

            if (currentStorageVersion == STORAGE_VERSION_0 || 
                currentStorageVersion == STORAGE_VERSION_1) 
            {
                    string sol1 = PlayerPrefs.GetString(STATS_SOLUTION1, "");
                    string sol2 = PlayerPrefs.GetString(STATS_SOLUTION2, "");
                    string sol3 = PlayerPrefs.GetString(STATS_SOLUTION3, "");
                    string solution = sol1 + "=" + sol2 + "=" + sol3 + "=";
                    uint lastGameDay = Stats.GetLastGameDay();
                    Stats.SetSolution(lastGameDay, solution, 0);

                    PlayerPrefs.SetInt(STORAGE_VERSION, STORAGE_VERSION_2);
                    PlayerPrefs.Save();
            }
        }
    }
}