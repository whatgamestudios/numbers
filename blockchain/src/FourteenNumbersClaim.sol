// Copyright (c) Whatgame Studios 2024 - 2025
// SPDX-License-Identifier: PROPRIETORY
pragma solidity ^0.8.24;

import {Initializable} from "@openzeppelin/contracts-upgradeable/proxy/utils/Initializable.sol";
import {UUPSUpgradeable} from "@openzeppelin/contracts-upgradeable/proxy/utils/UUPSUpgradeable.sol";
import {AccessControlEnumerableUpgradeable} from "@openzeppelin/contracts-upgradeable/access/extensions/AccessControlEnumerableUpgradeable.sol";
import {IERC1155} from "@openzeppelin/contracts/token/erc1155/IERC1155.sol";

import {FourteenNumbersSolutionsV2} from "./FourteenNumbersSolutionsV2.sol";
import {PassportCheck} from "./PassportCheck.sol";


/**
 * Allow players to submit claim requests.
 *
 * Each ERC1155 NFT added to this contract has a percentage likely to be returned and the
 * number available for claim. NFTs can be given the percentage likelihood of claim of 0%.
 * This indicates that the token is a fallback option if other claims aren't going to be 
 * returned.
 *
 * The algorithm for determining which token id to return is:
 *
 * generate random_value between 0% and 100%
 * cumulative_percentage = 0
 * for (all token ids where the remaining balance to be claimed is > 0, and the token isn't a fallback option) {
 *   if random_value is between cumulative_percentage and cumulative_percentage + token ids percentage {
 *     allocate that token id
 *   }
 * }
 * if (no token has been allocated) {
 *   Allocate token from the first fallback option available.
 * }
 *
 *
 * This contract is designed to be upgradeable.
 */
