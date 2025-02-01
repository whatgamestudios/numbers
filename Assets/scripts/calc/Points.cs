// Copyright Whatgame Studios 2024 - 2025
namespace FourteenNumbers {

    public class Points {
        private const int MAX_POINTS_PER_ATTEMPT = 50;
        private const int BONUS_POINTS = 20;

        public static uint CalcPoints(uint result, uint target) {
            int howFarOff = (int) (target - result);
            if (howFarOff < 0) {
                howFarOff = -howFarOff;
            }

            int points = MAX_POINTS_PER_ATTEMPT - howFarOff;
            if (points < 0) {
                points = 0;
            }
            if (howFarOff == 0) {
                points = MAX_POINTS_PER_ATTEMPT + BONUS_POINTS;
            }

            return (uint) points;
        }

    }
}