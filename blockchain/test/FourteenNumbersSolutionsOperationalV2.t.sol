// Copyright Whatgame Studios 2024 - 2025
// SPDX-License-Identifier: PROPRIETORY
// solhint-disable not-rely-on-time

pragma solidity ^0.8.24;

// solhint-disable-next-line no-global-import
import "forge-std/Test.sol";
import {FourteenNumbersSolutionsOperationalTest} from "./FourteenNumbersSolutionsOperational.t.sol";
import {FourteenNumbersSolutions} from "../src/FourteenNumbersSolutions.sol";
import {FourteenNumbersSolutionsV2} from "../src/FourteenNumbersSolutionsV2.sol";
import {ERC1967Proxy} from "@openzeppelin/contracts/proxy/ERC1967/ERC1967Proxy.sol";


contract FourteenNumbersSolutionsOperationalV2Test is FourteenNumbersSolutionsOperationalTest {
    FourteenNumbersSolutionsV2 fourteenNumbersSolutionsV2;

    function setUp() public virtual override {
        super.setUp();

        FourteenNumbersSolutions impl = new FourteenNumbersSolutions();
        bytes memory initData = abi.encodeWithSelector(
            FourteenNumbersSolutions.initialize.selector, roleAdmin, owner, upgradeAdmin);
        proxy = new ERC1967Proxy(address(impl), initData);
        fourteenNumbersSolutions = FourteenNumbersSolutions(address(proxy));

        FourteenNumbersSolutionsV2 v2Impl = new FourteenNumbersSolutionsV2();
        initData = abi.encodeWithSelector(FourteenNumbersSolutionsV2.upgradeStorage.selector, bytes(""));
        vm.prank(upgradeAdmin);
        fourteenNumbersSolutions.upgradeToAndCall(address(v2Impl), initData);

        uint256 ver = fourteenNumbersSolutions.version();
        assertEq(ver, 2, "Upgrade did not upgrade version");

        fourteenNumbersSolutionsV2 = FourteenNumbersSolutionsV2(address(fourteenNumbersSolutions));
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
        assertEq(mostRecentGameDay, 34);
        assertEq(totalPoints, 0);
        assertEq(daysPlayed, 1);
        assertEq(fourteenNumbersSolutionsV2.numberOfPlayers(34), 1);
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
        assertEq(mostRecentGameDay, 35);
        assertEq(totalPoints, 0);
        assertEq(daysPlayed, 2);
        assertEq(fourteenNumbersSolutionsV2.numberOfPlayers(34), 1);
        assertEq(fourteenNumbersSolutionsV2.numberOfPlayers(35), 1);
    }

    function testCheckInDay34TwoPlayers() public {
        // 	Sat Jan 04 2025 13:00:00 GMT+0000
        vm.warp(1735995600);
        // Min game day is 34, max is 35

        vm.prank(player1);
        fourteenNumbersSolutions.checkIn(34);
        vm.prank(player2);
        fourteenNumbersSolutions.checkIn(34);

        assertEq(fourteenNumbersSolutionsV2.numberOfPlayers(34), 2);
    }

}
