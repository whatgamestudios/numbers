// Copyright (c) Whatgame Studios 2024 - 2025
using UnityEngine;

namespace FourteenNumbers {
    public class PassportStore {
        public const string PASS_LOGGED_IN = "PASS_LOGGED_IN";


        public static bool IsLoggedIn() {
            return PlayerPrefs.GetInt(PASS_LOGGED_IN, 0) != 0;
        }

        public static void SetLoggedIn(bool isLoggedIn) {
            if (isLoggedIn) {
                PlayerPrefs.SetInt(PASS_LOGGED_IN, 1);
            }
            else {
                PlayerPrefs.SetInt(PASS_LOGGED_IN, 0);
            }
            PlayerPrefs.Save();
        }
    }
}