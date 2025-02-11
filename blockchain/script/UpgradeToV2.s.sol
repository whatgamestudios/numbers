// Copyright (c) Whatgame Studios 2024 - 2024
// SPDX-License-Identifier: PROPRIETORY
pragma solidity ^0.8.20;

import "forge-std/Script.sol";
import {FourteenNumbersSolutions} from "../src/FourteenNumbersSolutions.sol";


contract UpgradeToV2 is Script {
    function run() public {
        address proxyDeployedAddress = 0xe2E762770156FfE253C49Da6E008b4bECCCf2812;
        address v2Address = 0x3B6378DDa9037F3a98C685fec3990100ee7Cf4Ff;

        FourteenNumbersSolutions fourteenNumbersSolutions = FourteenNumbersSolutions(proxyDeployedAddress);
        bytes memory initData = abi.encodeWithSelector(FourteenNumbersSolutions.upgradeStorage.selector, bytes(""));

        vm.broadcast();
        fourteenNumbersSolutions.upgradeToAndCall(v2Address, initData);

        console.logString("Done");
    }
}
