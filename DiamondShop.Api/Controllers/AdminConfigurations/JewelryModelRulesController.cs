using DiamondShop.Application.Usecases.AdminConfigurations.JewelryModels.Commands;
using DiamondShop.Application.Usecases.AdminConfigurations.JewelryModels.Queries;
using DiamondShop.Application.Usecases.AdminConfigurations.Locations.Commands;
using DiamondShop.Application.Usecases.AdminConfigurations.Locations.Queries;
using MapsterMapper;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DiamondShop.Api.Controllers.AdminConfigurations
{
    [Route("api/Configuration/JewelryModelRules")]
    [Tags("Configuration")]
    public class JewelryModelRulesController : ApiControllerBase
    {
        private readonly ISender _sender;
        private readonly IMapper _mapper;

        public JewelryModelRulesController(ISender sender, IMapper mapper)
        {
            _sender = sender;
            _mapper = mapper;
        }
        [HttpGet()]
        public async Task<ActionResult> GetJewelryModelRule()
        {
            var diamondRule = await _sender.Send(new GetJewelryModelQueryRule());
            return Ok(diamondRule.Value);
        }
        [HttpPost()]
        public async Task<ActionResult> UpdateJewelryModelRules([FromForm] UpdateJewelryModelRuleRequestDto requets)
        {
            var updateResul = await _sender.Send(new UpdateJewelryModelRuleCommand(requets));
            if (updateResul.IsFailed)
            {
                return MatchError(updateResul.Errors, ModelState);
            }
            return Ok(updateResul.Value);
        }
    }
}
