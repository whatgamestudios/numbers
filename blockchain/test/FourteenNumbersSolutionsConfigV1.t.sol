// Copyright Whatgame Studios 2024 - 2025
// SPDX-License-Identifier: PROPRIETORY
// solhint-disable not-rely-on-time

pragma solidity ^0.8.24;

// solhint-disable-next-line no-global-import
import "forge-std/Test.sol";
import {FourteenNumbersSolutionsConfigTest} from "./FourteenNumbersSolutionsConfig.t.sol";
import {FourteenNumbersSolutions} from "../src/FourteenNumbersSolutions.sol";
import {ERC1967Proxy} from "@openzeppelin/contracts/proxy/ERC1967/ERC1967Proxy.sol";


contract FourteenNumbersSolutionsV2a is FourteenNumbersSolutions {
    function upgradeStorage(bytes memory /* _data */) external override {
        version = 1;
    }
}


contract FourteenNumbersSolutionsConfigV1Test is FourteenNumbersSolutionsConfigTest {
    function setUp() public virtual override {
        super.setUp();
        deployV1();
    }


    function testUpgradeToV1() public {
        FourteenNumbersSolutionsV2a v2Impl = new FourteenNumbersSolutionsV2a();
        bytes memory initData = abi.encodeWithSelector(FourteenNumbersSolutions.upgradeStorage.selector, bytes(""));
        vm.prank(upgradeAdmin);
        fourteenNumbersSolutions.upgradeToAndCall(address(v2Impl), initData);

        uint256 ver = fourteenNumbersSolutions.version();
        assertEq(ver, 1, "Upgrade did not upgrade version");
    }

    function testUpgradeToV0() public {
        FourteenNumbersSolutions v1Impl = new FourteenNumbersSolutions();
        bytes memory initData = abi.encodeWithSelector(FourteenNumbersSolutions.upgradeStorage.selector, bytes(""));
        vm.expectRevert(abi.encodeWithSelector(FourteenNumbersSolutions.CanNotUpgradeToLowerOrSameVersion.selector, 0));
        vm.prank(upgradeAdmin);
        fourteenNumbersSolutions.upgradeToAndCall(address(v1Impl), initData);
    }

    function testDowngradeV1ToV0() public {
        // Upgrade from V0 to V1
        FourteenNumbersSolutionsV2a v2Impl = new FourteenNumbersSolutionsV2a();
        bytes memory initData = abi.encodeWithSelector(FourteenNumbersSolutions.upgradeStorage.selector, bytes(""));
        vm.prank(upgradeAdmin);
        fourteenNumbersSolutions.upgradeToAndCall(address(v2Impl), initData);

        // Attempt to downgrade from V1 to V0.
        FourteenNumbersSolutions v1Impl = new FourteenNumbersSolutions();
        vm.expectRevert(abi.encodeWithSelector(FourteenNumbersSolutions.CanNotUpgradeToLowerOrSameVersion.selector, 1));
        vm.prank(upgradeAdmin);
        fourteenNumbersSolutions.upgradeToAndCall(address(v1Impl), initData);
    }
}
