// Copyright Whatgame Studios 2024 - 2025
// SPDX-License-Identifier: PROPRIETORY
// solhint-disable not-rely-on-time

pragma solidity ^0.8.24;

// solhint-disable-next-line no-global-import
import "forge-std/Test.sol";
import {FourteenNumbersSolutionsOperationalTest} from "./FourteenNumbersSolutionsOperational.t.sol";
import {FourteenNumbersSolutions} from "../src/FourteenNumbersSolutions.sol";
import {FourteenNumbersSolutionsV2} from "../src/FourteenNumbersSolutionsV2.sol";
import {FourteenNumbersSolutionsV3} from "../src/FourteenNumbersSolutionsV3.sol";
import {ERC1967Proxy} from "@openzeppelin/contracts/proxy/ERC1967/ERC1967Proxy.sol";


contract FourteenNumbersSolutionsOperationalV3Test is FourteenNumbersSolutionsOperationalTest {
    FourteenNumbersSolutionsV3 fourteenNumbersSolutionsV3;

    function setUp() public virtual override {
        super.setUp();
        deployV3();

        fourteenNumbersSolutionsV3 = FourteenNumbersSolutionsV3(address(fourteenNumbersSolutions));
    }

    function testCheckInDay34() public {
        // 	Sat Jan 04 2025 13:00:00 GMT+0000
        vm.warp(1735995600);
        // Min game day is 34, max is 35

        vm.prank(player1);
        vm.expectEmit(true, true, true, true);
        emit FourteenNumbersSolutionsV3.CheckIn(34, player1, 1);
        fourteenNumbersSolutions.checkIn(34);

        (uint32 firstGameDay, uint32 mostRecentGameDay, uint256 totalPoints, uint256 daysPlayed) = 
            fourteenNumbersSolutions.stats(player1);
        assertEq(firstGameDay, 0);
        assertEq(mostRecentGameDay, 34);
        assertEq(totalPoints, 0);
        assertEq(daysPlayed, 1);
    }

    function testCheckInDay35() public {
        // 	Sat Jan 04 2025 13:00:00 GMT+0000
        vm.warp(1735995600);
        // Min game day is 34, max is 35

        vm.prank(player1);
        vm.expectEmit(true, true, true, true);
        emit FourteenNumbersSolutionsV3.CheckIn(34, player1, 1);
        fourteenNumbersSolutions.checkIn(34);
        vm.prank(player1);
        vm.expectEmit(true, true, true, true);
        emit FourteenNumbersSolutionsV3.CheckIn(35, player1, 2);
        fourteenNumbersSolutions.checkIn(35);

        (uint32 firstGameDay, uint32 mostRecentGameDay, uint256 totalPoints, uint256 daysPlayed) = 
            fourteenNumbersSolutions.stats(player1);
        assertEq(firstGameDay, 0);
        assertEq(mostRecentGameDay, 35);
        assertEq(totalPoints, 0);
        assertEq(daysPlayed, 2);
    }

    function testCheckInSameDayTwice() public {
        // 	Sat Jan 04 2025 13:00:00 GMT+0000
        vm.warp(1735995600);
        // Min game day is 34, max is 35

        vm.prank(player1);
        fourteenNumbersSolutions.checkIn(34);

        // This doesn't revert - just quietly ignores.
        fourteenNumbersSolutions.checkIn(34);

        (uint32 firstGameDay, uint32 mostRecentGameDay, uint256 totalPoints, uint256 daysPlayed) = 
            fourteenNumbersSolutions.stats(player1);
        assertEq(firstGameDay, 0);
        assertEq(mostRecentGameDay, 34);
        assertEq(totalPoints, 0);
        assertEq(daysPlayed, 1);
    }


    function testStoreBestScore() public override {
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
        emit FourteenNumbersSolutionsV3.BestScoreToday(34, player1, "(100+10+1)*4=75*6=50*9-7", 163, 0);
        vm.prank(player1);
        fourteenNumbersSolutions.storeResults(34, sol1, sol2, sol3, true);

        (combinedSolution, points, player) = 
            fourteenNumbersSolutions.solutions(34);
        assertEq(combinedSolution, "(100+10+1)*4=75*6=50*9-7", "Combined solution1");
        assertEq(points, 163, "points");
        assertEq(player, player1, "player");
    }

    function testStoreResultsBetterResult() public override {
        // 	Sat Jan 04 2025 13:00:00 GMT+0000
        vm.warp(1735995600);
        // Min game day is 34, max is 35

        bytes memory sol1 = "(100+10+1)*4"; // 444: perfect: 70 points
        bytes memory sol2 = "75*6";         // 450: 6 off:   44 points
        bytes memory sol3 = "50*9";         // 450: 6 off:   44 points

        vm.expectEmit(true, true, true, true);
        emit FourteenNumbersSolutionsV3.BestScoreToday(34, player1, "(100+10+1)*4=75*6=50*9", 158, 0);
        vm.prank(player1);
        fourteenNumbersSolutions.storeResults(34, sol1, sol2, sol3, true);

        (bytes memory combinedSolution, uint256 points, address player) = 
            fourteenNumbersSolutions.solutions(34);
        assertEq(combinedSolution, bytes("(100+10+1)*4=75*6=50*9"));
        assertEq(points, 158, "points");
        assertEq(player, player1, "player");

        sol1 = "(100+10+1)*4"; // 444: perfect: 70 points
        sol2 = "75*6";         // 450: 6 off:   44 points
        sol3 = "50*9-7";       // 443: 1 off:   49 points

        vm.expectEmit(true, true, true, true);
        emit FourteenNumbersSolutionsV3.BestScoreToday(34, player2, "(100+10+1)*4=75*6=50*9-7", 163, 158);
        vm.prank(player2);
        fourteenNumbersSolutions.storeResults(34, sol1, sol2, sol3, true);

        (combinedSolution, points, player) = 
            fourteenNumbersSolutions.solutions(34);
        assertEq(combinedSolution, "(100+10+1)*4=75*6=50*9-7", "Combined solution1");
        assertEq(points, 163, "points");
        assertEq(player, player2, "player");
    }

    function testStoreResultsNotBetterResult() public override {
        // 	Sat Jan 04 2025 13:00:00 GMT+0000
        vm.warp(1735995600);
        // Min game day is 34, max is 35

        bytes memory sol1 = "(100+10+1)*4"; // 444: perfect: 70 points
        bytes memory sol2 = "75*6";         // 450: 6 off:   44 points
        bytes memory sol3 = "50*9";         // 450: 6 off:   44 points

        vm.expectEmit(true, true, true, true);
        emit FourteenNumbersSolutionsV3.BestScoreToday(34, player1, "(100+10+1)*4=75*6=50*9", 158, 0);
        vm.prank(player1);
        fourteenNumbersSolutions.storeResults(34, sol1, sol2, sol3, true);

        sol1 = "(100+10+1)*4"; // 444: perfect: 70 points
        sol2 = "75*6";         // 450: 6 off:   44 points
        sol3 = "50*9+7";       // 457: 13off:   37 points

        vm.expectEmit(true, true, true, true);
        emit FourteenNumbersSolutionsV3.BetterScoreAlready(34, player2, "(100+10+1)*4=75*6=50*9+7", 151, 158);
        vm.prank(player2);
        fourteenNumbersSolutions.storeResults(34, sol1, sol2, sol3, true);

        (bytes memory combinedSolution, uint256 points, address player) = 
            fourteenNumbersSolutions.solutions(34);
        assertEq(combinedSolution, "(100+10+1)*4=75*6=50*9", "Combined solution1");
        assertEq(points, 158, "points");
        assertEq(player, player1, "player");
    }

}
