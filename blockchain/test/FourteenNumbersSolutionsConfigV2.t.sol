// Copyright Whatgame Studios 2024 - 2025
// SPDX-License-Identifier: PROPRIETORY
// solhint-disable not-rely-on-time

pragma solidity ^0.8.24;

// solhint-disable-next-line no-global-import
import "forge-std/Test.sol";
import {FourteenNumbersSolutionsConfigTest} from "./FourteenNumbersSolutionsConfig.t.sol";
import {FourteenNumbersSolutions} from "../src/FourteenNumbersSolutions.sol";
import {FourteenNumbersSolutionsV2} from "../src/FourteenNumbersSolutionsV2.sol";
import {ERC1967Proxy} from "@openzeppelin/contracts/proxy/ERC1967/ERC1967Proxy.sol";

contract FourteenNumbersSolutionsV3a is FourteenNumbersSolutionsV2 {
    function upgradeStorage(bytes memory /* _data */) external override {
        version = 3;
    }
}



contract FourteenNumbersSolutionsConfigV2Test is FourteenNumbersSolutionsConfigTest {
    function setUp() public virtual override {
        super.setUp();
        deployV2();
    }

    function testNonUpgradeDeployV2() public {
        FourteenNumbersSolutionsV2 impl = new FourteenNumbersSolutionsV2();
        bytes memory initData = abi.encodeWithSelector(
            FourteenNumbersSolutionsV2.initialize.selector, roleAdmin, owner, upgradeAdmin);
        proxy = new ERC1967Proxy(address(impl), initData);
        fourteenNumbersSolutions = FourteenNumbersSolutions(address(proxy));

        uint256 ver = fourteenNumbersSolutions.version();
        assertEq(ver, 2, "Wrong version");
    }

    // Make sure that it is possible to upgrade from V2 to V3.
    function testUpgradeToV3() public {
        FourteenNumbersSolutionsV3a newImpl = new FourteenNumbersSolutionsV3a();
        bytes memory initData = abi.encodeWithSelector(FourteenNumbersSolutions.upgradeStorage.selector, bytes(""));
        vm.prank(upgradeAdmin);
        fourteenNumbersSolutions.upgradeToAndCall(address(newImpl), initData);

        uint256 ver = fourteenNumbersSolutions.version();
        assertEq(ver, 3, "Upgrade did not upgrade version");
    }

    function testNonUpgradeToV2() public {
        FourteenNumbersSolutionsV2 notNewImpl = new FourteenNumbersSolutionsV2();
        bytes memory initData = abi.encodeWithSelector(FourteenNumbersSolutions.upgradeStorage.selector, bytes(""));
        vm.expectRevert(abi.encodeWithSelector(FourteenNumbersSolutions.CanNotUpgradeToLowerOrSameVersion.selector, 2));
        vm.prank(upgradeAdmin);
        fourteenNumbersSolutions.upgradeToAndCall(address(notNewImpl), initData);
    }

    function testDowngradeV2ToV0() public {
        // Attempt to downgrade from V2 to V0.
        FourteenNumbersSolutions v1Impl = new FourteenNumbersSolutions();
        bytes memory initData = abi.encodeWithSelector(FourteenNumbersSolutions.upgradeStorage.selector, bytes(""));
        vm.expectRevert(abi.encodeWithSelector(FourteenNumbersSolutions.CanNotUpgradeToLowerOrSameVersion.selector, 2));
        vm.prank(upgradeAdmin);
        fourteenNumbersSolutions.upgradeToAndCall(address(v1Impl), initData);
    }
}
