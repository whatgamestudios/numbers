// Copyright (c) Whatgame Studios 2024 - 2025
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Collections;
using UnityEngine.SceneManagement;
using Immutable.Passport;
using System.Threading.Tasks;

namespace FourteenNumbers {

    public class Calculator : BestScoreLoader {
        // Maximum number of numbers in a solution
        private const int MAX_NUMBERS = CalcProcessor.MAX_NUMBERS;

        // Maximum number of left brackets in a solution
        private const int MAX_BRACKETS = CalcProcessor.MAX_BRACKETS;

        private const int NUM_ATTEMPTS = 3;


        public enum PlayerState {
            Init = 0,
            Playing = 1,
            Done = 3
        }

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
        public TextMeshProUGUI helpTextMesh;

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

        public TextMeshProUGUI buttonPublishText;

        public GameObject panelShare;
        public GameObject panelPublish;
        private string currentInput = "";

        private uint targetValue = 0;

        // Counts of the number of numbers, and left and right brackets.
        private uint leftBracketCount;
        private uint rightBracketCount;
        int numberCount;

        // Indicates if a button corresponding to a number has been used.
        bool used1;
        bool used2;
        bool used3;
        bool used4;
        bool used5;
        bool used6;
        bool used7;
        bool used8;
        bool used9;
        bool used10;
        bool used25;
        bool used50;
        bool used75;
        bool used100;

        Stack usedThisAttemptStack;

        uint pointsEarned1;
        uint pointsEarned2;
        uint pointsEarned3;

        uint attempt;

        string helpScreenMessage;

        // Int representing which game day is being played.
        // Stored here to detect when the game was loaded into memory, switch focus away and then 
        // back to the game, but the game day had changed.
        uint gameDayInt;

        private const int TIME_PER_FLASH = 500;
        DateTime timeOfLastFlash = DateTime.Now;
        bool cursorOn = false;

        // True if the player is new to the game.
        bool newPlayer;

        PlayerState playerState;

        string help = "Use each number once to find three equations for the target number";

        public void Start() {
            AuditLog.Log("Game Play screen start");
            int daysPlayed = Stats.GetNumDaysPlayed();
            newPlayer = daysPlayed < 2;
            gameDayInt = Timeline.GameDay();

            startANewDay(gameDayInt);
        }

        public async Task OnApplicationFocus(bool hasFocus) {
            AuditLog.Log("Game Play screen has focus: " + hasFocus);
            if (hasFocus) {
                // Check network connectivity
                bool hasNetwork =  Application.internetReachability != NetworkReachability.NotReachable;
                bool isLoggedIn = PassportStore.IsLoggedIn();
                if (isLoggedIn && hasNetwork) {
                    await PassportLogin.Init();
                    await PassportLogin.Login();
                }
                uint gameDayNow = Timeline.GameDay();
                AuditLog.Log("Game Day: Existing: " + gameDayInt + " Now: " + gameDayNow);
                if (gameDayNow != gameDayInt) {
                    gameDayInt = gameDayNow;
                    startANewDay(gameDayInt);
                }
            }

        }

        public void OnButtonClick(string buttonText) {
            playerState = PlayerState.Playing;
            OnButtonClickInternal(buttonText, true);
        }

        public void OnButtonClickInternal(string buttonText, bool updateStats) {
            if (buttonText == "Help") {
                MessagePass.SetMsg(help);
                SceneStack.Instance().PushScene();
                SceneManager.LoadScene("HelpContextScene", LoadSceneMode.Additive);
                return;
            }

            // No more button presses after the game is done.
            if (attempt >= NUM_ATTEMPTS)
            {
                return;
            }


            if (buttonText == "C")
            {
                clearUsedButtons(true);
                clearCurrentAttempt();
            }
            else if (buttonText == "B")
            {
                if (currentInput.Length == 0)
                {
                    // Ignore backspace when there is no text.
                    return;
                }
                string lastChars = determineLastSymbol();
                updateUsed(lastChars, false);

                // Decrease the bracket counts if necessary.
                if (isLeftBracket(lastChars))
                {
                    leftBracketCount--;
                }
                if (isRightBracket(lastChars))
                {
                    rightBracketCount--;
                }
                if (isNumber(lastChars))
                {
                    numberCount--;
                }


                // Remove the character(s) from the input string.
                currentInput = currentInput.Substring(0, currentInput.Length - lastChars.Length);

                if (currentInput.Length == 0)
                {
                    clearCurrentAttempt();
                }
                else
                {
                    lastChars = determineLastSymbol();
                    enableButtons(lastChars);
                }
            }
            else if (buttonText == "=")
            {
                bool success = calculateResult(updateStats);
                if (success)
                {
                    attempt++;
                    switch (attempt)
                    {
                        case 1:
                            playerState = PlayerState.Playing;
                            break;
                        case 2:
                            playerState = PlayerState.Playing;
                            break;
                        case 3:
                            playerState = PlayerState.Done;
                            panelShare.SetActive(true);
                            activatePublishButton();
                            setEndResult();
                            break;
                    }
                    if (attempt < NUM_ATTEMPTS)
                    {
                        clearCurrentAttempt();
                    }
                }
                return;
            }
            else
            {
                if (isLeftBracket(buttonText))
                {
                    leftBracketCount++;
                }
                else if (isRightBracket(buttonText))
                {
                    rightBracketCount++;
                }
                currentInput += buttonText;
                updateUsed(buttonText, true);
                if (isNumber(buttonText))
                {
                    usedThisAttemptStack.Push(buttonText);
                    numberCount++;
                }
                enableButtons(buttonText);
            }
            updateInputGui(currentInput);
        }

