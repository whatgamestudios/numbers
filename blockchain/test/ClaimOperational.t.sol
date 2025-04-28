// Copyright Whatgame Studios 2024 - 2025
// SPDX-License-Identifier: PROPRIETORY
pragma solidity ^0.8.24;

// solhint-disable-next-line no-global-import
import "forge-std/Test.sol";
import {ClaimBaseTest} from "./ClaimBase.t.sol";
import {FourteenNumbersClaim} from "../src/FourteenNumbersClaim.sol";
import {FourteenNumbersSolutions} from "../src/FourteenNumbersSolutions.sol";
import {FourteenNumbersSolutionsV2} from "../src/FourteenNumbersSolutionsV2.sol";
import {IERC1155} from "@openzeppelin/contracts/token/erc1155/IERC1155.sol";

import {ERC1967Proxy} from "@openzeppelin/contracts/proxy/ERC1967/ERC1967Proxy.sol";
import {ImmutableERC1155} from "../src/immutable/ImmutableERC1155.sol";


contract FakeFourteenNumbersClaim is FourteenNumbersClaim {
    uint256 rand;

    function setRand(uint256 _rand) public {
        rand = _rand;
    }

    function _generateRandom() internal override view returns (uint256) {
        return rand;
    }
}

contract FakeFourteenNumbersSolutions is FourteenNumbersSolutionsV2 {
    function setDaysPlayed(uint256 _daysPlayed) public {
        Stats storage playerStats = stats[msg.sender];
        playerStats.daysPlayed = _daysPlayed;
    }
}


