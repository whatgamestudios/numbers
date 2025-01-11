// Copyright (c) Whatgame Studios 2024 - 2025
// SPDX-License-Identifier: PROPRIETORY
pragma solidity ^0.8.24;

abstract contract TargetValue {
    error GameDayInvalid(uint32 _requestedGameDay, uint32 _minGameDay, uint32 _maxGameDay);

    // GMT	Sun Dec 01 2024 00:00:00 GMT+0000
    uint256 public constant UNIX_TIME_GAME_START = 1733011200;
    // To convert to days: 24 x 60 x 60
    uint256 public constant SECONDS_PER_DAY = 86400;
    // GMT+14 timezone offset: 14 x 60 x 60
    // Kiribati: Line Islands: https://en.wikipedia.org/wiki/Time_zone
    uint256 public constant PLUS_FORTEEN = 50400;
    // GMT-12 timezone offset: 12 x 60 x 60
    // USA: Baker and Howard Island: https://en.wikipedia.org/wiki/Time_zone
    uint256 public constant MINUS_TWELVE = 43200;

    uint256 public constant MIN_TARGET_VALUE = 250;
    uint256 public constant MAX_TARGET_VALUE = 1000;


    function getTargetValue(uint32 _gameDay) public view returns (uint256) {
        (uint32 minGameDay, uint32 maxGameDay) = determineCurrentGameDays();
        if (_gameDay < minGameDay || _gameDay > maxGameDay) {
            revert GameDayInvalid(_gameDay, minGameDay, maxGameDay);
        }
        return getTarget(_gameDay, MIN_TARGET_VALUE, MAX_TARGET_VALUE);
    }

    function getTarget(uint32 _gameDay, uint256 _low, uint256 _high) private pure returns (uint256) {
        bytes32 seed = generateSeed(_gameDay, 0, 0);
        uint32 count = 0;
        uint256 val;
        do {
            val = getNextValue(seed, count++, _high);
        } while (val < _low);
        return val;
    }

    /**
     * Determine the maximum and minimum game days.
     *
     * @return minGameDay, maxGameDay: Minimum and maximum possible game days, given the current time.
     */
    function determineCurrentGameDays() public view returns (uint32, uint32) {
        // Time now, GMT+0
        uint256 timeNow = block.timestamp;
        uint256 timeNowGmtPlus14 = timeNow + PLUS_FORTEEN;
        uint256 maxDay = (timeNowGmtPlus14 - UNIX_TIME_GAME_START) / SECONDS_PER_DAY;
        uint256 timeNowGmtMinus12 = timeNow - MINUS_TWELVE;
        uint256 minDay = (timeNowGmtMinus12 - UNIX_TIME_GAME_START) / SECONDS_PER_DAY;
        return (uint32(minDay), uint32(maxDay));
    }


    /**
     * Generate a seed value to generate values from, based on the number of days since 
     * the start of the game epoch, the game being played, and the iteration of the game. 
     *
     * @param _gameDay The day since the start of the game epoch.
     * @param _game The game being played.
     * @param _iteration The iteration of the game being played.
     * @return A seed value to be presented to calls to GetNextValue.
     */
    function generateSeed(uint32 _gameDay, uint32 _game, uint32 _iteration) private pure returns(bytes32) {
        return sha256(abi.encodePacked(_gameDay, _game, _iteration));
    }

    /**
     * Calculate the next value in the sequence.
     *
     * @param _seed Value generated by generateSeed
     * @param _count The value from the sequence to return. This should increase from 0 as a game is played.
     * @param _mod One more than the maximum value that should be returned.
     * @return a value between 0 and _mod
     */
    function getNextValue(bytes32 _seed, uint32 _count, uint256 _mod) private pure returns(uint256) {
        bytes32 raw = sha256(abi.encodePacked(_seed, _count));
        uint32 raw2 = uint32(uint256(raw));
        return (uint256(raw2)) % _mod;
    }

    /// @notice storage gap for additional variables for upgrades
    // slither-disable-start unused-state
    // solhint-disable-next-line var-name-mixedcase
    uint256[100] private __CalcProcessorGap;
    // slither-disable-end unused-state
}
