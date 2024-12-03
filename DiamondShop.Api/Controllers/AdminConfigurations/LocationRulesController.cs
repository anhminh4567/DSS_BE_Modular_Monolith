using DiamondShop.Application.Usecases.AdminConfigurations.Diamonds.Commands;
using DiamondShop.Application.Usecases.AdminConfigurations.Diamonds.Queries;
using DiamondShop.Application.Usecases.AdminConfigurations.Diamonds;
using MapsterMapper;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using DiamondShop.Application.Usecases.AdminConfigurations.Locations.Queries;
using DiamondShop.Application.Usecases.AdminConfigurations.Locations.Commands;

namespace DiamondShop.Api.Controllers.AdminConfigurations
{
    [Route("api/Configuration/LocationRules")]
    [Tags("Configuration")]
    [ApiController]
    public class LocationRulesController : ApiControllerBase
    {
        private readonly ISender _sender;
        private readonly IMapper _mapper;

        public LocationRulesController(ISender sender, IMapper mapper)
        {
            _sender = sender;
            _mapper = mapper;
        }
        [HttpGet()]
        public async Task<ActionResult> GetLocationRule()
        {
            var diamondRule = await _sender.Send(new GetLocationRuleQuery());
            return Ok(diamondRule.Value);
        }
        [HttpPost()]
        public async Task<ActionResult> UpdateLocationRule([FromForm] LocationRulesRequestDto requets)
        {
            var updateResul = await _sender.Send(new UpdateLocationRulesCommand(requets));
            if (updateResul.IsFailed)
            {
                return MatchError(updateResul.Errors, ModelState);
            }
            return Ok(updateResul.Value);
        }
    }
}
