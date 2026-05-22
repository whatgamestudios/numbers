// Copyright (c) Whatgame Studios 2024 - 2025
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Collections;
using UnityEngine.SceneManagement;
using System.Threading.Tasks;

namespace FourteenNumbers {

    public class Calculator : MonoBehaviour {
        // Maximum number of numbers in a solution
        private const int MAX_NUMBERS = CalcProcessor.MAX_NUMBERS;

        // Maximum number of left brackets in a solution
        private const int MAX_BRACKETS = CalcProcessor.MAX_BRACKETS;

        private const int NUM_ATTEMPTS = 3;

        private const string help = "Use each number once to find three equations for the target number";


        public TextMeshProUGUI target;
        public TextMeshProUGUI timeToNext;
        public TextMeshProUGUI gameDay;

        public TextMeshProUGUI input1;
        public TextMeshProUGUI calculated1;
        public TextMeshProUGUI points1;
        public TextMeshProUGUI input2;
        public TextMeshProUGUI calculated2;
        public TextMeshProUGUI points2;
        public TextMeshProUGUI input3;
        public TextMeshProUGUI calculated3;
        public TextMeshProUGUI points3;
        public TextMeshProUGUI pointsTotal;

        public Button button1;
        public Button button2;
        public Button button3;
        public Button button4;
        public Button button5;
        public Button button6;
        public Button button7;
        public Button button8;
        public Button button9;
        public Button button10;
        public Button button25;
        public Button button50;
        public Button button75;
        public Button button100;
        public Button buttonPlus;
        public Button buttonMultiply;
        public Button buttonMinus;
        public Button buttonDivide;
        public Button buttonLeft;
        public Button buttonRight;

        // All three solutions concatenated, with = sign at the end of each to indicate
        // that particular solution is done.
        private string allSolutions = "";

        // The solution number being attempts. Attempt 0 is the first solution.
        uint attempt;


        private uint targetValue = 0;

        // Counts of the number of numbers, and left and right brackets.
        private uint leftBracketCount;
        private uint rightBracketCount;
        private int numberCount;

        // Indicates if a button corresponding to a number has been used.
        private bool used1;
        private bool used2;
        private bool used3;
        private bool used4;
        private bool used5;
        private bool used6;
        private bool used7;
        private bool used8;
        private bool used9;
        private bool used10;
        private bool used25;
        private bool used50;
        private bool used75;
        private bool used100;

        private uint pointsEarned1;
        private uint pointsEarned2;
        private uint pointsEarned3;


        // Int representing which game day is being played.
        // Stored here to detect when the game was loaded into memory, switch focus away and then 
        // back to the game, but the game day had changed.
        private uint TodaysGameDay;

        // Used for flashing ? at the end of the active equation.
        private const int TIME_PER_FLASH = 500;
        DateTime timeOfLastFlash = DateTime.Now;
        bool cursorOn = false;



        public void Start()
        {
            TodaysGameDay = Timeline.GameDay();
            gameDay.text = Timeline.GameDayStr();
            AuditLog.Log($"Game Play screen for day {TodaysGameDay}");
            startANewDay(false);
            setGameState();
        }
        

        public void OnDisable()
        {
            GameState.Instance().SetPlayerState(GameState.PlayerState.Unknown);
        }

        private void setGameState() {
            GameState gameState = GameState.Instance();
            gameState.SetGameDayBeingPlayed(TodaysGameDay);
            gameState.SetPointsEarnedTotal(pointsEarnedTotalToday());
            if (gameState.IsPlayerStateUnknown())
            {
                gameState.SetPlayerState(GameState.PlayerState.Playing);
            }
        }

