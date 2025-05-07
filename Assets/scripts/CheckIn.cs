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
            try {
                AuditLog.Log("Checkin start");
                
                // Check network connectivity
                if (Application.internetReachability == NetworkReachability.NotReachable) {
                    AuditLog.Log("No network connectivity available");
                    return;
                }

                bool isLoggedIn = PassportStore.IsLoggedIn();
                if (isLoggedIn) {
                    await PassportLogin.Init();
                    await PassportLogin.Login();

                    uint gameDay = Timeline.GameDay();
                    if (CheckInStore.DoINeedToCheckIn(gameDay)) {
                        FourteenNumbersSolutionsContract contract = new FourteenNumbersSolutionsContract();
                        contract.SubmitCheckIn(gameDay);
                    }
                }
            }
            catch (Exception ex) {
                string errorMessage = $"Exception in CheckIn: {ex.Message}\nStack Trace: {ex.StackTrace}";
                AuditLog.Log(errorMessage);
            }
        }
    }
}