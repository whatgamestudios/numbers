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
import {FourteenNumbersSolutionsV4} from "../src/FourteenNumbersSolutionsV4.sol";
import {ERC1967Proxy} from "@openzeppelin/contracts/proxy/ERC1967/ERC1967Proxy.sol";

contract FourteenNumbersSolutionsV5a is FourteenNumbersSolutionsV4 {
    function upgradeStorage(bytes memory /* _data */) external override {
        version = 5;
    }
}



contract FourteenNumbersSolutionsConfigV4Test is FourteenNumbersSolutionsConfigTest {
    function setUp() public virtual override {
        super.setUp();
        deployV4();
    }

    function testNonUpgradeDeployV4() public {
        FourteenNumbersSolutionsV4 impl = new FourteenNumbersSolutionsV4();
        bytes memory initData = abi.encodeWithSelector(
            FourteenNumbersSolutionsV4.initialize.selector, roleAdmin, owner, upgradeAdmin);
        proxy = new ERC1967Proxy(address(impl), initData);
        fourteenNumbersSolutions = FourteenNumbersSolutions(address(proxy));

        uint256 ver = fourteenNumbersSolutions.version();
        assertEq(ver, 4, "Wrong version");
    }

    // Make sure that it is possible to upgrade from V4 to V5.
    function testUpgradeToV5() public {
        FourteenNumbersSolutionsV5a newImpl = new FourteenNumbersSolutionsV5a();
        bytes memory initData = abi.encodeWithSelector(FourteenNumbersSolutions.upgradeStorage.selector, bytes(""));
        vm.prank(upgradeAdmin);
        fourteenNumbersSolutions.upgradeToAndCall(address(newImpl), initData);

        uint256 ver = fourteenNumbersSolutions.version();
        assertEq(ver, 5, "Upgrade did not upgrade version");
    }

    function testNonUpgradeToV4() public {
        FourteenNumbersSolutionsV4 notNewImpl = new FourteenNumbersSolutionsV4();
        bytes memory initData = abi.encodeWithSelector(FourteenNumbersSolutions.upgradeStorage.selector, bytes(""));
        vm.expectRevert(abi.encodeWithSelector(FourteenNumbersSolutions.CanNotUpgradeToLowerOrSameVersion.selector, 4));
        vm.prank(upgradeAdmin);
        fourteenNumbersSolutions.upgradeToAndCall(address(notNewImpl), initData);
    }

    function testDowngradeV4ToV0() public {
        // Attempt to downgrade from V4 to V0.
        FourteenNumbersSolutions v1Impl = new FourteenNumbersSolutions();
        bytes memory initData = abi.encodeWithSelector(FourteenNumbersSolutions.upgradeStorage.selector, bytes(""));
        vm.expectRevert(abi.encodeWithSelector(FourteenNumbersSolutions.CanNotUpgradeToLowerOrSameVersion.selector, 4));
        vm.prank(upgradeAdmin);
        fourteenNumbersSolutions.upgradeToAndCall(address(v1Impl), initData);
    }

    function testDowngradeV4ToV2() public {
        // Attempt to downgrade from V4 to V2.
        FourteenNumbersSolutionsV2 v2Impl = new FourteenNumbersSolutionsV2();
        bytes memory initData = abi.encodeWithSelector(FourteenNumbersSolutions.upgradeStorage.selector, bytes(""));
        vm.expectRevert(abi.encodeWithSelector(FourteenNumbersSolutions.CanNotUpgradeToLowerOrSameVersion.selector, 4));
        vm.prank(upgradeAdmin);
        fourteenNumbersSolutions.upgradeToAndCall(address(v2Impl), initData);
    }

    function testDowngradeV4ToV3() public {
        // Attempt to downgrade from V4 to V3.
        FourteenNumbersSolutionsV3 v3Impl = new FourteenNumbersSolutionsV3();
        bytes memory initData = abi.encodeWithSelector(FourteenNumbersSolutions.upgradeStorage.selector, bytes(""));
        vm.expectRevert(abi.encodeWithSelector(FourteenNumbersSolutions.CanNotUpgradeToLowerOrSameVersion.selector, 4));
        vm.prank(upgradeAdmin);
        fourteenNumbersSolutions.upgradeToAndCall(address(v3Impl), initData);
    }
}
