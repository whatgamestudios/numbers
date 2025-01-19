// Copyright (c) Whatgame Studios 2024 - 2025
// SPDX-License-Identifier: PROPRIETORY
pragma solidity ^0.8.24;

abstract contract Points {
    function calcPoints(uint256 _target, uint256 _res1, uint256 _res2, uint256 _res3) public pure returns (uint256) {
        uint256 points1 = calcPointsSingle(_target, _res1);
        uint256 points2 = calcPointsSingle(_target, _res2);
        uint256 points3 = calcPointsSingle(_target, _res3);
        return points1 + points2 + points3;
    }

    function calcPointsSingle(uint256 _target, uint256 _res) public pure returns (uint256) {
        if (_target == _res) {
            return 70;
        }
        uint256 diff = 0;
        if (_target > _res) {
            diff = _target - _res;
        }
        else {
            diff = _res - _target;
        }
        if (diff > 50) {
            return 0;
        }
        return 50 - diff;
    }


    /// @notice storage gap for additional variables for upgrades
    // slither-disable-start unused-state
    // solhint-disable-next-line var-name-mixedcase
    uint256[100] private __PointsGap;
    // slither-disable-end unused-state
}
