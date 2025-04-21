// Copyright (c) Whatgame Studios 2024 - 2025
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Threading.Tasks;
using System;



namespace FourteenNumbers {

    public class CheckIn : MonoBehaviour {

        public async Task Start() {
            await PassportLogin.Init();
            await PassportLogin.Login();

            uint gameDay = Timeline.GameDay();
            if (CheckInStore.DoINeedToCheckIn(gameDay)) {
                FourteenNumbersContract contract = new FourteenNumbersContract();
                contract.SubmitCheckIn(gameDay);
            }
        }
    }
}