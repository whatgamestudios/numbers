// Copyright Whatgame Studios 2024 - 2025
// SPDX-License-Identifier: PROPRIETORY
// solhint-disable not-rely-on-time

pragma solidity ^0.8.24;

// solhint-disable-next-line no-global-import
import "forge-std/Test.sol";
import {FourteenNumbersSolutions} from "../src/FourteenNumbersSolutions.sol";
import {FourteenNumbersSolutionsV2} from "../src/FourteenNumbersSolutionsV2.sol";
import {FourteenNumbersClaim} from "../src/FourteenNumbersClaim.sol";

import {ERC1967Proxy} from "@openzeppelin/contracts/proxy/ERC1967/ERC1967Proxy.sol";
import {ImmutableERC1155} from "../src/immutable/ImmutableERC1155.sol";
import {OperatorAllowlistUpgradeable} from "../src/immutable/allowlist/OperatorAllowlistUpgradeable.sol";

contract FakePassportMainModule {
    FourteenNumbersClaim claimContract;
    function setClaimContract(address _claimContract) external {
        claimContract = FourteenNumbersClaim(_claimContract);
    }
    function claim() external {
        claimContract.claim();
    }
}


abstract contract ClaimBaseTest is Test {
    uint256 constant TOK1_TOKEN_ID = 1;
    uint256 constant TOK1_AMOUNT = 100;
    uint256 constant TOK1_PERCENTAGE = 4900;
    uint256 constant TOK2_TOKEN_ID = 2;
    uint256 constant TOK2_AMOUNT = 1000;
    uint256 constant TOK2_PERCENTAGE = 100;
    uint256 constant TOK3_TOKEN_ID = 3;
    uint256 constant TOK3_AMOUNT = 10000;
    uint256 constant TOK3_PERCENTAGE = 90;

    uint256 constant DEFAULT_TOKEN_ID = TOK1_TOKEN_ID;
    uint256 constant DEFAULT_AMOUNT = TOK1_AMOUNT;
    uint256 constant DEFAULT_PERCENTAGE = TOK1_PERCENTAGE;



    bytes32 public defaultAdminRole;
    bytes32 public configRole;
    bytes32 public ownerRole;

    address public roleAdmin;
    address public configAdmin;
    address public tokenAdmin;
    address public owner;
    address public operatorRegistrarAdmin;

    address public player1;
    address public player2;

    FourteenNumbersClaim fourteenNumbersClaim;
    FourteenNumbersSolutions fourteenNumbersSolutions;

    OperatorAllowlistUpgradeable allowList;
    ImmutableERC1155 public mockERC1155;
    FakePassportMainModule public passportWallet;



    function setUp() public virtual {
        roleAdmin = makeAddr("RoleAdmin");
        configAdmin = makeAddr("ConfigAdmin");
        tokenAdmin = makeAddr("TokenAdmin");
        owner = makeAddr("Owner");

        player1 = makeAddr("Player1");
        player2 = makeAddr("Player2");

        FourteenNumbersClaim temp = new FourteenNumbersClaim();
        defaultAdminRole = temp.DEFAULT_ADMIN_ROLE();
        configRole = temp.CONFIG_ROLE();
        ownerRole = temp.OWNER_ROLE();

        setUpFourteenNumbersSolutionsV2();
        setUpPassportWallet();
        setUpFourteenNumbersClaim();
        passportWallet.setClaimContract(address(fourteenNumbersClaim));
        setUpOperatorAllowlist();
        setUpMockERC1155();
    }

    function setUpFourteenNumbersSolutionsV2() private {
        address upgradeAdmin = makeAddr("UpgradeAdmin");
        FourteenNumbersSolutions impl = new FourteenNumbersSolutions();
        bytes memory initData = abi.encodeWithSelector(
            FourteenNumbersSolutions.initialize.selector, address(0), address(0), upgradeAdmin);
        ERC1967Proxy proxy = new ERC1967Proxy(address(impl), initData);
        fourteenNumbersSolutions = FourteenNumbersSolutions(address(proxy));

        FourteenNumbersSolutionsV2 v2Impl = new FourteenNumbersSolutionsV2();
        initData = abi.encodeWithSelector(FourteenNumbersSolutionsV2.upgradeStorage.selector, bytes(""));
        vm.prank(upgradeAdmin);
        fourteenNumbersSolutions.upgradeToAndCall(address(v2Impl), initData);

        uint256 ver = fourteenNumbersSolutions.version();
        assertEq(ver, 2, "Upgrade did not upgrade version");
    }

    bytes internal constant creationCode = hex"6054600f3d396034805130553df3fe63906111273d3560e01c14602b57363d3d373d3d3d3d369030545af43d82803e156027573d90f35b3d90fd5b30543d5260203df3";
    function setUpPassportWallet() private {
        bytes32 salt = bytes32(0);
        FakePassportMainModule fakeMainModule = new FakePassportMainModule();
        bytes memory code = abi.encodePacked(creationCode, uint256(uint160(address(fakeMainModule))));
        address deployedContract;
        assembly {
            deployedContract := create2(callvalue(), add(code, 32), mload(code), salt)
        }
        // check deployment success
        require(deployedContract != address(0), 'WalletFactory: deployment failed');
        passportWallet = FakePassportMainModule(deployedContract);
    }

    function setUpFourteenNumbersClaim() private {
        FourteenNumbersClaim impl = new FourteenNumbersClaim();
        bytes memory initData = abi.encodeWithSelector(
            FourteenNumbersClaim.initialize.selector, 
            roleAdmin, owner, configAdmin, tokenAdmin, passportWallet, fourteenNumbersSolutions);
        ERC1967Proxy proxy = new ERC1967Proxy(address(impl), initData);
        fourteenNumbersClaim = FourteenNumbersClaim(address(proxy));
    }

    function setUpOperatorAllowlist() private {
        address admin = address(0);
        address upgradeAdmin = address(0);
        operatorRegistrarAdmin = makeAddr("operatorRegistrarAdmin");
        OperatorAllowlistUpgradeable impl = new OperatorAllowlistUpgradeable();
        bytes memory initData = abi.encodeWithSelector(
            OperatorAllowlistUpgradeable.initialize.selector, admin, upgradeAdmin, operatorRegistrarAdmin
        );
        ERC1967Proxy proxy = new ERC1967Proxy(address(impl), initData);
        allowList = OperatorAllowlistUpgradeable(address(proxy));

        // Add Passport Wallet to the allowlist
        vm.prank(operatorRegistrarAdmin);
        allowList.addWalletToAllowlist(address(passportWallet));

        // Add claim contract to the allowlist
        address[] memory contracts = new address[](1);
        contracts[0] = address(fourteenNumbersClaim);
        vm.prank(operatorRegistrarAdmin);
        allowList.addAddressesToAllowlist(contracts);
    }

    function setUpMockERC1155() private {
        string memory name = "Test Collection";
        string memory baseURI = "https://test.com/nfts/{id}.json";
        string memory contractURI = "https://test.com/collection.json";
        address operatorAllowlist = address(allowList);
        address receiver = address(1);
        uint96 feeNumerator = 10;
        address minter = makeAddr("minter");

        mockERC1155 = new ImmutableERC1155(
            owner,
            name,
            baseURI,
            contractURI,
            operatorAllowlist,
            receiver,
            feeNumerator);
        vm.prank(owner);
        mockERC1155.grantMinterRole(minter);
        vm.startPrank(minter);
        mockERC1155.safeMint(tokenAdmin, TOK1_TOKEN_ID, TOK1_AMOUNT, new bytes(0));
        mockERC1155.safeMint(tokenAdmin, TOK2_TOKEN_ID, TOK2_AMOUNT, new bytes(0));
        mockERC1155.safeMint(tokenAdmin, TOK3_TOKEN_ID, TOK3_AMOUNT, new bytes(0));    
        vm.stopPrank();
    }
}
