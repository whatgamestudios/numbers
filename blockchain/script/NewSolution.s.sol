// Copyright (c) Whatgame Studios 2024 - 2024
// SPDX-License-Identifier: PROPRIETORY
pragma solidity ^0.8.20;

import "forge-std/Script.sol";
import "@openzeppelin/contracts/proxy/ERC1967/ERC1967Proxy.sol";
import {FourteenNumbersSolutions} from "../src/FourteenNumbersSolutions.sol";


// TODO this doesn't currently work

contract NewSolution is Script {
    function run() public {

        address proxyAddress = 0xe2E762770156FfE253C49Da6E008b4bECCCf2812;
        //address implAddress = 0x2b9c63DF973ec282404d9F9fA7688Ee403b7E311;

        ERC1967Proxy proxy = ERC1967Proxy(payable(address(proxyAddress)));
        //FourteenNumbersSolutions impl1 = FourteenNumbersSolutions(implAddress);
        FourteenNumbersSolutions impl = FourteenNumbersSolutions(address(proxy));

        uint32 gameDay = 56;

        (uint32 start, uint32 end) = impl.determineCurrentGameDays();
        require(start <= gameDay, "Too early");
        require(end >= gameDay, "Too late");

        bytes memory sol1 = "(100+10+1)*5";
        bytes memory sol2 = "50*(9+2)+8-3";
        bytes memory sol3 = "(75+25-7)*6-4";

        vm.broadcast();
        impl.storeResults(gameDay, sol1, sol2, sol3, false);
    }
}
