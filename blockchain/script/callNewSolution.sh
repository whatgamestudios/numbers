#!/bin/bash

forge script --rpc-url https://rpc.immutable.com \
    --priority-gas-price 10000000000 \
    --with-gas-price     10000000100 \
    -vvv \
    --private-key $PKEY \
    script/NewSolution.s.sol:NewSolution

