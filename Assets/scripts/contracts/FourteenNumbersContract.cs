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

        public const string RPC_URL = "https://rpc.immutable.com/";

        public static string contractAddress;

        public static TransactionStatus LastTransactionStatus = TransactionStatus.Init;

        public FourteenNumbersContract(string contractAddr) {
            contractAddress = contractAddr;
        }


        public async Task<(bool success, TransactionReceiptResponse receipt)> executeTransaction(byte[] abiEncoding) {
            LastTransactionStatus = TransactionStatus.Init;         
            // Debug.Log("ExeTx: " + HexDump.Dump(abiEncoding));

            try {
                TransactionReceiptResponse response = 
                    await Passport.Instance.ZkEvmSendTransactionWithConfirmation(
                        new TransactionRequest() {
                            to = contractAddress,
                            data = "0x" + BitConverter.ToString(abiEncoding).Replace("-", "").ToLower(),
                            value = "0"
                        }
                    );
                AuditLog.Log($"Transaction status: {response.status}, hash: {response.transactionHash}");

                if (response.status != "1") {
                    LastTransactionStatus = TransactionStatus.Failed;
                    return (false, response);
                }
                else {
                    LastTransactionStatus = TransactionStatus.Success;
                    return (true, response);
                }
            }
            catch (System.Exception ex) {
                LastTransactionStatus = TransactionStatus.Failed;
                string errorMessage = $"Tx exception: {ex.Message}\nStack: {ex.StackTrace}";
                AuditLog.Log(errorMessage);
                return (false, null);
            }
        }

    }
}



