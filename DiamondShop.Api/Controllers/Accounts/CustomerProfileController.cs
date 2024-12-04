using DiamondShop.Application.Dtos.Responses.Accounts;
using DiamondShop.Application.Usecases.Accounts.Commands.Update;
using MapsterMapper;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DiamondShop.Api.Controllers.Accounts
{
    [Route("api/Account")]
    [ApiController]
    [Tags("Account")]
    public class CustomerProfileController : ApiControllerBase
    {
        private readonly ISender _sender;
        private readonly IMapper _mapper;

        public CustomerProfileController(ISender sender, IMapper mapper)
        {
            _sender = sender;
            _mapper = mapper;
        }
        [HttpPut("{accountid}/Profile")]
        [Produces(typeof(AccountDto))]
        public async Task<ActionResult> UpdateProfile([FromRoute] string accountid,[FromBody] UpdateUserAccountRequest updateUserAccountCommand)
        {
            var command= new UpdateUserAccountCommand(accountid, updateUserAccountCommand.ChangedFullName, updateUserAccountCommand.ChangedAddress,updateUserAccountCommand.newDefaultAddressId, updateUserAccountCommand.newPhoneNumber);
            var result = await _sender.Send(command);
            if(result.IsSuccess)
            {
                var mappedResult = _mapper.Map<AccountDto>(result.Value);
                return Ok(mappedResult);
            }
            return MatchError(result.Errors, ModelState);
        }
        [HttpPut("{accountId}/Profile/SetAddressDefault")]
        [Produces(typeof(AccountDto))]
        public async Task<ActionResult> SetDefaultProfile([FromRoute] string accountId,[FromBody] string addressId )
        {
            var command = new UpdateUserAccountCommand(accountId,null,null,addressId,null);
            var result = await _sender.Send(command);
            if (result.IsSuccess)
            {
                var mappedResult = _mapper.Map<AccountDto>(result.Value);
                return Ok(mappedResult);
            }
            return MatchError(result.Errors, ModelState);
        }
    }
}
