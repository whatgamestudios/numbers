// Copyright (c) Whatgame Studios 2024 - 2025
using UnityEngine;
using System;
using System.Numerics;
using System.Threading.Tasks;
using System.Linq;
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

        public async Task<bool> PrepareForClaim(int salt) {
            var func = new PrepareForClaimFunction() {
                Salt = new BigInteger(salt)
            };
            var (success, _) = await executeTransaction(func.GetCallData());
            return success;
        }

        public async Task<(bool success, BigInteger tokenId)> Claim(int salt) {
            var func = new ClaimFunction() {
                Salt = new BigInteger(salt)
            };
            var (success, receipt) = await executeTransaction(func.GetCallData());
            if (!success) {
                return (false, BigInteger.Zero);
            }

            return (true, 103);
            // var claimEvent = GetClaimEvent(receipt);
            // if (claimEvent == null) {
            //     AuditLog.Log("No Claim event found in transaction receipt");
            //     return (false, BigInteger.Zero);
            // }

            // return (true, claimEvent.TokenId);
        }

        // public ClaimEventDTO GetClaimEvent(TransactionReceiptResponse receipt) {
        //     if (receipt == null || receipt.logs == null) {
        //         Debug.LogError("Transaction receipt or logs are null");
        //         return null;
        //     }

        //     try {
        //         // SHA3 hash of "Claim(address,address,uint256,uint256,uint256)"
        //         const string CLAIM_EVENT_SIGNATURE = "0x7f4091b46c33e918a0f3aa42307641d17bb67029427a5369e54b353984238705";

        //         var eventABI = service.ContractHandler.GetEvent<ClaimEventDTO>().EventABI;
        //         var claimEvent = receipt.logs
        //             .Where(log => log.topics != null && 
        //                          log.topics.Length > 0 && 
        //                          log.topics[0] == CLAIM_EVENT_SIGNATURE)
        //             .Select(log => {
        //                 try {
        //                     return eventABI.DecodeEvent<ClaimEventDTO>(log);
        //                 }
        //                 catch (Exception ex) {
        //                     Debug.LogError($"Failed to decode event log: {ex.Message}");
        //                     return null;
        //                 }
        //             })
        //             .FirstOrDefault(e => e != null);

        //         if (claimEvent == null) {
        //             Debug.LogWarning("No valid Claim event found in transaction receipt");
        //         }

        //         return claimEvent;
        //     }
        //     catch (Exception ex) {
        //         Debug.LogError($"Error processing Claim event: {ex.Message}");
        //         return null;
        //     }
        // }
    }
} 