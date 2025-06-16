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
contract FourteenNumbersSolutionsV3 is 
    AccessControlEnumerableUpgradeable, GameDayCheck, TargetValue, CalcProcessor, Points, UUPSUpgradeable {

    /// @notice Error: Attempting to upgrade contract storage to version 0.
    error CanNotUpgradeToLowerOrSameVersion(uint256 _storageVersion);

    // One or more numbers is used in more than one equation.
    error NumbersRepeated();

    // Events used prior to V3.
    event Congratulations(address _player, bytes _solution1, bytes _solution2, bytes _solution3, uint256 _points);
    event NextTime(address _player, bytes _solution1, bytes _solution2, bytes _solution3, uint256 _points, uint256 _bestPoints);

    // V3 events
    event BestScoreToday(uint256 _gameDay, address _player, bytes _solution, uint256 _playersPoints, uint256 _previousPoints);
    event BetterScoreAlready(uint256 _gameDay, address _player, bytes _solution, uint256 _playerPoints, uint256 _bestPoints);
    event CheckIn(uint256 _gameDay, address _player, uint256 _numDaysPlayed);


    /// @notice Only UPGRADE_ROLE can upgrade the contract
    bytes32 public constant UPGRADE_ROLE = bytes32("UPGRADE_ROLE");

    /// @notice The first Owner role is returned as the owner of the contract.
    bytes32 public constant OWNER_ROLE = bytes32("OWNER_ROLE");

    /// @notice Version 0 version number
    uint256 internal constant _VERSION0 = 0;
    /// @notice Version 2 version number
    uint256 private constant _VERSION2 = 2;
    /// @notice Version 3 version number
    uint256 private constant _VERSION3 = 3;


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
        uint32 notUsedFirstGameDay; // Not used since V3 deployed.
        uint32 mostRecentGameDay;
        uint256 notUsedTotalPoints; // Not used since V2 deployed.
        uint256 daysPlayed;
    }

    // Map: player address => player's stats.
    mapping(address => Stats) public stats;

    // Not used since V3 deployed
    mapping(uint256 gameDay => uint256 numberOfPlayers) private noLongerUsedNumberOfPlayers;

    /**
     * @notice Initialises the upgradeable contract, setting up admin accounts.
     * @param _roleAdmin the address to grant `DEFAULT_ADMIN_ROLE` to.
     * @param _owner the address to grant `OWNER_ROLE` to.
     * @param _upgradeAdmin the address to grant `UPGRADE_ROLE` to.
     */
    function initialize(address _roleAdmin, address _owner, address _upgradeAdmin) public virtual initializer {
        __UUPSUpgradeable_init();
        __AccessControl_init();
        _grantRole(DEFAULT_ADMIN_ROLE, _roleAdmin);
        _grantRole(OWNER_ROLE, _owner);
        _grantRole(UPGRADE_ROLE, _upgradeAdmin);
        version = _VERSION3;
    }

    /**
     * @notice Function to be called when upgrading this contract.
     * @dev Call this function as part of upgradeToAndCall().
     * @ param _data ABI encoded data to be used as part of the contract storage upgrade.
     */
    function upgradeStorage(bytes memory /* _data */) external virtual {
        if (version == _VERSION2) {
            version = _VERSION3;
        }
        else {
            revert CanNotUpgradeToLowerOrSameVersion(version);
        }
    }

    /**
     * @notice Store results and update the best solution of the day.
     * @dev Same as V2.
     *
     * @param _gameDay The day since the game epoch start.
     * @param _sol1 First equation.
     * @param _sol2 Second equation.
     * @param _sol3 Third equation.
     */
    function storeResults(
        uint32 _gameDay, bytes calldata _sol1, bytes calldata _sol2, bytes calldata _sol3, bool /* not used */) external virtual {

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
        bytes1 separator = "=";
        bytes memory solution = bytes.concat(_sol1, separator, _sol2, separator, _sol3);
        if (points > bestPoints) {
            solutions[_gameDay].combinedSolution = solution;
            solutions[_gameDay].points = points;
            solutions[_gameDay].player = msg.sender;
            emit BestScoreToday(_gameDay, msg.sender, solution, points, bestPoints);
        }
        else {
            emit BetterScoreAlready(_gameDay, msg.sender, solution, points, bestPoints);
        }
    }


    /**
     * Log the first day the game was played. 
     * Subsequent calls to this function are used to determine the number of people playing the game,
     * who are using Passport login.
     *
     * @param _gameDay The day since the game epoch start.
     */
    function checkIn(uint32 _gameDay) external virtual {
        // Reverts if game day is in the future or in the past.
        checkGameDay(_gameDay);

        Stats storage playerStats = stats[msg.sender];
        if (_gameDay > playerStats.mostRecentGameDay) {
            playerStats.mostRecentGameDay = _gameDay;
            uint256 daysPlayed = playerStats.daysPlayed + 1;
            playerStats.daysPlayed = daysPlayed;
            emit CheckIn(_gameDay, msg.sender, daysPlayed);
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
