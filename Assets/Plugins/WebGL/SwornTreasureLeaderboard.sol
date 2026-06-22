// SPDX-License-Identifier: MIT
pragma solidity ^0.8.0;

contract SwornTreasureLeaderboard {
    struct Entry {
        address player;
        uint256 score;
        uint256 timestamp;
    }

    Entry[] public entries;

    event ScoreSubmitted(address indexed player, uint256 score, uint256 timestamp);

    function submitScore(uint256 _score) external {
        entries.push(Entry(msg.sender, _score, block.timestamp));
        emit ScoreSubmitted(msg.sender, _score, block.timestamp);
    }

    function getEntryCount() external view returns (uint256) {
        return entries.length;
    }

    function getEntries(uint256 from, uint256 count) external view returns (Entry[] memory) {
        uint256 total = entries.length;
        if (from >= total) return new Entry[](0);
        uint256 end = from + count;
        if (end > total) end = total;
        uint256 len = end - from;
        Entry[] memory result = new Entry[](len);
        for (uint256 i = 0; i < len; i++) {
            result[i] = entries[from + i];
        }
        return result;
    }
}
