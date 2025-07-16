// Copyright Whatgame Studios 2024 - 2025
// SPDX-License-Identifier: PROPRIETORY
// solhint-disable not-rely-on-time

pragma solidity ^0.8.24;

// solhint-disable-next-line no-global-import
import "forge-std/Test.sol";
import {FourteenNumbersSolutionsInitTest} from "./FourteenNumbersSolutionsInit.t.sol";
import {FourteenNumbersSolutions} from "../src/FourteenNumbersSolutions.sol";
import {ERC1967Proxy} from "@openzeppelin/contracts/proxy/ERC1967/ERC1967Proxy.sol";

contract FourteenNumbersSolutionsInitV1Test is FourteenNumbersSolutionsInitTest {
    function setUp() public virtual override {
        super.setUp();
        deployV1();
    }

    function testVersionV0() public view {
        assertEq(fourteenNumbersSolutions.version(), 0);
    }
}
