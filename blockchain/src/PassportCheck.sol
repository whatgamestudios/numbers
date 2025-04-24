// Copyright (c) Whatgame Studios 2024 - 2025
// SPDX-License-Identifier: PROPRIETORY
pragma solidity ^0.8.24;

import {AccessControlEnumerableUpgradeable} from "@openzeppelin/contracts-upgradeable/access/extensions/AccessControlEnumerableUpgradeable.sol";

// Interface to retrieve the implementation stored inside the Proxy contract
/// Interface for Passport Wallet's proxy contract.
interface IWalletProxy {
    // Returns the current implementation address used by the proxy contract
    // solhint-disable-next-line func-name-mixedcase
    function PROXY_getImplementation() external view returns (address);
}


/**
 *
 * This contract is designed to be upgradeable.
 */
contract PassportCheck is AccessControlEnumerableUpgradeable {
    /// @notice Emitted when a target smart contract wallet is added or removed from the Allowlist
    event WalletAllowlistChanged(bytes32 indexed targetBytes, address indexed targetAddress, bool added);

    /// @notice Only REGISTRAR_ROLE can invoke white listing registration and removal
    bytes32 public constant REGISTRAR_ROLE = bytes32("REGISTRAR_ROLE");

    /// @notice Mapping of Allowlisted implementation addresses
    mapping(address impl => bool allowed) private addressImplementationAllowlist;

    /// @notice Mapping of Allowlisted bytecodes
    mapping(bytes32 bytecodeHash => bool allowed) private bytecodeAllowlist;

    /**
     */
    function __PassportCheck_init(address _registerarAdmin) internal {
        _grantRole(REGISTRAR_ROLE, _registerarAdmin);
    }

    /**
     * @notice Add a smart contract wallet to the Allowlist.
     * This will allowlist the proxy and implementation contract pair.
     * First, the bytecode of the proxy is added to the bytecode allowlist.
     * Second, the implementation address stored in the proxy is stored in the
     * implementation address allowlist.
     * @param walletAddr the wallet address to be added to the allowlist
     */
    function addWalletToAllowlist(address walletAddr) external onlyRole(REGISTRAR_ROLE) {
        // get bytecode of wallet
        bytes32 codeHash;
        // solhint-disable-next-line no-inline-assembly
        assembly {
            codeHash := extcodehash(walletAddr)
        }
        bytecodeAllowlist[codeHash] = true;
        // get address of wallet module
        address impl = IWalletProxy(walletAddr).PROXY_getImplementation();
        addressImplementationAllowlist[impl] = true;

        emit WalletAllowlistChanged(codeHash, walletAddr, true);
    }

    /**
     * @notice Remove  a smart contract wallet from the Allowlist
     * This will remove the proxy bytecode hash and implementation contract address pair from the allowlist
     * @param walletAddr the wallet address to be removed from the allowlist
     */
    function removeWalletFromAllowlist(address walletAddr) external onlyRole(REGISTRAR_ROLE) {
        // get bytecode of wallet
        bytes32 codeHash;
        // solhint-disable-next-line no-inline-assembly
        assembly {
            codeHash := extcodehash(walletAddr)
        }
        delete bytecodeAllowlist[codeHash];
        // get address of wallet module
        address impl = IWalletProxy(walletAddr).PROXY_getImplementation();
        delete addressImplementationAllowlist[impl];

        emit WalletAllowlistChanged(codeHash, walletAddr, false);
    }



    /**
     * @notice Returns true if an address is Allowlisted, false otherwise
     * @param target the address that will be checked for presence in the allowlist
     */
    function isPassport(address target) public view returns (bool) {
        // Check if caller is a Allowlisted smart contract wallet
        bytes32 codeHash;
        // solhint-disable-next-line no-inline-assembly
        assembly {
            codeHash := extcodehash(target)
        }
        if (bytecodeAllowlist[codeHash]) {
            // If wallet proxy bytecode is approved, check addr of implementation contract
            address impl = IWalletProxy(target).PROXY_getImplementation();

            return addressImplementationAllowlist[impl];
        }

        return false;
    }



    /// @notice storage gap for additional variables for upgrades
    // slither-disable-start unused-state
    // solhint-disable-next-line var-name-mixedcase
    uint256[20] private __PassportCheckGap;
    // slither-disable-end unused-state
}
