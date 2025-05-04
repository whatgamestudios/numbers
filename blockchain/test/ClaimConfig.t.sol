// Copyright Whatgame Studios 2024 - 2025
// SPDX-License-Identifier: PROPRIETORY

pragma solidity ^0.8.24;

// solhint-disable-next-line no-global-import
import "forge-std/Test.sol";
import {ClaimBaseTest} from "./ClaimBase.t.sol";
import {FourteenNumbersClaim} from "../src/FourteenNumbersClaim.sol";
import {PassportCheck} from "../src/PassportCheck.sol";
import {IERC1155} from "@openzeppelin/contracts/token/erc1155/IERC1155.sol";

contract FourteenNumbersClaimV2a is FourteenNumbersClaim {
    function upgradeStorage(bytes memory /* _data */) external override virtual {
        // Note real version of V2 contract would need to check for downgrades.
        version = 2;
    }
}


contract ClaimConfigTest is ClaimBaseTest {
    error AddMoreTokensBalanceMustBeNonZero();
    error AddMoreTokensPercentageTooLarge();

    error ERC1155InsufficientBalance(address sender, uint256 balance, uint256 needed, uint256 tokenId);
    error ERC1155MissingApprovalForAll(address target, address sender);
    error ERC1155InvalidReceiver(address receiver);
    error AccessControlUnauthorizedAccount(address account, bytes32 role);

    error EnforcedPause();

    event SettingDaysPlayedToClaim(uint256 _newDaysPlayedToClaim);
    event TokensAdded(uint256 _slot, address _erc1155Contract, uint256 _tokenId, uint256 _amount, uint256 _percentage);
    event TokensRemoved(uint256 _slot, address _erc1155Contract, uint256 _tokenId, uint256 _amount);
    event Claimed(address _gamePlayer, address _erc1155Contract, uint256 _tokenId, uint256 _daysPlayed, uint256 _claimedSoFar);


    function setUp() public virtual override {
        super.setUp();
    }

    function testUpgradeToV2() public {
        FourteenNumbersClaimV2a v2Impl = new FourteenNumbersClaimV2a();
        bytes memory initData = abi.encodeWithSelector(FourteenNumbersClaim.upgradeStorage.selector, bytes(""));
        vm.prank(configAdmin);
        fourteenNumbersClaim.upgradeToAndCall(address(v2Impl), initData);

        uint256 ver = fourteenNumbersClaim.version();
        assertEq(ver, 2, "Upgrade did not upgrade version");
    }

    function testUpgradeToV1() public {
        FourteenNumbersClaim v1Impl = new FourteenNumbersClaim();
        bytes memory initData = abi.encodeWithSelector(FourteenNumbersClaim.upgradeStorage.selector, bytes(""));
        vm.expectRevert(abi.encodeWithSelector(FourteenNumbersClaim.CanNotUpgradeToLowerOrSameVersion.selector, 0));
        vm.prank(configAdmin);
        fourteenNumbersClaim.upgradeToAndCall(address(v1Impl), initData);
    }

    function testDowngradeV1ToV0() public {
        // Upgrade from V0 to V1
        FourteenNumbersClaimV2a v2Impl = new FourteenNumbersClaimV2a();
        bytes memory initData = abi.encodeWithSelector(FourteenNumbersClaim.upgradeStorage.selector, bytes(""));
        vm.prank(configAdmin);
        fourteenNumbersClaim.upgradeToAndCall(address(v2Impl), initData);

        // Attempt to downgrade from V1 to V0.
        FourteenNumbersClaim v1Impl = new FourteenNumbersClaim();
        vm.expectRevert(abi.encodeWithSelector(FourteenNumbersClaim.CanNotUpgradeToLowerOrSameVersion.selector, 2));
        vm.prank(configAdmin);
        fourteenNumbersClaim.upgradeToAndCall(address(v1Impl), initData);
    }

    function testSetDaysPlayedToClaim() public {
        uint256 newDays = 45;
        vm.prank(configAdmin);
        vm.expectEmit(true, true, true, true);
        emit SettingDaysPlayedToClaim(newDays);
        fourteenNumbersClaim.setDaysPlayedToClaim(newDays);
        assertEq(fourteenNumbersClaim.daysPlayedToClaim(), newDays, "Days played to claim should be updated");
    }

    function testSetDaysPlayedToClaimTooSmall() public {
        uint256 newDays = 6;
        vm.prank(configAdmin);
        vm.expectRevert(abi.encodeWithSelector(
            FourteenNumbersClaim.ProposedNewDaysPlayedToClaimTooSmall.selector, newDays));
        fourteenNumbersClaim.setDaysPlayedToClaim(newDays);
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

        assertEq(mockERC1155.balanceOf(tokenAdmin, TOK1_TOKEN_ID), 0, "Token admin balance wrong");
        assertEq(mockERC1155.balanceOf(address(fourteenNumbersClaim), TOK1_TOKEN_ID), TOK1_AMOUNT, "Contract balance wrong");
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


    function testRemoveTokens() public {
        // First add tokens
        testAddMoreTokens();

        // Then remove some tokens
        uint256 removeAmount = TOK1_AMOUNT / 3;
        vm.prank(tokenAdmin);
        vm.expectEmit(true, true, true, true);
        emit TokensRemoved(1, address(mockERC1155), TOK1_TOKEN_ID, removeAmount);
        fourteenNumbersClaim.removeTokens(1, removeAmount);

        // Check transfer.
        (, , uint256 balance, ) = fourteenNumbersClaim.claimableTokens(1);
        assertEq(balance, TOK1_AMOUNT - removeAmount, "Balance should match");
        assertEq(mockERC1155.balanceOf(tokenAdmin, TOK1_TOKEN_ID), removeAmount, "Token admin balance wrong");
        assertEq(mockERC1155.balanceOf(address(fourteenNumbersClaim), TOK1_TOKEN_ID), TOK1_AMOUNT - removeAmount, "Contract balance wrong");
    }

    function testRemoveTokensBadAccessControl() public {
        // First add tokens
        testAddMoreTokens();

        // Then remove some tokens
        uint256 removeAmount = 0;
        vm.prank(player1);
        vm.expectRevert(abi.encodeWithSelector(AccessControlUnauthorizedAccount.selector, player1, tokenRole));
        fourteenNumbersClaim.removeTokens(1, removeAmount);
    }

    function testRemoveTokensWithZeroAmount() public {
        // First add tokens
        testAddMoreTokens();

        // Then remove some tokens
        uint256 removeAmount = 0;
        vm.prank(tokenAdmin);
        vm.expectRevert(abi.encodeWithSelector(FourteenNumbersClaim.CantRemoveNoTokens.selector));
        fourteenNumbersClaim.removeTokens(1, removeAmount);
    }

    function testRemoveTokensExceedingBalance() public {
        // First add tokens
        testAddMoreTokens();

        // Then remove some tokens
        uint256 removeAmount = TOK1_AMOUNT + 1;
        vm.prank(tokenAdmin);
        vm.expectRevert(abi.encodeWithSelector(FourteenNumbersClaim.BalanceTooLow.selector, 1, TOK1_AMOUNT+1, TOK1_AMOUNT));
        fourteenNumbersClaim.removeTokens(1, removeAmount);
    }

    function testRemoveAllTokens() public {
        // First add tokens
        testAddMoreTokens();

        // Then remove all tokens
        vm.prank(tokenAdmin);
        vm.expectEmit(true, true, true, true);
        emit TokensRemoved(1, address(mockERC1155), TOK1_TOKEN_ID, TOK1_AMOUNT);
        fourteenNumbersClaim.removeAllTokens(1);

        // Check transfer.
        (, , uint256 balance, ) = fourteenNumbersClaim.claimableTokens(1);
        assertEq(balance, 0, "Balance should match");
        assertEq(mockERC1155.balanceOf(tokenAdmin, TOK1_TOKEN_ID), TOK1_AMOUNT, "Token admin balance wrong");
        assertEq(mockERC1155.balanceOf(address(fourteenNumbersClaim), TOK1_TOKEN_ID), 0, "Contract balance wrong");
    }

    function testRemoveAllTokensBadAuth() public {
        // First add tokens
        testAddMoreTokens();

        // Then remove all tokens
        vm.prank(player1);
        vm.expectRevert(abi.encodeWithSelector(AccessControlUnauthorizedAccount.selector, player1, tokenRole));
        fourteenNumbersClaim.removeAllTokens(1);
    }

    function testPassportCheck() public view {
        assertTrue(fourteenNumbersClaim.isPassport(address(passportWallet)), "Passport wallet");
        assertFalse(fourteenNumbersClaim.isPassport(address(fourteenNumbersClaim)), "fourteenNumbersClaim");
    }

    function testRemovePassportWallet() public {
        vm.prank(configAdmin);
        fourteenNumbersClaim.removePassportWallet(address(passportWallet));
        assertFalse(fourteenNumbersClaim.isPassport(address(passportWallet)), "Passport wallet");
    }

    function testAddPassportWallet() public {
        testRemovePassportWallet();
        vm.prank(configAdmin);
        fourteenNumbersClaim.addPassportWallet(address(passportWallet));
        assertTrue(fourteenNumbersClaim.isPassport(address(passportWallet)), "Passport wallet");
    }

    function testAddPassportWalletBadAuth() public {
        testRemovePassportWallet();
        vm.prank(player1);
        vm.expectRevert(abi.encodeWithSelector(AccessControlUnauthorizedAccount.selector, player1, configRole));
        fourteenNumbersClaim.addPassportWallet(address(passportWallet));
    }

    function testRemovePassportWalletBadAuth() public {
        vm.prank(player1);
        vm.expectRevert(abi.encodeWithSelector(AccessControlUnauthorizedAccount.selector, player1, configRole));
        fourteenNumbersClaim.removePassportWallet(address(passportWallet));
    }

    function testPause() public {
        vm.prank(configAdmin);
        fourteenNumbersClaim.pause();
        assertTrue(fourteenNumbersClaim.paused(), "Contract should be paused");
    }

    function testPauseBadAuth() public {
        vm.prank(player1);
        vm.expectRevert();
        fourteenNumbersClaim.pause();
    }

    function testUnpause() public {
        vm.prank(configAdmin);
        fourteenNumbersClaim.pause();
        vm.prank(configAdmin);
        fourteenNumbersClaim.unpause();
        assertFalse(fourteenNumbersClaim.paused(), "Contract should be unpaused");
    }

    function testUnpauseBadAuth() public {
        vm.prank(configAdmin);
        fourteenNumbersClaim.pause();
        vm.prank(player1);
        vm.expectRevert();
        fourteenNumbersClaim.unpause();
    }

    function testPrepareForClaimWhilePaused() public {
        vm.prank(configAdmin);
        fourteenNumbersClaim.pause();
        vm.prank(player1);
        vm.expectRevert(abi.encodeWithSelector(EnforcedPause.selector));
        fourteenNumbersClaim.prepareForClaim(0);
    }

    function testClaimWhilePaused() public {
        vm.prank(configAdmin);
        fourteenNumbersClaim.pause();
        vm.prank(player1);
        vm.expectRevert(abi.encodeWithSelector(EnforcedPause.selector));
        fourteenNumbersClaim.claim(0);
    }
}
