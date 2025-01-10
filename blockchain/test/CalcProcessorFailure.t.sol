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
        vm.expectRevert(TooManyNumbers.selector);
        impl.calc("100+25-10-9-6+1");
        vm.expectRevert(TooManyNumbers.selector);
        impl.calc("100+25-10-9-6+1+2");
    }


    // TODO
    // error TooManyNumbers();
    // error InvalidNumber2();
    // error InvalidNumber3();
    // error InvalidNumber4();
    // error NoMatchingRightBracket();
    // error NotDivisible();
    // error TooManyLeftBrackets();
    // error RightBracketBeforeLeft();
    // error LessThanZero();


}
