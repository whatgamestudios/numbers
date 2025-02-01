// Copyright Whatgame Studios 2024 - 2025
using UnityEngine;

namespace FourteenNumbers {

    public class TargetValue {
        private const int MAX_POINTS_PER_ATTEMPT = 50;
        private const int BONUS_POINTS = 20;

        public static uint GetTarget(uint gameDay) {
            return getTarget(gameDay, 250, 1000);
        }

        private static uint getTarget(uint gameDay, uint low, uint high) {
            byte[] seed = SeedGen.GenerateSeed(gameDay, 0, 0);
            uint count = 0;
            uint val = 0;
            do {
                val = SeedGen.GetNextValue(seed, count++, high);
                Debug.Log("Seed value: count: " + count + ", val: " + val);
            } while (val < low);
            return val;
        }
    }
}



