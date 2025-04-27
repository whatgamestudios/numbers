// Copyright Whatgame Studios 2024 - 2025
// SPDX-License-Identifier: PROPRIETORY
// solhint-disable not-rely-on-time

pragma solidity ^0.8.24;

// solhint-disable-next-line no-global-import
import "forge-std/Test.sol";
import {ClaimBaseTest} from "./ClaimBase.t.sol";
import {FourteenNumbersClaim} from "../src/FourteenNumbersClaim.sol";
import {IERC1155} from "@openzeppelin/contracts/token/erc1155/IERC1155.sol";

contract ClaimConfigTest is ClaimBaseTest {
    error AddMoreTokensBalanceMustBeNonZero();
    error AddMoreTokensPercentageTooLarge();

    error ERC1155InsufficientBalance(address sender, uint256 balance, uint256 needed, uint256 tokenId);
    error ERC1155MissingApprovalForAll(address target, address sender);
    error ERC1155InvalidReceiver(address receiver);
    error AccessControlUnauthorizedAccount(address account, bytes32 role);

    event SettingDaysPlayedToClaim(uint256 _newDaysPlayedToClaim);
    event TokensAdded(uint256 _slot, address _erc1155Contract, uint256 _tokenId, uint256 _amount, uint256 _percentage);
    event TokensRemoved(uint256 _slot, address _erc1155Contract, uint256 _tokenId, uint256 _amount);
    event Claimed(address _gamePlayer, address _erc1155Contract, uint256 _tokenId, uint256 _daysPlayed, uint256 _claimedSoFar);


    function setUp() public virtual override {
        super.setUp();
        
        // Add user1 to passport allowlist
        // vm.startPrank(configAdmin);
        // fourteenNumbersClaim.addWalletToAllowlist(user1);
        // vm.stopPrank();
    }

    function testSetDaysPlayedToClaim() public {
        uint256 newDays = 45;
        vm.prank(configAdmin);
        vm.expectEmit(true, true, true, true);
        emit SettingDaysPlayedToClaim(newDays);
        fourteenNumbersClaim.setDaysPlayedToClaim(newDays);
        assertEq(fourteenNumbersClaim.daysPlayedToClaim(), newDays, "Days played to claim should be updated");
    }

    function testAddMoreTokens() public {
        vm.prank(tokenAdmin);
        mockERC1155.setApprovalForAll(address(fourteenNumbersClaim), true);

        FourteenNumbersClaim.ClaimableToken memory token = FourteenNumbersClaim.ClaimableToken({
            erc1155Contract: address(mockERC1155),
            tokenId: TOK1_TOKEN_ID,
            balance: TOK1_AMOUNT,
            percentage: TOK1_PERCENTAGE
        });
        vm.prank(tokenAdmin);
        vm.expectEmit(true, true, true, true);
        emit TokensAdded(1, address(mockERC1155), TOK1_TOKEN_ID, TOK1_AMOUNT, TOK1_PERCENTAGE);
        fourteenNumbersClaim.addMoreTokens(token);

        (address erc1155Contract, uint256 tokenId, uint256 balance, uint256 percentage) = fourteenNumbersClaim.claimableTokens(1);
        assertEq(erc1155Contract, address(mockERC1155), "ERC1155 contract should match");
        assertEq(tokenId, TOK1_TOKEN_ID, "Token ID should match");
        assertEq(balance, TOK1_AMOUNT, "Balance should match");
        assertEq(percentage, TOK1_PERCENTAGE, "Percentage should match");
    }

    function testAddMoreTokensBadAccess() public {
        vm.prank(tokenAdmin);
        mockERC1155.setApprovalForAll(address(fourteenNumbersClaim), true);

        FourteenNumbersClaim.ClaimableToken memory token = FourteenNumbersClaim.ClaimableToken({
            erc1155Contract: address(mockERC1155),
            tokenId: TOK1_TOKEN_ID,
            balance: TOK1_AMOUNT,
            percentage: TOK1_PERCENTAGE
        });
        vm.prank(player1);
        vm.expectRevert(abi.encodeWithSelector(AccessControlUnauthorizedAccount.selector, player1, tokenRole));
        fourteenNumbersClaim.addMoreTokens(token);
    }

    function testAddMoreTokensWithZeroBalance() public {
        vm.prank(tokenAdmin);
        mockERC1155.setApprovalForAll(address(fourteenNumbersClaim), true);

        FourteenNumbersClaim.ClaimableToken memory token = FourteenNumbersClaim.ClaimableToken({
            erc1155Contract: address(mockERC1155),
            tokenId: TOK1_TOKEN_ID,
            balance: 0,
            percentage: TOK1_PERCENTAGE
        });
        vm.prank(tokenAdmin);
        vm.expectRevert(abi.encodeWithSelector(AddMoreTokensBalanceMustBeNonZero.selector));
        fourteenNumbersClaim.addMoreTokens(token);
    }

    function testAddMoreTokensWithERC1155ZeroBalance() public {
        vm.prank(tokenAdmin);
        mockERC1155.setApprovalForAll(address(fourteenNumbersClaim), true);

        FourteenNumbersClaim.ClaimableToken memory token = FourteenNumbersClaim.ClaimableToken({
            erc1155Contract: address(mockERC1155),
            tokenId: 1000,
            balance: TOK1_AMOUNT,
            percentage: TOK1_PERCENTAGE
        });
        vm.prank(tokenAdmin);
        vm.expectRevert(abi.encodeWithSelector(ERC1155InsufficientBalance.selector, tokenAdmin, 0, TOK1_AMOUNT, 1000));
        fourteenNumbersClaim.addMoreTokens(token);
    }

    function testAddMoreTokensERC1155NotApproved() public {
        FourteenNumbersClaim.ClaimableToken memory token = FourteenNumbersClaim.ClaimableToken({
            erc1155Contract: address(mockERC1155),
            tokenId: 1000,
            balance: TOK1_AMOUNT,
            percentage: TOK1_PERCENTAGE
        });
        vm.prank(tokenAdmin);
        vm.expectRevert(abi.encodeWithSelector(ERC1155MissingApprovalForAll.selector, address(fourteenNumbersClaim), tokenAdmin));
        fourteenNumbersClaim.addMoreTokens(token);
    }

    function testAddMoreTokensWithInvalidPercentage() public {
        vm.prank(tokenAdmin);
        mockERC1155.setApprovalForAll(address(fourteenNumbersClaim), true);

        FourteenNumbersClaim.ClaimableToken memory token = FourteenNumbersClaim.ClaimableToken({
            erc1155Contract: address(mockERC1155),
            tokenId: DEFAULT_TOKEN_ID,
            balance: DEFAULT_AMOUNT,
            percentage: 10001 // More than 100%
        });
        vm.prank(tokenAdmin);
        vm.expectRevert(abi.encodeWithSelector(AddMoreTokensPercentageTooLarge.selector));
        fourteenNumbersClaim.addMoreTokens(token);
    }

    function testDirectTransferFail() public {
        // Fake add token admin, which is perceived as a contract by the code, this so that the test will work.
        address[] memory contracts = new address[](1);
        contracts[0] = address(tokenAdmin);
        vm.prank(operatorRegistrarAdmin);
        allowList.addAddressesToAllowlist(contracts);

        // Now check that a direct transfer will fail.
        vm.prank(tokenAdmin);
        vm.expectRevert(abi.encodeWithSelector(ERC1155InvalidReceiver.selector, address(fourteenNumbersClaim)));
        mockERC1155.safeTransferFrom(tokenAdmin, address(fourteenNumbersClaim), TOK1_TOKEN_ID, TOK1_AMOUNT, new bytes(0));
    }

    function testBatchDirectTransferFail() public {
        // Fake add token admin, which is perceived as a contract by the code, this so that the test will work.
        address[] memory contracts = new address[](1);
        contracts[0] = address(tokenAdmin);
        vm.prank(operatorRegistrarAdmin);
        allowList.addAddressesToAllowlist(contracts);

        // Now check that a direct batch transfer will fail.
        uint256[] memory ids = new uint256[](1);
        ids[0] = TOK1_TOKEN_ID;
        uint256[] memory amounts = new uint256[](1);
        amounts[0] = TOK1_AMOUNT;

        vm.prank(tokenAdmin);
        vm.expectRevert(abi.encodeWithSelector(ERC1155InvalidReceiver.selector, address(fourteenNumbersClaim)));
        mockERC1155.safeBatchTransferFrom(tokenAdmin, address(fourteenNumbersClaim), ids, amounts, new bytes(0));
    }



// TODO Not reviewed


    // function testRemoveTokens() public {
    //     // First add tokens
    //     vm.startPrank(configAdmin);
    //     FourteenNumbersClaim.ClaimableToken memory token = FourteenNumbersClaim.ClaimableToken({
    //         erc1155Contract: address(mockERC1155),
    //         tokenId: DEFAULT_TOKEN_ID,
    //         balance: DEFAULT_AMOUNT,
    //         percentage: DEFAULT_PERCENTAGE
    //     });
    //     fourteenNumbersClaim.addMoreTokens(token);

    //     // Then remove some tokens
    //     uint256 removeAmount = 50;
    //     vm.expectEmit(true, true, true, true);
    //     emit TokensRemoved(1, address(mockERC1155), DEFAULT_TOKEN_ID, removeAmount);
    //     fourteenNumbersClaim.removeTokens(1, removeAmount);
    //     vm.stopPrank();
    // }

    // function testFailRemoveTokensWithZeroAmount() public {
    //     vm.startPrank(configAdmin);
    //     fourteenNumbersClaim.removeTokens(1, 0);
    //     vm.stopPrank();
    // }

    // function testFailRemoveTokensExceedingBalance() public {
    //     // First add tokens
    //     vm.startPrank(configAdmin);
    //     FourteenNumbersClaim.ClaimableToken memory token = FourteenNumbersClaim.ClaimableToken({
    //         erc1155Contract: address(mockERC1155),
    //         tokenId: DEFAULT_TOKEN_ID,
    //         balance: DEFAULT_AMOUNT,
    //         percentage: DEFAULT_PERCENTAGE
    //     });
    //     fourteenNumbersClaim.addMoreTokens(token);

    //     // Try to remove more than available
    //     fourteenNumbersClaim.removeTokens(1, DEFAULT_AMOUNT + 1);
    //     vm.stopPrank();
    // }

    // function testPassportCheck() public {
    //     vm.startPrank(configAdmin);
    //     // Test adding wallet to allowlist
    //     fourteenNumbersClaim.addWalletToAllowlist(user2);
    //     // Test removing wallet from allowlist
    //     fourteenNumbersClaim.removeWalletFromAllowlist(user2);
    //     vm.stopPrank();
    // }

    // function testPause() public {
    //     vm.startPrank(configAdmin);
    //     fourteenNumbersClaim.pause();
    //     assertTrue(fourteenNumbersClaim.paused(), "Contract should be paused");
    //     fourteenNumbersClaim.unpause();
    //     assertFalse(fourteenNumbersClaim.paused(), "Contract should be unpaused");
    //     vm.stopPrank();
    // }

    // function testGetClaimableNfts() public {
    //     vm.startPrank(configAdmin);
    //     // Add multiple tokens
    //     FourteenNumbersClaim.ClaimableToken memory token1 = FourteenNumbersClaim.ClaimableToken({
    //         erc1155Contract: address(mockERC1155),
    //         tokenId: 1,
    //         balance: 100,
    //         percentage: 3000
    //     });
    //     FourteenNumbersClaim.ClaimableToken memory token2 = FourteenNumbersClaim.ClaimableToken({
    //         erc1155Contract: address(mockERC1155),
    //         tokenId: 2,
    //         balance: 200,
    //         percentage: 4000
    //     });
        
    //     fourteenNumbersClaim.addMoreTokens(token1);
    //     fourteenNumbersClaim.addMoreTokens(token2);

    //     FourteenNumbersClaim.ClaimableToken[] memory tokens = fourteenNumbersClaim.getClaimableNfts();
    //     assertEq(tokens.length, 2, "Should return 2 tokens");
    //     vm.stopPrank();
    // }
}
