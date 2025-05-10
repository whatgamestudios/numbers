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

        // This function is no longer used in the claim contract.
        public async Task<bool> PrepareForClaim(int salt) {
            var func = new PrepareForClaimFunction() {
                Salt = new BigInteger(salt)
            };
            var (success, _) = await executeTransaction(func.GetCallData());
            return success;
        }

        public async Task<(bool success, BigInteger tokenId)> Claim() {
            int salt = 0;
            var func = new ClaimFunction() {
                Salt = new BigInteger(salt)
            };
            var (success, response) = await executeTransaction(func.GetCallData());
            if (!success) {
                return (false, BigInteger.Zero);
            }

            var tokenId = await GetClaimEventTokenId(response);
            if (tokenId == BigInteger.Zero) {
                AuditLog.Log("No Claim event found in transaction receipt");
                return (false, BigInteger.Zero);
            }

            return (true, tokenId);
        }

        public async Task<BigInteger> GetClaimEventTokenId(TransactionReceiptResponse response) {
            if (response == null) {
                AuditLog.Log("Transaction response is null");
                return BigInteger.Zero;
            }

            try {
                var web3 = new Web3(RPC_URL);
                var fetchedReceipt = await web3.Eth.Transactions.GetTransactionReceipt.SendRequestAsync(response.transactionHash);
                
                if (fetchedReceipt == null) {
                    AuditLog.Log("Failed to fetch transaction receipt from RPC");
                    return BigInteger.Zero;
                }

                var transferEventOutputs = fetchedReceipt.DecodeAllEvents<ClaimEventDTO>();
                if (transferEventOutputs == null) {
                    AuditLog.Log("Transaction logs are null");
                    return BigInteger.Zero;
                }
                var transferEventOutput = transferEventOutputs[0];
                return transferEventOutput.Event.TokenId;


                // // SHA3 hash of "Claim(address,address,uint256,uint256,uint256)"
                // const string CLAIM_EVENT_SIGNATURE = "0x7f4091b46c33e918a0f3aa42307641d17bb67029427a5369e54b353984238705";

                // var eventABI = service.ContractHandler.GetEvent<ClaimEventDTO>().EventABI;
                // var claimEvent = fetchedReceipt.Logs
                //     .Where(log => log.topics != null && 
                //                  log.topics.Length > 0 && 
                //                  log.topics[0] == CLAIM_EVENT_SIGNATURE)
                //     .Select(log => {
                //         try {
                //             AuditLog.Log("Event data: " + log.data.ToString());
                //             return eventABI.DecodeEvent<ClaimEventDTO>(log);
                //         }
                //         catch (Exception ex) {
                //             AuditLog.Log($"Failed to decode event log: {ex.Message}");
                //             return null;
                //         }
                //     })
                //     .FirstOrDefault(e => e != null);

                // if (claimEvent == null) {
                //     AuditLog.Log("No valid Claim event found in transaction receipt");
                // }

                //return claimEvent.Event.TokenId;
            }
            catch (Exception ex) {
                AuditLog.Log($"Error processing Claim event: {ex.Message}");
                return BigInteger.Zero;
            }
        }
    }
} 