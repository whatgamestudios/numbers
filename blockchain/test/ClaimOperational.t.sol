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

    function _generateRandom(uint256 /* _salt */) internal override view returns (uint256) {
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

        assertEq(claimableTokens.length, 5, "Length");
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

        assertEq(claimableTokens[3].erc1155Contract, address(mockERC1155), "ERC1155[3]");
        assertEq(claimableTokens[3].tokenId, TOK4_TOKEN_ID, "Token ID[3]");
        assertEq(claimableTokens[3].balance, TOK4_AMOUNT, "Balance[3]");
        assertEq(claimableTokens[3].percentage, TOK4_PERCENTAGE, "Percentage[3]");

        assertEq(claimableTokens[4].erc1155Contract, address(mockERC1155), "ERC1155[4]");
        assertEq(claimableTokens[4].tokenId, TOK5_TOKEN_ID, "Token ID[4]");
        assertEq(claimableTokens[4].balance, TOK5_AMOUNT, "Balance[4]");
        assertEq(claimableTokens[4].percentage, TOK5_PERCENTAGE, "Percentage[4]");
    }

    function testCheckSlotIds() public {
        addTokens();

        assertEq(fakeFourteenNumbersClaim.firstInUseClaimableTokenSlot(), 1, "First");
        assertEq(fakeFourteenNumbersClaim.nextSpareClaimableTokenSlot(), 6, "Next");
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

        uint256 daysPlayedToClaim = fakeFourteenNumbersClaim.daysPlayedToClaim();
        uint256 daysPlayed = daysPlayedToClaim + 3;
        vm.prank(passportWalletAddress);
        fakeFourteenNumbersSolutions.setDaysPlayed(daysPlayed);

        fakeFourteenNumbersClaim.setRand(1);

        (, , uint256 balance1, ) = fakeFourteenNumbersClaim.claimableTokens(1);
        assertEq(balance1, TOK1_AMOUNT, "Balance start should match");

        uint256 _salt = 0;
        vm.prank(passportWalletAddress);
        fakeFourteenNumbersClaim.prepareForClaim(_salt);
        vm.roll(block.number + fakeFourteenNumbersClaim.RANDOM_DELAY());

        vm.prank(passportWalletAddress);
        vm.expectEmit(true, true, true, true);
        emit TransferSingle(address(fakeFourteenNumbersClaim), address(fakeFourteenNumbersClaim), 
            passportWalletAddress, TOK1_TOKEN_ID, 1);
        vm.expectEmit(true, true, true, true);
        emit Claimed(passportWalletAddress, address(mockERC1155), TOK1_TOKEN_ID, daysPlayed, 0);
        fakeFourteenNumbersClaim.claim(_salt);

        (, , uint256 balance, ) = fakeFourteenNumbersClaim.claimableTokens(1);
        assertEq(balance, TOK1_AMOUNT - 1, "Balance should match");
        assertEq(mockERC1155.balanceOf(passportWalletAddress, TOK1_TOKEN_ID), 1, "Player balance wrong");
        assertEq(mockERC1155.balanceOf(address(fakeFourteenNumbersClaim), TOK1_TOKEN_ID), TOK1_AMOUNT - 1, "Contract balance wrong");

        assertEq(fakeFourteenNumbersClaim.claimedDay(passportWalletAddress), daysPlayedToClaim, "Claimed day after claim");
    }

    function testClaimTok1a() public {
        checkClaim(0, TOK1_TOKEN_ID);
    }

    function testClaimTok1b() public {
        checkClaim(TOK1_PERCENTAGE-1, TOK1_TOKEN_ID);
    }

    function testClaimTok2a() public {
        checkClaim(TOK1_PERCENTAGE, TOK2_TOKEN_ID);
    }

    function testClaimTok2b() public {
        checkClaim(TOK1_PERCENTAGE + TOK2_PERCENTAGE - 1, TOK2_TOKEN_ID);
    }

    function testClaimTok4a() public {
        checkClaim(TOK1_PERCENTAGE + TOK2_PERCENTAGE, TOK4_TOKEN_ID);
    }

    function testClaimTok4b() public {
        checkClaim(TOK1_PERCENTAGE + TOK2_PERCENTAGE + TOK4_PERCENTAGE - 1, TOK4_TOKEN_ID);
    }

    function testClaimTok5a() public {
        checkClaim(TOK1_PERCENTAGE + TOK2_PERCENTAGE + TOK4_PERCENTAGE, TOK3_TOKEN_ID);
    }

    function testClaimTok5b() public {
        uint256 ONE_HUNDRED_PERCENT = 10000;
        checkClaim(ONE_HUNDRED_PERCENT, TOK3_TOKEN_ID);
    }

    function testClaimMultiple() public {
        addTokens();

        uint256 daysPlayedToClaim = fakeFourteenNumbersClaim.daysPlayedToClaim();
        uint256 daysPlayed = 10 * daysPlayedToClaim;
        vm.prank(passportWalletAddress);
        fakeFourteenNumbersSolutions.setDaysPlayed(daysPlayed);

        fakeFourteenNumbersClaim.setRand(0);

        uint256 _salt = 0;
        vm.prank(passportWalletAddress);
        fakeFourteenNumbersClaim.prepareForClaim(_salt);
        vm.roll(block.number + fakeFourteenNumbersClaim.RANDOM_DELAY());

        vm.prank(passportWalletAddress);
        vm.expectEmit(true, true, true, true);
        emit TransferSingle(address(fakeFourteenNumbersClaim), address(fakeFourteenNumbersClaim), 
            passportWalletAddress, TOK1_TOKEN_ID, 1);
        vm.expectEmit(true, true, true, true);
        emit Claimed(passportWalletAddress, address(mockERC1155), TOK1_TOKEN_ID, daysPlayed, 0);
        fakeFourteenNumbersClaim.claim(_salt);

        vm.prank(passportWalletAddress);
        fakeFourteenNumbersClaim.prepareForClaim(_salt);
        vm.roll(block.number + fakeFourteenNumbersClaim.RANDOM_DELAY());

        vm.prank(passportWalletAddress);
        vm.expectEmit(true, true, true, true);
        emit TransferSingle(address(fakeFourteenNumbersClaim), address(fakeFourteenNumbersClaim), 
            passportWalletAddress, TOK1_TOKEN_ID, 1);
        vm.expectEmit(true, true, true, true);
        emit Claimed(passportWalletAddress, address(mockERC1155), TOK1_TOKEN_ID, daysPlayed, daysPlayedToClaim);
        fakeFourteenNumbersClaim.claim(_salt);

        vm.prank(passportWalletAddress);
        fakeFourteenNumbersClaim.prepareForClaim(_salt);
        vm.roll(block.number + fakeFourteenNumbersClaim.RANDOM_DELAY());

        vm.prank(passportWalletAddress);
        vm.expectEmit(true, true, true, true);
        emit TransferSingle(address(fakeFourteenNumbersClaim), address(fakeFourteenNumbersClaim), 
            passportWalletAddress, TOK1_TOKEN_ID, 1);
        vm.expectEmit(true, true, true, true);
        emit Claimed(passportWalletAddress, address(mockERC1155), TOK1_TOKEN_ID, daysPlayed, 2 * daysPlayedToClaim);
        fakeFourteenNumbersClaim.claim(_salt);

        vm.prank(passportWalletAddress);
        fakeFourteenNumbersClaim.prepareForClaim(_salt);
        vm.roll(block.number + fakeFourteenNumbersClaim.RANDOM_DELAY());

        vm.prank(passportWalletAddress);
        vm.expectEmit(true, true, true, true);
        emit TransferSingle(address(fakeFourteenNumbersClaim), address(fakeFourteenNumbersClaim), 
            passportWalletAddress, TOK2_TOKEN_ID, 1);
        vm.expectEmit(true, true, true, true);
        emit Claimed(passportWalletAddress, address(mockERC1155), TOK2_TOKEN_ID, daysPlayed, 3 * daysPlayedToClaim);
        fakeFourteenNumbersClaim.claim(_salt);

        assertEq(fakeFourteenNumbersClaim.firstInUseClaimableTokenSlot(), 2, "First");
    }

    function testClaimMultipleInfinite() public {
        addTokens();

        uint256 daysPlayedToClaim = fakeFourteenNumbersClaim.daysPlayedToClaim();
        uint256 daysPlayed = 10 * daysPlayedToClaim;
        vm.prank(passportWalletAddress);
        fakeFourteenNumbersSolutions.setDaysPlayed(daysPlayed);

        uint256 ONE_HUNDRED_PERCENT = 10000;
        fakeFourteenNumbersClaim.setRand(ONE_HUNDRED_PERCENT);

        uint256 _salt = 0;
        vm.prank(passportWalletAddress);
        fakeFourteenNumbersClaim.prepareForClaim(_salt);
        vm.roll(block.number + fakeFourteenNumbersClaim.RANDOM_DELAY());

        vm.prank(passportWalletAddress);
        vm.expectEmit(true, true, true, true);
        emit TransferSingle(address(fakeFourteenNumbersClaim), address(fakeFourteenNumbersClaim), 
            passportWalletAddress, TOK3_TOKEN_ID, 1);
        vm.expectEmit(true, true, true, true);
        emit Claimed(passportWalletAddress, address(mockERC1155), TOK3_TOKEN_ID, daysPlayed, 0);
        fakeFourteenNumbersClaim.claim(_salt);

        vm.prank(passportWalletAddress);
        fakeFourteenNumbersClaim.prepareForClaim(_salt);
        vm.roll(block.number + fakeFourteenNumbersClaim.RANDOM_DELAY());

        vm.prank(passportWalletAddress);
        vm.expectEmit(true, true, true, true);
        emit TransferSingle(address(fakeFourteenNumbersClaim), address(fakeFourteenNumbersClaim), 
            passportWalletAddress, TOK3_TOKEN_ID, 1);
        vm.expectEmit(true, true, true, true);
        emit Claimed(passportWalletAddress, address(mockERC1155), TOK3_TOKEN_ID, daysPlayed, daysPlayedToClaim);
        fakeFourteenNumbersClaim.claim(_salt);

        vm.prank(passportWalletAddress);
        fakeFourteenNumbersClaim.prepareForClaim(_salt);
        vm.roll(block.number + fakeFourteenNumbersClaim.RANDOM_DELAY());

        vm.prank(passportWalletAddress);
        vm.expectEmit(true, true, true, true);
        emit TransferSingle(address(fakeFourteenNumbersClaim), address(fakeFourteenNumbersClaim), 
            passportWalletAddress, TOK5_TOKEN_ID, 1);
        vm.expectEmit(true, true, true, true);
        emit Claimed(passportWalletAddress, address(mockERC1155), TOK5_TOKEN_ID, daysPlayed, 2 * daysPlayedToClaim);
        fakeFourteenNumbersClaim.claim(_salt);

        assertEq(fakeFourteenNumbersClaim.firstInUseClaimableTokenSlot(), 1, "First");
    }

    function testClaimTooSoon() public {
        addTokens();

        uint256 daysPlayedToClaim = fakeFourteenNumbersClaim.daysPlayedToClaim();
        uint256 daysPlayed = daysPlayedToClaim + 3;
        vm.prank(passportWalletAddress);
        fakeFourteenNumbersSolutions.setDaysPlayed(daysPlayed);

        fakeFourteenNumbersClaim.setRand(1);

        uint256 _salt = 0;
        vm.prank(passportWalletAddress);
        fakeFourteenNumbersClaim.prepareForClaim(_salt);
        uint256 targetBlockNum = block.number + fakeFourteenNumbersClaim.RANDOM_DELAY() - 1;
        vm.roll(targetBlockNum);

        vm.prank(passportWalletAddress);
        vm.expectRevert(abi.encodeWithSelector(
            FourteenNumbersClaim.ClaimTooSoon.selector, passportWalletAddress, _salt, targetBlockNum, block.number));
        fakeFourteenNumbersClaim.claim(_salt);
    }

    function testClaimTooLate() public {
        addTokens();

        uint256 daysPlayedToClaim = fakeFourteenNumbersClaim.daysPlayedToClaim();
        uint256 daysPlayed = daysPlayedToClaim + 3;
        vm.prank(passportWalletAddress);
        fakeFourteenNumbersSolutions.setDaysPlayed(daysPlayed);

        fakeFourteenNumbersClaim.setRand(1);

        uint256 _salt = 0;
        vm.prank(passportWalletAddress);
        fakeFourteenNumbersClaim.prepareForClaim(_salt);
        uint256 targetBlockNum = block.number + fakeFourteenNumbersClaim.RANDOM_DELAY() - 1;
        vm.roll(targetBlockNum + 256);

        vm.prank(passportWalletAddress);
        vm.expectRevert(abi.encodeWithSelector(
            FourteenNumbersClaim.ClaimTooLate.selector, passportWalletAddress, _salt, targetBlockNum, block.number));
        fakeFourteenNumbersClaim.claim(_salt);
    }

    function testReusePrepare() public {
        addTokens();

        uint256 daysPlayedToClaim = fakeFourteenNumbersClaim.daysPlayedToClaim();
        uint256 daysPlayed = daysPlayedToClaim + 3;
        vm.prank(passportWalletAddress);
        fakeFourteenNumbersSolutions.setDaysPlayed(daysPlayed);

        fakeFourteenNumbersClaim.setRand(1);

        uint256 _salt = 0;
        vm.prank(passportWalletAddress);
        fakeFourteenNumbersClaim.prepareForClaim(_salt);
        uint256 targetBlockNum = block.number + fakeFourteenNumbersClaim.RANDOM_DELAY();
        vm.roll(targetBlockNum);

        vm.prank(passportWalletAddress);
        fakeFourteenNumbersClaim.claim(_salt);

        vm.prank(passportWalletAddress);
        vm.expectRevert(abi.encodeWithSelector(
            FourteenNumbersClaim.ClaimBeforeClaimPrepare.selector, passportWalletAddress, _salt));
        fakeFourteenNumbersClaim.claim(_salt);
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

        token = FourteenNumbersClaim.ClaimableToken({
            erc1155Contract: address(mockERC1155),
            tokenId: TOK4_TOKEN_ID,
            balance: TOK4_AMOUNT,
            percentage: TOK4_PERCENTAGE
        });
        vm.prank(tokenAdmin);
        vm.expectEmit(true, true, true, true);
        emit TokensAdded(4, address(mockERC1155), TOK4_TOKEN_ID, TOK4_AMOUNT, TOK4_PERCENTAGE);
        fakeFourteenNumbersClaim.addMoreTokens(token);

        token = FourteenNumbersClaim.ClaimableToken({
            erc1155Contract: address(mockERC1155),
            tokenId: TOK5_TOKEN_ID,
            balance: TOK5_AMOUNT,
            percentage: TOK5_PERCENTAGE
        });
        vm.prank(tokenAdmin);
        vm.expectEmit(true, true, true, true);
        emit TokensAdded(5, address(mockERC1155), TOK5_TOKEN_ID, TOK5_AMOUNT, TOK5_PERCENTAGE);
        fakeFourteenNumbersClaim.addMoreTokens(token);
    }

    function checkClaim(uint256 _percentage, uint256 _tokenId) public {
        addTokens();

        uint256 daysPlayedToClaim = fakeFourteenNumbersClaim.daysPlayedToClaim();
        uint256 daysPlayed = daysPlayedToClaim + 3;
        vm.prank(passportWalletAddress);
        fakeFourteenNumbersSolutions.setDaysPlayed(daysPlayed);

        fakeFourteenNumbersClaim.setRand(_percentage);

        uint256 _salt = 0;
        vm.prank(passportWalletAddress);
        fakeFourteenNumbersClaim.prepareForClaim(_salt);
        vm.roll(block.number + fakeFourteenNumbersClaim.RANDOM_DELAY());

        vm.prank(passportWalletAddress);
        vm.expectEmit(true, true, true, true);
        emit TransferSingle(address(fakeFourteenNumbersClaim), address(fakeFourteenNumbersClaim), 
            passportWalletAddress, _tokenId, 1);
        vm.expectEmit(true, true, true, true);
        emit Claimed(passportWalletAddress, address(mockERC1155), _tokenId, daysPlayed, 0);
        fakeFourteenNumbersClaim.claim(_salt);
    }
}

