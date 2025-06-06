// Copyright (c) Whatgame Studios 2024 - 2025
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;


namespace FourteenNumbers {

    public class ShareManager : MonoBehaviour {
        public GameObject panelShare;

        public void Start()
        {
            panelShare.SetActive(false);
        }


        public void Update()
        {
            GameState gameState = GameState.Instance();
            if (GameState.Instance().IsPlayerStateDone())
            {
                panelShare.SetActive(true);
            }
        }

        public void OnButtonClick(string buttonText)
        {
            if (buttonText == "Share")
            {
                uint gameDay = (uint)Stats.GetLastGameDay();
                uint target = TargetValue.GetTarget(gameDay);
                (string sol1, string sol2, string sol3) = Stats.GetSolutions();
                (int result1, int err1) = (new CalcProcessor()).Calc(sol1);
                (int result2, int err2) = (new CalcProcessor()).Calc(sol2);
                (int result3, int err3) = (new CalcProcessor()).Calc(sol3);
                sol1 = replace(sol1);
                sol2 = replace(sol2);
                sol3 = replace(sol3);
                uint points1 = Points.CalcPoints((uint)result1, target);
                uint points2 = Points.CalcPoints((uint)result2, target);
                uint points3 = Points.CalcPoints((uint)result3, target);
                uint total = points1 + points2 + points3;

                string msg =
                    "14Numbers\n" +
                    "Game day " + gameDay + ", Target " + target + "\n" +
                    format(sol1, (uint)result1, points1) + "\n" +
                    format(sol2, (uint)result2, points2) + "\n" +
                    format(sol3, (uint)result3, points3) + "\n" +
                    "Total: " + total + " points";
                AuditLog.Log("Share: \n" + msg);
                SunShineNativeShare.instance.ShareText(msg, msg);
            }
            else
            {
                AuditLog.Log($"Share Manager: Unknown button: {buttonText}");
            }
        }

        private string replace(string solution) {
            string output = solution.Replace("100", "?");
            output = output.Replace("75", "?");
            output = output.Replace("50", "?");
            output = output.Replace("25", "?");
            output = output.Replace("10", "?");
            output = output.Replace('9', '?');
            output = output.Replace('8', '?');
            output = output.Replace('7', '?');
            output = output.Replace('6', '?');
            output = output.Replace('5', '?');
            output = output.Replace('4', '?');
            output = output.Replace('3', '?');
            output = output.Replace('2', '?');
            output = output.Replace('1', '?');
            output = output.Replace('×', '*');
            output = output.Replace('÷', '/');
            return output;
        }

        private string format(string sol, uint result, uint points) {
            string output = sol + " = " + result + " :  " + points + " points" ;
            return output;
        }
    }
}