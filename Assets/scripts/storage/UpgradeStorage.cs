// Copyright (c) Whatgame Studios 2024 - 2025
using UnityEngine;

/**
 * Manage data upgrades.
 */
public class UpgradeStorage {
    // Storage version keys.
    public const string STORAGE_VERSION = "STORAGE_VERSION";

    public const int STORAGE_VERSION_0 = 0;
    public const int STORAGE_VERSION_1 = 0;


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
                Debug.Log("Upgrading storage from " + STORAGE_VERSION_0 + " to " + STORAGE_VERSION);
                PlayerPrefs.SetInt(STORAGE_VERSION, STORAGE_VERSION_1);
                PlayerPrefs.Save();
                break;
            default:
                Debug.Log("Unknown storage version: " + currentStorageVersion);
                break;            
        }
    }
}