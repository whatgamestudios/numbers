// Copyright (c) Whatgame Studios 2024 - 2025
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;

namespace FourteenNumbers {

    public class SceneStack
    {
        private Stack<int> sceneStack = new Stack<int>();

        private static SceneStack instance;

        public static SceneStack Instance() {
            if (instance == null)
            {
                instance = new SceneStack();
            }
            return instance;
        }


        public void PushScene() {
            int sceneId = SceneManager.GetActiveScene().buildIndex;
            //AuditLog.Log($"Push: {sceneId}");
            sceneStack.Push(sceneId);
        }
        public void Reset() {
            //AuditLog.Log($"Resetting scene stack");
            while (sceneStack.Count > 0) {
                sceneStack.Pop();
            }
        }

        public int PopScene() {
            if (sceneStack.Count > 0)
            {
                return sceneStack.Pop();
            }
            else
            {
                return 2; // Menu scene
            }
        }
    }
}