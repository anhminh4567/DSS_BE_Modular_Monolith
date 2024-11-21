using DiamondShop.Application.Dtos.Requests.ApplicationConfigurations.Diamonds;
using DiamondShop.Application.Dtos.Requests.ApplicationConfigurations.Promotions;
using DiamondShop.Application.Services.Interfaces;
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
        private readonly IOptionsMonitor<ApplicationSettingGlobal> _optionsMonitor;
        private readonly IApplicationSettingService _applicationSettingService;
        private readonly IValidator<PromotionRule> _validator;
        public PromotionRuleConfigurationController(ISender sender, IMapper mapper, IOptionsMonitor<ApplicationSettingGlobal> optionsMonitor, IApplicationSettingService applicationSettingService, IValidator<PromotionRule> validator)
        {
            _sender = sender;
            _mapper = mapper;
            _optionsMonitor = optionsMonitor;
            _applicationSettingService = applicationSettingService;
            _validator = validator;
        }
        [HttpGet]
        public async Task<ActionResult> GetPromotionRule()
        {
            var promotionRule = _optionsMonitor.CurrentValue.PromotionRule;
            return Ok(promotionRule);
        }
        [HttpPost]
        public async Task<ActionResult> UpdatePromotionRule([FromBody] PromotionRuleRequestDto promotionRuleRequest) 
        {
            var promotionRule = _optionsMonitor.CurrentValue.PromotionRule;
            promotionRuleRequest.Adapt(promotionRule);
            var validationResult = _validator.Validate(promotionRule);
            if(validationResult.IsValid is false)
            {
                Dictionary<string, object> validationErrors = new();
                validationResult.Errors
                    .ForEach(input =>
                    {
                        if (validationErrors.ContainsKey(input.PropertyName))
                        {
                            var errorList = (List<object>)validationErrors[input.PropertyName];
                            errorList.Add(input.ErrorMessage);
                        }
                        else
                            validationErrors.Add(input.PropertyName, new List<object> { input.ErrorMessage });
                    });
                ValidationError validationError = new ValidationError($"validation error ", validationErrors);
                return MatchError(Result.Fail(validationError).Errors, ModelState);
            }
            else
            {
                _applicationSettingService.Set(PromotionRule.key, promotionRule);
            }
            return Ok(promotionRule);
        }
    }
}
