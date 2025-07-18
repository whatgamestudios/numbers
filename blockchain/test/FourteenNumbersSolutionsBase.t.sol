// Copyright Whatgame Studios 2024 - 2025
// SPDX-License-Identifier: PROPRIETORY
// solhint-disable not-rely-on-time

pragma solidity ^0.8.24;

// solhint-disable-next-line no-global-import
import "forge-std/Test.sol";
import {FourteenNumbersSolutions} from "../src/FourteenNumbersSolutions.sol";
import {FourteenNumbersSolutionsV2} from "../src/FourteenNumbersSolutionsV2.sol";
import {FourteenNumbersSolutionsV3} from "../src/FourteenNumbersSolutionsV3.sol";
import {FourteenNumbersSolutionsV4} from "../src/FourteenNumbersSolutionsV4.sol";
import {ERC1967Proxy} from "@openzeppelin/contracts/proxy/ERC1967/ERC1967Proxy.sol";

abstract contract FourteenNumbersSolutionsBaseTest is Test {
    bytes32 public defaultAdminRole;
    bytes32 public upgradeRole;
    bytes32 public ownerRole;

    address public roleAdmin;
    address public upgradeAdmin;
    address public owner;

    address public player1;
    address public player2;
    address public player3;

    FourteenNumbersSolutions fourteenNumbersSolutions;
    ERC1967Proxy public proxy;

    function setUp() public virtual {
        roleAdmin = makeAddr("RoleAdmin");
        upgradeAdmin = makeAddr("UpgradeAdmin");
        owner = makeAddr("Owner");
        player1 = makeAddr("Player1");
        player2 = makeAddr("Player2");
        player3 = makeAddr("Player3");

        FourteenNumbersSolutions temp = new FourteenNumbersSolutions();
        defaultAdminRole = temp.DEFAULT_ADMIN_ROLE();
        upgradeRole = temp.UPGRADE_ROLE();
        ownerRole = temp.OWNER_ROLE();
    }

    function deployV1() internal {
        FourteenNumbersSolutions impl = new FourteenNumbersSolutions();
        bytes memory initData = abi.encodeWithSelector(
            FourteenNumbersSolutions.initialize.selector, roleAdmin, owner, upgradeAdmin);
        proxy = new ERC1967Proxy(address(impl), initData);
        fourteenNumbersSolutions = FourteenNumbersSolutions(address(proxy));
    }

    function deployV2() internal {
        deployV1();
        FourteenNumbersSolutionsV2 v2Impl = new FourteenNumbersSolutionsV2();
        bytes memory initData = abi.encodeWithSelector(FourteenNumbersSolutionsV2.upgradeStorage.selector, bytes(""));
        vm.prank(upgradeAdmin);
        fourteenNumbersSolutions.upgradeToAndCall(address(v2Impl), initData);

        uint256 ver = fourteenNumbersSolutions.version();
        assertEq(ver, 2, "Upgrade did not upgrade version");
    }

    function deployV3() internal {
        deployV2();

        FourteenNumbersSolutionsV3 v3Impl = new FourteenNumbersSolutionsV3();
        bytes memory initData = abi.encodeWithSelector(FourteenNumbersSolutionsV3.upgradeStorage.selector, bytes(""));
        vm.prank(upgradeAdmin);
        fourteenNumbersSolutions.upgradeToAndCall(address(v3Impl), initData);

        uint256 ver = fourteenNumbersSolutions.version();
        assertEq(ver, 3, "Upgrade did not upgrade version");
    }

    function upgradeToV4() internal {
        FourteenNumbersSolutionsV4 v4Impl = new FourteenNumbersSolutionsV4();
        bytes memory initData = abi.encodeWithSelector(FourteenNumbersSolutionsV4.upgradeStorage.selector, bytes(""));
        vm.prank(upgradeAdmin);
        fourteenNumbersSolutions.upgradeToAndCall(address(v4Impl), initData);

        uint256 ver = fourteenNumbersSolutions.version();
        assertEq(ver, 4, "Upgrade did not upgrade version");
    }

    function deployV4() internal {
        deployV3();
        upgradeToV4();
    }
}
