// Copyright Whatgame Studios 2024 - 2025
using UnityEngine;


public class CalcProcessor {

    public const int ERR_NO_ERROR = 1;
    public const int ERR_LEADING_ZERO = 1;
    public const int ERR_OPERATION_AFTER_NON_NUMERIC = 2;
    public const int ERR_INVALID_NUMBER1 = 3;
    public const int ERR_LEFT_BRACKET_AFTER_NUMERIC = 4;
    public const int ERR_RIGHT_BRACKET_AFTER_NON_NUMERIC = 5;
    public const int ERR_UNKNOWN_SYMBOL = 6;
    public const int ERR_EMPTY_INPUT = 7;
    public const int ERR_ENDED_ON_INVALID_CHARACTER = 8;
    public const int ERR_DIVIDE_BY_ZERO = 9;
    public const int ERR_TOO_MANY_NUMBERS = 10;
    public const int ERR_INVALID_NUMBER2 = 11;
    public const int ERR_INVALID_NUMBER3 = 12;
    public const int ERR_INVALID_NUMBER4 = 13;
    public const int ERR_NO_MATCHING_RIGHT_BRACKET = 14;
    public const int ERR_NOT_DIVISIBLE = 15;
    public const int ERR_TOO_MANY_LEFT_BRACKETS = 16;
    public const int ERR_RIGHT_BRACKET_BEFORE_LEFT = 17;

    public const int TOKEN_PLUS = 200;
    public const int TOKEN_MINUS = 201;
    public const int TOKEN_MULTIPLY = 202;
    public const int TOKEN_DIVIDE = 203;
    public const int TOKEN_LEFT = 204;
    public const int TOKEN_RIGHT = 205;

    public const int MAX_NUMBERS = 5;
    public const int MAX_BRACKETS = 5;
    
    private int[] tokens;
    private int tokensUsed;

    public CalcProcessor() {

    }


    public (int, int) Calc(string input) {
        //Debug.Log("Input: " + input);
        int error = Parse(input);
        if (error != ERR_NO_ERROR) {
            return (0, error);
        }

        int result;
        (result, error) = process(0, tokensUsed);
        return (result, error);
    }

    /**
     * Return tokens determined by the parse function.
     */
    public string[] GetTokensAsStrings() {
        string[] strs = new string[tokensUsed];
        for (int i = 0; i < tokensUsed; i++) {
            int token = tokens[i];
            string s = "";
            switch (token) {
                case TOKEN_PLUS:
                    s = "+";
                    break;
                case TOKEN_MINUS:
                    s = "-";
                    break;
                case TOKEN_MULTIPLY:
                    s = "*";
                    break;
                case TOKEN_DIVIDE:
                    s = "/";
                    break;
                case TOKEN_LEFT:
                    s = "(";
                    break;
                case TOKEN_RIGHT:
                    s = ")";
                    break;
                case 0:
                    s = "0";
                    break;
                case 1:
                    s = "1";
                    break;
                case 2:
                    s = "2";
                    break;
                case 3:
                    s = "3";
                    break;
                case 4:
                    s = "4";
                    break;
                case 5:
                    s = "5";
                    break;
                case 6:
                    s = "6";
                    break;
                case 7:
                    s = "7";
                    break;
                case 8:
                    s = "8";
                    break;
                case 9:
                    s = "9";
                    break;
                case 10:
                    s = "10";
                    break;
                case 25:
                    s = "25";
                    break;
                case 50:
                    s = "50";
                    break;
                case 75:
                    s = "75";
                    break;
                case 100:
                    s = "100";
                    break;
                default:
                    Debug.Log("Unknown symbol");
                    break;
            }
            strs[i] = s;
        }
        return strs;
    }

