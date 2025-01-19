// Copyright (c) Whatgame Studios 2024 - 2025
using UnityEngine;

public class PassportStore {
    public const string PASS_LOGGED_IN = "PASS_LOGGED_IN";


    public static bool IsLoggedIn() {
        return PlayerPrefs.GetInt(PASS_LOGGED_IN, 0) != 0;
    }

    public static void SetLoggedIn() {
        PlayerPrefs.SetInt(PASS_LOGGED_IN, 1);
        PlayerPrefs.Save();
    }
}