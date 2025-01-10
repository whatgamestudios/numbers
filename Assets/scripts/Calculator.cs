// Copyright (c) Whatgame Studios 2024 - 2025
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Collections;


public class Calculator : MonoBehaviour {
    // Maximum number of numbers in a solution
    private const int MAX_NUMBERS = CalcProcessor.MAX_NUMBERS;

    // Maximum number of left brackets in a solution
    private const int MAX_BRACKETS = CalcProcessor.MAX_BRACKETS;

    private const int NUM_ATTEMPTS = 3;
    private const int MAX_POINTS_PER_ATTEMPT = 50;
    private const int BONUS_POINTS = 20;

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

    uint pointsEarnedToday;

    uint attempt;

    bool waitingForNextDay = false;

    // Int representing which game day is being played.
    // Stored here to detect when the game was loaded into memory, switch focus away and then 
    // back to the game, but the game day had changed.
    int gameDayInt;

    private const int TIME_PER_FLASH = 500;
    DateTime timeOfLastFlash = DateTime.Now;
    bool cursorOn = false;

    // True if the player is new to the game.
    bool newPlayer;

    public void Start() {
        Debug.Log("Game Scene Start");
        int daysPlayed = Stats.GetNumDaysPlayed();
        newPlayer = daysPlayed < 4;
        gameDayInt = Timeline.DaysSinceEpochStart();
        startANewDay(gameDayInt);
    }

    public void OnApplicationFocus(bool hasFocus) {
        Debug.Log("OnApplicationFocus: " + hasFocus);
        if (hasFocus) {
            int gameDayNow = Timeline.DaysSinceEpochStart();
            Debug.Log("Game Day: Existing: " + gameDayInt + " Now: " + gameDayNow);
            if (gameDayNow != gameDayInt) {
                gameDayInt = gameDayNow;
                startANewDay(gameDayInt);
            }
        }

    }

    public void OnButtonClick(string buttonText) {
        showAfterFirstCharacter();
        OnButtonClickInternal(buttonText, true);
    }

    public void OnButtonClickInternal(string buttonText, bool updateStats) {
        if (buttonText == "C") {
            clearUsedButtons(true);
            clearCurrentAttempt();
        }
        else if (buttonText == "B") {
            if (currentInput.Length == 0) {
                // Ignore backspace when there is no text.
                return;
            }
            string lastChars = determineLastSymbol();
            updateUsed(lastChars, false);

            // Decrease the bracket counts if necessary.
            if (isLeftBracket(lastChars)) {
                leftBracketCount--;
            }
            if (isRightBracket(lastChars)) {
                rightBracketCount--;
            }
            if (isNumber(lastChars)) {
                numberCount--;
            }


            // Remove the character(s) from the input string.
            currentInput = currentInput.Substring(0, currentInput.Length - lastChars.Length);

            if (currentInput.Length == 0) {
                clearCurrentAttempt();
            }
            else {
                lastChars = determineLastSymbol();
                enableButtons(lastChars);
            }
        }
        else if (buttonText == "=") {
            calculateResult(updateStats);
            attempt++;
            switch (attempt) {
                case 1:
                    showSecondSoltionHelp();
                    break;
                case 2:
                    showThirdSolutionHelp();
                    break;
                case 3:
                    showEndResult();
                    waitingForNextDay = true;
                    break;
            }
            if (attempt < NUM_ATTEMPTS) {
                clearCurrentAttempt();
            }
            return;
        }
        else {
            if (isLeftBracket(buttonText)) {
                leftBracketCount++;
            }
            else if (isRightBracket(buttonText)) {
                rightBracketCount++;
            }
            currentInput += buttonText;
            updateUsed(buttonText, true);
            if (isNumber(buttonText)) {
                usedThisAttemptStack.Push(buttonText);
                numberCount++;
            }
            enableButtons(buttonText);
        }
        updateInputGui(currentInput);
    }

