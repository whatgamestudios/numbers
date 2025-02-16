// Copyright (c) Whatgame Studios 2024 - 2025
using UnityEngine;

namespace FourteenNumbers {
    public class MessagePass {

        private static string errorMsg = "";


        public static void SetErrorMsg(string msg) {
            errorMsg = msg;
        }

        public static string GetErrorMsg() {
            return errorMsg;
        }

    }
}