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

    public class FourteenNumbersClaimContract : FourteenNumbersContract {
        private FourteenNumbersClaimService service;
        public const string FOURTEEN_NUMBERS_CLAIM_CONTRACT = "0xb427336d725943BA4300EEC219587E207ad21146";

        public FourteenNumbersClaimContract() : base(FOURTEEN_NUMBERS_CLAIM_CONTRACT) {
            var web3 = new Web3(RPC_URL);
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

        public async void PrepareForClaim(int salt) {
            var func = new PrepareForClaimFunction() {
                Salt = new BigInteger(salt)
            };
            executeTransaction(func.GetCallData());
        }


        public async void Claim(int salt) {
            var func = new ClaimFunction() {
                Salt = new BigInteger(salt)
            };
            executeTransaction(func.GetCallData());
        }


    }
} 