// Copyright Whatgame Studios 2024 - 2025
// SPDX-License-Identifier: PROPRIETORY
pragma solidity ^0.8.24;

// solhint-disable-next-line no-global-import
import "forge-std/Test.sol";
import {ClaimBaseTest} from "./ClaimBase.t.sol";
import {FourteenNumbersClaim} from "../src/FourteenNumbersClaim.sol";
import {IERC1155} from "@openzeppelin/contracts/token/erc1155/IERC1155.sol";

contract ClaimOperationalTest is ClaimBaseTest {
    // error AddMoreTokensBalanceMustBeNonZero();
    // error AddMoreTokensPercentageTooLarge();

    // error ERC1155InsufficientBalance(address sender, uint256 balance, uint256 needed, uint256 tokenId);
    // error ERC1155MissingApprovalForAll(address target, address sender);
    // error ERC1155InvalidReceiver(address receiver);
    // error AccessControlUnauthorizedAccount(address account, bytes32 role);

    // event SettingDaysPlayedToClaim(uint256 _newDaysPlayedToClaim);
    // event TokensAdded(uint256 _slot, address _erc1155Contract, uint256 _tokenId, uint256 _amount, uint256 _percentage);
    // event TokensRemoved(uint256 _slot, address _erc1155Contract, uint256 _tokenId, uint256 _amount);
    // event Claimed(address _gamePlayer, address _erc1155Contract, uint256 _tokenId, uint256 _daysPlayed, uint256 _claimedSoFar);


    function setUp() public virtual override {
        super.setUp();
    }




}

