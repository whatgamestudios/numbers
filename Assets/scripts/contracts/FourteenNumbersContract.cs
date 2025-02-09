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

using Immutable.Passport;
using Immutable.Passport.Model;


namespace FourteenNumbers {

    public class FourteenNumbersContract {
        public enum TransactionStatus {
            Init = 0,
            Success = 1,
            Failed = 2
        }

        private FourteenNumbersSolutionsService service;

        private static string contractAddress = "0xe2E762770156FfE253C49Da6E008b4bECCCf2812";


        public static TransactionStatus LastTransactionStatus = TransactionStatus.Init;

        public FourteenNumbersContract() {
            var web3 = new Web3("https://rpc.immutable.com/");
            service = new FourteenNumbersSolutionsService(web3, contractAddress);
        }


        public async Task<uint> GetBestScore(uint gameDay) {
            SolutionsOutputDTO solution;
            solution = await service.SolutionsQueryAsync(gameDay);
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

        public async Task<SolutionsOutputDTO> GetSolution(uint gameDay) {
            return await service.SolutionsQueryAsync(gameDay);
        }



        public async void SubmitBestScore(uint gameDay, string sol1, string sol2, string sol3) {
            LastTransactionStatus = TransactionStatus.Init;         

            StoreResultsFunction func = new StoreResultsFunction() {
                GameDay = gameDay,
                Sol1 = Encoding.UTF8.GetBytes(sol1),
                Sol2 = Encoding.UTF8.GetBytes(sol2),
                Sol3 = Encoding.UTF8.GetBytes(sol3),
                Store = false,
            };

            byte[] abiEncoding = func.GetCallData();
            Debug.Log("Publish: " + HexDump.Dump(abiEncoding));

            TransactionReceiptResponse response = 
                await Passport.Instance.ZkEvmSendTransactionWithConfirmation(
                    new TransactionRequest() {
                        to = contractAddress,
                        data = "0x" + BitConverter.ToString(abiEncoding).Replace("-", "").ToLower(),
                        value = "0"
                    }
                );
            Debug.Log($"Transaction hash: {response.transactionHash}");

            if (response.status != "1") {
                LastTransactionStatus = TransactionStatus.Failed;
            }
            else {
                LastTransactionStatus = TransactionStatus.Success;
            }
        }
    }
}



