// Copyright Whatgame Studios 2024 - 2025
// SPDX-License-Identifier: PROPRIETORY
// solhint-disable not-rely-on-time


pragma solidity ^0.8.24;

// solhint-disable-next-line no-global-import
import "forge-std/Test.sol";
import {ClaimBaseTest} from "./ClaimBase.t.sol";
import {FourteenNumbersClaim} from "../src/FourteenNumbersClaim.sol";


contract ClaimInitTest is ClaimBaseTest {
    uint256 private constant DEFAULT_DAYS_PLAYED_TO_CLAIM = 30;

    function testInitAdmins() public view {
        assertEq(fourteenNumbersClaim.owner(), owner, "Owner should be set correctly");
        assertTrue(fourteenNumbersClaim.hasRole(configRole, configAdmin), "Config admin should have config role");
        assertTrue(fourteenNumbersClaim.hasRole(defaultAdminRole, roleAdmin), "Role admin should have admin role");
    }

    function testInitPublics() public view {
        assertEq(fourteenNumbersClaim.daysPlayedToClaim(), DEFAULT_DAYS_PLAYED_TO_CLAIM, "Default days to claim should be set");
        assertEq(fourteenNumbersClaim.nextSpareClaimableTokenSlot(), 1, "nextSpareClaimableTokenSlot should be 1");
        assertEq(fourteenNumbersClaim.firstInUseClaimableTokenSlot(), 1, "firstInUseClaimableTokenSlot should be 1");
        assertEq(address(fourteenNumbersClaim.fourteenNumbersSolutions()), address(fourteenNumbersSolutions), "solutions");
        assertEq(fourteenNumbersClaim.version(), 0, "version");
        assertEq(fourteenNumbersClaim.claimedDay(player1), 0, "claimedDay");

        (address erc1155Contract, uint256 tokenId, uint256 balance, uint256 percentage) = fourteenNumbersClaim.claimableTokens(1);
        assertEq(erc1155Contract, address(0), "ERC1155 contract");
        assertEq(tokenId, 0, "Token ID");
        assertEq(balance, 0, "Balance");
        assertEq(percentage, 0, "Percentage");
    }

    function testInitGetClaimableNfts() public view {
        FourteenNumbersClaim.ClaimableToken[] memory claimableTokens = fourteenNumbersClaim.getClaimableNfts();
        assertEq(claimableTokens.length, 0, "No claimable tokens should be set");
    }

    function testRemoveCoverageWarnings() public view {
        uint256[] memory none = new uint256[](0);
        fourteenNumbersClaim.onERC1155BatchReceived(address(0), address(0), none, none, bytes(""));
    }
}
