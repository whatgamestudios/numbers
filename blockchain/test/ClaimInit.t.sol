// Copyright Whatgame Studios 2024 - 2025
// SPDX-License-Identifier: PROPRIETORY
// solhint-disable not-rely-on-time

pragma solidity ^0.8.24;

// solhint-disable-next-line no-global-import
import "forge-std/Test.sol";
import {ClaimBaseTest} from "./ClaimBase.t.sol";
import {FourteenNumbersClaim} from "../src/FourteenNumbersClaim.sol";

contract ClaimInitTest is ClaimBaseTest {

    function testInit() public view {
        assertEq(fourteenNumbersClaim.owner(), owner, "Owner");
        assertTrue(fourteenNumbersClaim.hasRole(configRole, configAdmin), "Config");
        assertTrue(fourteenNumbersClaim.hasRole(defaultAdminRole, roleAdmin), "Role admin");
    }
}
