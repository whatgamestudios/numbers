// Copyright (c) Whatgame Studios 2024 - 2024
// SPDX-License-Identifier: PROPRIETORY
pragma solidity ^0.8.20;

import "forge-std/Script.sol";
import "@openzeppelin/contracts/proxy/ERC1967/ERC1967Proxy.sol";
import {FourteenNumbersSolutions} from "../src/FourteenNumbersSolutions.sol";
import {FourteenNumbersSolutionsV2} from "../src/FourteenNumbersSolutionsV2.sol";
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
}
