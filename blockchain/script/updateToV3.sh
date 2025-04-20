#!/bin/bash
# useMainNet: 1 for mainnet, 0 for testnet
useMainNet=0
# useLedger: 1 for ledger, 0 for private key
useLedger=0
# Set-up variables
source $(dirname "$0")/common.sh


# NOTE WELL ---------------------------------------------
# Add resume option if the script fails part way through:
#     --resume \
# NOTE WELL ---------------------------------------------
if [[ $useLedger -eq 1 ]]
then
    forge script --rpc-url $RPC \
        --priority-gas-price 10000000000 \
        --with-gas-price     10000000100 \
        -vvv \
        --broadcast \
        --verify \
        --verifier blockscout \
        --verifier-url $BLOCKSCOUT_URI$BLOCKSCOUT_APIKEY \
        --sig "update()" \
        --ledger \
        --hd-paths "$LEDGER_HD_PATH" \
        script/staking/UpdateToV3Script.t.sol:UpdateToV3Script 
else
    forge script --rpc-url $RPC \
        --priority-gas-price 10000000000 \
        --with-gas-price     10000000100 \
        -vvv \
        --broadcast \
        --verify \
        --verifier blockscout \
        --verifier-url $BLOCKSCOUT_URI$BLOCKSCOUT_APIKEY \
        --sig "update()" \
        --private-key $PRIVATE_KEY \
        script/staking/UpdateToV3Script.t.sol:UpdateToV3Script  
fi
