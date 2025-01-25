// Copyright (c) Whatgame Studios 2024 - 2025
using UnityEngine;
using System.Numerics;
using System.Threading.Tasks;
using Nethereum.Web3;
using Nethereum.Contracts;
using Nethereum.Web3.Accounts;
using Nethereum.Hex.HexTypes;

namespace FourteenNumbers {

    public class FourteenNumbersContract {

        // public async Task<uint> GetBestScore(uint gameDay) {
        //     var web3 = new Web3("https://rpc.immutable.com");
        //     var contractAddress = "0xe2E762770156FfE253C49Da6E008b4bECCCf2812";
        //     var contract = web3.Eth.GetContract(NUMBERS_ABI.ABI, contractAddress);
        //     var solutionsFunction = contract.GetFunction("solutions");
            
        //     // string combinedSolution;
        //     BigInteger points;
        //     // string player;
        //     // (combinedSolution, points, player) = await solutionsFunction.CallAsync<string, BigInteger, string>(gameDay);
        //     return points.ToInt();
        // }

        private static class NUMBERS_ABI {
            public static string ABI = @"[
                {
                    'type': 'function',
                    'name': 'solutions',
                    'inputs': [
                    {
                        'name': 'gameDay',
                        'type': 'uint256',
                    }
                    ],
                    'outputs': [
                    {
                        'name': 'combinedSolution',
                        'type': 'bytes',
                    },
                    {
                        'name': 'points',
                        'type': 'uint256',
                    },
                    {
                        'name': 'player',
                        'type': 'address',
                    }
                    ],
                    'stateMutability': 'view'
                },
            ]";
        }


    }
}