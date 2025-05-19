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
        public const int MAX_RETRIES = 3;

        public enum TransactionStatus {
            Init = 0,
            Success = 1,
            Failed = 2
        }

        public const string RPC_URL = "https://rpc.immutable.com/";

        public static string contractAddress;

        public int retryCount;

        public FourteenNumbersContract(string contractAddr) {
            contractAddress = contractAddr;
            retryCount = 0;
        }

        public async Task<(bool success, TransactionReceiptResponse receipt)> executeTransaction(byte[] abiEncoding) {
            while (true) {
                try {
                    DateTime start = DateTime.Now;
                    TransactionReceiptResponse response = 
                        await Passport.Instance.ZkEvmSendTransactionWithConfirmation(
                            new TransactionRequest() {
                                to = contractAddress,
                                data = "0x" + BitConverter.ToString(abiEncoding).Replace("-", "").ToLower(),
                                value = "0"
                            }
                        );
                    DateTime end = DateTime.Now;
                    TimeSpan diff = end.Subtract(start);
                    AuditLog.Log($"Transaction status: {response.status}, time span: {diff.TotalMilliseconds}, hash: {response.transactionHash}");

                    if (response.status != "1") {
                        return (false, response);
                    }
                    else {
                        return (true, response);
                    }
                }
                catch (System.Exception ex) {
                    string errorMessage = $"Tx exception: {ex.Message}\nStack: {ex.StackTrace}";
                    if (errorMessage.IndexOf("TimeoutException:") != -1) {
                        if (retryCount == MAX_RETRIES) {
                            AuditLog.Log($"Transaction: Timed out: Too many retries: {retryCount}");
                            return (false, null);
                        }
                        else {
                            AuditLog.Log($"Transaction: Timed out: Retry count: {retryCount}");
                            retryCount++;
                        }
                    }
                    else {
                        AuditLog.Log(errorMessage);
                        return (false, null);
                    }
                }
            }
        }

    }
}