    public int Parse(string input) {
        tokens = new int[100]; // TODO how big should this be?
        int len = input.Length;
        int numberCount = 0;
        int leftBracketCount = 0;
        int rightBracketCount = 0;
        bool inNumber = false;
        int currentNumber = 0;

        int index = 0;

        if (len == 0) {
            return ERR_EMPTY_INPUT;
        }

        for (int i = 0; i < len; i++) { 
            char c = input[i];

            switch (c) {
                case '0':
                case '1':
                case '2': 
                case '3':
                case '4':
                case '5':
                case '6':
                case '7':
                case '8':
                case '9':
                    if (c == '0' && !inNumber) {
                        return ERR_LEADING_ZERO;
                    }
                    int val = c - '0';
                    
                    currentNumber = currentNumber * 10 + val;
                    inNumber = true;
                    break;
                case '+':
                case '-':
                case '/':
                case '*':
                    if (!inNumber && tokens[index - 1] != TOKEN_RIGHT) {
                        return ERR_OPERATION_AFTER_NON_NUMERIC;
                    }
                    if (inNumber) {
                        if (!isValidNumber(currentNumber)) {
                            //Debug.Log("Invalid number 1: " + currentNumber);
                            return ERR_INVALID_NUMBER1;
                        }
                        tokens[index++] = currentNumber;
                        numberCount++;
                        if (numberCount > MAX_NUMBERS) {
                            return ERR_TOO_MANY_NUMBERS;
                        }
                        currentNumber = 0;
                        inNumber = false;
                    }
                    switch (c) {
                        case '+':
                            tokens[index++] = TOKEN_PLUS;
                            break;
                        case '-':
                            tokens[index++] = TOKEN_MINUS;
                            break;
                        case '*':
                            tokens[index++] = TOKEN_MULTIPLY;
                            break;
                        case '/':
                            tokens[index++] = TOKEN_DIVIDE;
                            break;
                    }
                    break;
                case '(':
                    if (leftBracketCount > MAX_BRACKETS) {
                        return ERR_TOO_MANY_LEFT_BRACKETS;
                    }
                    if (inNumber) {
                        return ERR_LEFT_BRACKET_AFTER_NUMERIC;
                    }
                    tokens[index++] = TOKEN_LEFT;
                    currentNumber = 0;
                    inNumber = false;
                    leftBracketCount++;
                    break;
                case ')':
                    if (leftBracketCount < rightBracketCount) {
                        return ERR_RIGHT_BRACKET_BEFORE_LEFT;
                    }
                    if (!inNumber && tokens[index-1] != TOKEN_RIGHT) {
                        return ERR_RIGHT_BRACKET_AFTER_NON_NUMERIC;
                    }
                    if (inNumber) {
                        if (!isValidNumber(currentNumber)) {
                            return ERR_INVALID_NUMBER3;
                        }
                        tokens[index++] = currentNumber;                    
                        numberCount++;
                        if (numberCount > MAX_NUMBERS) {
                            return ERR_TOO_MANY_NUMBERS;
                        }
                        currentNumber = 0;
                        inNumber = false;
                    }
                    tokens[index++] = TOKEN_RIGHT;
                    break;
                default:
                    return ERR_UNKNOWN_SYMBOL;
            }
        }


        if (!inNumber && tokens[index - 1] != TOKEN_RIGHT) {
            return ERR_ENDED_ON_INVALID_CHARACTER;
        }
        if (inNumber) {
            if (!isValidNumber(currentNumber)) {
                return ERR_INVALID_NUMBER4;
            }
            tokens[index++] = currentNumber;
        }

        tokensUsed = index;
        return ERR_NO_ERROR;
    }

    private static bool isValidNumber(int number) {
        return (number != 0 && 
            (number <= 10 || 
                (number == 25 || number == 50 ||
                    number == 50 || number == 75 || 
                    number == 100)));
    }


