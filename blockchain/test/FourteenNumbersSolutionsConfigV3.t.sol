// Copyright Whatgame Studios 2024 - 2025
// SPDX-License-Identifier: PROPRIETORY
// solhint-disable not-rely-on-time

pragma solidity ^0.8.24;

// solhint-disable-next-line no-global-import
import "forge-std/Test.sol";
import {FourteenNumbersSolutionsConfigTest} from "./FourteenNumbersSolutionsConfig.t.sol";
import {FourteenNumbersSolutions} from "../src/FourteenNumbersSolutions.sol";
import {FourteenNumbersSolutionsV2} from "../src/FourteenNumbersSolutionsV2.sol";
import {FourteenNumbersSolutionsV3} from "../src/FourteenNumbersSolutionsV3.sol";
import {ERC1967Proxy} from "@openzeppelin/contracts/proxy/ERC1967/ERC1967Proxy.sol";

contract FourteenNumbersSolutionsV4a is FourteenNumbersSolutionsV3 {
    function upgradeStorage(bytes memory /* _data */) external override {
        version = 4;
    }
}



contract FourteenNumbersSolutionsConfigV2Test is FourteenNumbersSolutionsConfigTest {
    function setUp() public virtual override {
        super.setUp();
        deployV3();
    }


    // Make sure that it is possible to upgrade from V2 to V3.
    function testUpgradeToV4() public {
        FourteenNumbersSolutionsV4a newImpl = new FourteenNumbersSolutionsV4a();
        bytes memory initData = abi.encodeWithSelector(FourteenNumbersSolutions.upgradeStorage.selector, bytes(""));
        vm.prank(upgradeAdmin);
        fourteenNumbersSolutions.upgradeToAndCall(address(newImpl), initData);

        uint256 ver = fourteenNumbersSolutions.version();
        assertEq(ver, 4, "Upgrade did not upgrade version");
    }

    function testNonUpgradeToV3() public {
        FourteenNumbersSolutionsV3 notNewImpl = new FourteenNumbersSolutionsV3();
        bytes memory initData = abi.encodeWithSelector(FourteenNumbersSolutions.upgradeStorage.selector, bytes(""));
        vm.expectRevert(abi.encodeWithSelector(FourteenNumbersSolutions.CanNotUpgradeToLowerOrSameVersion.selector, 3));
        vm.prank(upgradeAdmin);
        fourteenNumbersSolutions.upgradeToAndCall(address(notNewImpl), initData);
    }

    function testDowngradeV3ToV0() public {
        // Attempt to downgrade from V3 to V0.
        FourteenNumbersSolutions v1Impl = new FourteenNumbersSolutions();
        bytes memory initData = abi.encodeWithSelector(FourteenNumbersSolutions.upgradeStorage.selector, bytes(""));
        vm.expectRevert(abi.encodeWithSelector(FourteenNumbersSolutions.CanNotUpgradeToLowerOrSameVersion.selector, 3));
        vm.prank(upgradeAdmin);
        fourteenNumbersSolutions.upgradeToAndCall(address(v1Impl), initData);
    }

    function testDowngradeV3ToV2() public {
        // Attempt to downgrade from V3 to V2.
        FourteenNumbersSolutionsV2 v2Impl = new FourteenNumbersSolutionsV2();
        bytes memory initData = abi.encodeWithSelector(FourteenNumbersSolutions.upgradeStorage.selector, bytes(""));
        vm.expectRevert(abi.encodeWithSelector(FourteenNumbersSolutions.CanNotUpgradeToLowerOrSameVersion.selector, 3));
        vm.prank(upgradeAdmin);
        fourteenNumbersSolutions.upgradeToAndCall(address(v2Impl), initData);
    }
}
