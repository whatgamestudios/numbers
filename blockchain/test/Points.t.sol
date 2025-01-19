// Copyright Whatgame Studios 2024 - 2025
// SPDX-License-Identifier: PROPRIETORY
// solhint-disable not-rely-on-time

pragma solidity ^0.8.24;

// solhint-disable-next-line no-global-import
import "forge-std/Test.sol";
import {Points} from "../src/Points.sol";

contract PointsImpl is Points {

}

contract PointsTest is Test {
    PointsImpl impl;

    function setUp() public {
        impl = new PointsImpl();
    }

    function testCalcPointsSingle() public view {
        assertEq(impl.calcPointsSingle(500, 500), 70, "Perfect");

        assertEq(impl.calcPointsSingle(500, 499), 49, "Less1");
        assertEq(impl.calcPointsSingle(500, 475), 25, "Less25");
        assertEq(impl.calcPointsSingle(500, 451), 1, "Less49");
        assertEq(impl.calcPointsSingle(500, 450), 0, "Less50");
        assertEq(impl.calcPointsSingle(500, 400), 0, "Less100");

        assertEq(impl.calcPointsSingle(500, 501), 49, "Plus1");
        assertEq(impl.calcPointsSingle(500, 525), 25, "Plus25");
        assertEq(impl.calcPointsSingle(500, 549), 1, "Plus49");
        assertEq(impl.calcPointsSingle(500, 550), 0, "Plus50");
        assertEq(impl.calcPointsSingle(500, 600), 0, "Plus100");
    }

    function testCalcPoints() public view {
        assertEq(impl.calcPoints(500, 500, 500, 500), 210, "Perfect");

        assertEq(impl.calcPointsSingle(500, 546), 4, "Plus46");
        assertEq(impl.calcPointsSingle(500, 541), 9, "Plus41");
        assertEq(impl.calcPointsSingle(500, 540), 10, "Plus40");
        assertEq(impl.calcPoints(500, 540, 541, 546), 23, "Not so perfect");
    }
}
