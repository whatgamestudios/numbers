using System;
using System.Threading.Tasks;
using System.Numerics;
using System.Collections.Generic;
using Nethereum.Hex.HexTypes;
using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.Web3;
using Nethereum.RPC.Eth.DTOs;
using Nethereum.Contracts.CQS;
using Nethereum.Contracts.ContractHandlers;
using Nethereum.Contracts;
using System.Threading;

namespace FourteenNumbers {
    public partial class FourteenNumbersClaimService: FourteenNumbersClaimServiceBase {
        public FourteenNumbersClaimService(Nethereum.Web3.IWeb3 web3, string contractAddress) : base(web3, contractAddress) {
        }
    }

    public partial class FourteenNumbersClaimServiceBase: ContractWeb3ServiceBase {
        public FourteenNumbersClaimServiceBase(Nethereum.Web3.IWeb3 web3, string contractAddress) : base(web3, contractAddress) {
        }

        public virtual Task<ClaimedDayOutputDTO> ClaimedDayQueryAsync(ClaimedDayFunction claimedDayFunction, BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<ClaimedDayFunction, ClaimedDayOutputDTO>(claimedDayFunction, blockParameter);
        }

        public virtual Task<ClaimedDayOutputDTO> ClaimedDayQueryAsync(string account, BlockParameter blockParameter = null)
        {
            var claimedDayFunction = new ClaimedDayFunction();
            claimedDayFunction.Account = account;
            return ContractHandler.QueryAsync<ClaimedDayFunction, ClaimedDayOutputDTO>(claimedDayFunction, blockParameter);
        }


        public override List<Type> GetAllFunctionTypes()
        {
            return new List<Type>
            {
                typeof(ClaimedDayFunction),
            };
        }

        public override List<Type> GetAllEventTypes()
        {
            return new List<Type>
            {
            };
        }

        public override List<Type> GetAllErrorTypes()
        {
            return new List<Type>
            {
            };
        }        
    }





    public partial class ClaimedDayFunction : ClaimedDayFunctionBase { }

    [Function("claimedDay", typeof(ClaimedDayOutputDTO))]
    public class ClaimedDayFunctionBase : FunctionMessage
    {
        [Parameter("address", "", 1)]
        public virtual string Account { get; set; }
    }

    public partial class ClaimedDayOutputDTO : ClaimedDayOutputDTOBase { }

    [FunctionOutput]
    public class ClaimedDayOutputDTOBase : IFunctionOutputDTO 
    {
        [Parameter("uint256", "", 1)]
        public virtual BigInteger DaysClaimed { get; set; }
    }
} 