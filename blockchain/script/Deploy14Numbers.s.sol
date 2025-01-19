// Copyright (c) Whatgame Studios 2024 - 2024
// SPDX-License-Identifier: PROPRIETORY
pragma solidity ^0.8.20;

import "forge-std/Script.sol";
import "@openzeppelin/contracts/proxy/ERC1967/ERC1967Proxy.sol";
import {FourteenNumbersSolutions} from "../src/FourteenNumbersSolutions.sol";


contract Deploy14Numbers is Script {
    function run() public {
        address deployer = vm.envAddress("DEPLOYER_ADDRESS");
        address roleAdmin = deployer;
        address upgradeAdmin = deployer;
        address owner = deployer;

        vm.broadcast();
        FourteenNumbersSolutions impl = new FourteenNumbersSolutions();
        bytes memory initData = abi.encodeWithSelector(
            FourteenNumbersSolutions.initialize.selector, roleAdmin, owner, upgradeAdmin);

        //impl = FourteenNumbersSolutions(address(0x2b9c63DF973ec282404d9F9fA7688Ee403b7E311));
        vm.broadcast();
        ERC1967Proxy proxy = new ERC1967Proxy(address(impl), initData);

        //FourteenNumbersSolutions fourteenNumbersSolutions = FourteenNumbersSolutions(address(proxy));

        console.logString("Deployment address");
        console.logAddress(address(proxy));
    }
}