        /**
         * Reset board and optionally reload current state.
         * @param forceReset true if current state should be discarded.
         */
        private void startANewDay(bool forceReset) {
            targetValue = TargetValue.GetTarget(TodaysGameDay);
            target.text = targetValue.ToString();

            pointsEarned1 = 0;
            pointsEarned2 = 0;
            pointsEarned3 = 0;
            attempt = 0;

            input1.text = "";
            calculated1.text = "";
            points1.text = "";
            input2.text = "Solution 2";
            calculated2.text = "";
            points2.text = "";
            input3.text = "Solution 3";
            calculated3.text = "";
            points3.text = "";
            pointsTotal.text = "0";

            used1 = false;
            used2 = false;
            used3 = false;
            used4 = false;
            used5 = false;
            used6 = false;
            used7 = false;
            used8 = false;
            used9 = false;
            used10 = false;
            used25 = false;
            used50 = false;
            used75 = false;
            used100 = false;

            allSolutions = "";
            prepStartSolutionEntry();
            uint lastPlayedGameDay = Stats.GetLastGameDay();
            if (!forceReset && lastPlayedGameDay == TodaysGameDay) {
                // The game was knocked out of memory after one or more solutions for today's game.
                reprocessSolutions();
            }
            else {
                // The game has not been played today yet.
                Stats.StartNewGameDay();
            }
        }


        public void OnButtonClick(string buttonText) {
            OnButtonClickInternal(buttonText);
        }

        public void OnButtonClickInternal(string buttonText) {
            if (buttonText == "Help") {
                MessagePass.SetMsg(help);
                SceneStack.Instance().PushScene();
                SceneManager.LoadScene("HelpContextScene", LoadSceneMode.Additive);
                return;
            }

            // No more button presses after the game is done, except C and B.
            if (attempt >= NUM_ATTEMPTS)
            {
                if (buttonText != "C" && (buttonText != "B"))
                {
                    return;
                }
            }


            if (buttonText == "C")
            {
                startANewDay(true);
                setGameState();
            }
            else if (buttonText == "B")
            {
                if (allSolutions.Length != 0) {
                    // Back is not enabled initially, so there should always be some
                    // text to backspace.
                    string lastChars = determineLastSymbol();
                    //Remove the character(s) from the input string.
                    allSolutions = allSolutions.Substring(0, allSolutions.Length - lastChars.Length);
                    Stats.SetSolution(TodaysGameDay, allSolutions, pointsEarnedTotalToday());
                    // Reset and replay the solution.
                    startANewDay(false);
                    setGameState();
                }
            } 
            else if (buttonText == "=")
            {
                bool success = calculateResult();
                if (success)
                {
                    attempt++;
                    if (attempt == NUM_ATTEMPTS)
                    {
                        GameState.Instance().SetPlayerState(GameState.PlayerState.Done);
                    }
                    prepStartSolutionEntry();
                    allSolutions += "=";
                    Stats.SetSolution(TodaysGameDay, allSolutions, pointsEarnedTotalToday());
                }
            }
            else
            {
                allSolutions += buttonText;
                Stats.SetSolution(TodaysGameDay, allSolutions, pointsEarnedTotalToday());

                if (isLeftBracket(buttonText))
                {
                    leftBracketCount++;
                }
                else if (isRightBracket(buttonText))
                {
                    rightBracketCount++;
                }
                if (isNumber(buttonText))
                {
                    numberCount++;
                    indicateNumberUsed(buttonText);
                }
                enableButtons(buttonText);
            }
        }

