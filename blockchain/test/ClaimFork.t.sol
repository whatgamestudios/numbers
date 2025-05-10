// Copyright Whatgame Studios 2024 - 2025
// SPDX-License-Identifier: PROPRIETORY
// solhint-disable not-rely-on-time

pragma solidity ^0.8.24;

// solhint-disable-next-line no-global-import
import "forge-std/Test.sol";
import {FourteenNumbersClaim} from "../src/FourteenNumbersClaim.sol";


contract ClaimForkTest is Test {
    FourteenNumbersClaim fourteenNumbersClaim;

    function setUp() public virtual {
        /// @dev Fork the Immutable zkEVM for this test
        string memory rpcURL = "https://rpc.immutable.com";
        vm.createSelectFork(rpcURL);

        address fourteenNumbersClaimContract = 0xb427336d725943BA4300EEC219587E207ad21146;
        fourteenNumbersClaim = FourteenNumbersClaim(fourteenNumbersClaimContract);
    }

    function testClaim() public {
        address passportWallet = 0xA654b48E5a9e58A8626F81168FEBA1B3AB4AF4EF;

        vm.prank(passportWallet);
        fourteenNumbersClaim.claim(1);
    }

}
