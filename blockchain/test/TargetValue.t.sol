// Copyright Whatgame Studios 2024 - 2025
// SPDX-License-Identifier: PROPRIETORY
// solhint-disable not-rely-on-time

pragma solidity ^0.8.24;

// solhint-disable-next-line no-global-import
import "forge-std/Test.sol";
import {TargetValue} from "../src/TargetValue.sol";

contract TargetValueImpl is TargetValue {

}

contract TargetValueTest is Test {
    TargetValueImpl impl;

    function setUp() public {
        impl = new TargetValueImpl();
    }

    function testTargetValueDay10() public view {
        uint256 target = impl.getTargetValue(10);
        assertEq(target, 492, "Wrong target1");
    }

    function testTargetValueDay34() public view {
        uint256 target = impl.getTargetValue(34);
        assertEq(target, 444, "Wrong target34");
    }

    function testTargetValueDay35() public view {
        uint256 target = impl.getTargetValue(35);
        assertEq(target, 575, "Wrong target35");
    }

    function testTargetValueDay38() public view {
        uint256 target = impl.getTargetValue(38);
        assertEq(target, 948, "Wrong target2");
    }

    function testTargetValueDay39() public view {
        uint256 target = impl.getTargetValue(39);
        assertEq(target, 881, "Wrong target3");
    }
}
