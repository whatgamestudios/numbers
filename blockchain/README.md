## 14Numbers

This directory contains code to hold the best solutions for each day.

### Build and test:

```
forge build
forge test -vvv
forge coverage
```

### Deployment:

```
export DEPLOYER_ADDRESS=0x575710d33d35d5274343ecb7A4Bc67D932303Fa2
export APIKEY= <blockscout API key>
sh script/deploy14Numbers.sh  
```


### C# Code Generation:

Following the instructions here: [https://docs.nethereum.com/en/latest/nethereum-codegen-vscodesolidity/#step-2-single-contract](https://docs.nethereum.com/en/latest/nethereum-codegen-vscodesolidity/#step-2-single-contract). That is:

* Build the code using `forge build`.
* Open Visual Studio Code and select `./out/FourteenNumbersSolutions.sol/FouteenNumbersSolutions.json
* In Visual Studio Code, use Command-Shift-P to open the command pallete, and choose `Solidity: Code generate CSharp from contract definition`.
* Files will be written to `./FourteenNumbersSolutions`.


# Lib installation

To install Immutable's contracts:

```
forge install https://github.com/GNSPS/solidity-bytes-utils.git --no-commit
```