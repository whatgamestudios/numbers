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

    function testDetermineCurrentGameDays() public {
        // Move to January 7, 7:22:37 GMT
        vm.warp(1736234560);

        uint32 gameDay = uint32((block.timestamp - UNIX_TIME_GAME_START) / SECONDS_PER_DAY);
        (uint32 minGameDay, uint32 maxGameDay) = impl.determineCurrentGameDays();
        // console.log("game day: %s", gameDay);
        // console.log("min game day: %s", minGameDay);
        // console.log("max game day: %s", maxGameDay);
        assertTrue(gameDay >= minGameDay && gameDay <= maxGameDay, "Game day incorrect");
    }

    function testTargetValue() public {
        // Move to January 7, 7:22:37 GMT
        vm.warp(1736290738);
        uint32 gameDay = uint32((block.timestamp - UNIX_TIME_GAME_START) / SECONDS_PER_DAY);
        console.log("game day: %s", gameDay);
        gameDay = 38;
        console.log("game day: %s", gameDay);

        uint256 target = impl.getTargetValue(gameDay);

        console.log("target: %s", target);
        assertEq(target, 948, "Wrong target");
    }


}
