// Copyright (c) Whatgame Studios 2024 - 2025
using UnityEngine;
using System;
using System.Numerics;
using System.Threading.Tasks;
using Nethereum.Web3;
using Nethereum.Contracts;
using Nethereum.Web3.Accounts;
using Nethereum.Hex.HexTypes;
using Nethereum.ABI.Encoders;
using Nethereum.ABI;

using Immutable.Passport;
using Immutable.Passport.Model;

namespace FourteenNumbers {

    public class FourteenNumbersClaimContract {
        private FourteenNumbersClaimService service;
        private static string contractAddress = "0xb427336d725943BA4300EEC219587E207ad21146";

        public FourteenNumbersClaimContract() {
            var web3 = new Web3("https://rpc.immutable.com/");
            service = new FourteenNumbersClaimService(web3, contractAddress);
        }

        public async Task<uint> GetDaysClaimed(string address) {
            ClaimedDayOutputDTO claimedDaysObj = await service.ClaimedDayQueryAsync(address);
            BigInteger daysClaimedInt = claimedDaysObj.DaysClaimed;
            if (daysClaimedInt < 0 || daysClaimedInt > uint.MaxValue) {
                Debug.LogError($"DaysClaimedInt {daysClaimedInt} is outside uint range");
                // Use 7 to indicate an error.
                return 7;
            }
            else {
                return (uint) claimedDaysObj.DaysClaimed;
            }
        }

        public async void Claim() {
            var func = new ClaimFunction() {};

            byte[] abiEncoding = func.GetCallData();
            Debug.Log("Claim: " + HexDump.Dump(abiEncoding));

            TransactionReceiptResponse response = 
                await Passport.Instance.ZkEvmSendTransactionWithConfirmation(
                    new TransactionRequest() {
                        to = contractAddress,
                        data = "0x" + BitConverter.ToString(abiEncoding).Replace("-", "").ToLower(),
                        value = "0"
                    }
                );
            Debug.Log($"Transaction status: {response.status}, hash: {response.transactionHash}");
        }


    }
} 