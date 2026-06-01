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
                (string sol1, bool complete1, string sol2, bool complete2, string sol3, bool complete3) = Stats.GetAllSolutions();
                int result1 = 0;
                int result2 = 0;
                int result3 = 0;
                int err = 0;
                if (complete1) 
                {
                    (result1, err) = (new CalcProcessor()).Calc(sol1);
                }
                if (complete2) 
                {
                    (result2, err) = (new CalcProcessor()).Calc(sol2);

                }
                if (complete3)
                {
                    (result3, err) = (new CalcProcessor()).Calc(sol3);
                }
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
            string output = solution.Replace("*", " × ");
            output = output.Replace("/", " ÷ ");
            output = output.Replace("+", " + ");
            output = output.Replace("-", " - ");
            return output;
        }

        private string format(string sol, uint result, uint points) {
            string output = sol + " = " + result + " :  " + points + " points" ;
            return output;
        }
    }
}