    private (int, int) process(int startOfs, int endOfs) {
        //Debug.Log("process start: " + startOfs + " endOfs:" + endOfs);
        (int leftVal, int newStart, int error) = getVal(startOfs, endOfs);
        //Debug.Log("getVal leftVal: " + leftVal + " newStart:" + newStart);
        if (error != ERR_NO_ERROR) {
            return (0, error);
        }

        int result = leftVal;

        while (newStart < endOfs) {
            int operation = getOp(newStart);
            int rightVal;
            (rightVal, newStart, error) = getVal(newStart + 1, endOfs);
            //Debug.Log("getVal rightVal: " + rightVal + " newStart:" + newStart);
            if (error != ERR_NO_ERROR) {
                return (0, error);
            }

            // If the current operation is + or -, look ahead to see if
            // the next operation is * or /, and process it first if it is.
            if (operation == TOKEN_PLUS || operation == TOKEN_MINUS) {
                while (newStart < endOfs) {
                    int nextOperation = getOp(newStart);
                    if (nextOperation == TOKEN_PLUS || nextOperation == TOKEN_MINUS) {
                        // When we encounter + or -, we can go back to processing 
                        // right to left.
                        break;
                    }
                    int nextRightVal;
                    (nextRightVal, newStart, error) = getVal(newStart + 1, endOfs);
                    //Debug.Log("getVal nextRightVal: " + rightVal + " newStart:" + newStart);
                    if (error != ERR_NO_ERROR) {
                        return (0, error);
                    }
                    (rightVal, error) = processSingleCalc(rightVal, nextRightVal, nextOperation);
                    if (error != ERR_NO_ERROR) {
                        return (0, error);
                    }
                }
            }

            // Now calculate left to right.
            (result, error) = processSingleCalc(result, rightVal, operation);
            if (error != ERR_NO_ERROR) {
                return (0, error);
            }
        }
        return (result, ERR_NO_ERROR);
    }

    private int getOp(int offset) {
        return tokens[offset];
    }

    private (int, int, int) getVal(int startOfs, int endOfs) {
        //Debug.Log("getVal: " + startOfs + " endOfs:" + endOfs);
        int result;
        int error;
        if (tokens[startOfs] == TOKEN_LEFT) {
            int ofsRight;
            (ofsRight, error) = scanForMatchingRight(startOfs+1);
            if (error != ERR_NO_ERROR) {
                return (0, 0, error);
            }
            //Debug.Log("if left ofsright: " + ofsRight);
            (result, error) = process(startOfs+1, ofsRight - 1);
            //Debug.Log("process result: " + result);
            if (error != ERR_NO_ERROR) {
                return (0, 0, error);
            }
            return (result, ofsRight+1, error);
        }

        // Process a number.
        result = tokens[startOfs];
        return (result, startOfs + 1, ERR_NO_ERROR);
    }


    private (int, int) scanForMatchingRight(int startOfs) {
        //Debug.Log("scan start: " + startOfs + " len: " + tokens.Length);
        int ofs = startOfs;
        int leftRightCount = 0;
        for (ofs = startOfs; ofs < tokens.Length; ofs++) {
            //Debug.Log("scan at ofs: " + ofs);
            if (tokens[ofs] == TOKEN_LEFT) {
                //Debug.Log("scan left found");
                leftRightCount++;
            }
            if (tokens[ofs] == TOKEN_RIGHT) {
                //Debug.Log("scan right found");
                if (leftRightCount == 0) {
                    break;
                }
                leftRightCount--;
            }
        }

        if (ofs >= tokens.Length) {
            return (0, ERR_NO_MATCHING_RIGHT_BRACKET);
        }

        return (ofs, ERR_NO_ERROR);
    }

   private (int, int) processSingleCalc(int leftVal, int rightVal, int operation) {
        int result = 0;
        switch (operation) {
            case TOKEN_PLUS: 
                result = leftVal + rightVal;
                break;
            case TOKEN_MINUS:
                result = leftVal - rightVal;
                break;
            case TOKEN_MULTIPLY:
                result = leftVal * rightVal;
                break;
            case TOKEN_DIVIDE:
                if (rightVal == 0) {
                    return (0, ERR_DIVIDE_BY_ZERO);
                }
                result = leftVal / rightVal;
                if (result * rightVal != leftVal) {
                    return (0, ERR_NOT_DIVISIBLE);
                }
                break;
        }
        return (result, ERR_NO_ERROR);
    }

}
