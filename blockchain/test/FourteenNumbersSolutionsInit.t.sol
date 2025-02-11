// Copyright Whatgame Studios 2024 - 2025
// SPDX-License-Identifier: PROPRIETORY
// solhint-disable not-rely-on-time

pragma solidity ^0.8.24;

// solhint-disable-next-line no-global-import
import "forge-std/Test.sol";
import {FourteenNumbersSolutionsBaseTest} from "./FourteenNumbersSolutionsBase.t.sol";
import {FourteenNumbersSolutions} from "../src/FourteenNumbersSolutions.sol";

abstract contract FourteenNumbersSolutionsInitTest is FourteenNumbersSolutionsBaseTest {

    function testInit() public view {
        assertEq(fourteenNumbersSolutions.owner(), owner);
        assertTrue(fourteenNumbersSolutions.hasRole(upgradeRole, upgradeAdmin));
        assertTrue(fourteenNumbersSolutions.hasRole(defaultAdminRole, roleAdmin));

        (uint32 firstGameDay, uint32 mostRecentGameDay, uint256 totalPoints, uint256 daysPlayed) = 
            fourteenNumbersSolutions.stats(player1);
        assertEq(firstGameDay, 0);
        assertEq(mostRecentGameDay, 0);
        assertEq(totalPoints, 0);
        assertEq(daysPlayed, 0);

        (bytes memory combinedSolution, uint256 points, address player) = 
            fourteenNumbersSolutions.solutions(0);
        assertEq(combinedSolution, "");
        assertEq(points, 0);
        assertEq(player, address(0));
    }
}
