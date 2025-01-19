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

    function testCalcSingleNumbers() public view {
        (uint256 val, uint256 mask) = impl.calc("1");
        assertEq(1, val, "1a");
        assertEq(2, mask, "1b");

        (val, mask) = impl.calc("2");
        assertEq(2, val, "2a");
        assertEq(4, mask, "2b");

        (val, mask) = impl.calc("2");
        assertEq(2, val, "2a");
        assertEq(4, mask, "2b");

        (val, mask) = impl.calc("3");
        assertEq(3, val, "3a");
        assertEq(8, mask, "3b");

        (val, mask) = impl.calc("4");
        assertEq(4, val, "4a");
        assertEq(16, mask, "4b");

        (val, mask) = impl.calc("5");
        assertEq(5, val, "5a");
        assertEq(32, mask, "5b");

        (val, mask) = impl.calc("6");
        assertEq(6, val, "6a");
        assertEq(64, mask, "6b");

        (val, mask) = impl.calc("7");
        assertEq(7, val, "7a");
        assertEq(128, mask, "7b");

        (val, mask) = impl.calc("8");
        assertEq(8, val, "8a");
        assertEq(256, mask, "8b");

        (val, mask) = impl.calc("9");
        assertEq(9, val, "9a");
        assertEq(512, mask, "9b");

        (val, mask) = impl.calc("10");
        assertEq(10, val, "10a");
        assertEq(1024, mask, "10b");

        (val, mask) = impl.calc("25");
        assertEq(25, val, "25a");
        assertEq(1 << 25, mask, "25b");

        (val, mask) = impl.calc("50");
        assertEq(50, val, "50a");
        assertEq(1 << 50, mask, "50b");

        (val, mask) = impl.calc("75");
        assertEq(75, val, "75a");
        assertEq(1 << 75, mask, "75b");

        (val, mask) = impl.calc("100");
        assertEq(100, val, "100a");
        assertEq(1 << 100, mask, "100b");
    }

    function testTwoNumAdd() public view {
        (uint256 val, uint256 mask) = impl.calc("1+2");
        assertEq(3, val, "1+2a");
        assertEq(6, mask, "1+2b");

        (val, mask) = impl.calc("100+9");
        assertEq(109, val, "100+9a");
        assertEq(1 << 100 | 1 << 9, mask, "100+9b");

        (val, mask) = impl.calc("100+9");
        assertEq(109, val, "100+9a");
        assertEq(1 << 100 | 1 << 9, mask, "100+9b");

        (val, mask) = impl.calc("75+25");
        assertEq(100, val, "75+25a");
        assertEq(1 << 75 | 1 << 25, mask, "75+25b");

        (val, mask) = impl.calc("50+7");
        assertEq(57, val, "50+7a");
        assertEq(1 << 50 | 1 << 7, mask, "50+7b");
    }

    function testTwoNumMul() public view {
        (uint256 val, uint256 mask) = impl.calc("5*100");
        assertEq(500, val, "5*100a");
        assertEq(1 << 5 | 1 << 100, mask, "5*100b");
    }

    function testTwoNumSub() public view {
        (uint256 val, uint256 mask) = impl.calc("100-25");
        assertEq(75, val, "100-25");
        assertEq(1 << 25 | 1 << 100, mask, "5*100b");
    }

    function testTwoNumDiv() public view {
        (uint256 val, uint256 mask) = impl.calc("8/4");
        assertEq(2, val, "8/4a");
        assertEq(1 << 8 | 1 << 4, mask, "8/4b");
    }

    function testScenarios() public view {
        (uint256 val, uint256 mask) = impl.calc("5*100+1");
        assertEq(501, val, "5*100+1a");
        assertEq(1 << 5 | 1 << 100 | 2, mask, "5*100+1b");

        (val, mask) = impl.calc("1+5*100");
        assertEq(501, val, "1+5*100a");

        (val, mask) = impl.calc("1+100/5");
        assertEq(21, val, "1+100/5a");

        (val, mask) = impl.calc("100/5+1");
        assertEq(21, val, "100/5+1a");

        (val, mask) = impl.calc("5*100/4");
        assertEq(125, val, "5*100/4a");

        (val, mask) = impl.calc("100-8*25/4");
        assertEq(50, val, "100-8*25/4a");

        (val, mask) = impl.calc("8*25/4-2");
        assertEq(48, val, "8*25/4-2a");

        (val, mask) = impl.calc("1+2+3-5");
        assertEq(1, val, "1+2+3-5a");

        (val, mask) = impl.calc("2*10+3-5*4");
        assertEq(3, val, "2*10+3-5*4a");
    }

    function testBrackets() public view {
        (uint256 val, uint256 mask) = impl.calc("5*(100+1)");
        assertEq(505, val, "5*(100+1)a");
        assertEq(1 << 5 | 1 << 100 | 2, mask, "5*(100+1)b");

        (val, mask) = impl.calc("(1+5)*100");
        assertEq(600, val, "(1+5)*100a");

        (val, mask) = impl.calc("(100+25)/(4+1)");
        assertEq(25, val, "(100+25)/(4+1)a");
    }

}
