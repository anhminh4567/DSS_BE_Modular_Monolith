using DiamondShop.Application.Services.Interfaces;
using DiamondShop.Application.Usecases.AdminConfigurations.Promotions;
using DiamondShop.Application.Usecases.AdminConfigurations.Promotions.Commands;
using DiamondShop.Application.Usecases.AdminConfigurations.Promotions.Queries;
using DiamondShop.Commons;
using DiamondShop.Domain.BusinessRules;
using DiamondShop.Domain.Common;
using FluentResults;
using FluentValidation;
using Mapster;
using MapsterMapper;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace DiamondShop.Api.Controllers.AdminConfigurations
{
    [Route("api/Configuration/PromotionRule")]
    [Tags("Configuration")]
    [ApiController]
    public class PromotionRuleConfigurationController : ApiControllerBase
    {
        private readonly ISender _sender;
        private readonly IMapper _mapper;

        public PromotionRuleConfigurationController(ISender sender, IMapper mapper)
        {
            _sender = sender;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult> GetPromotionRule()
        {
            var promotionRule = await _sender.Send(new GetPromotionRuleQuery());
            return Ok(promotionRule.Value);
        }
        [HttpPost]
        public async Task<ActionResult> UpdatePromotionRule([FromBody] PromotionRuleRequestDto promotionRuleRequest) 
        {
            var updateResult = await _sender.Send(new UpdatePromotionRuleCommand(promotionRuleRequest));
            if (updateResult.IsFailed)
            {
                return MatchError(updateResult.Errors,ModelState);
            }
            return Ok(updateResult.Value);
        }
    }
}
