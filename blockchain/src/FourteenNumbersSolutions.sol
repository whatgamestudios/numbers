// Copyright (c) Whatgame Studios 2024 - 2025
// SPDX-License-Identifier: PROPRIETORY
pragma solidity ^0.8.24;

import {Initializable} from "@openzeppelin/contracts-upgradeable/proxy/utils/Initializable.sol";
import {UUPSUpgradeable} from "@openzeppelin/contracts-upgradeable/proxy/utils/UUPSUpgradeable.sol";
import {AccessControlEnumerableUpgradeable} from "@openzeppelin/contracts-upgradeable/access/extensions/AccessControlEnumerableUpgradeable.sol";

import {TargetValue} from "./TargetValue.sol";
import {CalcProcessor} from "./CalcProcessor.sol";
import {Points} from "./Points.sol";


contract FourteenNumbersSolutions is 
    AccessControlEnumerableUpgradeable, TargetValue, CalcProcessor, Points, UUPSUpgradeable {

    /// @notice Error: Attempting to upgrade contract storage to version 0.
    error CanNotUpgradeToLowerOrSameVersion(uint256 _storageVersion);

    event Congratulations(address player, bytes solution1, bytes solution2, bytes solution3, uint256 points);
    event NextTime(address player, bytes solution1, bytes solution2, bytes solution3, uint256 points, uint256 bestPoints);


    /// @notice Only UPGRADE_ROLE can upgrade the contract
    bytes32 public constant UPGRADE_ROLE = bytes32("UPGRADE_ROLE");

    /// @notice Version 0 version number
    uint256 private constant _VERSION0 = 0;


    /// @notice version number of the storage variable layout.
    uint256 public version;

    struct BestGame {
        bytes solution1;
        bytes solution2;
        bytes solution3;
        uint256 points;
        address player;
    }

    mapping(uint256 => BestGame) solutions;

    /**
     * @notice Initialises the upgradeable contract, setting up admin accounts.
     * @param _roleAdmin the address to grant `DEFAULT_ADMIN_ROLE` to
     * @param _upgradeAdmin the address to grant `UPGRADE_ROLE` to
     */
    function initialize(address _roleAdmin, address _upgradeAdmin) public initializer {
        __UUPSUpgradeable_init();
        __AccessControl_init();
        _grantRole(DEFAULT_ADMIN_ROLE, _roleAdmin);
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


    function newBestGame(uint32 _gameDay, bytes calldata _sol1, bytes calldata _sol2, bytes calldata _sol3) external {
        uint256 points;
        {
            uint256 target = getTargetValue(_gameDay);
            uint256 res1 = calc(_sol1);
            uint256 res2 = calc(_sol2);
            uint256 res3 = calc(_sol3);
            points = calcPoints(target, res1, res2, res3);
        }

        uint256 bestPoints = solutions[_gameDay].points;
        if (points > bestPoints) {
            solutions[_gameDay].solution1 = _sol1;
            solutions[_gameDay].solution2 = _sol2;
            solutions[_gameDay].solution3 = _sol3;
            solutions[_gameDay].points = points;
            solutions[_gameDay].player = msg.sender;
            emit Congratulations(msg.sender, _sol1, _sol2, _sol3, points);
        }
        else {
            emit NextTime(msg.sender, _sol1, _sol2, _sol3, points, bestPoints);
        }
    }




    /**
     * @dev Returns the address of the current owner, for use by systems that need an "owner".
     * This is the first role admin.
     */
    function owner() public view virtual returns (address) {
        return getRoleMember(DEFAULT_ADMIN_ROLE, 0);
    }



    // Override the _authorizeUpgrade function
    // solhint-disable-next-line no-empty-blocks
    function _authorizeUpgrade(address newImplementation) internal override onlyRole(UPGRADE_ROLE) {}
}
