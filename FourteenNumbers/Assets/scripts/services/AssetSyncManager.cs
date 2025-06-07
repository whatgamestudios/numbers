// Copyright (c) Whatgame Studios 2024 - 2025
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;
using UnityEngine.SceneManagement;

namespace FourteenNumbers {
    public class AssetSyncManager : MonoBehaviour
    {
        public static AssetSyncManager Instance { get; private set; }

        public void Start()
        {
            if (Instance == null)
            {
                Instance = this;
                SyncIfNeeded();
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        public void SyncIfNeeded()
        {
            bool needToSync = SceneStore.DoINeedToCheckOwnedNfts();
            AuditLog.Log($"AssetSync: Need to sync: {needToSync}");
            if (needToSync)
            {
                ScreenBackground.FetchAndProcessNfts(this);
            }
        }

        public void SyncNow()
        {
            ScreenBackground.FetchAndProcessNfts(this);
        }
    }
}