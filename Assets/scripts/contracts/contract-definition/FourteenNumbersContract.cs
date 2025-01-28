// Copyright (c) Whatgame Studios 2024 - 2025
using UnityEngine;
using System;
using System.Collections;
using System.Numerics;
using System.Threading.Tasks;
using System.Text;

using Nethereum.Web3;
using Nethereum.Contracts;
using Nethereum.Web3.Accounts;
using Nethereum.Hex.HexTypes;
using Nethereum.ABI.Encoders;
using Nethereum.ABI;


namespace FourteenNumbers {

    public class FourteenNumbersContract : MonoBehaviour {
        private FourteenNumbersSolutionsService service;

        private static string contractAddress = "0xe2E762770156FfE253C49Da6E008b4bECCCf2812";


        public uint lastTransactionStatus = 0;

        public FourteenNumbersContract() {
            var web3 = new Web3("https://rpc.immutable.com/");
            service = new FourteenNumbersSolutionsService(web3, contractAddress);
        }


        public async Task<uint> GetBestScore(uint gameDay) {
            SolutionsOutputDTO solution = await service.SolutionsQueryAsync(gameDay);
            BigInteger bestScoreBigInt = solution.Points;
            if (bestScoreBigInt < 0 || bestScoreBigInt > uint.MaxValue) {
                Debug.LogError($"Number {bestScoreBigInt} is outside uint range");
                // Use 7 to indicate an error.
                return 7;
            }
            else {
                return (uint) solution.Points;
            }
        }


        public async void SubmitBestScore(uint gameDay, string sol1, string sol2, string sol3) {
            lastTransactionStatus = 0;         

            // StoreResultsFunction func = new StoreResultsFunction() {
            //     GameDay = gameDay,
            //     Sol1 = new ABIValue("bytes", sol1),
            //     Sol2 = sol2,
            //     Sol3 = sol3,
            //     Store = false,
            // };

            // byte[] abiEncoding = func.GetData();

            // TransactionReceiptResponse response = 
            //     await Passport.Instance.ZkEvmSendTransactionWithConfirmation(
            //         new TransactionRequest() {
            //             to = contractAddress,
            //             data = abiEncoding,
            //             value = "0"
            //         }
            //     );
            // Debug.Log($"Transaction hash: {response.transactionHash}");

            // if (response.status != "1") {
            //     lastTransactionStatus = 1;
            //     return;
            // }

            // lastTransactionStatus = 2;            
        }

    }
}



