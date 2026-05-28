using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace FourteenNumbers { 

    public class HowToPlayScene : MonoBehaviour
    {
        public GameObject InfoPanel;
        public TextMeshProUGUI InfoText;

        private Calculator gamePlay;

        private uint TimeInShow;


        private const int TIME_MOVE = 250;
        private DateTime timeOfLastMove = DateTime.Now;


        private uint Delay;
        private uint NextMove;

        void Awake()
        {
            if (gamePlay == null)
            {
                gamePlay = FindFirstObjectByType<Calculator>();
            }
        }

        void Start()
        {
            AuditLog.Log($"How to Play screen");
            TimeInShow = 0;
            InfoText.text = "";
            InfoPanel.SetActive(false);
            Delay = 0;
            NextMove = 1;
        }
        
        void Update()
        {
            DateTime now = DateTime.Now;
            if ((now - timeOfLastMove).TotalMilliseconds < TIME_MOVE)
            {
                return;
            }
            timeOfLastMove = now;

            AuditLog.Log($"Delay: {Delay}, NextMove: {NextMove}");

            if (Delay++ < NextMove)
            {
                AuditLog.Log($"zzDelay: {Delay}, NextMove: {NextMove}");
                return;
            }
            Delay = 0;

            AuditLog.Log($"Time in show: {TimeInShow}");
            switch (TimeInShow++)
            {
                case 0:
                    resetGame(583);
                    NextMove = 0;
                    break;
                case 1:
                    InfoText.text = "Find three equations that solve for the";
                    InfoPanel.SetActive(true);
                    NextMove = 6;
                    break;
                case 2:
                    InfoText.text = "target number";
                    InfoPanel.SetActive(true);
                    NextMove = 6;
                    break;
                case 3:
                    InfoPanel.SetActive(false);
                    NextMove = 1;
                    break;
                case 4:
                    InfoText.text = "Enter the first equation";
                    InfoPanel.SetActive(true);
                    NextMove = 4;
                    break;
                case 5:
                    pressButton("(");
                    NextMove = 1;
                    break;
                case 6:
                    pressButton("75");
                    NextMove = 1;
                    break;
                case 7:
                    pressButton("-");
                    NextMove = 1;
                    break;
                case 8:
                    pressButton("2");
                    NextMove = 1;
                    break;
                case 9:
                    pressButton(")");
                    NextMove = 1;
                    break;
                case 10:
                    pressButton("*");
                    NextMove = 1;
                    break;
                case 11:
                    pressButton("8");
                    NextMove = 1;
                    break;
                case 12:
                    pressButton("-");
                    NextMove = 1;
                    break;
                case 13:
                    pressButton("1");
                    NextMove = 4;
                    break;
                case 14:
                    InfoPanel.SetActive(false);
                    NextMove = 1;
                    break;
                case 15:
                    InfoText.text = "Press = to finish the equation";
                    InfoPanel.SetActive(true);
                    NextMove = 3;
                    break;
                case 16:
                    pressButton("=");
                    NextMove = 4;
                    break;
                case 17:
                    InfoPanel.SetActive(false);
                    NextMove = 1;
                    break;
                case 18:
                    InfoText.text = "Enter the second equation";
                    InfoPanel.SetActive(true);
                    NextMove = 3;
                    break;
                case 19:
                    pressButton("100");
                    NextMove = 1;
                    break;
                case 20:
                    pressButton("*");
                    NextMove = 1;
                    break;
                case 21:
                    pressButton("6");
                    NextMove = 1;
                    break;
                case 22:
                    pressButton("*");
                    NextMove = 1;
                    break;
                case 23:
                    pressButton("10");
                    NextMove = 4;
                    break;
                case 24:
                    InfoPanel.SetActive(false);
                    NextMove = 1;
                    break;
                case 25:
                    InfoText.text = "Use backspace if you make a mistake"; //<size=+30>←</size>";
                    InfoPanel.SetActive(true);
                    NextMove = 3;
                    break;
                case 26:
                    InfoText.text = "<size=+50>←</size>";
                    InfoPanel.SetActive(true);
                    NextMove = 3;
                    break;
                case 27:
                    pressButton("B");
                    NextMove = 2;
                    break;
                case 28:
                    pressButton("B");
                    NextMove = 2;
                    break;
                case 29:
                    InfoText.text = "Enter the rest of the equation";
                    InfoPanel.SetActive(true);
                    NextMove = 4;
                    break;
                case 30:
                    pressButton("-");
                    NextMove = 1;
                    break;
                case 31:
                    pressButton("10");
                    NextMove = 4;
                    break;
                case 32:
                    pressButton("-");
                    NextMove = 1;
                    break;
                case 33:
                    pressButton("7");
                    NextMove = 4;
                    break;
                case 34:
                    pressButton("=");
                    NextMove = 1;
                    break;
                case 35:
                    InfoPanel.SetActive(false);
                    NextMove = 1;
                    break;
                case 36:
                    InfoText.text = "Enter the third equation";
                    InfoPanel.SetActive(true);
                    NextMove = 3;
                    break;
                case 37:
                    pressButton("50");
                    NextMove = 1;
                    break;
                case 38:
                    pressButton("*");
                    NextMove = 1;
                    break;
                case 39:
                    pressButton("4");
                    NextMove = 1;
                    break;
                case 40:
                    pressButton("*");
                    NextMove = 1;
                    break;
                case 41:
                    pressButton("3");
                    NextMove = 1;
                    break;
                case 42:
                    pressButton("-");
                    NextMove = 1;
                    break;
                case 43:
                    pressButton("25");
                    NextMove = 1;
                    break;
                case 44:
                    pressButton("+");
                    NextMove = 1;
                    break;
                case 45:
                    pressButton("9");
                    NextMove = 3;
                    break;
                case 46:
                    pressButton("=");
                    NextMove = 6;
                    break;
                case 47:
                    InfoPanel.SetActive(false);
                    NextMove = 1;
                    break;
                case 48:
                    InfoText.text = "You can use the backspace button";
                    InfoPanel.SetActive(true);
                    NextMove = 6;
                    break;
                case 49:
                    InfoText.text = "at any time";
                    NextMove = 6;
                    break;
                case 50:
                    InfoText.text = "to retry an equation";
                    NextMove = 6;
                    break;
                case 51:
                    InfoText.text = "<size=+50>←</size>";
                    NextMove = 3;
                    break;
                case 52:
                    pressButton("B");
                    NextMove = 6;
                    break;
                case 53:
                    InfoPanel.SetActive(false);
                    NextMove = 1;
                    break;
                case 54:
                    InfoText.text = "You can use the clear button";
                    InfoPanel.SetActive(true);
                    NextMove = 6;
                    break;
                case 55:
                    InfoText.text = "<size=+50>¢</size>";
                    NextMove = 3;
                    break;
                case 56:
                    InfoText.text = "to restart";
                    NextMove = 6;
                    break;
                case 57:
                    pressButton("C");
                    NextMove = 10;
                    break;
                case 58:
                    InfoPanel.SetActive(false);
                    NextMove = 1;
                    break;
                case 59:
                    InfoText.text = "There is a new target value";
                    InfoPanel.SetActive(true);
                    NextMove = 6;
                    break;
                case 60:
                    InfoText.text = "each day";
                    resetGame(919);
                    NextMove = 3;
                    break;
                case 61:
                    resetGame(267);
                    NextMove = 3;
                    break;
                case 62:
                    resetGame(625);
                    NextMove = 3;
                    break;
                case 63:
                    TimeInShow = 0;
                    NextMove = 10;
                    InfoPanel.SetActive(false);
                    break;

                default:
                    // Do nothing.
                    break;
            }
        }


        private void resetGame(uint target)
        {
            gamePlay.SetTargetOverride(target);
        }

        private void pressButton(string chars)
        {
            gamePlay.OnButtonClickInternal(chars);
        }
    }
}
