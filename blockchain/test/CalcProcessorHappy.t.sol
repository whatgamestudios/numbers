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
    CalcProcessorImpl impl;

    function setUp() public {
        impl = new CalcProcessorImpl();
    }

    function testCalcSingleNumbers() public {
        assertEq(1, impl.calc("1"), "1");
        assertEq(2, impl.calc("2"), "1");
        assertEq(3, impl.calc("3"), "1");
        assertEq(4, impl.calc("4"), "1");
        assertEq(5, impl.calc("5"), "1");
        assertEq(6, impl.calc("6"), "1");
        assertEq(7, impl.calc("7"), "1");
        assertEq(8, impl.calc("8"), "1");
        assertEq(9, impl.calc("9"), "1");
        assertEq(10, impl.calc("10"), "1");
        assertEq(25, impl.calc("25"), "1");
        assertEq(50, impl.calc("50"), "1");
        assertEq(75, impl.calc("75"), "1");
        assertEq(100, impl.calc("100"), "1");
    }

    function testTwoNumAdd() public {
        assertEq(3, impl.calc("1+2"), "1+2");
        assertEq(109, impl.calc("100+9"), "100+9");
        assertEq(100, impl.calc("75+25"), "75+25");
        assertEq(57, impl.calc("50+7"), "50+7");
    }

    function testTwoNumMul() public {
        assertEq(500, impl.calc("5*100"), "5*100");
    }

    function testTwoNumSub() public {
        assertEq(75, impl.calc("100-25"), "100-25");
    }

    function testTwoNumDiv() public {
        assertEq(2, impl.calc("8/4"), "8/4");
    }

    function testScenarios() public {
        assertEq(501, impl.calc("5*100+1"), "5*100+1");
        assertEq(501, impl.calc("1+5*100"), "1+5*100");
        assertEq(21, impl.calc("1+100/5"), "1+100/5");
        assertEq(21, impl.calc("100/5+1"), "100/5+1");
        assertEq(125, impl.calc("5*100/4"), "5*100/4");
        assertEq(50, impl.calc("100-8*25/4"), "100-8*25/4");
        assertEq(48, impl.calc("8*25/4-2"), "8*25/4-2");
        assertEq(1, impl.calc("1+2+3-5"), "1+2+3-5");
        assertEq(3, impl.calc("2*10+3-5*4"), "2*10+3-5*4");
    }

    function testBrackets() public {
        assertEq(505, impl.calc("5*(100+1)"), "5*(100+1)");
        assertEq(600, impl.calc("(1+5)*100"), "(1+5)*100");
        assertEq(25, impl.calc("(100+25)/(4+1)"), "(100+25)/(4+1)");
    }

}
