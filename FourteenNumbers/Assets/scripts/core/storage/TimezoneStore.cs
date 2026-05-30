// Copyright (c) Whatgame Studios 2024 - 2025
using UnityEngine;
using System;

namespace FourteenNumbers {
    // Audit log to store an audit of activity in the game.
    public class TimezoneStore {
        public const string TIMEZONE = "TIMEZONE";

        public static void SetTimeZone(bool useLocal) {
            int useLocalInt = useLocal ? 0 : 1;
            PlayerPrefs.SetInt(TIMEZONE, useLocalInt);
        }

        public static bool UseLocalTimeZone() {
            // From v3 onwards, always use local time zone. 
            // int useLocalInt = PlayerPrefs.GetInt(TIMEZONE, 0);
            // return useLocalInt == 0;
            return true;
        }
    }
}