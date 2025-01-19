// Copyright (c) Whatgame Studios 2024 - 2025
// SPDX-License-Identifier: PROPRIETORY
pragma solidity ^0.8.24;

import {Initializable} from "@openzeppelin/contracts-upgradeable/proxy/utils/Initializable.sol";
import {UUPSUpgradeable} from "@openzeppelin/contracts-upgradeable/proxy/utils/UUPSUpgradeable.sol";
import {AccessControlEnumerableUpgradeable} from "@openzeppelin/contracts-upgradeable/access/extensions/AccessControlEnumerableUpgradeable.sol";

import {GameDayCheck} from "./GameDayCheck.sol";
import {TargetValue} from "./TargetValue.sol";
import {CalcProcessor} from "./CalcProcessor.sol";
import {Points} from "./Points.sol";


/**
 * Manage players submitting solutions for the 14Numbers game.
 *
 * This contract is designed to be upgradeable.
 */
contract FourteenNumbersSolutions is 
    AccessControlEnumerableUpgradeable, GameDayCheck, TargetValue, CalcProcessor, Points, UUPSUpgradeable {

    /// @notice Error: Attempting to upgrade contract storage to version 0.
    error CanNotUpgradeToLowerOrSameVersion(uint256 _storageVersion);

    // One or more numbers is used in more than one equation.
    error NumbersRepeated();

    event Congratulations(address player, bytes solution1, bytes solution2, bytes solution3, uint256 points);
    event NextTime(address player, bytes solution1, bytes solution2, bytes solution3, uint256 points, uint256 bestPoints);


    /// @notice Only UPGRADE_ROLE can upgrade the contract
    bytes32 public constant UPGRADE_ROLE = bytes32("UPGRADE_ROLE");

    /// @notice The first Owner role is returned as the owner of the contract.
    bytes32 public constant OWNER_ROLE = bytes32("OWNER_ROLE");

    /// @notice Version 0 version number
    uint256 private constant _VERSION0 = 0;


    /// @notice version number of the storage variable layout.
    uint256 public version;

    struct BestGame {
        bytes combinedSolution;
        uint256 points;
        address player;
    }

    // Map: game day => best solution
    mapping(uint256 => BestGame) public solutions;

    // Holds a player's stats.
    struct Stats {
        uint32 firstGameDay;
        uint32 mostRecentGameDay;
        uint256 totalPoints;
        uint256 daysPlayed;
    }

    // Map: player address => player's stats.
    mapping(address => Stats) public stats;

    /**
     * @notice Initialises the upgradeable contract, setting up admin accounts.
     * @param _roleAdmin the address to grant `DEFAULT_ADMIN_ROLE` to.
     * @param _owner the address to grant `OWNER_ROLE` to.
     * @param _upgradeAdmin the address to grant `UPGRADE_ROLE` to.
     */
    function initialize(address _roleAdmin, address _owner, address _upgradeAdmin) public initializer {
        __UUPSUpgradeable_init();
        __AccessControl_init();
        _grantRole(DEFAULT_ADMIN_ROLE, _roleAdmin);
        _grantRole(OWNER_ROLE, _owner);
        _grantRole(UPGRADE_ROLE, _upgradeAdmin);
        version = _VERSION0;
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


    /**
     * Store results and update the best solution of the day.
     *
     * @param _gameDay The day since the game epoch start.
     * @param _sol1 First equation.
     * @param _sol2 Second equation.
     * @param _sol3 Third equation.
     * @param _store True if the game player stats should be updated.
     */
    function storeResults(
        uint32 _gameDay, bytes calldata _sol1, bytes calldata _sol2, bytes calldata _sol3, bool _store) external {

        // Reverts if game day is in the future or in the past.
        checkGameDay(_gameDay);

        uint256 points;
        {
            uint256 target = getTargetValue(_gameDay);
            (uint256 res1, uint256 numbersUsed1) = calc(_sol1);
            (uint256 res2, uint256 numbersUsed2) = calc(_sol2);
            (uint256 res3, uint256 numbersUsed3) = calc(_sol3);

            if (numbersUsed1 & numbersUsed2 != 0 || 
                numbersUsed1 & numbersUsed3 != 0 ||
                numbersUsed2 & numbersUsed3 != 0) {
                revert NumbersRepeated();
            }

            points = calcPoints(target, res1, res2, res3);
        }

        // Update the best solution of the day.
        uint256 bestPoints = solutions[_gameDay].points;
        if (points > bestPoints) {
            bytes1 separator = "=";
            bytes memory solution = bytes.concat(_sol1, separator, _sol2, separator, _sol3);
            solutions[_gameDay].combinedSolution = solution;
            solutions[_gameDay].points = points;
            solutions[_gameDay].player = msg.sender;
            emit Congratulations(msg.sender, _sol1, _sol2, _sol3, points);
        }
        else {
            emit NextTime(msg.sender, _sol1, _sol2, _sol3, points, bestPoints);
        }

        // Store statistics if requested.
        if (_store) {
            Stats storage playerStats = stats[msg.sender];
            if (_gameDay > playerStats.mostRecentGameDay) {
                if (playerStats.firstGameDay == 0) {
                    playerStats.firstGameDay = _gameDay;
                }
                playerStats.mostRecentGameDay = _gameDay;
                playerStats.totalPoints += points;
                playerStats.daysPlayed++;
            }
        }
    }

    /**
     * Log the first day the game was played. 
     * Subsequent calls to this function are used to determine the number of people playing the game,
     * who are using Passport login.
     *
     * @param _gameDay The day since the game epoch start.
     */
    function checkIn(uint32 _gameDay) external {
        // Reverts if game day is in the future or in the past.
        checkGameDay(_gameDay);

        Stats storage playerStats = stats[msg.sender];
        if (playerStats.firstGameDay == 0) {
            playerStats.firstGameDay = _gameDay;
        }
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
}
