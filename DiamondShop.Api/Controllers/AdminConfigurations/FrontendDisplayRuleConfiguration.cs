using DiamondShop.Application.Services.Interfaces.AdminConfigurations.PromotionRuleConfig.Models;
using DiamondShop.Application.Usecases.AdminConfigurations.Frontends;
using DiamondShop.Domain.BusinessRules;
using MapsterMapper;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Runtime.CompilerServices;

namespace DiamondShop.Api.Controllers.AdminConfigurations
{
    [Route("api/Configuration/FrontendDisplayRule")]
    [Tags("Configuration")]
    [ApiController]
    public class FrontendDisplayRuleConfiguration : ApiControllerBase
    {
        private readonly ISender _sender;
        private readonly IMapper _mapper;

        public FrontendDisplayRuleConfiguration(ISender sender, IMapper mapper)
        {
            _sender = sender;
            _mapper = mapper;
        }
        [HttpGet]
        public async Task<ActionResult> GetConfiguration()
        {
            var get = await _sender.Send(new GetFrontendDisplayRuleQuery());
            return Ok(get.Value);
        }
        [HttpPost]
        public async Task<ActionResult> UpdateConfiguration([FromBody]FrontendDisplayConfiguration command)
        {
            var updateResul = await _sender.Send(new UpdateFrontendDisplayConfigurationCommand(command));
            if(updateResul.IsFailed)
            {
                return MatchError(updateResul.Errors, ModelState);
            }
            return Ok(updateResul.Value);
        }
    }
}