contract FourteenNumbersClaim is AccessControlEnumerableUpgradeable, PassportCheck, UUPSUpgradeable {

    /// @notice Error: Attempting to upgrade contract storage to version 0.
    error CanNotUpgradeToLowerOrSameVersion(uint256 _storageVersion);

    error AddMoreTokensBalanceMustBeNonZero();
    error AddMoreTokensPercentageTooLarge();
    error NoTokensAvailableForClaim();
    error ClaimTooEarly(uint256 _daysPlayed, uint256 _claimedSoFar);
    error ClaimNonPassportAccount(address _claimer);


    event TokensAdded(uint256 _slot, address _erc1155Contract, uint256 _tokenId, uint256 _amount, uint256 _percentage);
    event TokensRemoved(uint256 _slot, address _erc1155Contract, uint256 _tokenId, uint256 _amount);

    /// @notice Only UPGRADE_ROLE can upgrade the contract
    bytes32 public constant UPGRADE_ROLE = bytes32("UPGRADE_ROLE");

    /// @notice The first Owner role is returned as the owner of the contract.
    bytes32 public constant OWNER_ROLE = bytes32("OWNER_ROLE");

    bytes32 public constant TOKEN_ADMIN_ROLE = bytes32("TOKEN_ADMIN_ROLE");


    /// @notice Version 0 version number
    uint256 internal constant _VERSION0 = 0;

    uint256 public constant DEFAULT_DAYS_PLAYED_TO_CLAIM = 30;
    uint256 public constant ONE_HUNDRED_PERCENT = 10000;

    /// @notice version number of the storage variable layout.
    uint256 public version;

    // Mapping to player to the most recent days played they have claimed.
    mapping(address player => uint256 alreadyClaimed) public claimedDay;

    // Holds a player's stats.
    struct ClaimableToken {
        address erc1155Contract;
        uint256 tokenId;
        uint256 balance;
        uint256 percentage; // Percentage to two decimal places. Hence 23.45% is 2345
    }

    mapping (uint256 => ClaimableToken) public claimableTokens;

    uint256 public constant INVALID = 0;
    uint256 public nextSpareClaimableTokenSlot = 1;
    uint256 public firstInUseClaimableTokenSlot = 1;

    FourteenNumbersSolutionsV2 public fourteenNumbersSolutions;

    uint256 public daysPlayedToClaim = DEFAULT_DAYS_PLAYED_TO_CLAIM;


    /**
     * @notice Initialises the upgradeable contract, setting up admin accounts.
     * @param _roleAdmin the address to grant `DEFAULT_ADMIN_ROLE` to.
     * @param _owner the address to grant `OWNER_ROLE` to.
     * @param _upgradeAdmin the address to grant `UPGRADE_ROLE` to.
     */
    function initialize(address _roleAdmin, address _owner, address _upgradeAdmin, 
        address _registerarAdmin, address _tokenAdmin,
        address _fourteenNumbersSolutions) public virtual initializer {
        __UUPSUpgradeable_init();
        __AccessControl_init();
        __PassportCheck_init(_registerarAdmin);
        _grantRole(DEFAULT_ADMIN_ROLE, _roleAdmin);
        _grantRole(OWNER_ROLE, _owner);
        _grantRole(UPGRADE_ROLE, _upgradeAdmin);
        _grantRole(TOKEN_ADMIN_ROLE, _tokenAdmin);
        version = _VERSION0;
        fourteenNumbersSolutions = FourteenNumbersSolutionsV2(_fourteenNumbersSolutions);
    }

    /**
     * @notice Function to be called when upgrading this contract.
     * @dev Call this function as part of upgradeToAndCall().
     *      This initial version of this function reverts. There is no situation
     *      in which it makes sense to upgrade to the V0 storage layout.
     *      Note that this function is permissionless. Future versions must
     *      compare the code version and the storage version and upgrade
     *      appropriately. As such, the code will revert if an attacker calls
     *      this function attempting a malicious upgrade.
     * @ param _data ABI encoded data to be used as part of the contract storage upgrade.
     */
    function upgradeStorage(bytes memory /* _data */) external virtual {
        revert CanNotUpgradeToLowerOrSameVersion(version);
    }


// TODO only token admin
// remove tokens or transfer some or all of balance



    function addMoreTokens(ClaimableToken calldata _claimableToken) external onlyRole(TOKEN_ADMIN_ROLE) {
        if (_claimableToken.balance == 0) {
            revert AddMoreTokensBalanceMustBeNonZero();
        }
        if (_claimableToken.percentage > ONE_HUNDRED_PERCENT) {
            revert AddMoreTokensPercentageTooLarge();
        }

        IERC1155 erc1155 = IERC1155(_claimableToken.erc1155Contract);
        erc1155.safeTransferFrom(msg.sender, address(this), _claimableToken.tokenId, _claimableToken.balance, new bytes(0));

        uint256 slot = nextSpareClaimableTokenSlot;
        claimableTokens[slot] = _claimableToken;
        nextSpareClaimableTokenSlot = slot + 1;

        emit TokensAdded(slot, address(_claimableToken.erc1155Contract), _claimableToken.tokenId, 
            _claimableToken.balance, _claimableToken.percentage);
    }

    function removeTokens() external onlyRole(TOKEN_ADMIN_ROLE) {

        // TODO

         //event TokensRemoved(uint256 _slot, address _erc1155Contract, uint256 _tokenId, uint256 _amount);
    }



    function claim() external {
        if (!isPassport(msg.sender)) {
            revert ClaimNonPassportAccount(msg.sender);
        }
        // TODO check if passport user.

        _checkAndUpdateClaimedDay();
        uint256 randomValue = _generateRandom();
        (address nftContract, uint256 tokenId) = _determineRandomNft(randomValue);

        IERC1155 erc1155 = IERC1155(nftContract);
        erc1155.safeTransferFrom(address(this), msg.sender, tokenId, 1, new bytes(0));

        // TODO claim event
    }


    function setDaysPlayedToClaim(uint256 _newDaysPlayedToClaim) external {
        daysPlayedToClaim = _newDaysPlayedToClaim;
    }



    function _checkAndUpdateClaimedDay() private {
        // Check if days played is DAYS_PLAYED_TO_CLAIM more than when they previously claimed.
        uint256 daysPlayed;
        (, , , daysPlayed) = fourteenNumbersSolutions.stats(msg.sender);
        uint256 claimedSoFar = claimedDay[msg.sender];
        if (daysPlayed + daysPlayedToClaim > claimedSoFar) {
            // Claim approved!
            claimedDay[msg.sender] = claimedSoFar + daysPlayedToClaim;
        }
        else {
            revert ClaimTooEarly(daysPlayed, claimedSoFar);
        }
    }


    /**
     * Return a random number between 0 and 100000.
     * NOTE that this system isn't fool proof. A player could on-chain check the value returned, 
     * and if it wasn't they wanted, revert the transaction.
     * An improved, two phase system will be created if needed.
     */
    function _generateRandom() private view returns (uint256) {
        bytes32 bhash1 = blockhash(block.number - 1);
        bytes32 bhash2 = blockhash(block.number - 50);
        bytes32 bhash3 = blockhash(block.number - 100);
        bytes32 bhash4 = blockhash(block.number - 200);
        bytes32 bhash5 = blockhash(block.number - 250);
        bytes32 bhash = keccak256(abi.encodePacked(bhash1, bhash2, bhash3, bhash4, bhash5));
        return uint256(bhash) % ONE_HUNDRED_PERCENT;
    }


    function _determineRandomNft(uint256 _randomValue) private returns (address, uint256) {
        uint256 infiniteToken = INVALID;
        uint256 runningTotalPercentage = 0;

        for (uint256 i = firstInUseClaimableTokenSlot; i < nextSpareClaimableTokenSlot; i++) {
            ClaimableToken storage claimableToken = claimableTokens[i];
            uint256 balance = claimableToken.balance;
            if (balance == 0) {
                if (i == firstInUseClaimableTokenSlot) {
                    firstInUseClaimableTokenSlot++;
                }
                continue;
            }
            uint256 percentage = claimableToken.percentage;
            if (percentage == 0) {
                if (infiniteToken == INVALID) {
                    infiniteToken = i;
                }
                continue;
            }
            percentage = runningTotalPercentage + percentage;
            if (percentage < _randomValue) {
                claimableToken.balance = balance - 1;
                return (claimableToken.erc1155Contract, claimableToken.tokenId);
            }
        }

        if (infiniteToken != INVALID) {
            ClaimableToken storage claimableToken = claimableTokens[infiniteToken];
            return (claimableToken.erc1155Contract, claimableToken.tokenId);
        }

        revert NoTokensAvailableForClaim();
    }





    /**
     * @dev Returns the address of the current owner, for use by systems that need an "owner".
     * This is the first role admin.
     */
    function owner() public view virtual returns (address) {
        return getRoleMember(OWNER_ROLE, 0);
    }


    // Override the _authorizeUpgrade function
    // solhint-disable-next-line no-empty-blocks
    function _authorizeUpgrade(address newImplementation) internal override onlyRole(UPGRADE_ROLE) {}


    /// @notice storage gap for additional variables for upgrades
    // slither-disable-start unused-state
    // solhint-disable-next-line var-name-mixedcase
    uint256[20] private __FourteenNumbersClaimGap;
    // slither-disable-end unused-state

}
