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

        FourteenNumbersSolutionsContract contract;

        string status;
        private bool isProcessing = false;

        public void Start() {
            AuditLog.Log("Checkin start");
            contract = new FourteenNumbersSolutionsContract();
            StartCheckinProcess();
        }

        private async void StartCheckinProcess() {
            if (isProcessing) {
                return;
            }
            if (!PassportStore.IsLoggedIn()) {
                return;
            }

            // Check network connectivity
            if (Application.internetReachability == NetworkReachability.NotReachable) {
                AuditLog.Log("Checkin: No network connectivity available");
                return;
            }

            isProcessing = true;

            try {
                await PassportLogin.Init();
                await PassportLogin.Login();

                uint gameDay = Timeline.GameDay();
                if (CheckInStore.DoINeedToCheckIn(gameDay)) {
                    AuditLog.Log("Checkin transaction");
                    var checkInSuccess = await contract.SubmitCheckIn(gameDay);
                    AuditLog.Log("Checkin: " + checkInSuccess.ToString());
                    CheckInStore.DoCheckIn(gameDay);
                } else {
                    AuditLog.Log("Checked in today already");
                }
            }
            catch (Exception ex) {
                AuditLog.Log($"Exception in checkin process: {ex.Message}");
            }
            finally {
                isProcessing = false;
            }
        }
    }
}