contract ClaimOperationalTest is ClaimBaseTest {
    // error AddMoreTokensBalanceMustBeNonZero();
    // error AddMoreTokensPercentageTooLarge();

    // error ERC1155InsufficientBalance(address sender, uint256 balance, uint256 needed, uint256 tokenId);
    // error ERC1155MissingApprovalForAll(address target, address sender);
    // error ERC1155InvalidReceiver(address receiver);
    // error AccessControlUnauthorizedAccount(address account, bytes32 role);
    event TokensAdded(uint256 _slot, address _erc1155Contract, uint256 _tokenId, uint256 _amount, uint256 _percentage);

    event Claimed(address _gamePlayer, address _erc1155Contract, uint256 _tokenId, uint256 _daysPlayed, uint256 _claimedSoFar);
    event TransferSingle(address indexed operator, address indexed from, address indexed to, uint256 id, uint256 value);


    FakeFourteenNumbersClaim fakeFourteenNumbersClaim;
    FakeFourteenNumbersSolutions fakeFourteenNumbersSolutions;

    function setUp() public virtual override {
        super.setUp();
        setUpFakeFourteenNumbersSolutionsV2();
        setUpFakeFourteenNumbersClaim();
        configureOperatorAllowlist();
    }

    function setUpFakeFourteenNumbersSolutionsV2() private {
        address upgradeAdmin = makeAddr("UpgradeAdmin");
        FourteenNumbersSolutions impl = new FourteenNumbersSolutions();
        bytes memory initData = abi.encodeWithSelector(
            FourteenNumbersSolutions.initialize.selector, address(0), address(0), upgradeAdmin);
        ERC1967Proxy proxy = new ERC1967Proxy(address(impl), initData);
        fakeFourteenNumbersSolutions = FakeFourteenNumbersSolutions(address(proxy));

        FakeFourteenNumbersSolutions v2Impl = new FakeFourteenNumbersSolutions();
        initData = abi.encodeWithSelector(FourteenNumbersSolutionsV2.upgradeStorage.selector, bytes(""));
        vm.prank(upgradeAdmin);
        fakeFourteenNumbersSolutions.upgradeToAndCall(address(v2Impl), initData);
    }

    function setUpFakeFourteenNumbersClaim() private {
        FakeFourteenNumbersClaim impl = new FakeFourteenNumbersClaim();
        bytes memory initData = abi.encodeWithSelector(
            FourteenNumbersClaim.initialize.selector, 
            roleAdmin, owner, configAdmin, tokenAdmin, passportWallet, fakeFourteenNumbersSolutions);
        ERC1967Proxy proxy = new ERC1967Proxy(address(impl), initData);
        fakeFourteenNumbersClaim = FakeFourteenNumbersClaim(address(proxy));
    }

    function configureOperatorAllowlist() private {
        // Add fake claim contract to the allowlist
        address[] memory contracts = new address[](1);
        contracts[0] = address(fakeFourteenNumbersClaim);
        vm.prank(operatorRegistrarAdmin);
        allowList.addAddressesToAllowlist(contracts);
    }


    function testGetClaimableNfts() public {
        addTokens();
        FourteenNumbersClaim.ClaimableToken[] memory claimableTokens = fakeFourteenNumbersClaim.getClaimableNfts();

        assertEq(claimableTokens.length, 3, "Length");
        assertEq(claimableTokens[0].erc1155Contract, address(mockERC1155), "ERC1155[0]");
        assertEq(claimableTokens[0].tokenId, TOK1_TOKEN_ID, "Token ID[0]");
        assertEq(claimableTokens[0].balance, TOK1_AMOUNT, "Balance[0]");
        assertEq(claimableTokens[0].percentage, TOK1_PERCENTAGE, "Percentage[0]");

        assertEq(claimableTokens[1].erc1155Contract, address(mockERC1155), "ERC1155[1]");
        assertEq(claimableTokens[1].tokenId, TOK2_TOKEN_ID, "Token ID[1]");
        assertEq(claimableTokens[1].balance, TOK2_AMOUNT, "Balance[1]");
        assertEq(claimableTokens[1].percentage, TOK2_PERCENTAGE, "Percentage[1]");

        assertEq(claimableTokens[2].erc1155Contract, address(mockERC1155), "ERC1155[2]");
        assertEq(claimableTokens[2].tokenId, TOK3_TOKEN_ID, "Token ID[2]");
        assertEq(claimableTokens[2].balance, TOK3_AMOUNT, "Balance[2]");
        assertEq(claimableTokens[2].percentage, TOK3_PERCENTAGE, "Percentage[2]");
    }

    function testCheckSlotIds() public {
        addTokens();

        assertEq(fakeFourteenNumbersClaim.firstInUseClaimableTokenSlot(), 1, "First");
        assertEq(fakeFourteenNumbersClaim.nextSpareClaimableTokenSlot(), 4, "Next");
    }

    function testClaimableNfts() public {
        addTokens();

        (address erc1155Contract, uint256 tokenId, uint256 balance, uint256 percentage) = fakeFourteenNumbersClaim.claimableTokens(1);
        assertEq(erc1155Contract, address(mockERC1155), "ERC1155[0]");
        assertEq(tokenId, TOK1_TOKEN_ID, "Token ID[0]");
        assertEq(balance, TOK1_AMOUNT, "Balance[0]");
        assertEq(percentage, TOK1_PERCENTAGE, "Percentage[0]");

        (erc1155Contract, tokenId, balance, percentage) = fakeFourteenNumbersClaim.claimableTokens(2);
        assertEq(erc1155Contract, address(mockERC1155), "ERC1155[1]");
        assertEq(tokenId, TOK2_TOKEN_ID, "Token ID[1]");
        assertEq(balance, TOK2_AMOUNT, "Balance[1]");
        assertEq(percentage, TOK2_PERCENTAGE, "Percentage[1]");

        (erc1155Contract, tokenId, balance, percentage) = fakeFourteenNumbersClaim.claimableTokens(3);
        assertEq(erc1155Contract, address(mockERC1155), "ERC1155[2]");
        assertEq(tokenId, TOK3_TOKEN_ID, "Token ID[2]");
        assertEq(balance, TOK3_AMOUNT, "Balance[2]");
        assertEq(percentage, TOK3_PERCENTAGE, "Percentage[2]");
    }


    function testClaim() public {
        addTokens();

        uint256 daysPlayed = fakeFourteenNumbersClaim.daysPlayedToClaim() + 3;
        vm.prank(passportWalletAddress);
        fakeFourteenNumbersSolutions.setDaysPlayed(daysPlayed);

        fakeFourteenNumbersClaim.setRand(1);

        (, , uint256 balance1, ) = fakeFourteenNumbersClaim.claimableTokens(1);
        assertEq(balance1, TOK1_AMOUNT, "Balance start should match");

        vm.prank(passportWalletAddress);
        vm.expectEmit(true, true, true, true);
        emit TransferSingle(address(fakeFourteenNumbersClaim), address(fakeFourteenNumbersClaim), 
            passportWalletAddress, TOK1_TOKEN_ID, 1);
        vm.expectEmit(true, true, true, true);
        emit Claimed(passportWalletAddress, address(mockERC1155), TOK1_TOKEN_ID, daysPlayed, 0);
        fakeFourteenNumbersClaim.claim();

        (, , uint256 balance, ) = fakeFourteenNumbersClaim.claimableTokens(1);
        assertEq(balance, TOK1_AMOUNT - 1, "Balance should match");
        assertEq(mockERC1155.balanceOf(passportWalletAddress, TOK1_TOKEN_ID), 1, "Player balance wrong");
        assertEq(mockERC1155.balanceOf(address(fakeFourteenNumbersClaim), TOK1_TOKEN_ID), TOK1_AMOUNT - 1, "Contract balance wrong");
    }

    function addTokens() public {
        vm.prank(tokenAdmin);
        mockERC1155.setApprovalForAll(address(fakeFourteenNumbersClaim), true);

        FourteenNumbersClaim.ClaimableToken memory token;
        token = FourteenNumbersClaim.ClaimableToken({
            erc1155Contract: address(mockERC1155),
            tokenId: TOK1_TOKEN_ID,
            balance: TOK1_AMOUNT,
            percentage: TOK1_PERCENTAGE
        });
        vm.prank(tokenAdmin);
        vm.expectEmit(true, true, true, true);
        emit TokensAdded(1, address(mockERC1155), TOK1_TOKEN_ID, TOK1_AMOUNT, TOK1_PERCENTAGE);
        fakeFourteenNumbersClaim.addMoreTokens(token);

        token = FourteenNumbersClaim.ClaimableToken({
            erc1155Contract: address(mockERC1155),
            tokenId: TOK2_TOKEN_ID,
            balance: TOK2_AMOUNT,
            percentage: TOK2_PERCENTAGE
        });
        vm.prank(tokenAdmin);
        vm.expectEmit(true, true, true, true);
        emit TokensAdded(2, address(mockERC1155), TOK2_TOKEN_ID, TOK2_AMOUNT, TOK2_PERCENTAGE);
        fakeFourteenNumbersClaim.addMoreTokens(token);

        token = FourteenNumbersClaim.ClaimableToken({
            erc1155Contract: address(mockERC1155),
            tokenId: TOK3_TOKEN_ID,
            balance: TOK3_AMOUNT,
            percentage: TOK3_PERCENTAGE
        });
        vm.prank(tokenAdmin);
        vm.expectEmit(true, true, true, true);
        emit TokensAdded(3, address(mockERC1155), TOK3_TOKEN_ID, TOK3_AMOUNT, TOK3_PERCENTAGE);
        fakeFourteenNumbersClaim.addMoreTokens(token);
    }


}

