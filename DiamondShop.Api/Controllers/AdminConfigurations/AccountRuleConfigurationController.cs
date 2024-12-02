using MapsterMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using DiamondShop.Application.Usecases.AdminConfigurations.Accounts.Queries;
using MediatR;
using DiamondShop.Application.Usecases.AdminConfigurations.Accounts.Commands;
using DiamondShop.Domain.BusinessRules;

namespace DiamondShop.Api.Controllers.AdminConfigurations
{
    [Route("api/Configuration/AccountRule")]
    [Tags("Configuration")]
    [ApiController]
    public class AccountRuleConfigurationController : ApiControllerBase
    {
        private readonly ISender _sender;
        private readonly IMapper _mapper;

        public AccountRuleConfigurationController(ISender sender, IMapper mapper)
        {
            _sender = sender;
            _mapper = mapper;
        }
        [HttpGet()]
        public async Task<ActionResult> GetAccountRule()
        {
            var accountRule = await _sender.Send(new  GetAccountRuleQuery());
            return Ok(accountRule.Value);
        }
        [HttpPost()]
        public async Task<ActionResult> UpdateDiamondRule([FromForm] AccountRuleRequestDto request)
        {
            var updateResul = await _sender.Send(new UpdateAccountRuleCommand(request));
            if (updateResul.IsFailed)
            {
                return MatchError(updateResul.Errors, ModelState);
            }
            return Ok(updateResul.Value);
        }
    }
}
