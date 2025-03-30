// Copyright (c) Whatgame Studios 2024 - 2025
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;
using System;

namespace FourteenNumbers {
    public class ScreenBackground {
        public const string BG_OPTION = "OPTION_BACKGROUND";

        public const int BG_DEFAULT = 1;

        public const string BG_OWNED = "BACKGROUND_OWNED";
        public const string BG_LAST_CHECKED = "BACKGROUND_LAST_CHECKED";

        // Interval between rechecking whether NFTs are owned, in hours.
        private const int RECHECK_INTERVAL = 24;

        /**
        * Set the background used by all scenes.
        */
        public static void SetBackground(int option) {
            PlayerPrefs.SetInt(BG_OPTION, option);
            PlayerPrefs.Save();
        }

        /**
        * Get the background option to be used.
        *
        * @return the background option number to use.
        */
        public static int GetBackground() {
            return PlayerPrefs.GetInt(BG_OPTION, BG_DEFAULT);
        }

        /**
         * Save the list of owned backgrounds.
         *
         * @param owned Array of owned background ids.
         */
        public static void SetOwned(int[] owned) {
            string ownedString = string.Join(",", owned);
            PlayerPrefs.SetString(BG_OWNED, ownedString);
            PlayerPrefs.Save();
        }

        /**
         * Get the list of owned backgrounds.
         * 
         * @return array of owned background ids.
         */
        public static int[] GetOwned() {
            string ownedString = PlayerPrefs.GetString(BG_OWNED, "");
            if (string.IsNullOrEmpty(ownedString)) {
                return new int[0];
            }
            return Array.ConvertAll(ownedString.Split(','), int.Parse);
        }


        /**
         * Determine whether the NFTs owned by the player has been checked recently.
         * 
         * @return true if the NFTs owned by the player should be checked.
         */
        public static bool DoINeedToCheckOwnedNfts() {
            DateTime now = DateTime.Now;
            int currentTimeInHours = ((now.Year * 12 + now.Month) * 31 + now.Day) * 24 + now.Hour;

            int lastChecked = PlayerPrefs.GetInt(BG_LAST_CHECKED, 0);
            if (currentTimeInHours > lastChecked + RECHECK_INTERVAL) {
                PlayerPrefs.SetInt(BG_LAST_CHECKED, currentTimeInHours);
                return true;
            }
            return false;
        }
    }
}