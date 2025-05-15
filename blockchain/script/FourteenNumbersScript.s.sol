// Copyright (c) Whatgame Studios 2024 - 2024
// SPDX-License-Identifier: PROPRIETORY
pragma solidity ^0.8.20;

import "forge-std/Script.sol";
import "@openzeppelin/contracts/proxy/ERC1967/ERC1967Proxy.sol";
import {FourteenNumbersSolutions} from "../src/FourteenNumbersSolutions.sol";
import {FourteenNumbersSolutionsV2} from "../src/FourteenNumbersSolutionsV2.sol";
import {FourteenNumbersClaim} from "../src/FourteenNumbersClaim.sol";
import {FourteenNumbersClaimV2} from "../src/FourteenNumbersClaimV2.sol";
import {FourteenNumbersClaimV3} from "../src/FourteenNumbersClaimV3.sol";
import {ImmutableERC1155} from "../src/immutable/ImmutableERC1155.sol";

contract FourteenNumbersScript is Script {
    function deployV1() public {
        address deployer = vm.envAddress("DEPLOYER_ADDRESS");
        address roleAdmin = deployer;
        address upgradeAdmin = deployer;
        address owner = deployer;

        vm.broadcast();
        FourteenNumbersSolutions impl = new FourteenNumbersSolutions();
        bytes memory initData = abi.encodeWithSelector(
            FourteenNumbersSolutions.initialize.selector, roleAdmin, owner, upgradeAdmin);

        //impl = FourteenNumbersSolutions(address(0x2b9c63DF973ec282404d9F9fA7688Ee403b7E311));
        vm.broadcast();
        ERC1967Proxy proxy = new ERC1967Proxy(address(impl), initData);

        //FourteenNumbersSolutions fourteenNumbersSolutions = FourteenNumbersSolutions(address(proxy));

        console.logString("Deployment address");
        console.logAddress(address(proxy));
    }

    function deployV2() public {
        vm.broadcast();
        FourteenNumbersSolutionsV2 v2Impl = new FourteenNumbersSolutionsV2();
        console.logString("Deployed v2");
        console.logAddress(address(v2Impl));
    }

    function upgradeToV2() public {
        address proxyDeployedAddress = 0xe2E762770156FfE253C49Da6E008b4bECCCf2812;
        address v2Address = 0x3B6378DDa9037F3a98C685fec3990100ee7Cf4Ff;

        FourteenNumbersSolutions fourteenNumbersSolutions = FourteenNumbersSolutions(proxyDeployedAddress);
        bytes memory initData = abi.encodeWithSelector(FourteenNumbersSolutions.upgradeStorage.selector, bytes(""));

        vm.broadcast();
        fourteenNumbersSolutions.upgradeToAndCall(v2Address, initData);

        console.logString("Done");
    }

    // Transfer all tokens owned by one account to another account.
    function transferAllTokens() public {
        address fourteenNumbersScenes = 0xe2E762770156FfE253C49Da6E008b4bECCCf2812;
        address from = vm.envAddress("OLD_TOKEN_OWNER");
        address to = vm.envAddress("NEW_TOKEN_OWNER");

        ImmutableERC1155 erc1155 = ImmutableERC1155(fourteenNumbersScenes);
        transferAll(erc1155, from, to, 100);
        transferAll(erc1155, from, to, 101);
        transferAll(erc1155, from, to, 102);
        transferAll(erc1155, from, to, 103);
        console.logString("Done");
    }


    function transferAll(ImmutableERC1155 _erc1155, address _from, address _to, uint256 _tokenId) private {
        uint256 bal = _erc1155.balanceOf(_from, _tokenId);
        console.log("Token: %x, balance: %x", _tokenId, bal);
        if (bal > 0) {
            _erc1155.safeTransferFrom(_from, _to, _tokenId, bal, bytes(""));
        }
    }

    function deployClaimV1() public {
        address deployer = vm.envAddress("DEPLOYER_ADDRESS");
        address roleAdmin = deployer;
        address configAdmin = deployer;
        address tokenAdmin = deployer;
        address owner = deployer;

        // A randomly selected passport wallet on mainnet.
        address aPassportWalletMainnet = 0xDa77D416bb4238c9424b8d27A7f90fA2Bdf4911E;
        address fourteenNumbersSolutionsMainnet = 0xe2E762770156FfE253C49Da6E008b4bECCCf2812;

        vm.broadcast();
        FourteenNumbersClaim impl = new FourteenNumbersClaim();
        bytes memory initData = abi.encodeWithSelector(
            FourteenNumbersClaim.initialize.selector, 
            roleAdmin, owner, configAdmin, tokenAdmin, aPassportWalletMainnet, fourteenNumbersSolutionsMainnet);

        vm.broadcast();
        ERC1967Proxy proxy = new ERC1967Proxy(address(impl), initData);

        console.logString("Deployment address");
        console.logAddress(address(proxy));
    }

    function grantTokenRole() public {
        address admin = 0x575710d33d35d5274343ecb7A4Bc67D932303Fa2;
        address tokenReserve = 0xD44D3C3EDC6D1dDBe429E6662Bd79F262DF25132;
        bytes32 tokenRole = 0x544f4b454e5f524f4c4500000000000000000000000000000000000000000000;
        address fourteenNumbersClaimMainnet = 0xb427336d725943BA4300EEC219587E207ad21146;

        FourteenNumbersClaim claimContract = FourteenNumbersClaim(fourteenNumbersClaimMainnet);

        vm.broadcast(admin);
        claimContract.grantRole(tokenRole, tokenReserve);
        console.logString("Done");
    }


    function populateTokens() public {
        address tokenReserve = 0xD44D3C3EDC6D1dDBe429E6662Bd79F262DF25132;
        address fourteenNumbersScenes = 0x29c3A209d8423f9A53Bf8AD39bBb85087a2A938B;
        address fourteenNumbersClaim = 0xb427336d725943BA4300EEC219587E207ad21146;

        ImmutableERC1155 erc1155 = ImmutableERC1155(fourteenNumbersScenes);

        FourteenNumbersClaim.ClaimableToken memory token = FourteenNumbersClaim.ClaimableToken({
            erc1155Contract: fourteenNumbersScenes,
            tokenId: 100,
            balance: 6,
            percentage: 100
        });

        FourteenNumbersClaim claimContract = FourteenNumbersClaim(fourteenNumbersClaim);

        vm.broadcast(tokenReserve);
        erc1155.setApprovalForAll(address(fourteenNumbersClaim), true);

        vm.broadcast(tokenReserve);
        claimContract.addMoreTokens(token);
        console.logString("Done");
    }

    function deployClaimV2() public {
        vm.broadcast();
        FourteenNumbersClaimV2 v2Impl = new FourteenNumbersClaimV2();
        console.logString("Deployed v2");
        console.logAddress(address(v2Impl));

        address proxyDeployedAddress = 0xb427336d725943BA4300EEC219587E207ad21146;
        address v2Address = address(v2Impl);

        FourteenNumbersClaim fourteenNumbersClaim = FourteenNumbersClaim(proxyDeployedAddress);
        bytes memory initData = abi.encodeWithSelector(FourteenNumbersSolutions.upgradeStorage.selector, bytes(""));

        vm.broadcast();
        fourteenNumbersClaim.upgradeToAndCall(v2Address, initData);

        console.logString("Done");
    }

    function deployClaimV3() public {
        vm.broadcast();
        FourteenNumbersClaimV3 v3Impl = new FourteenNumbersClaimV3();
        console.logString("Deployed v3");
        console.logAddress(address(v3Impl));
    }

    function upgradeToClaimV3() public {
        address proxyDeployedAddress = 0xb427336d725943BA4300EEC219587E207ad21146;
        address v3Address = 0x92256FD8cC7F9D385A4A417cF90e0dC141c7e1D4;

        FourteenNumbersClaim fourteenNumbersClaim = FourteenNumbersClaim(proxyDeployedAddress);
        bytes memory initData = abi.encodeWithSelector(FourteenNumbersSolutions.upgradeStorage.selector, bytes(""));

        vm.broadcast();
        fourteenNumbersClaim.upgradeToAndCall(v3Address, initData);

        console.logString("Done");
    }


    function transferFromReserve() public {
        address tokenReserve = 0xD44D3C3EDC6D1dDBe429E6662Bd79F262DF25132;
        address fourteenNumbersScenes = 0x29c3A209d8423f9A53Bf8AD39bBb85087a2A938B;
        address recipient = 0x44599A0afa551F3EB487A9E5717e67dB198f3125;

        ImmutableERC1155 erc1155 = ImmutableERC1155(fourteenNumbersScenes);

        vm.broadcast(tokenReserve);
        erc1155.safeTransferFrom(tokenReserve, recipient, 102, 1, bytes(""));
        console.logString("Done");
    }
}
