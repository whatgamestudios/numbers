// Copyright Whatgame Studios 2024 - 2025
// SPDX-License-Identifier: PROPRIETORY
// solhint-disable not-rely-on-time

pragma solidity ^0.8.24;

// solhint-disable-next-line no-global-import
import "forge-std/Test.sol";
import {FourteenNumbersSolutions} from "../src/FourteenNumbersSolutions.sol";
import {ERC1967Proxy} from "@openzeppelin/contracts/proxy/ERC1967/ERC1967Proxy.sol";

abstract contract FourteenNumbersSolutionsBaseTest is Test {
    bytes32 public defaultAdminRole;
    bytes32 public upgradeRole;

    address public roleAdmin;
    address public upgradeAdmin;
    address public owner;


    FourteenNumbersSolutions fourteenNumbersSolutions;
    ERC1967Proxy public proxy;

    function setUp() public {
        roleAdmin = makeAddr("RoleAdmin");
        upgradeAdmin = makeAddr("UpgradeAdmin");
        owner = makeAddr("Owner");

        FourteenNumbersSolutions impl = new FourteenNumbersSolutions();
        bytes memory initData = abi.encodeWithSelector(
            FourteenNumbersSolutions.initialize.selector, roleAdmin, owner, upgradeAdmin);
        proxy = new ERC1967Proxy(address(impl), initData);
        fourteenNumbersSolutions = FourteenNumbersSolutions(address(proxy));

        defaultAdminRole = fourteenNumbersSolutions.DEFAULT_ADMIN_ROLE();
        upgradeRole = fourteenNumbersSolutions.UPGRADE_ROLE();
    }
}
