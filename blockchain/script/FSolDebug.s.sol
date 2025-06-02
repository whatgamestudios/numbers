// Copyright (c) Whatgame Studios 2024 - 2024
// SPDX-License-Identifier: PROPRIETORY
pragma solidity ^0.8.20;

import "forge-std/Script.sol";
import "@openzeppelin/contracts/proxy/ERC1967/ERC1967Proxy.sol";
import {FourteenNumbersSolutions} from "../src/FourteenNumbersSolutions.sol";
import {FourteenNumbersSolutionsV2} from "../src/FourteenNumbersSolutionsV2.sol";
import {FourteenNumbersClaim} from "../src/FourteenNumbersClaim.sol";
import {FourteenNumbersClaimV2} from "../src/FourteenNumbersClaimV2.sol";
import {FourteenNumbersClaimV3} from "../src/FourteenNumbersClaimV3.sol";
import {ImmutableERC1155} from "../src/immutable/ImmutableERC1155.sol";

contract FSolDebug is Script {
    function run() public {
        address deployer = makeAddr("deployer");
        address roleAdmin = deployer;
        address upgradeAdmin = deployer;
        address owner = deployer;

        FourteenNumbersSolutions impl = new FourteenNumbersSolutions();
        bytes memory initData = abi.encodeWithSelector(
            FourteenNumbersSolutions.initialize.selector, roleAdmin, owner, upgradeAdmin);
        ERC1967Proxy proxy = new ERC1967Proxy(address(impl), initData);
        FourteenNumbersSolutionsV2 v2Impl = new FourteenNumbersSolutionsV2();
        bytes memory upgradeData = abi.encodeWithSelector(FourteenNumbersSolutions.upgradeStorage.selector, bytes(""));
        FourteenNumbersSolutionsV2 fourteenNumbersSolutions = FourteenNumbersSolutionsV2(address(proxy));
        vm.prank(upgradeAdmin);
        fourteenNumbersSolutions.upgradeToAndCall(address(v2Impl), upgradeData);

        console.log("GameDay, Target");
        for (uint32 i = 0; i < 10000; i++) {
            uint256 target = fourteenNumbersSolutions.getTargetValue(i);
            console.log("%s, %s", i, target);
        }

        console.logString("Done");
    }
}
