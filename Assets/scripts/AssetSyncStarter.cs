// Copyright (c) Whatgame Studios 2024 - 2025
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;
using UnityEngine.SceneManagement;

namespace FourteenNumbers {

    /**
     * Kick off asset syncing if needed. 
     */
    public class AssetSyncStarter : MonoBehaviour {

        public void Start() {
            Debug.Log("AssetSync starter");
            Debug.Log("DoINeedToCheckOwnedNfts: " + SceneStore.DoINeedToCheckOwnedNfts());

            if (SceneStore.DoINeedToCheckOwnedNfts()) {
                ScreenBackground.FetchAndProcessNfts(this);
            }
        }
    }
}