        /**
         * Calculate the result of the current solution.
         * If the solution results in an error, then reject the calculation.
         *
         * @return true If no error was encountered while calculating.
         */
        private bool calculateResult() {
            string inProgressSolution = SolutionResolver.ResolveInProgress(allSolutions);

            // If the last character will make the equation invalid, remove it.
            if (inProgressSolution.EndsWith("+") || inProgressSolution.EndsWith("-") || 
                inProgressSolution.EndsWith("*") || inProgressSolution.EndsWith("/")) {
                allSolutions = allSolutions.Substring(0, allSolutions.Length - 1);
                inProgressSolution = inProgressSolution.Substring(0, inProgressSolution.Length - 1);
            }
            // If the brackets don't match, add extra brackets.
            int left = 0;
            int right = 0;
            foreach (char c in inProgressSolution) {
                if (c == '(') {
                    left++;
                }
                else if (c == ')') {
                    right++;
                }
            }
            for (uint i = 0; i < (left - right); i++) {
                allSolutions += ')';
                inProgressSolution += ')';
            }

            string resultText;
            uint pointsEarnedThisAttempt;
            try {
                CalcProcessor calcProcessor = new CalcProcessor();
                int resultInt;
                int err;
                (resultInt, err) = calcProcessor.Calc(inProgressSolution);
                if (err != CalcProcessor.ERR_NO_ERROR) {
                    resultText = "E" + err;
                    pointsEarnedThisAttempt = 0;
                    switch (err) {
                        case CalcProcessor.ERR_DIVIDE_BY_ZERO:
                            MessagePass.SetErrorMsg("Divide by zero detected.");
                            break;
                        case CalcProcessor.ERR_NOT_DIVISIBLE:
                            MessagePass.SetErrorMsg("Division with remainder detected.");
                            break;
                        case CalcProcessor.ERR_LESS_THAN_ZERO:
                            MessagePass.SetErrorMsg("Subtraction resulted in negative number.");
                            break;
                        default:
                            AuditLog.Log("CalcProcessor error: " + err);
                            MessagePass.SetErrorMsg("CalcProcessor error: " + err);
                            break;
                    }
                    SceneManager.LoadScene("ErrorScene", LoadSceneMode.Additive);
                    return false;
                }

                resultText =  resultInt.ToString();
                pointsEarnedThisAttempt = Points.CalcPoints((uint) resultInt, targetValue);
                updatePointsEarned(pointsEarnedThisAttempt);
                updateCalcGui(resultText);
                return true;
            }
            catch (System.Exception ex) {
                MessagePass.SetErrorMsg("Err: " + ex.Message);
                AuditLog.Log("Calc Error: " + ex.Message);
                SceneManager.LoadScene("ErrorScene", LoadSceneMode.Additive);
                return false;
            }
        }   

        public void Update() {
            timeToNext.text = Timeline.TimeToNextDayStr();

            if (attempt < NUM_ATTEMPTS) {
                // Show a flashing ? as the end of the input line.
                DateTime now = DateTime.Now;
                if ((now - timeOfLastFlash).TotalMilliseconds > TIME_PER_FLASH) {
                    timeOfLastFlash = now;
                    cursorOn = !cursorOn;
                    updateInputGui(cursorOn);
                }
            }
        }


        private void prepStartSolutionEntry() {
            leftBracketCount = 0;
            rightBracketCount = 0;
            numberCount = 0;

            enableAllAvailableNumbers();
            enableLeftBracket();
            disableRightBracket();
            disableAllOperations();
        }


        private void enableButtons(string buttonText) {
                if (isNumber(buttonText)) {
                    disableAllNumbers();
                    disableLeftBracket();
                    if (leftBracketCount > rightBracketCount) {
                        enableRightBracket();
                    }
                    else {
                        disableRightBracket();
                    }
                    enableAllOperations();
                }
                else if (isLeftBracket(buttonText)) {
                    enableAllAvailableNumbers();
                    enableLeftBracket();
                    disableRightBracket();
                    disableAllOperations();
                }
                else if (isRightBracket(buttonText)) {
                    disableAllNumbers();
                    disableLeftBracket();
                    if (leftBracketCount > rightBracketCount) {
                        enableRightBracket();
                    }
                    else {
                        disableRightBracket();
                    }
                    enableAllOperations();
                }
                else {
                    // Else it is an operation
                    enableAllAvailableNumbers();
                    enableLeftBracket();
                    disableRightBracket();
                    disableAllOperations();
                }
        }


        private void disableAllNumbers() {
            button1.interactable = false;
            button2.interactable = false;
            button3.interactable = false;
            button4.interactable = false;
            button5.interactable = false;
            button6.interactable = false;
            button7.interactable = false;
            button8.interactable = false;
            button9.interactable = false;
            button10.interactable = false;
            button25.interactable = false;
            button50.interactable = false;
            button75.interactable = false;
            button100.interactable = false;
        }

