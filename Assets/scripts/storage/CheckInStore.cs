// Copyright (c) Whatgame Studios 2024 - 2025
using UnityEngine;

namespace FourteenNumbers {
    public class CheckInStore {
        public const string CHECKIN_DAY = "CHECKIN_DAY";


        public static bool DoINeedToCheckIn(uint gameDay) {
            int lastCheckedIn = PlayerPrefs.GetInt(CHECKIN_DAY, 0);
            if ((uint) lastCheckedIn != gameDay) {
                PlayerPrefs.SetInt(CHECKIN_DAY, (int) gameDay);
                return true;
            }
            return false;
        }
    }
}