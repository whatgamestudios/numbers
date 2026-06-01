// Copyright (c) Whatgame Studios 2024 - 2026
using System;
using System.Threading.Tasks;

using Newtonsoft.Json.Linq;

namespace FourteenNumbers {

    public class SolutionSubmitResult {
        public string Status;
        public int Score;
        public int BestScore;
        public int Result1;
        public int Result2;
        public int Result3;
    }

    public class SolutionEntry {
        public string UserId;
        public string Part1;
        public string Part2;
        public string Part3;
        public int Result1;
        public int Result2;
        public int Result3;
        public int Score;
    }

    public class SolutionResultsResult {
        public int GameDay;
        public int? BestScore;
        public SolutionEntry[] Solutions;
    }

    public class SolutionProcessor {

        /// <summary>
        /// Submit a three-part solution for a game day.
        /// Returns status: "submitted", "not_competitive", or "duplicate".
        /// Higher score is better; 210 is optimal.
        /// </summary>
        public async Task<SolutionSubmitResult> Submit(int gameDay, string userId, string part1, string part2, string part3) {
            var parameters = new JObject {
                ["game_day"] = gameDay,
                ["user_id"] = userId,
                ["part1"] = part1,
                ["part2"] = part2,
                ["part3"] = part3
            };
            JObject result = await ServerClient.SendAsync("solution.submit", parameters);
            return new SolutionSubmitResult {
                Status = (string)result["status"],
                Score = (int)result["score"],
                BestScore = (int)result["best_score"],
                Result1 = (int)result["result1"],
                Result2 = (int)result["result2"],
                Result3 = (int)result["result3"]
            };
        }

        /// <summary>
        /// Return the best score and all top-scoring submissions for a game day (up to 20).
        /// BestScore is null when no submissions exist.
        /// </summary>
        public async Task<SolutionResultsResult> GetResults(int gameDay) {
            var parameters = new JObject { ["game_day"] = gameDay };
            JObject result = await ServerClient.SendAsync("solution.results", parameters);

            JToken bestScoreToken = result["best_score"];
            int? bestScore = bestScoreToken?.Type == JTokenType.Null ? null : (int?)bestScoreToken;

            var solutionsArray = result["solutions"] as JArray ?? new JArray();
            var solutions = new SolutionEntry[solutionsArray.Count];
            for (int i = 0; i < solutionsArray.Count; i++) {
                JObject s = (JObject)solutionsArray[i];
                solutions[i] = new SolutionEntry {
                    UserId = (string)s["user_id"],
                    Part1 = (string)s["part1"],
                    Part2 = (string)s["part2"],
                    Part3 = (string)s["part3"],
                    Result1 = (int)s["result1"],
                    Result2 = (int)s["result2"],
                    Result3 = (int)s["result3"],
                    Score = (int)s["score"]
                };
            }

            return new SolutionResultsResult {
                GameDay = (int)result["game_day"],
                BestScore = bestScore,
                Solutions = solutions
            };
        }
    }
}