        private void enableAllAvailableNumbers() {
            if (numberCount < MAX_NUMBERS) {
                if (!used1) {
                    button1.interactable = true;
                }
                if (!used2) {
                    button2.interactable = true;
                }
                if (!used3) {
                    button3.interactable = true;
                }
                if (!used4) {
                    button4.interactable = true;
                }
                if (!used5) {
                    button5.interactable = true;
                }
                if (!used6) {
                    button6.interactable = true;
                }
                if (!used7) {
                    button7.interactable = true;
                }
                if (!used8) {
                    button8.interactable = true;
                }
                if (!used9) {
                    button9.interactable = true;
                }
                if (!used10) {
                    button10.interactable = true;
                }
                if (!used25) {
                    button25.interactable = true;
                }
                if (!used50) {
                    button50.interactable = true;
                }
                if (!used75) {
                    button75.interactable = true;
                }
                if (!used100) {
                    button100.interactable = true;
                }
            }
        }
        private void disableAllOperations() {
            buttonPlus.interactable = false;
            buttonMinus.interactable = false;
            buttonMultiply.interactable = false;
            buttonDivide.interactable = false;
        }
        private void enableAllOperations() {
            if (numberCount < MAX_NUMBERS) {
                buttonPlus.interactable = true;
                buttonMinus.interactable = true;
                buttonMultiply.interactable = true;
                buttonDivide.interactable = true;
            }
        }
        private void disableLeftBracket() {
            buttonLeft.interactable = false;
        }
        private void enableLeftBracket() {
            buttonLeft.interactable = numberCount < MAX_NUMBERS && leftBracketCount < MAX_BRACKETS;
        }
        private void disableRightBracket() {
            buttonRight.interactable = false;
        }
        private void enableRightBracket() {
            buttonRight.interactable = true;
        }


        private bool isNumber(string buttonText) {
            // Note that 0 is possible when back arrow is pressed before 100+
            if (buttonText == "0" ||
                buttonText == "1" || buttonText == "2" || buttonText == "3" ||
                buttonText == "4" || buttonText == "5" || buttonText == "6" ||
                buttonText == "7" || buttonText == "8" || buttonText == "9" ||
                buttonText == "10" || buttonText == "25" || buttonText == "50" || 
                buttonText == "75" || buttonText == "100") {
                return true;
            }
            return false;
        }

        private bool isLeftBracket(string buttonText) {
            return buttonText == "(";
        }

        private bool isRightBracket(string buttonText) {
            return buttonText == ")";
        }

        private string determineLastSymbol() {
            int len = allSolutions.Length;
            string lastChar = allSolutions.Substring(len - 1, 1);
            if (lastChar == "0") {
                string twoChars = allSolutions.Substring(len - 2, 2);
                if (twoChars == "00") {
                    return "100";
                }
                else if (twoChars == "10") {
                    return "10";
                }
                else {
                    return "50";
                }
            }
            if (lastChar == "5") {
                if (len == 1)
                {
                    return "5";
                }
                string twoChars = allSolutions.Substring(len - 2, 2);
                if (twoChars == "25") {
                    return "25";
                }
                else if (twoChars == "75") {
                    return "75";
                }
            }
            return lastChar;
        }


        /**
         * When a number button is pressed, indicate that it can no longer be used.
         * @param buttonText The string representing the number. 
         */
        private void indicateNumberUsed(string buttonText) {
            if (buttonText == "1") {
                used1 = true;
            }
            if (buttonText == "2") {
                used2 = true;
            }
            if (buttonText == "3") {
                used3 = true;
            }
            if (buttonText == "4") {
                used4 = true;
            }
            if (buttonText == "5") {
                used5 = true;
            }
            if (buttonText == "6") {
                used6 = true;
            }
            if (buttonText == "7") {
                used7 = true;
            }
            if (buttonText == "8") {
                used8 = true;
            }
            if (buttonText == "9") {
                used9 = true;
            }
            if (buttonText == "10") {
                used10 = true;
            }
            if (buttonText == "25") {
                used25 = true;
            }
            if (buttonText == "50") {
                used50 = true;
            }
            if (buttonText == "75") {
                used75 = true;
            }
            if (buttonText == "100") {
                used100 = true;
            }
        }

