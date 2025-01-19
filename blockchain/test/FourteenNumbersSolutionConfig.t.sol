// Copyright Whatgame Studios 2024 - 2025
// SPDX-License-Identifier: PROPRIETORY
// solhint-disable not-rely-on-time

pragma solidity ^0.8.24;

// solhint-disable-next-line no-global-import
import "forge-std/Test.sol";
import {FourteenNumbersSolutionsBaseTest} from "./FourteenNumbersSolutionsBase.t.sol";
import {FourteenNumbersSolutions} from "../src/FourteenNumbersSolutions.sol";


contract FourteenNumbersSolutionsV2 is FourteenNumbersSolutions {
    function upgradeStorage(bytes memory /* _data */) external override {
        version = 1;
    }
}


contract FourteenNumbersSolutionsConfigTest is FourteenNumbersSolutionsBaseTest {

    function testGetOwner() public view {
        assertEq(fourteenNumbersSolutions.owner(), owner);
    }

    function testChangeOwner() public {
        address owner2 = makeAddr("Owner2");
        vm.prank(roleAdmin);
        fourteenNumbersSolutions.grantRole(ownerRole, owner2);
        vm.prank(roleAdmin);
        fourteenNumbersSolutions.revokeRole(ownerRole, owner);
        assertEq(fourteenNumbersSolutions.owner(), owner2);
    }


    function testUpgradeToV1() public {
        FourteenNumbersSolutionsV2 v2Impl = new FourteenNumbersSolutionsV2();
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
        FourteenNumbersSolutionsV2 v2Impl = new FourteenNumbersSolutionsV2();
        bytes memory initData = abi.encodeWithSelector(FourteenNumbersSolutions.upgradeStorage.selector, bytes(""));
        vm.prank(upgradeAdmin);
        fourteenNumbersSolutions.upgradeToAndCall(address(v2Impl), initData);

        // Attempt to downgrade from V1 to V0.
        FourteenNumbersSolutions v1Impl = new FourteenNumbersSolutions();
        vm.expectRevert(abi.encodeWithSelector(FourteenNumbersSolutions.CanNotUpgradeToLowerOrSameVersion.selector, 1));
        vm.prank(upgradeAdmin);
        fourteenNumbersSolutions.upgradeToAndCall(address(v1Impl), initData);
    }

    function testUpgradeAuthFail() public {
        FourteenNumbersSolutionsV2 v2Impl = new FourteenNumbersSolutionsV2();
        bytes memory initData = abi.encodeWithSelector(FourteenNumbersSolutions.upgradeStorage.selector, bytes(""));
        // Error will be of the form: 
        // AccessControl: account 0x7fa9385be102ac3eac297483dd6233d62b3e1496 is missing role 0x555047524144455f524f4c450000000000000000000000000000000000000000
        vm.expectRevert();
        fourteenNumbersSolutions.upgradeToAndCall(address(v2Impl), initData);
    }

    function testAddRevokeRenounceRoleAdmin() public {
        bytes32 role = fourteenNumbersSolutions.DEFAULT_ADMIN_ROLE();
        address newRoleAdmin = makeAddr("NewRoleAdmin");
        vm.prank(roleAdmin);
        fourteenNumbersSolutions.grantRole(role, newRoleAdmin);

        vm.startPrank(newRoleAdmin);
        fourteenNumbersSolutions.revokeRole(role, roleAdmin);
        fourteenNumbersSolutions.grantRole(role, roleAdmin);
        fourteenNumbersSolutions.renounceRole(role, newRoleAdmin);
        vm.stopPrank();
    }

    function testAddRevokeRenounceUpgradeAdmin() public {
        bytes32 role = fourteenNumbersSolutions.UPGRADE_ROLE();
        address newUpgradeAdmin = makeAddr("NewUpgradeAdmin");
        vm.startPrank(roleAdmin);
        fourteenNumbersSolutions.grantRole(role, newUpgradeAdmin);
        assertTrue(fourteenNumbersSolutions.hasRole(role, newUpgradeAdmin), "New upgrade admin should have role");
        fourteenNumbersSolutions.revokeRole(role, newUpgradeAdmin);
        assertFalse(fourteenNumbersSolutions.hasRole(role, newUpgradeAdmin), "New upgrade admin should not have role");
        vm.stopPrank();
        vm.prank(upgradeAdmin);
        fourteenNumbersSolutions.renounceRole(role, upgradeAdmin);
        assertFalse(fourteenNumbersSolutions.hasRole(role, upgradeAdmin), "Upgrade admin should not have role");
    }

    function testRoleAdminAuthFail () public {
        bytes32 role = fourteenNumbersSolutions.DEFAULT_ADMIN_ROLE();
        address newRoleAdmin = makeAddr("NewRoleAdmin");
        // Error will be of the form: 
        // AccessControl: account 0x7fa9385be102ac3eac297483dd6233d62b3e1496 is missing role 0x555047524144455f524f4c450000000000000000000000000000000000000000
        vm.expectRevert();
        fourteenNumbersSolutions.grantRole(role, newRoleAdmin);
    }
}
