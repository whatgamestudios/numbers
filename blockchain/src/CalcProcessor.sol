// Copyright (c) Whatgame Studios 2024 - 2025
// SPDX-License-Identifier: PROPRIETORY
pragma solidity ^0.8.24;

abstract contract CalcProcessor {
    error LeadingZero();
    error OperationAfterNonNumeric();
    error InvalidNumber1();
    error LeftBracketAfterNumeric();
    error RightBracketAfterNonNumeric();
    error UnknownSymbol();
    error EmptyInput();
    error EndedOnInvalidCharacter(uint256 _unknownChar);
    error DivideByZero();
    error TooManyNumbers();
    error InvalidNumber3();
    error InvalidNumber4();
    error NoMatchingRightBracket();
    error NotDivisible();
    error TooManyLeftBrackets();
    error RightBracketBeforeLeft();
    error LessThanZero();
    error InvalidStart();

    uint256 private constant TOKEN_PLUS = 200;
    uint256 private constant TOKEN_MINUS = 201;
    uint256 private constant TOKEN_MULTIPLY = 202;
    uint256 private constant TOKEN_DIVIDE = 203;
    uint256 private constant TOKEN_LEFT = 204;
    uint256 private constant TOKEN_RIGHT = 205;

    uint256 private constant MAX_NUMBERS = 5;
    uint256 private constant MAX_BRACKETS = 5;
    
    /**
     * Process an equation and return a result.
     *
     * @param _input Equation to process.
     * @return result of the equation.
     */
    function calc(bytes memory _input) public pure returns (uint256) {
        (uint256[] memory tokens, uint256 numTokens) = parse(_input);
        return process(tokens, 0, numTokens);
    }

    /**
     * Parse an equation to create a set of tokens representing the equation.
     * 
     * @param _input The equation to process.
     * @return tokens array and length of array used.
     */
    function parse(bytes memory _input) private pure returns (uint256[] memory, uint256) {
        uint256[] memory tokens = new uint256[](100); // TODO how big should this be?
        uint256 len = _input.length;
        uint256 numberCount = 0;
        uint256 leftBracketCount = 0;
        uint256 rightBracketCount = 0;
        bool inNumber = false;
        uint256 currentNumber = 0;

        uint256 index = 0;

        if (len == 0) {
            revert EmptyInput();
        }

        for (uint256 i = 0; i < len; i++) { 
            bytes1 c = _input[i];

            if (c == '0' || c == '1' || c == '2' || c == '3' || c == '4' ||
                c == '5' || c == '6' || c == '7' || c == '8' || c == '9') {
                if (c == '0' && !inNumber) {
                    revert LeadingZero();
                }
                uint256 val = uint8(c) - uint8(bytes1('0'));
                
                currentNumber = currentNumber * 10 + val;
                inNumber = true;
            }
            else if (c == '+' || c == '-' || c == '/' || c == '*') {
                if (i == 0) {
                    revert InvalidStart();
                }
                if (!inNumber && tokens[index - 1] != TOKEN_RIGHT) {
                    revert OperationAfterNonNumeric();
                }
                if (inNumber) {
                    if (!isValidNumber(currentNumber)) {
                        revert InvalidNumber1();
                    }
                    tokens[index++] = currentNumber;
                    numberCount++;
                    if (numberCount > MAX_NUMBERS) {
                        revert TooManyNumbers();
                    }
                    currentNumber = 0;
                    inNumber = false;
                }
                if (c == '+') {
                    tokens[index++] = TOKEN_PLUS;
                }
                else if (c == '-') {
                    tokens[index++] = TOKEN_MINUS;
                }
                else if (c == '*') {
                    tokens[index++] = TOKEN_MULTIPLY;
                }
                else {
                    // c must == /
                    tokens[index++] = TOKEN_DIVIDE;
                }
            }
            else if (c == '(') {
                if (leftBracketCount >= MAX_BRACKETS) {
                    revert TooManyLeftBrackets();
                }
                if (inNumber) {
                    revert LeftBracketAfterNumeric();
                }
                tokens[index++] = TOKEN_LEFT;
                currentNumber = 0;
                inNumber = false;
                leftBracketCount++;
            }
            else if (c == ')') {
                if (i == 0) {
                    revert InvalidStart();
                }
                if (leftBracketCount <= rightBracketCount) {
                    revert RightBracketBeforeLeft();
                }
                if (!inNumber && tokens[index-1] != TOKEN_RIGHT) {
                    revert RightBracketAfterNonNumeric();
                }
                if (inNumber) {
                    if (!isValidNumber(currentNumber)) {
                        revert InvalidNumber3();
                    }
                    tokens[index++] = currentNumber;                    
                    numberCount++;
                    if (numberCount > MAX_NUMBERS) {
                        revert TooManyNumbers();
                    }
                    currentNumber = 0;
                    inNumber = false;
                }
                tokens[index++] = TOKEN_RIGHT;
                rightBracketCount++;
            }
            else {
                revert UnknownSymbol();
            }
        }

        if (!inNumber && tokens[index - 1] != TOKEN_RIGHT) {
            revert EndedOnInvalidCharacter(tokens[index - 1]);
        }
        if (inNumber) {
            if (!isValidNumber(currentNumber)) {
                revert InvalidNumber4();
            }
            tokens[index++] = currentNumber;
            numberCount++;
            if (numberCount > MAX_NUMBERS) {
                revert TooManyNumbers();
            }
        }

        return (tokens, index);
    }

    /**
     * Check if the number is valid.
     *
     * @param number The number to check.
     * @return true if the number is 1 to 10 or 25, 50, 75, or 100.
     */
    function isValidNumber(uint256 number) private pure returns (bool) {
        return (number != 0 && 
            (number <= 10 || 
                (number == 25 || number == 50 ||
                    number == 50 || number == 75 || 
                    number == 100)));
    }


    /**
     * Process the tokens between start and end offset, calculating the result.
     *
     * @param _tokens The tokens representing the equation.
     * @param _startOfs The offset to start processing from.
     * @param _endOfs The offset to end processing at.
     */
    function process(uint256[] memory _tokens, uint256 _startOfs, uint256 _endOfs) private pure returns (uint256) {
        (uint256 leftVal, uint256 newStart) = getVal(_tokens, _startOfs, _endOfs);

        uint256 result = leftVal;

        while (newStart < _endOfs) {
            uint256 operation = _tokens[newStart];
            uint256 rightVal;
            (rightVal, newStart) = getVal(_tokens, newStart + 1, _endOfs);

            // If the current operation is + or -, look ahead to see if
            // the next operation is * or /, and process it first if it is.
            if (operation == TOKEN_PLUS || operation == TOKEN_MINUS) {
                while (newStart < _endOfs) {
                    uint256 nextOperation = _tokens[newStart];
                    if (nextOperation == TOKEN_PLUS || nextOperation == TOKEN_MINUS) {
                        // When we encounter + or -, we can go back to processing 
                        // right to left.
                        break;
                    }
                    uint256 nextRightVal;
                    (nextRightVal, newStart) = getVal(_tokens, newStart + 1, _endOfs);
                    rightVal = processSingleCalc(rightVal, nextRightVal, nextOperation);
                }
            }

            // Now calculate left to right.
            result = processSingleCalc(result, rightVal, operation);
        }
        return result;
    }



    /**
     * Extract a value from the next token or tokens. This could be just retrieving a
     * number. However, if the next token is a left bracket, do the calculation up to the
     * matching right bracket.
     *
     * @param _tokens The tokens to process.
     * @param _startOfs The offset within the tokens to start processing.
     * @param _endOfs The offset immediately after the last token to process.
     * @return result and next start offset.
     */
    function getVal(uint256[] memory _tokens, uint256 _startOfs, uint256 _endOfs) private pure returns(uint256, uint256) {
        uint256 result;
        if (_tokens[_startOfs] == TOKEN_LEFT) {
            uint ofsRight;
            ofsRight = scanForMatchingRight(_tokens, _startOfs+1, _endOfs);
            result = process(_tokens, _startOfs+1, ofsRight - 1);
            return (result, ofsRight+1);
        }

        // Process a number.
        result = _tokens[_startOfs];
        return (result, _startOfs + 1);
    }


    /**
     * Scan through tokens to find the matching right bracket.
     * 
     * @param _tokens The tokens to process.
     * @param _startOfs The offset to start scanning from.
     * @param _endOfs The offset to end processing at.
     * @return offset of matching right bracket.
     */
    function scanForMatchingRight(uint256[] memory _tokens, uint256 _startOfs, uint256 _endOfs) private pure returns(uint256) {
        uint256 leftRightCount = 0;
        for (uint256 ofs = _startOfs; ofs <= _endOfs; ofs++) {
            if (_tokens[ofs] == TOKEN_LEFT) {
                leftRightCount++;
            }
            if (_tokens[ofs] == TOKEN_RIGHT) {
                if (leftRightCount == 0) {
                    return ofs;
                }
                leftRightCount--;
            }
        }
        revert NoMatchingRightBracket();
    }


    /**
     * Process a single simple calculation.
     * 
     * @param _leftVal First operand.
     * @param _rightVal Second operand.
     * @param _operation Mathematical operation to execute.
     * @return result of operation.
     */
    function processSingleCalc(uint256 _leftVal, uint256 _rightVal, uint256 _operation) private pure returns(uint256) {
        uint256 result;
        if (_operation == TOKEN_PLUS) {
            result = _leftVal + _rightVal;
        }
        else if (_operation == TOKEN_MINUS) {
            if (_leftVal < _rightVal) {
                revert LessThanZero();
            }
            result = _leftVal - _rightVal;
        }
        else if (_operation == TOKEN_MULTIPLY) {
            result = _leftVal * _rightVal;
        }
        else {
            // Must be TOKEN_DIVIDE
            if (_rightVal == 0) {
                revert DivideByZero();
            }
            result = _leftVal / _rightVal;
            if (result * _rightVal != _leftVal) {
                revert NotDivisible();
            }
        }
        return result;
    }
}
