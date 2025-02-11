// Copyright (c) Whatgame Studios 2024 - 2025
// SPDX-License-Identifier: PROPRIETORY
pragma solidity ^0.8.24;

import {Initializable} from "@openzeppelin/contracts-upgradeable/proxy/utils/Initializable.sol";
import {UUPSUpgradeable} from "@openzeppelin/contracts-upgradeable/proxy/utils/UUPSUpgradeable.sol";
import {AccessControlEnumerableUpgradeable} from "@openzeppelin/contracts-upgradeable/access/extensions/AccessControlEnumerableUpgradeable.sol";

// import {GameDayCheck} from "./GameDayCheck.sol";
// import {TargetValue} from "./TargetValue.sol";
// import {CalcProcessor} from "./CalcProcessor.sol";
// import {Points} from "./Points.sol";
import {FourteenNumbersSolutions} from "./FourteenNumbersSolutions.sol";


/**
 * Manage players submitting solutions for the 14Numbers game.
 *
 * This contract is designed to be upgradeable.
 */
contract FourteenNumbersSolutionsV2 is FourteenNumbersSolutions {

    /// @notice Version 2 version number
    uint256 private constant _VERSION2 = 2;

    mapping(uint256 gameDay => uint256 numberOfPlayers) public numberOfPlayers;

    /**
     * @notice Initialises the upgradeable contract, setting up admin accounts.
     * @param _roleAdmin the address to grant `DEFAULT_ADMIN_ROLE` to.
     * @param _owner the address to grant `OWNER_ROLE` to.
     * @param _upgradeAdmin the address to grant `UPGRADE_ROLE` to.
     */
    function initialize(address _roleAdmin, address _owner, address _upgradeAdmin) public virtual override(FourteenNumbersSolutions) initializer {
        __UUPSUpgradeable_init();
        __AccessControl_init();
        _grantRole(DEFAULT_ADMIN_ROLE, _roleAdmin);
        _grantRole(OWNER_ROLE, _owner);
        _grantRole(UPGRADE_ROLE, _upgradeAdmin);
        version = _VERSION2;
    }

    /**
     * @notice Function to be called when upgrading this contract.
     * @dev Call this function as part of upgradeToAndCall().
     * @ param _data ABI encoded data to be used as part of the contract storage upgrade.
     */
    function upgradeStorage(bytes memory /* _data */) external virtual override(FourteenNumbersSolutions) {
        if (version == _VERSION0) {
            version = _VERSION2;
        }
        else {
            revert CanNotUpgradeToLowerOrSameVersion(version);
        }
    }


    /**
     * Store results and update the best solution of the day.
     *
     * @param _gameDay The day since the game epoch start.
     * @param _sol1 First equation.
     * @param _sol2 Second equation.
     * @param _sol3 Third equation.
     */
    function storeResults(
        uint32 _gameDay, bytes calldata _sol1, bytes calldata _sol2, bytes calldata _sol3, bool /* not used */) external virtual override(FourteenNumbersSolutions) {

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
    }

    /**
     * Log the first day the game was played. 
     * Subsequent calls to this function are used to determine the number of people playing the game,
     * who are using Passport login.
     *
     * @param _gameDay The day since the game epoch start.
     */
    function checkIn(uint32 _gameDay) external virtual override(FourteenNumbersSolutions) {
        // Reverts if game day is in the future or in the past.
        checkGameDay(_gameDay);

        Stats storage playerStats = stats[msg.sender];
        if (_gameDay > playerStats.mostRecentGameDay) {
            if (playerStats.firstGameDay == 0) {
                playerStats.firstGameDay = _gameDay;
            }
            playerStats.mostRecentGameDay = _gameDay;
            playerStats.daysPlayed++;
            numberOfPlayers[_gameDay]++;
        }
    }
}