        /**
         * Calculate the result of the current solution.
         * If the solution results in an error, then reject the calculation.
         *
         * @param publishStats True if the statistics for this run should be published.
         * @retunr true If no error was encountered while calculating.
         */
        private bool calculateResult(bool publishStats) {
            // If the last character will make the equation invalid, remove it.
            if (currentInput.EndsWith("+") || currentInput.EndsWith("-") || 
                currentInput.EndsWith("*") || currentInput.EndsWith("/")) {
                currentInput = currentInput.Substring(0, currentInput.Length - 1);
            }
            // If the brackets don't match, add extra brackets.
            int left = 0;
            int right = 0;
            foreach (char c in currentInput) {
                if (c == '(') {
                    left++;
                }
                else if (c == ')') {
                    right++;
                }
            }
            for (uint i = 0; i < (left - right); i++) {
                currentInput += ')';
            }

            string resultText;
            uint pointsEarnedThisAttempt;
            try {
                CalcProcessor calcProcessor = new CalcProcessor();
                int resultInt;
                int err;
                (resultInt, err) = calcProcessor.Calc(currentInput);
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
                            MessagePass.SetErrorMsg("CalcProcessor error: " + err);
                            break;
                    }
                    SceneManager.LoadScene("ErrorScene", LoadSceneMode.Additive);
                    return false;
                }
                else {
                    // double result = System.Convert.ToDouble(new System.Data.DataTable().Compute(currentInput, ""));
                    // int resultInt = Convert.ToInt32(result);
                    resultText =  resultInt.ToString();
                    pointsEarnedThisAttempt = Points.CalcPoints((uint) resultInt, targetValue);
                    updatePointsEarned(pointsEarnedThisAttempt);
                }
            }
            catch (System.Exception ex) {
                MessagePass.SetErrorMsg("Err: " + ex.Message);
                AuditLog.Log("Error: " + ex.Message);
                SceneManager.LoadScene("ErrorScene", LoadSceneMode.Additive);
                return false;
            }
            updateCalcGui(resultText);
            updateInputGui(currentInput);
            updatePointsGui();
            if (publishStats) {
                publishStatsThisSolution(currentInput, pointsEarnedThisAttempt);
            }
            return true;
        }   

        public void Update() {
            gameDay.text = Timeline.GameDayStr();
            timeToNext.text = Timeline.TimeToNextDayStr();

            if (attempt < NUM_ATTEMPTS) {
                // Show a flashing ? as the end of the input line.
                DateTime now = DateTime.Now;
                if ((now - timeOfLastFlash).TotalMilliseconds > TIME_PER_FLASH) {
                    timeOfLastFlash = now;
                    if (cursorOn) {
                        updateInputGui(currentInput);
                        cursorOn = false;
                    }
                    else {
                        if (currentInput.Length > 0) {
                            updateInputGui(currentInput + " ?");
                        }
                        else {
                            updateInputGui(currentInput + "?");
                        }
                        cursorOn = true;
                    }
                }
            }
            else {
                // Flash the colour of the publish panel.
                // Note: the publish panel may not be active.
                DateTime now = DateTime.Now;
                if ((now - timeOfLastFlash).TotalMilliseconds > TIME_PER_FLASH) {
                    timeOfLastFlash = now;

                    Image img = panelPublish.GetComponent<Image>();
                    if (cursorOn) {
                        img.color = UnityEngine.Color.green;
                        cursorOn = false;
                    }
                    else {
                        img.color = UnityEngine.Color.red;
                        cursorOn = true;
                    }
                }

            }

            showHelpMessage();
        }

        private void startANewDay(uint todaysGameDay) {
            AuditLog.Log("Starting new game day: " + todaysGameDay);
            panelShare.SetActive(false);
            panelPublish.SetActive(false);

            targetValue = TargetValue.GetTarget(todaysGameDay);
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
            usedThisAttemptStack = new Stack();

            clearCurrentAttempt();

            int lastPlayedGameDay = Stats.GetLastGameDay();
            if (lastPlayedGameDay == todaysGameDay) {
                // The game was knocked out of memory after one or more solutions for today's game.
                reprocessSolutions();
            }
            else {
                // The game has not been played today yet.
                Stats.StartNewGameDay();
                playerState = PlayerState.Init;
            }
        }

        private void clearUsedButtons(bool resetButtons) {
            while (usedThisAttemptStack.Count > 0) {
                var buttonPressed = (string) usedThisAttemptStack.Pop(); 
                if (resetButtons) {
                    updateUsed(buttonPressed, false);
                }
            }
        }

        private void clearCurrentAttempt() {
            currentInput = "";
            clearUsedButtons(false);

            updateInputGui("");

            leftBracketCount = 0;
            rightBracketCount = 0;
            numberCount = 0;

            enableAllNumbers();
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
                    enableAllNumbers();
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
                    enableAllNumbers();
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

        private void enableAllNumbers() {
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
            string lastChar = currentInput.Substring(currentInput.Length - 1, 1);
            if (lastChar == "0") {
                string twoChars = currentInput.Substring(currentInput.Length - 2, 2);
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
                string twoChars = currentInput.Substring(currentInput.Length - 2, 2);
                if (twoChars == "25") {
                    return "25";
                }
                else if (twoChars == "75") {
                    return "75";
                }
                // Else it will be some symbol followed by 5. Just return 5.
            }
            return lastChar;
        }


        public void updateUsed(string buttonText, bool updateTo) {
            if (buttonText == "1") {
                used1 = updateTo;
            }
            if (buttonText == "2") {
                used2 = updateTo;
            }
            if (buttonText == "3") {
                used3 = updateTo;
            }
            if (buttonText == "4") {
                used4 = updateTo;
            }
            if (buttonText == "5") {
                used5 = updateTo;
            }
            if (buttonText == "6") {
                used6 = updateTo;
            }
            if (buttonText == "7") {
                used7 = updateTo;
            }
            if (buttonText == "8") {
                used8 = updateTo;
            }
            if (buttonText == "9") {
                used9 = updateTo;
            }
            if (buttonText == "10") {
                used10 = updateTo;
            }
            if (buttonText == "25") {
                used25 = updateTo;
            }
            if (buttonText == "50") {
                used50 = updateTo;
            }
            if (buttonText == "75") {
                used75 = updateTo;
            }
            if (buttonText == "100") {
                used100 = updateTo;
            }
        }

        private void updateInputGui(string val) {
            val = val.Replace('*', '×');
            val = val.Replace('/', '÷');
            switch (attempt) {
                case 0:
                    input1.text = val;
                    break;
                case 1:
                    input2.text = val;
                    break;
                case 2:
                    input3.text = val;
                    break;
                default:
                    AuditLog.Log("ERROR: Attempt not supported2: {attempt}");
                    break;
            }
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
        }

        private uint pointsEarnedTotalToday() {
            return pointsEarned1 + pointsEarned2 + pointsEarned3;
        }


        private void updatePointsGui() {
            points1.text = pointsEarned1.ToString();
            points2.text = pointsEarned2.ToString();
            points3.text = pointsEarned3.ToString();
            uint total = pointsEarnedTotalToday();
            pointsTotal.text = total.ToString();
        }

        private void publishStatsThisSolution(string solution, uint points) {
            switch (attempt) {
                case 0:
                    Stats.SetSolution1(gameDayInt, solution, (int)points);
                    break;
                case 1:
                    Stats.SetSolution2(gameDayInt, solution, (int)points);
                    break;
                case 2:
                    Stats.SetSolution3(gameDayInt, solution, (int)points);
                    break;
                default:
                    AuditLog.Log("ERROR: Attempt not supported4: {attempt}");
                    break;
            }
        }


        /**
        * If the last day played was today, then there should be at least one solution.
        * Process all solutions.
        */
        private void reprocessSolutions() {
            string solution1;
            string solution2;
            string solution3;
            (solution1, solution2, solution3) = Stats.GetSolutions();
            if (solution1.Length == 0) {
                // This shouldn't happen, because the first solution should have been submitted
                // to register the new game day.
                AuditLog.Log("Unexpectedly, solution1 has zero length");
                return;
            }
            reprocessSingleSolution(solution1);

            if (solution3.Length != 0) {
                reprocessSingleSolution(solution2);
                reprocessSingleSolution(solution3);
            }
            else if (solution2.Length != 0) {
                reprocessSingleSolution(solution2);
            }
        }

        private void reprocessSingleSolution(string solution) {
            CalcProcessor processor = new CalcProcessor();
            int errorCode = processor.Parse(solution);
            if (errorCode != CalcProcessor.ERR_NO_ERROR) {
                // This shouldn't happen as the input should be valid coming from storage.
                AuditLog.Log("Reprocessing solution error: " + errorCode);
                return;
            }
            string[] buttonPresses = processor.GetTokensAsStrings();
            foreach (string buttonPress in buttonPresses) {
                OnButtonClickInternal(buttonPress, false);
            }
            OnButtonClickInternal("=", false);
        }



        private void showHelpMessage() {
            string text = "";

            uint pointsToday = (uint) pointsEarnedTotalToday();
            if (playerState == PlayerState.Done) {
                text = helpScreenMessage + "\n";
                if (LoadedBestScore) {
                    if (pointsToday > BestScore) {
                        text = text + "New high score!\n";
                    }
                    else {
                        text = text + "Best score so far today is " + BestScore + "\n";
                    }
                }
                text = text + "Next game in " + Timeline.TimeToNextDayStrShort();
            }
            else {
                text = "Find three solutions for the target number\n";
                if (pointsToday < Stats.STATS_SILVER_STREAK_THRESHOLD) {
                    uint diff = Stats.STATS_SILVER_STREAK_THRESHOLD - pointsToday;
                    text = text + diff + " points to extend silver streak";
                }
                else if (pointsToday < Stats.STATS_GOLD_STREAK_THRESHOLD) {
                    uint diff = Stats.STATS_GOLD_STREAK_THRESHOLD - pointsToday;
                    text = text + diff + " points to extend gold streak";
                }
                else if (pointsToday < Stats.STATS_DIAMOND_STREAK_THRESHOLD) {
                    uint diff = Stats.STATS_DIAMOND_STREAK_THRESHOLD - pointsToday;
                    text = text + diff + " points to extend diamond streak";
                }
                else if (pointsToday < Stats.STATS_BDIAMOND_STREAK_THRESHOLD) {
                    uint diff = Stats.STATS_BDIAMOND_STREAK_THRESHOLD - pointsToday;
                    text = text + diff + " points to extend blue diamond streak";
                }
            }

            helpTextMesh.text = text;
        }

        private void setEndResult() {
            uint pointsEarnedTotal = pointsEarnedTotalToday();
            if (pointsEarnedTotal < 70) {
                helpScreenMessage = "Practice Makes Perfect.";
            }
            else if (pointsEarnedTotal < 120) {
                helpScreenMessage = "Good work!";
            }
            else if (pointsEarnedTotal < 140) {
                helpScreenMessage = "Well done!";
            }
            else if (pointsEarnedTotal < 150) {
                helpScreenMessage = "Very well done!";
            }
            else if (pointsEarnedTotal < 160) {
                helpScreenMessage = "Awesome day!";
            }
            else if (pointsEarnedTotal < 170) {
                helpScreenMessage = "So close...";
            }
            else if (pointsEarnedTotal < 210) {
                helpScreenMessage = "You are exceptional!";
            }
            else if (pointsEarnedTotal == 210) {
                helpScreenMessage = "Perfect Score Day!!!";
            }
            else {
                helpScreenMessage = "Well done";
            }
        }


        // Called from the best score loader once the score has been loaded.
        public override void BestScoreLoaded() {
            activatePublishButton();
        }

        // Called from the best score loader once the score has been loaded, or 
        // from the on click handler once the third equals sign has been pressed.
        private void activatePublishButton() {
            uint pointsToday = (uint) pointsEarnedTotalToday();
            if (playerState == PlayerState.Done &&
                LoadedBestScore && pointsToday > BestScore) {
                panelPublish.SetActive(true);
                if (!PassportStore.IsLoggedIn()) {
                    buttonPublishText.text = "Sign in to Publish";
                    buttonPublishText.fontSize = 50;
                }
           }
        }
    }
}