        private void updateInputGui(bool cursorOn) {
            (string sol1, bool complete1, string sol2, bool complete2, string sol3, bool complete3) =
                SolutionResolver.Resolve(allSolutions);
            input1.text = fix(sol1, complete1, true, 1, cursorOn);
            input2.text = fix(sol2, complete2, complete1, 2, cursorOn);
            input3.text = fix(sol3, complete3, complete2, 3, cursorOn);
        }
        private string fix(string sol, bool complete, bool prevComplete, int solNumber, bool cursorOn) {
            string val = sol.Replace('*', '×');
            val = val.Replace('/', '÷');

            if (prevComplete) 
            {
                if (!complete && cursorOn) {
                    if (val.Length == 0) 
                    {
                        val = "?";
                    }
                    else 
                    {
                        val += " ?";
                    }
                }
            }
            else 
            {
                switch (solNumber) {
                    case 1:
                        break;
                    case 2:
                        val = "Solution 2";
                        break;
                    case 3:
                        val = "Soluiton 3";
                        break;
                }
            }
            return val;
        }

        private void updateCalcGui(string val) {
            switch (attempt) {
                case 0:
                    calculated1.text = val;
                    break;
                case 1:
                    calculated2.text = val;
                    break;
                case 2:
                    calculated3.text = val;
                    break;
                default:
                    AuditLog.Log("ERROR: Attempt not supported3: {attempt}");
                    break;
            }
        }


        private void updatePointsEarned(uint pointsEarned) {
            switch (attempt) {
                case 0:
                    pointsEarned1 = pointsEarned;
                    break;
                case 1:
                    pointsEarned2 = pointsEarned;
                    break;
                case 2:
                    pointsEarned3 = pointsEarned;
                    break;
                default:
                    AuditLog.Log("ERROR: Attempt not supported3: {attempt}");
                    break;
            }

            points1.text = pointsEarned1.ToString();
            points2.text = pointsEarned2.ToString();
            points3.text = pointsEarned3.ToString();
            uint total = pointsEarnedTotalToday();
            pointsTotal.text = total.ToString();
            GameState.Instance().SetPointsEarnedTotal(total);
        }

        private uint pointsEarnedTotalToday() {
            return pointsEarned1 + pointsEarned2 + pointsEarned3;
        }



        /**
        * If the last day played was today, then there should be at least one solution.
        * Process all solutions.
        */
        private void reprocessSolutions() {
            string solutions = Stats.GetSolutions();
            (string sol1, bool sol1Done, string sol2, bool sol2Done, string sol3, bool sol3Done) = 
                SolutionResolver.Resolve(solutions);
            if (sol3Done) 
            {
                reprocessSingleSolution(sol1);
                reprocessSingleSolution(sol2);
                reprocessSingleSolution(sol3);
            }
            else if (sol2Done) 
            {
                reprocessSingleSolution(sol1);
                reprocessSingleSolution(sol2);
                reprocessPartialSolution(sol3);
            }
            else if (sol1Done) 
            {
                reprocessSingleSolution(sol1);
                reprocessPartialSolution(sol2);
            }
            else 
            {
                reprocessPartialSolution(sol1);
            }
        }

        private void reprocessSingleSolution(string solution) {
            reprocess(solution, false);
            OnButtonClickInternal("=");
        }

        private void reprocessPartialSolution(string solution) {
            reprocess(solution, true);
        }


        private void reprocess(string solution, bool okToHaveInvalidCharAtEnd) {
            CalcProcessor processor = new CalcProcessor();
            int errorCode = processor.Parse(solution);
            if (errorCode != CalcProcessor.ERR_NO_ERROR &&
               (!okToHaveInvalidCharAtEnd || errorCode != CalcProcessor.ERR_ENDED_ON_INVALID_CHARACTER)) 
            {
                // This shouldn't happen as the input should be valid coming from storage.
                AuditLog.Log("Reprocessing solution error: " + errorCode);
            }
            string[] buttonPresses = processor.GetTokensAsStrings();
            foreach (string buttonPress in buttonPresses) 
            {
                OnButtonClickInternal(buttonPress);
            }
        }
    }
}
