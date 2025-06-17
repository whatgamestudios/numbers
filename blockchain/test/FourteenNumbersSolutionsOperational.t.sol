// Copyright Whatgame Studios 2024 - 2025
// SPDX-License-Identifier: PROPRIETORY
// solhint-disable not-rely-on-time

pragma solidity ^0.8.24;

// solhint-disable-next-line no-global-import
import "forge-std/Test.sol";
import {FourteenNumbersSolutionsBaseTest} from "./FourteenNumbersSolutionsBase.t.sol";
import {FourteenNumbersSolutions} from "../src/FourteenNumbersSolutions.sol";
import {GameDayCheck} from "../src/GameDayCheck.sol";
import {CalcProcessor} from "../src/CalcProcessor.sol";

abstract contract FourteenNumbersSolutionsOperationalTest is FourteenNumbersSolutionsBaseTest {

    function testCheckInInvalidDay() public {
        // 	Sat Jan 04 2025 13:00:00 GMT+0000
        vm.warp(1735995600);
        // Min game day is 34, max is 35

        vm.expectRevert(abi.encodeWithSelector(GameDayCheck.GameDayInvalid.selector, 33, 34, 35));
        vm.prank(player1);
        fourteenNumbersSolutions.checkIn(33);
        vm.expectRevert(abi.encodeWithSelector(GameDayCheck.GameDayInvalid.selector, 36, 34, 35));
        vm.prank(player1);
        fourteenNumbersSolutions.checkIn(36);
    }

    function testStoreBestScore() public virtual {
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
        fourteenNumbersSolutions.storeResults(34, sol1, sol2, sol3, true);

        (combinedSolution, points, player) = 
            fourteenNumbersSolutions.solutions(34);
        assertEq(combinedSolution, "(100+10+1)*4=75*6=50*9-7", "Combined solution1");
        assertEq(points, 163, "points");
        assertEq(player, player1, "player");
    }

    function testStoreResultsBetterResult() public virtual {
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

        (bytes memory combinedSolution, uint256 points, address player) = 
            fourteenNumbersSolutions.solutions(34);
        assertEq(combinedSolution, bytes("(100+10+1)*4=75*6=50*9"));
        assertEq(points, 158, "points");
        assertEq(player, player1, "player");

        sol1 = "(100+10+1)*4"; // 444: perfect: 70 points
        sol2 = "75*6";         // 450: 6 off:   44 points
        sol3 = "50*9-7";       // 443: 1 off:   49 points

        vm.expectEmit(true, true, true, true);
        emit FourteenNumbersSolutions.Congratulations(player2, sol1, sol2, sol3, 163);
        vm.prank(player2);
        fourteenNumbersSolutions.storeResults(34, sol1, sol2, sol3, true);

        (combinedSolution, points, player) = 
            fourteenNumbersSolutions.solutions(34);
        assertEq(combinedSolution, "(100+10+1)*4=75*6=50*9-7", "Combined solution1");
        assertEq(points, 163, "points");
        assertEq(player, player2, "player");
    }

    function testStoreResultsNotBetterResult() public virtual {
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
        sol3 = "50*9+7";       // 457: 13off:   37 points

        vm.expectEmit(true, true, true, true);
        emit FourteenNumbersSolutions.NextTime(player2, sol1, sol2, sol3, 151, 158);
        vm.prank(player2);
        fourteenNumbersSolutions.storeResults(34, sol1, sol2, sol3, true);

        (bytes memory combinedSolution, uint256 points, address player) = 
            fourteenNumbersSolutions.solutions(34);
        assertEq(combinedSolution, "(100+10+1)*4=75*6=50*9", "Combined solution1");
        assertEq(points, 158, "points");
        assertEq(player, player1, "player");
    }

    function testStoreResultsRepeatedNumbers() public {
        // 	Sat Jan 04 2025 13:00:00 GMT+0000
        vm.warp(1735995600);
        // Min game day is 34, max is 35

        // 1 repeated between sol1 and sol3
        bytes memory sol1 = "(100+10+1)*4";
        bytes memory sol2 = "75*6";    
        bytes memory sol3 = "50*9-7+1";
        vm.prank(player1);
        vm.expectRevert(FourteenNumbersSolutions.NumbersRepeated.selector);
        fourteenNumbersSolutions.storeResults(34, sol1, sol2, sol3, true);

        // 1 repeated between sol1 and sol2
        sol1 = "(100+10+1)*4";
        sol2 = "75*6+1";    
        sol3 = "50*9-7";
        vm.prank(player1);
        vm.expectRevert(FourteenNumbersSolutions.NumbersRepeated.selector);
        fourteenNumbersSolutions.storeResults(34, sol1, sol2, sol3, true);

        // 1 repeated between sol2 and sol3
        sol1 = "(100+10)*4";
        sol2 = "75*6+1";    
        sol3 = "50*9-7+1";
        vm.prank(player1);
        vm.expectRevert(FourteenNumbersSolutions.NumbersRepeated.selector);
        fourteenNumbersSolutions.storeResults(34, sol1, sol2, sol3, true);
    }

    function testStoreResultsInvalidSolution() public {
        // 	Sat Jan 04 2025 13:00:00 GMT+0000
        vm.warp(1735995600);
        // Min game day is 34, max is 35

        bytes memory sol1 = "(100+10+1*4";
        bytes memory sol2 = "75*6";    
        bytes memory sol3 = "50*9-7";
        vm.prank(player1);
        vm.expectRevert(CalcProcessor.NoMatchingRightBracket.selector);
        fourteenNumbersSolutions.storeResults(34, sol1, sol2, sol3, true);

        sol1 = "?1*4";
        sol2 = "75*6";    
        sol3 = "50*9-7";
        vm.prank(player1);
        vm.expectRevert(CalcProcessor.UnknownSymbol.selector);
        fourteenNumbersSolutions.storeResults(34, sol1, sol2, sol3, true);
    }
}
