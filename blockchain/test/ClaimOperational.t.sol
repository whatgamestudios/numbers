// Copyright Whatgame Studios 2024 - 2025
// SPDX-License-Identifier: PROPRIETORY
// solhint-disable not-rely-on-time

pragma solidity ^0.8.24;

// solhint-disable-next-line no-global-import
import "forge-std/Test.sol";
import {ClaimBaseTest} from "./ClaimBase.t.sol";
import {FourteenNumbersClaim} from "../src/FourteenNumbersClaim.sol";

contract ClaimOperationalTest is ClaimBaseTest {

//     function testInit() public view {
//         assertEq(fourteenNumbersClaim.owner(), owner, "Owner");
//         assertTrue(fourteenNumbersClaim.hasRole(configRole, configAdmin), "Config");
//         assertTrue(fourteenNumbersClaim.hasRole(defaultAdminRole, roleAdmin), "Role admin");
//     }
// }

// // Copyright Whatgame Studios 2024 - 2025
// // SPDX-License-Identifier: PROPRIETORY
// // solhint-disable not-rely-on-time

// pragma solidity ^0.8.24;

// // solhint-disable-next-line no-global-import
// import "forge-std/Test.sol";
// import {ClaimBaseTest} from "./ClaimBase.t.sol";
// import {FourteenNumbersClaim} from "../src/FourteenNumbersClaim.sol";
// import {IERC1155} from "@openzeppelin/contracts/token/erc1155/IERC1155.sol";

// contract MockERC1155 is IERC1155 {
//     function balanceOf(address, uint256) external pure returns (uint256) {
//         return 100;
//     }
    
//     function balanceOfBatch(address[] calldata, uint256[] calldata) external pure returns (uint256[] memory) {
//         return new uint256[](0);
//     }
    
//     function setApprovalForAll(address, bool) external pure {}
    
//     function isApprovedForAll(address, address) external pure returns (bool) {
//         return true;
//     }

//     function safeTransferFrom(address, address, uint256, uint256, bytes calldata) external {}
    
//     function safeBatchTransferFrom(address, address, uint256[] calldata, uint256[] calldata, bytes calldata) external {}

//     function supportsInterface(bytes4) external pure returns (bool) {
//         return true;
//     }
// }

// contract ClaimInitTest is ClaimBaseTest {
//     event SettingDaysPlayedToClaim(uint256 _newDaysPlayedToClaim);
//     event TokensAdded(uint256 _slot, address _erc1155Contract, uint256 _tokenId, uint256 _amount, uint256 _percentage);
//     event TokensRemoved(uint256 _slot, address _erc1155Contract, uint256 _tokenId, uint256 _amount);
//     event Claimed(address _gamePlayer, address _erc1155Contract, uint256 _tokenId, uint256 _daysPlayed, uint256 _claimedSoFar);

//     MockERC1155 public mockERC1155;
//     address public user1;
//     address public user2;
//     uint256 constant DEFAULT_TOKEN_ID = 1;
//     uint256 constant DEFAULT_AMOUNT = 100;
//     uint256 constant DEFAULT_PERCENTAGE = 5000; // 50%

//     function setUp() public virtual override {
//         super.setUp();
//         mockERC1155 = new MockERC1155();
//         user1 = makeAddr("user1");
//         user2 = makeAddr("user2");
        
//         // Add user1 to passport allowlist
//         vm.startPrank(configAdmin);
//         fourteenNumbersClaim.addWalletToAllowlist(user1);
//         vm.stopPrank();
//     }

//     function testInit() public {
//         assertEq(fourteenNumbersClaim.owner(), owner, "Owner should be set correctly");
//         assertTrue(fourteenNumbersClaim.hasRole(configRole, configAdmin), "Config admin should have config role");
//         assertTrue(fourteenNumbersClaim.hasRole(defaultAdminRole, roleAdmin), "Role admin should have admin role");
//         assertEq(fourteenNumbersClaim.daysPlayedToClaim(), fourteenNumbersClaim.DEFAULT_DAYS_PLAYED_TO_CLAIM(), "Default days to claim should be set");
//     }

//     function testSetDaysPlayedToClaim() public {
//         uint256 newDays = 45;
//         vm.startPrank(configAdmin);
//         vm.expectEmit(true, true, true, true);
//         emit SettingDaysPlayedToClaim(newDays);
//         fourteenNumbersClaim.setDaysPlayedToClaim(newDays);
//         assertEq(fourteenNumbersClaim.daysPlayedToClaim(), newDays, "Days played to claim should be updated");
//         vm.stopPrank();
//     }

//     function testAddMoreTokens() public {
//         vm.startPrank(configAdmin);
//         FourteenNumbersClaim.ClaimableToken memory token = FourteenNumbersClaim.ClaimableToken({
//             erc1155Contract: address(mockERC1155),
//             tokenId: DEFAULT_TOKEN_ID,
//             balance: DEFAULT_AMOUNT,
//             percentage: DEFAULT_PERCENTAGE
//         });

//         vm.expectEmit(true, true, true, true);
//         emit TokensAdded(1, address(mockERC1155), DEFAULT_TOKEN_ID, DEFAULT_AMOUNT, DEFAULT_PERCENTAGE);
//         fourteenNumbersClaim.addMoreTokens(token);
//         vm.stopPrank();

//         FourteenNumbersClaim.ClaimableToken memory stored = fourteenNumbersClaim.claimableTokens(1);
//         assertEq(stored.erc1155Contract, address(mockERC1155), "ERC1155 contract should match");
//         assertEq(stored.tokenId, DEFAULT_TOKEN_ID, "Token ID should match");
//         assertEq(stored.balance, DEFAULT_AMOUNT, "Balance should match");
//         assertEq(stored.percentage, DEFAULT_PERCENTAGE, "Percentage should match");
//     }

//     function testFailAddMoreTokensWithZeroBalance() public {
//         vm.startPrank(configAdmin);
//         FourteenNumbersClaim.ClaimableToken memory token = FourteenNumbersClaim.ClaimableToken({
//             erc1155Contract: address(mockERC1155),
//             tokenId: DEFAULT_TOKEN_ID,
//             balance: 0,
//             percentage: DEFAULT_PERCENTAGE
//         });
//         fourteenNumbersClaim.addMoreTokens(token);
//         vm.stopPrank();
//     }

//     function testFailAddMoreTokensWithInvalidPercentage() public {
//         vm.startPrank(configAdmin);
//         FourteenNumbersClaim.ClaimableToken memory token = FourteenNumbersClaim.ClaimableToken({
//             erc1155Contract: address(mockERC1155),
//             tokenId: DEFAULT_TOKEN_ID,
//             balance: DEFAULT_AMOUNT,
//             percentage: 10001 // More than 100%
//         });
//         fourteenNumbersClaim.addMoreTokens(token);
//         vm.stopPrank();
//     }

//     function testRemoveTokens() public {
//         // First add tokens
//         vm.startPrank(configAdmin);
//         FourteenNumbersClaim.ClaimableToken memory token = FourteenNumbersClaim.ClaimableToken({
//             erc1155Contract: address(mockERC1155),
//             tokenId: DEFAULT_TOKEN_ID,
//             balance: DEFAULT_AMOUNT,
//             percentage: DEFAULT_PERCENTAGE
//         });
//         fourteenNumbersClaim.addMoreTokens(token);

//         // Then remove some tokens
//         uint256 removeAmount = 50;
//         vm.expectEmit(true, true, true, true);
//         emit TokensRemoved(1, address(mockERC1155), DEFAULT_TOKEN_ID, removeAmount);
//         fourteenNumbersClaim.removeTokens(1, removeAmount);
//         vm.stopPrank();
//     }

//     function testFailRemoveTokensWithZeroAmount() public {
//         vm.startPrank(configAdmin);
//         fourteenNumbersClaim.removeTokens(1, 0);
//         vm.stopPrank();
//     }

//     function testFailRemoveTokensExceedingBalance() public {
//         // First add tokens
//         vm.startPrank(configAdmin);
//         FourteenNumbersClaim.ClaimableToken memory token = FourteenNumbersClaim.ClaimableToken({
//             erc1155Contract: address(mockERC1155),
//             tokenId: DEFAULT_TOKEN_ID,
//             balance: DEFAULT_AMOUNT,
//             percentage: DEFAULT_PERCENTAGE
//         });
//         fourteenNumbersClaim.addMoreTokens(token);

//         // Try to remove more than available
//         fourteenNumbersClaim.removeTokens(1, DEFAULT_AMOUNT + 1);
//         vm.stopPrank();
//     }

//     function testPassportCheck() public {
//         vm.startPrank(configAdmin);
//         // Test adding wallet to allowlist
//         fourteenNumbersClaim.addWalletToAllowlist(user2);
//         // Test removing wallet from allowlist
//         fourteenNumbersClaim.removeWalletFromAllowlist(user2);
//         vm.stopPrank();
//     }

//     function testPause() public {
//         vm.startPrank(configAdmin);
//         fourteenNumbersClaim.pause();
//         assertTrue(fourteenNumbersClaim.paused(), "Contract should be paused");
//         fourteenNumbersClaim.unpause();
//         assertFalse(fourteenNumbersClaim.paused(), "Contract should be unpaused");
//         vm.stopPrank();
//     }

//     function testGetClaimableNfts() public {
//         vm.startPrank(configAdmin);
//         // Add multiple tokens
//         FourteenNumbersClaim.ClaimableToken memory token1 = FourteenNumbersClaim.ClaimableToken({
//             erc1155Contract: address(mockERC1155),
//             tokenId: 1,
//             balance: 100,
//             percentage: 3000
//         });
//         FourteenNumbersClaim.ClaimableToken memory token2 = FourteenNumbersClaim.ClaimableToken({
//             erc1155Contract: address(mockERC1155),
//             tokenId: 2,
//             balance: 200,
//             percentage: 4000
//         });
        
//         fourteenNumbersClaim.addMoreTokens(token1);
//         fourteenNumbersClaim.addMoreTokens(token2);

//         FourteenNumbersClaim.ClaimableToken[] memory tokens = fourteenNumbersClaim.getClaimableNfts();
//         assertEq(tokens.length, 2, "Should return 2 tokens");
//         vm.stopPrank();
//     }
}
