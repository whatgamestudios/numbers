// Copyright (c) Whatgame Studios 2024 - 2025
// SPDX-License-Identifier: PROPRIETORY
pragma solidity ^0.8.28;

import {Initializable} from "@openzeppelin/contracts-upgradeable/proxy/utils/Initializable.sol";
import {UUPSUpgradeable} from "@openzeppelin/contracts-upgradeable/proxy/utils/UUPSUpgradeable.sol";
import {AccessControlEnumerableUpgradeable} from "@openzeppelin/contracts-upgradeable/access/extensions/AccessControlEnumerableUpgradeable.sol";
import {PausableUpgradeable} from "@openzeppelin/contracts-upgradeable/utils/PausableUpgradeable.sol";
import {IERC1155} from "@openzeppelin/contracts/token/erc1155/IERC1155.sol";
import {IERC1155Receiver} from "@openzeppelin/contracts/token/erc1155/IERC1155Receiver.sol";

import {FourteenNumbersSolutionsV2} from "./FourteenNumbersSolutionsV2.sol";
import {PassportCheck} from "./PassportCheck.sol";
import {FourteenNumbersClaimV2} from "./FourteenNumbersClaimV2.sol";


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
 * This contract is designed to be upgradeable.
 */
contract FourteenNumbersClaimV3 is FourteenNumbersClaimV2 {
    /// @notice Version 2 version number
    uint256 internal constant _VERSION2 = 2;


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
    function upgradeStorage(bytes memory /* _data */) external override virtual {
        if (version != _VERSION1) {
            revert FourteenNumbersClaimV2.IncorrectPreviousVersion(version);
        }
        version = _VERSION2;
    }
}
