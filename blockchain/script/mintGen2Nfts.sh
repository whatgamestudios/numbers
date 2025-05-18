#!/bin/bash
# Use the minting API. This can be accessed here: 
# https://docs.immutable.com/api/zkevm/reference/#/operations/GetMintRequest
# This only mints one NFT at a time. Multiple NFTs could be minted at once using the array of assets.

useMainNet=1

# Initial owner is the 14Numbers token reserve account
NEWOWNER=0xD44D3C3EDC6D1dDBe429E6662Bd79F262DF25132

if [ ${useMainNet} -eq 1 ]
then
    echo Immutable zkEVM Mainnet Configuration
    API=https://api.immutable.com/
    CHAINNAME=imtbl-zkevm-mainnet
    // 14Numbers Scenes
    NFTCONTRACT=0x29c3a209d8423f9a53bf8ad39bbb85087a2a938b
else
    echo Immutable zkEVM Testnet Configuration
    API=https://api.sandbox.immutable.com/
    CHAINNAME=imtbl-zkevm-testnet
    // 14Numbers Scenes Test 
    NFTCONTRACT=0x1a8cBb0c5ADdA8345AdfC0d3B19D661236A4F709
fi

echo Immutable API Key: $IMMUTABLEAPIKEY
echo NFT Contract Address: $NFTCONTRACT
echo Owner of Minted NFTs: $NEWOWNER


if [ -z "${IMMUTABLEAPIKEY}" ]; then
    echo "Error: IMMUTABLEAPIKEY environment variable is not set"
    exit 1
fi
if [ -z "${NFTCONTRACT}" ]; then
    echo "Error: NFTCONTRACT environment variable is not set"
    exit 1
fi
if [ -z "${NEWOWNER}" ]; then
    echo "Error: NEWOWNER environment variable is not set"
    exit 1
fi


generate_post_data()
{
  cat <<EOF
{
  "assets": [
    {
      "reference_id": "200",
      "owner_address": "$NEWOWNER",
      "token_id": "200",
      "amount": "20",
      "metadata": {
        "name": "Clocks",
        "image": "https://whatgamestudios.github.io/14numbers/nfts/200clocks.png",
        "attributes": [
        {
            "trait_type": "Series",
            "value": "Gen2"
        },
        {
            "trait_type": "Rarity",
            "value": "Mythical"
        },
        {
            "trait_type": "Max Supply",
            "value": 20
        },
        {
            "trait_type": "Artist",
            "value": "Usova Olga"
        }
      ]
      }
    },
    {
      "reference_id": "201",
      "owner_address": "$NEWOWNER",
      "token_id": "201",
      "amount": "50",
      "metadata": {
        "name": "Cats",
        "image": "https://whatgamestudios.github.io/14numbers/nfts/201cats.png",
        "attributes": [
            {
                "trait_type": "Series",
                "value": "Gen2"
            },
            {
                "trait_type": "Rarity",
                "value": "Legendary"
            },
            {
                "trait_type": "Max Supply",
                "value": 50
            },
            {
                "trait_type": "Artist",
                "value": "Natalia Zagory"
            }
        ]
        }
    },
    {
      "reference_id": "202",
      "owner_address": "$NEWOWNER",
      "token_id": "202",
      "amount": "100",
      "metadata": {
        "name": "Tea",
        "image": "https://whatgamestudios.github.io/14numbers/nfts/202tea.png",
        "attributes": [
            {
                "trait_type": "Series",
                "value": "Gen2"
            },
            {
                "trait_type": "Rarity",
                "value": "Epic"
            },
            {
                "trait_type": "Max Supply",
                "value": 100
            },
            {
                "trait_type": "Artist",
                "value": "Olka Kostenko"
            }
        ]
        }
    },
    {
      "reference_id": "203",
      "owner_address": "$NEWOWNER",
      "token_id": "203",
      "amount": "100",
      "metadata": {
        "name": "Circuit",
        "image": "https://whatgamestudios.github.io/14numbers/nfts/203circuit.png",
        "tokenId": "203",
        "attributes": [
        {
            "trait_type": "Series",
            "value": "Gen2"
        },
        {
            "trait_type": "Rarity",
            "value": "Epic"
        },
        {
            "trait_type": "Max Supply",
            "value": 100
        },
        {
            "trait_type": "Artist",
            "value": "Amgun"
        }
        ]
        }
    },
    {
      "reference_id": "204",
      "owner_address": "$NEWOWNER",
      "token_id": "204",
      "amount": "400",
      "metadata": {
        "name": "Wild Tea",
        "image": "https://whatgamestudios.github.io/14numbers/nfts/204wildtea.png",
        "tokenId": "204",
        "attributes": [
        {
            "trait_type": "Series",
            "value": "Gen2"
        },
        {
            "trait_type": "Rarity",
            "value": "Rare"
        },
        {
            "trait_type": "Max Supply",
            "value": 400
        },
        {
            "trait_type": "Artist",
            "value": "Olka Kostenko"
        }
        ]
        }
    },
    {
      "reference_id": "205",
      "owner_address": "$NEWOWNER",
      "token_id": "205",
      "amount": "1000",
      "metadata": {
        "name": "Space",
        "image": "https://whatgamestudios.github.io/14numbers/nfts/205space.png",
        "tokenId": "205",
        "attributes": [
        {
            "trait_type": "Series",
            "value": "Gen2"
        },
        {
            "trait_type": "Rarity",
            "value": "Common"
        },
        {
            "trait_type": "Max Supply",
            "value": 1000
        },
        {
            "trait_type": "Artist",
            "value": "Hatcha"
        }
        ]
        }
    },
    {
      "reference_id": "206",
      "owner_address": "$NEWOWNER",
      "token_id": "206",
      "amount": "1000",
      "metadata": {
        "name": "Garden",
        "image": "https://whatgamestudios.github.io/14numbers/nfts/206garden.png",
        "tokenId": "206",
        "attributes": [
        {
            "trait_type": "Series",
            "value": "Gen2"
        },
        {
            "trait_type": "Rarity",
            "value": "Common"
        },
        {
            "trait_type": "Max Supply",
            "value": 1000
        },
        {
            "trait_type": "Artist",
            "value": "Burbura"
        }
        ]
        }
    }    
    
  ]
}
EOF
}

curl --request POST \
  --url ${API}v1/chains/${CHAINNAME}/collections/${NFTCONTRACT}/nfts/mint-requests \
  --header 'Accept: application/json' \
  --header 'Content-Type: application/json' \
  --header "x-immutable-api-key: ${IMMUTABLEAPIKEY}" \
  --data "$(generate_post_data)"

echo Minting API Call Done