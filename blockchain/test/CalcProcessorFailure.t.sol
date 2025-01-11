// Copyright Whatgame Studios 2024 - 2025
// SPDX-License-Identifier: PROPRIETORY
// solhint-disable not-rely-on-time

pragma solidity ^0.8.24;

// solhint-disable-next-line no-global-import
import "forge-std/Test.sol";
import {CalcProcessor} from "../src/CalcProcessor.sol";

contract CalcProcessorImpl is CalcProcessor {

}

contract CalcProcessorTest is Test {
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
    error InvalidNumber2();
    error InvalidNumber3();
    error InvalidNumber4();
    error NoMatchingRightBracket();
    error NotDivisible();
    error TooManyLeftBrackets();
    error RightBracketBeforeLeft();
    error LessThanZero();
    error InvalidStart();

    CalcProcessorImpl impl;

    function setUp() public {
        impl = new CalcProcessorImpl();
    }

    function testLeadingZero() public {
        vm.expectRevert(LeadingZero.selector);
        impl.calc("1+09");
    }

    function testOperationAfterNonNumeric() public {
        vm.expectRevert(OperationAfterNonNumeric.selector);
        impl.calc("1++09");
        vm.expectRevert(OperationAfterNonNumeric.selector);
        impl.calc("1+-09");
        vm.expectRevert(OperationAfterNonNumeric.selector);
        impl.calc("1+/09");
        vm.expectRevert(OperationAfterNonNumeric.selector);
        impl.calc("1+*09");
    }

    function testInvalidNumber1() public {
        vm.expectRevert(InvalidNumber1.selector);
        impl.calc("250+9");
    }

    function testLeftBracketAfterNumeric() public {
        vm.expectRevert(LeftBracketAfterNumeric.selector);
        impl.calc("2(9+1)");
    }

    function testRightBracketAfterNonNumeric() public {
        vm.expectRevert(RightBracketAfterNonNumeric.selector);
        impl.calc("2+(9+)+1");
        vm.expectRevert(RightBracketAfterNonNumeric.selector);
        impl.calc("()");
    }

    function testUnknownSymbol() public {
        vm.expectRevert(UnknownSymbol.selector);
        impl.calc("2+9%2");
    }

    function testEmptyInput() public {
        vm.expectRevert(EmptyInput.selector);
        impl.calc("");
    }

    function testEndedOnInvalidCharacter() public {
        vm.expectRevert(abi.encodeWithSelector(EndedOnInvalidCharacter.selector, 200));
        impl.calc("2+");
        vm.expectRevert(abi.encodeWithSelector(EndedOnInvalidCharacter.selector, 201));
        impl.calc("2-");
        vm.expectRevert(abi.encodeWithSelector(EndedOnInvalidCharacter.selector, 203));
        impl.calc("2/");
        vm.expectRevert(abi.encodeWithSelector(EndedOnInvalidCharacter.selector, 202));
        impl.calc("2*");
        vm.expectRevert(abi.encodeWithSelector(EndedOnInvalidCharacter.selector, 204));
        impl.calc("2+(");
    }

    function testDivideByZero() public {
        vm.expectRevert(DivideByZero.selector);
        impl.calc("100/(25-10-9-6)");
    }

    function testTooManyNumbers() public {
        // Five numbers OK
        impl.calc("100+25-10-9-6");
        impl.calc("(100+25-10-9-6)");

        // Six numbers, where the detection is the end of the equation.
        vm.expectRevert(TooManyNumbers.selector);
        impl.calc("100+25-10-9-6+1");

        // Six numbers, where the detection is an operation.
        vm.expectRevert(TooManyNumbers.selector);
        impl.calc("100+25-10-9-6+1+");

        // Six numbers, where the detection is a bracket.
        vm.expectRevert(TooManyNumbers.selector);
        impl.calc("(100+25-10-9-6+1)");
    }

    function testInvalidNumber3() public {
        vm.expectRevert(InvalidNumber3.selector);
        impl.calc("(100+250)");
    }

    function testInvalidNumber4() public {
        vm.expectRevert(InvalidNumber4.selector);
        impl.calc("100+250");
        vm.expectRevert(InvalidNumber4.selector);
        impl.calc("70");
    }

    function testNoMatchingRightBracket() public {
        vm.expectRevert(NoMatchingRightBracket.selector);
        impl.calc("(100+25");
        vm.expectRevert(NoMatchingRightBracket.selector);
        impl.calc("(((1+2)*3+4)*5");
    }

    function testNotDivisible() public {
        vm.expectRevert(NotDivisible.selector);
        impl.calc("7/2");
    }

    function testTooManyLeftBrackets() public {
        impl.calc("(1)");
        impl.calc("((1))");
        impl.calc("(((1)))");
        impl.calc("(((1+2)*3+4)*5)");
        impl.calc("((((1))))");
        impl.calc("((((1))))");
        vm.expectRevert(TooManyLeftBrackets.selector);
        impl.calc("((((((1))))))");
    }

    function testRightBracketBeforeLeft() public {
        vm.expectRevert(RightBracketBeforeLeft.selector);
        impl.calc("3+(2))+4");
    }

    function testLessThanZero() public {
        vm.expectRevert(LessThanZero.selector);
        impl.calc("3-4");
    }

    function testInvalidStart() public {
        vm.expectRevert(InvalidStart.selector);
        impl.calc("+4");
        vm.expectRevert(InvalidStart.selector);
        impl.calc("-4");
        vm.expectRevert(InvalidStart.selector);
        impl.calc("*4");
        vm.expectRevert(InvalidStart.selector);
        impl.calc("/4");
        vm.expectRevert(InvalidStart.selector);
        impl.calc(")4");
    }

}
