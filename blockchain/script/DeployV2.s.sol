// Copyright (c) Whatgame Studios 2024 - 2024
// SPDX-License-Identifier: PROPRIETORY
pragma solidity ^0.8.20;

import "forge-std/Script.sol";
import {FourteenNumbersSolutionsV2} from "../src/FourteenNumbersSolutionsV2.sol";


contract DeployV2 is Script {
    function run() public {
        vm.broadcast();
        FourteenNumbersSolutionsV2 v2Impl = new FourteenNumbersSolutionsV2();
        console.logString("Deployed v2");
        console.logAddress(address(v2Impl));
    }
}
