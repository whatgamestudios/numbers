// Copyright Whatgame Studios 2024 - 2025
// SPDX-License-Identifier: PROPRIETORY
// solhint-disable not-rely-on-time

pragma solidity ^0.8.24;

// solhint-disable-next-line no-global-import
import "forge-std/Test.sol";
import {TargetValue} from "../src/TargetValue.sol";

contract TargetValueImpl is TargetValue {

}

contract TargetValueTest is Test {
    // GMT	Sun Dec 01 2024 00:00:00 GMT+0000
    uint256 public constant UNIX_TIME_GAME_START = 1733011200;
    // To convert to days: 24 x 60 x 60
    uint256 public constant SECONDS_PER_DAY = 86400;

    TargetValueImpl impl;

    function setUp() public {
        impl = new TargetValueImpl();
    }

    function testDetermineCurrentGameDaysAcrossThree() public {
        // 	Sat Jan 04 2025 11:00:00 GMT+0000
        vm.warp(1735988400);
        uint32 gmt0GameDay = uint32((block.timestamp - UNIX_TIME_GAME_START) / SECONDS_PER_DAY);
        (uint32 minGameDay, uint32 maxGameDay) = impl.determineCurrentGameDays();
        assertEq(minGameDay, 33, "Min game day1");
        assertEq(gmt0GameDay, 34, "GMT+0 game day1");
        assertEq(maxGameDay, 35, "Max game day1");
    }

    function testDetermineCurrentGameDaysPreviousDay() public {
        // 	Sat Jan 04 2025 09:00:00 GMT+0000
        vm.warp(1735981200);
        uint32 gmt0GameDay = uint32((block.timestamp - UNIX_TIME_GAME_START) / SECONDS_PER_DAY);
        (uint32 minGameDay, uint32 maxGameDay) = impl.determineCurrentGameDays();
        assertEq(minGameDay, 33, "Min game day2");
        assertEq(gmt0GameDay, 34, "GMT+0 game day2");
        assertEq(maxGameDay, 34, "Max game day2");
    }

    function testDetermineCurrentGameDaysNextDay() public {
        // 	Sat Jan 04 2025 13:00:00 GMT+0000
        vm.warp(1735995600);
        uint32 gmt0GameDay = uint32((block.timestamp - UNIX_TIME_GAME_START) / SECONDS_PER_DAY);
        (uint32 minGameDay, uint32 maxGameDay) = impl.determineCurrentGameDays();
        assertEq(minGameDay, 34, "Min game day3");
        assertEq(gmt0GameDay, 34, "GMT+0 game day3");
        assertEq(maxGameDay, 35, "Max game day3");
    }

    function testTargetValueDay10() public {
        // Wed Dec 11 2024 02:00:00 GMT+0000
        vm.warp(1733882400);
        // Check that game day will be value based on the UNIX TIME just set.
        uint32 gameDay = uint32((block.timestamp - UNIX_TIME_GAME_START) / SECONDS_PER_DAY);
        assertEq(gameDay, 10, "UNIX time not right1");

        uint256 target = impl.getTargetValue(gameDay);
        assertEq(target, 492, "Wrong target1");
    }

    function testTargetValueDay38() public {
        // Wed Jan 08 2025 02:00:00 GMT+0000
        vm.warp(1736301600);
        // Check that game day will be value based on the UNIX TIME just set.
        uint32 gameDay = uint32((block.timestamp - UNIX_TIME_GAME_START) / SECONDS_PER_DAY);
        assertEq(gameDay, 38, "UNIX time not right2");

        uint256 target = impl.getTargetValue(gameDay);
        assertEq(target, 948, "Wrong target2");
    }

    function testTargetValueDay39() public {
        // 	Thu Jan 09 2025 02:00:00 GMT+0000
        vm.warp(1736388000);
        // Check that game day will be value based on the UNIX TIME just set.
        uint32 gameDay = uint32((block.timestamp - UNIX_TIME_GAME_START) / SECONDS_PER_DAY);
        assertEq(gameDay, 39, "UNIX time not right3");

        uint256 target = impl.getTargetValue(gameDay);
        assertEq(target, 881, "Wrong target3");
    }
}
