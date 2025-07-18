// Copyright Whatgame Studios 2024 - 2025
// SPDX-License-Identifier: PROPRIETORY
// solhint-disable not-rely-on-time

pragma solidity ^0.8.24;

// solhint-disable-next-line no-global-import
import "forge-std/Test.sol";
import {FourteenNumbersSolutionsOperationalTest} from "./FourteenNumbersSolutionsOperational.t.sol";
import {FourteenNumbersSolutions} from "../src/FourteenNumbersSolutions.sol";
import {ERC1967Proxy} from "@openzeppelin/contracts/proxy/ERC1967/ERC1967Proxy.sol";


contract FourteenNumbersSolutionsOperationalV1Test is FourteenNumbersSolutionsOperationalTest {
    function setUp() public virtual override {
        super.setUp();
        deployV1();
    }

    function testCheckInDay34() public {
        // 	Sat Jan 04 2025 13:00:00 GMT+0000
        vm.warp(1735995600);
        // Min game day is 34, max is 35

        vm.prank(player1);
        fourteenNumbersSolutions.checkIn(34);

        (uint32 firstGameDay, uint32 mostRecentGameDay, uint256 totalPoints, uint256 daysPlayed) = 
            fourteenNumbersSolutions.stats(player1);
        assertEq(firstGameDay, 34);
        assertEq(mostRecentGameDay, 0);
        assertEq(totalPoints, 0);
        assertEq(daysPlayed, 0);
    }

    function testCheckInDay35() public {
        // 	Sat Jan 04 2025 13:00:00 GMT+0000
        vm.warp(1735995600);
        // Min game day is 34, max is 35

        vm.prank(player1);
        fourteenNumbersSolutions.checkIn(34);
        vm.prank(player1);
        fourteenNumbersSolutions.checkIn(35);

        (uint32 firstGameDay, uint32 mostRecentGameDay, uint256 totalPoints, uint256 daysPlayed) = 
            fourteenNumbersSolutions.stats(player1);
        assertEq(firstGameDay, 34);
        assertEq(mostRecentGameDay, 0);
        assertEq(totalPoints, 0);
        assertEq(daysPlayed, 0);
    }

    function testStoreResults() public {
        // 	Sat Jan 04 2025 13:00:00 GMT+0000
        vm.warp(1735995600);
        // Min game day is 34, max is 35

        bytes memory sol1 = "(100+10+1)*4"; // 444: perfect: 70 points
        bytes memory sol2 = "75*6";         // 450: 6 off:   44 points
        bytes memory sol3 = "50*9-7";       // 443: 1 off:   49 points

        vm.prank(player1);
        fourteenNumbersSolutions.storeResults(34, sol1, sol2, sol3, true);

        (uint32 firstGameDay, uint32 mostRecentGameDay, uint256 totalPoints, uint256 daysPlayed) = 
            fourteenNumbersSolutions.stats(player1);
        assertEq(firstGameDay, 34, "first game day 1");
        assertEq(mostRecentGameDay, 34, "most recent game day1");
        assertEq(totalPoints, 163, "total points1");
        assertEq(daysPlayed, 1, "days played 1");
    }

    function testStoreResultsTwoDays() public {
        // 	Sat Jan 04 2025 13:00:00 GMT+0000
        vm.warp(1735995600);
        // Min game day is 34, max is 35

        bytes memory sol1 = "(100+10+1)*4"; // 444: perfect: 70 points
        bytes memory sol2 = "75*6";         // 450: 6 off:   44 points
        bytes memory sol3 = "50*9-7";       // 443: 1 off:   49 points

        vm.prank(player1);
        fourteenNumbersSolutions.storeResults(34, sol1, sol2, sol3, true);

        (uint32 firstGameDay, uint32 mostRecentGameDay, uint256 totalPoints, uint256 daysPlayed) = 
            fourteenNumbersSolutions.stats(player1);
        assertEq(firstGameDay, 34);
        assertEq(mostRecentGameDay, 34);
        assertEq(totalPoints, 163);
        assertEq(daysPlayed, 1);

        sol1 = "100*6-10";     // 590: 15 off:  35 points
        sol2 = "75*8-25";      // 575: perfect: 70 points
        sol3 = "(50-2)*(9+3)"; // 576: 1 off:   49 points

        vm.prank(player1);
        vm.expectEmit(true, true, true, true);
        emit FourteenNumbersSolutions.Congratulations(player1, sol1, sol2, sol3, 154);
        fourteenNumbersSolutions.storeResults(35, sol1, sol2, sol3, true);

        (firstGameDay, mostRecentGameDay, totalPoints, daysPlayed) = 
            fourteenNumbersSolutions.stats(player1);
        assertEq(firstGameDay, 34);
        assertEq(mostRecentGameDay, 35);
        assertEq(totalPoints, 163 + 154);
        assertEq(daysPlayed, 2);
    }

    function testDontStoreResults() public {
        // 	Sat Jan 04 2025 13:00:00 GMT+0000
        vm.warp(1735995600);
        // Min game day is 34, max is 35

        (bytes memory combinedSolution, uint256 points, address player) = 
            fourteenNumbersSolutions.solutions(34);
        assertEq(combinedSolution, bytes(""));
        assertEq(points, 0, "points");
        assertEq(player, address(0), "player");


        bytes memory sol1 = "(100+10+1)*4"; // 444: perfect: 70 points
        bytes memory sol2 = "75*6";         // 450: 6 off:   44 points
        bytes memory sol3 = "50*9-7";       // 443: 1 off:   49 points

        vm.expectEmit(true, true, true, true);
        emit FourteenNumbersSolutions.Congratulations(player1, sol1, sol2, sol3, 163);
        vm.prank(player1);
        fourteenNumbersSolutions.storeResults(34, sol1, sol2, sol3, false);

        (uint32 firstGameDay, uint32 mostRecentGameDay, uint256 totalPoints, uint256 daysPlayed) = 
            fourteenNumbersSolutions.stats(player1);
        assertEq(firstGameDay, 0, "first game day 1");
        assertEq(mostRecentGameDay, 0, "most recent game day1");
        assertEq(totalPoints, 0, "total points1");
        assertEq(daysPlayed, 0, "days played 1");

        (combinedSolution, points, player) = 
            fourteenNumbersSolutions.solutions(34);
        assertEq(combinedSolution, "(100+10+1)*4=75*6=50*9-7", "Combined solution1");
        assertEq(points, 163, "points");
        assertEq(player, player1, "player");
    }

    // Best solution can be set with a second attempt, but the player's stats
    // don't get updated
    function testStoreResultsAttemptToUpdateResult() public {
        // 	Sat Jan 04 2025 13:00:00 GMT+0000
        vm.warp(1735995600);
        // Min game day is 34, max is 35

        bytes memory sol1 = "(100+10+1)*4"; // 444: perfect: 70 points
        bytes memory sol2 = "75*6";         // 450: 6 off:   44 points
        bytes memory sol3 = "50*9";         // 450: 6 off:   44 points

        vm.expectEmit(true, true, true, true);
        emit FourteenNumbersSolutions.Congratulations(player1, sol1, sol2, sol3, 158);
        vm.prank(player1);
        fourteenNumbersSolutions.storeResults(34, sol1, sol2, sol3, true);

        sol1 = "(100+10+1)*4"; // 444: perfect: 70 points
        sol2 = "75*6";         // 450: 6 off:   44 points
        sol3 = "50*9-7";       // 443: 1 off:   49 points

        vm.expectEmit(true, true, true, true);
        emit FourteenNumbersSolutions.Congratulations(player1, sol1, sol2, sol3, 163);
        vm.prank(player1);
        fourteenNumbersSolutions.storeResults(34, sol1, sol2, sol3, true);

        (bytes memory combinedSolution, uint256 points, address player) = 
            fourteenNumbersSolutions.solutions(34);
        assertEq(combinedSolution, "(100+10+1)*4=75*6=50*9-7", "Combined solution1");
        assertEq(points, 163, "points");
        assertEq(player, player1, "player");

        (uint32 firstGameDay, uint32 mostRecentGameDay, uint256 totalPoints, uint256 daysPlayed) = 
            fourteenNumbersSolutions.stats(player1);
        assertEq(firstGameDay, 34, "first game day 1");
        assertEq(mostRecentGameDay, 34, "most recent game day1");
        assertEq(totalPoints, 158, "total points1");
        assertEq(daysPlayed, 1, "days played 1");
    }

}
