// Copyright Whatgame Studios 2024 - 2025
// SPDX-License-Identifier: PROPRIETORY
// solhint-disable not-rely-on-time

pragma solidity ^0.8.24;

// solhint-disable-next-line no-global-import
import "forge-std/Test.sol";
import {FourteenNumbersSolutionsOperationalV3Test} from "./FourteenNumbersSolutionsOperationalV3.t.sol";
import {FourteenNumbersSolutions} from "../src/FourteenNumbersSolutions.sol";
import {FourteenNumbersSolutionsV2} from "../src/FourteenNumbersSolutionsV2.sol";
import {FourteenNumbersSolutionsV3} from "../src/FourteenNumbersSolutionsV3.sol";
import {FourteenNumbersSolutionsV4} from "../src/FourteenNumbersSolutionsV4.sol";
import {ERC1967Proxy} from "@openzeppelin/contracts/proxy/ERC1967/ERC1967Proxy.sol";


contract FourteenNumbersSolutionsOperationalV4Test is FourteenNumbersSolutionsOperationalV3Test {
    FourteenNumbersSolutionsV4 fourteenNumbersSolutionsV4;

    function setUp() public virtual override {
        super.setUp();
        deployV4();

        fourteenNumbersSolutionsV3 = FourteenNumbersSolutionsV3(address(fourteenNumbersSolutions));
        fourteenNumbersSolutionsV4 = FourteenNumbersSolutionsV4(address(fourteenNumbersSolutions));
    }

    function testStoreResultsSameResult() public override {
        // 	Sat Jan 04 2025 13:00:00 GMT+0000
        vm.warp(1735995600);
        // Min game day is 34, max is 35

        bytes memory sol1 = "(100+10+1)*4"; // 444: perfect: 70 points
        bytes memory sol2 = "75*6";         // 450: 6 off:   44 points
        bytes memory sol3 = "50*9";         // 450: 6 off:   44 points

        vm.expectEmit(true, true, true, true);
        emit FourteenNumbersSolutionsV4.BestScoreToday(34, player1, "(100+10+1)*4=75*6=50*9", 158, 0);
        vm.prank(player1);
        fourteenNumbersSolutions.storeResults(34, sol1, sol2, sol3, true);

        vm.expectEmit(true, true, true, true);
        emit FourteenNumbersSolutionsV4.DuplicateBestScore(34, player3, "(100+10+1)*4=75*6=50*9", 158, 158);
        vm.prank(player3);
        fourteenNumbersSolutions.storeResults(34, sol1, sol2, sol3, true);

        sol2 = "(100+10+1)*4"; // 444: perfect: 70 points
        sol3 = "75*6";         // 450: 6 off:   44 points
        sol1 = "50*9";         // 450: 6 off:   44 points

        vm.expectEmit(true, true, true, true);
        emit FourteenNumbersSolutionsV4.EqualBestScoreToday(34, player2, "50*9=(100+10+1)*4=75*6", 158, 158);
        vm.prank(player2);
        fourteenNumbersSolutions.storeResults(34, sol1, sol2, sol3, true);

        (bytes memory combinedSolution, uint256 points, address player) = 
            fourteenNumbersSolutions.solutions(34);
        assertEq(combinedSolution, "(100+10+1)*4=75*6=50*9", "Combined solution1");
        assertEq(points, 158, "points");
        assertEq(player, player1, "player");
    }

    function testGetAllSolutions() public {
        // 	Sat Jan 04 2025 13:00:00 GMT+0000
        vm.warp(1735995600);
        // Min game day is 34, max is 35

        // No solutions.
        (uint256 points, FourteenNumbersSolutionsV4.ExtraSolution[] memory solutions) = 
            fourteenNumbersSolutionsV4.getAllSolutions(34);
        assertEq(points, 0, "points0");
        assertEq(solutions.length, 0, "num solutions0");

        // 1 solution
        bytes memory sol1 = "(100+10+1)*4"; // 444: perfect: 70 points
        bytes memory sol2 = "75*6";         // 450: 6 off:   44 points
        bytes memory sol3 = "50*9";         // 450: 6 off:   44 points
        vm.prank(player1);
        fourteenNumbersSolutions.storeResults(34, sol1, sol2, sol3, true);

        (points, solutions) = fourteenNumbersSolutionsV4.getAllSolutions(34);
        assertEq(points, 158, "points1");
        assertEq(solutions.length, 1, "num solutions1");
        assertEq(solutions[0].player, player1, "solutions[0].player");
        assertEq(solutions[0].combinedSolution, "(100+10+1)*4=75*6=50*9", "solutions[0].combinedSolution");

        // 2 solutions
        bytes memory temp = sol3;
        sol3 = sol2;
        sol2 = sol1;
        sol1 = temp;
        vm.prank(player2);
        fourteenNumbersSolutions.storeResults(34, sol1, sol2, sol3, true);

        (points, solutions) = fourteenNumbersSolutionsV4.getAllSolutions(34);
        assertEq(points, 158, "points1");
        assertEq(solutions.length, 2, "num solutions2");
        assertEq(solutions[0].player, player1, "solutions[0].player");
        assertEq(solutions[0].combinedSolution, "(100+10+1)*4=75*6=50*9", "solutions[0].combinedSolution");
        assertEq(solutions[1].player, player2, "solutions[1].player");
        assertEq(solutions[1].combinedSolution, "50*9=(100+10+1)*4=75*6", "solutions[1].combinedSolution");

        // 3 solutions
        temp = sol3;
        sol3 = sol2;
        sol2 = sol1;
        sol1 = temp;
        vm.prank(player3);
        fourteenNumbersSolutions.storeResults(34, sol1, sol2, sol3, true);

        (points, solutions) = fourteenNumbersSolutionsV4.getAllSolutions(34);
        assertEq(points, 158, "points1");
        assertEq(solutions.length, 3, "num solutions3");
        assertEq(solutions[0].player, player1, "solutions[0].player");
        assertEq(solutions[0].combinedSolution, "(100+10+1)*4=75*6=50*9", "solutions[0].combinedSolution");
        assertEq(solutions[1].player, player2, "solutions[1].player");
        assertEq(solutions[1].combinedSolution, "50*9=(100+10+1)*4=75*6", "solutions[1].combinedSolution");
        assertEq(solutions[2].player, player3, "solutions[2].player");
        assertEq(solutions[2].combinedSolution, "75*6=50*9=(100+10+1)*4", "solutions[1].combinedSolution");
    }
}
