using DiamondShop.Application.Usecases.AdminConfigurations.Diamonds.Commands;
using DiamondShop.Application.Usecases.AdminConfigurations.Diamonds.Queries;
using DiamondShop.Application.Usecases.AdminConfigurations.Diamonds;
using MapsterMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MediatR;
using DiamondShop.Application.Usecases.AdminConfigurations.DiamondPrices;

namespace DiamondShop.Api.Controllers.AdminConfigurations
{
    [Route("api/Configuration/DiamondPriceRule")]
    [Tags("Configuration")]
    [ApiController]
    public class DiamondPriceRulesConfigurationController : ApiControllerBase
    {
        private readonly ISender _sender;
        private readonly IMapper _mapper;

        public DiamondPriceRulesConfigurationController(ISender sender, IMapper mapper)
        {
            _sender = sender;
            _mapper = mapper;
        }
        [HttpGet()]
        public async Task<ActionResult> GetDiamondPriceRule()
        {
            var diamondRule = await _sender.Send(new GetDiamondPriceRuleQuery());
            return Ok(diamondRule.Value);
        }
        [HttpPost()]
        public async Task<ActionResult> UpdateDiamondPriceRule([FromBody] DiamondPriceRulesRequestDto requestDto)
        {
            var updateResul = await _sender.Send(new UpdateDiamondPriceRulesCommand(requestDto));
            if (updateResul.IsFailed)
            {
                return MatchError(updateResul.Errors, ModelState);
            }
            return Ok(updateResul.Value);
        }
    }
}