    private void calculateResult(bool publishStats) {
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
                        showDivideByZero();
                        break;
                    case CalcProcessor.ERR_NOT_DIVISIBLE:
                        showDivisionWithRemainder();
                        break;
                    case CalcProcessor.ERR_LESS_THAN_ZERO:
                        showLessThanZero();
                        break;
                    default:
                        Debug.Log("CalcProcessor error: " + err);
                        break;
                }
            }
            else {
                // double result = System.Convert.ToDouble(new System.Data.DataTable().Compute(currentInput, ""));
                // int resultInt = Convert.ToInt32(result);
                resultText =  "" + resultInt;

                int howFarOff = (int) (targetValue - resultInt);
                if (howFarOff < 0) {
                    howFarOff = -howFarOff;
                }

                int points = MAX_POINTS_PER_ATTEMPT - howFarOff;
                if (points < 0) {
                    points = 0;
                }
                if (howFarOff == 0) {
                    points = MAX_POINTS_PER_ATTEMPT + BONUS_POINTS;
                }
                pointsEarnedThisAttempt = (uint) points;
                pointsEarnedToday += pointsEarnedThisAttempt;
            }
        }
        catch (System.Exception ex) {
            resultText = "Err";
            resultText = ex.Message;
            Debug.Log("Ex Message: " + ex.Message);
            //Debug.Log("Ex Stack: " + ex.StackTrace);
            pointsEarnedThisAttempt = 0;
        }
        updateCalcGui(resultText);
        updateInputGui(currentInput);
        updatePointsGui(pointsEarnedThisAttempt.ToString());
        if (publishStats) {
            publishStatsThisSolution(currentInput, pointsEarnedThisAttempt);
        }
    }   

    public void OnCopyButtonPressed() {
        GUIUtility.systemCopyBuffer = "Test123";
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

        if (waitingForNextDay) {
            showEndResult();
        }
    }

    private void startANewDay(int todaysGameDay) {
        Debug.Log("Starting new game day: " + todaysGameDay);

        waitingForNextDay = false;

        targetValue = getTarget(250, 1000);
        target.text = targetValue.ToString();

        pointsEarnedToday = 0;
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
            showWelcomeMessage();
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
                Debug.Log("Attempt not supported2");
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
                Debug.Log("Attempt not supported3");
                break;
        }
    }

    private void updatePointsGui(string val) {
        switch (attempt) {
            case 0:
                points1.text = val;
                break;
            case 1:
                points2.text = val;
                break;
            case 2:
                points3.text = val;
                break;
            default:
                Debug.Log("Attempt not supported3");
                break;
        }
        pointsTotal.text = pointsEarnedToday.ToString();
    }

    private void publishStatsThisSolution(string solution, uint points) {
        switch (attempt) {
            case 0:
                Stats.SetSolution1(solution, (int)points);
                break;
            case 1:
                Stats.SetSolution2(solution, (int)points);
                break;
            case 2:
                Stats.SetSolution3(solution, (int)points);
                break;
            default:
                Debug.Log("Attempt not supported4");
                break;
        }
    }


    private uint getTarget(uint low, uint high) {
        byte[] seed = SeedGen.GenerateSeed(0, 0);
        uint count = 0;
        uint val = 0;
        do {
            val = SeedGen.GetNextValue(seed, count++, high);
            Debug.Log("Seed value: count: " + count + ", val: " + val);
        } while (val < low);
        return val;
    }



    /**
     * If the last day played was today, then there should be at least one solution.
     * Process all solutions.
     */
    private void reprocessSolutions() {
        Debug.Log("Reprocessing solutions");
        string solution1;
        string solution2;
        string solution3;
        (solution1, solution2, solution3) = Stats.GetSolutions();
        if (solution1.Length == 0) {
            // This shouldn't happen, because the first solution should have been submitted
            // to register the new game day.
            Debug.Log("Unexpectedly, solution1 has zero length");
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
        Debug.Log("Reprocessing solution: " + solution);
        CalcProcessor processor = new CalcProcessor();
        int errorCode = processor.Parse(solution);
        if (errorCode != CalcProcessor.ERR_NO_ERROR) {
            // This shouldn't happen as the input should be valid coming from storage.
            Debug.Log("Reprocessing solution error: " + errorCode);
            return;
        }
        string[] buttonPresses = processor.GetTokensAsStrings();
        foreach (string buttonPress in buttonPresses) {
            OnButtonClickInternal(buttonPress, false);
        }
        OnButtonClickInternal("=", false);
    }



    private void showWelcomeMessage() {
        if (newPlayer) {
            displayHelp("Find three solutions for the target number");
        }
        else {
            displayHelp("Welcome back!");
        }
    }

    private void showAfterFirstCharacter() {
        if (newPlayer) {
            displayHelp("Press the = button to complete your solution");
        }
        else {
            showBestScoreToday();
        }
    }

    private void showSecondSoltionHelp() {
        if (newPlayer) {
            displayHelp("Time to do Solution 2");
        }
        else {
            showBestScoreToday();
        }
    }

    private void showThirdSolutionHelp() {
        if (newPlayer) {
            displayHelp("Time to do Solution 3");
        }
        else {
            showBestScoreToday();
        }
    }

    private void showEndResult() {
        string baseText = "";

        if (pointsEarnedToday < 70) {
            baseText = "Practice Makes Perfect.";
        }
        else if (pointsEarnedToday < 120) {
            baseText = "Good work!";
        }
        else if (pointsEarnedToday < 140) {
            baseText = "Well done!";
        }
        else if (pointsEarnedToday < 150) {
            baseText = "Very well done!";
        }
        else if (pointsEarnedToday < 160) {
            baseText = "Awesome day!";
        }
        else if (pointsEarnedToday < 170) {
            baseText = "So close...";
        }
        else if (pointsEarnedToday < 210) {
            baseText = "You are exceptional!";
        }
        else if (pointsEarnedToday == 210) {
            baseText = "Perfect Score Day!!!";
        }
        else {
            baseText = "Well done";
            Debug.Log("ShowEndResult with: " + pointsEarnedToday);
        }

        string timeToNextDay = Timeline.TimeToNextDayStrShort();
        displayHelp(baseText + "\nNext game in " + timeToNextDay);
    }

    private void showDivideByZero() {
        displayHelp("Divide by zero detected. Zero points awarded.");
    }

    private void showDivisionWithRemainder() {
        displayHelp("Division with remainder detected. Zero points awarded.");
    }

    private void showLessThanZero() {
        displayHelp("Subtraction resulted in negative number. Zero points awarded.");
    }

    private void showBestScoreToday() {
        displayHelp("The best score so far today is TODO");
    }


    private void displayHelp(string text) {
        helpTextMesh.text = text;
    }

}


