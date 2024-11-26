using DiamondShop.Application.Services.Interfaces;
using DiamondShop.Application.Services.Interfaces.AdminConfigurations.DiamondRuleConfig.Models;
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
    [Route("api/Configuration/DiamondRule")]
    [Tags("Configuration")]
    [ApiController]
    public class DiamondRuleConfigurationController : ApiControllerBase
    {
        private readonly ISender _sender;
        private readonly IMapper _mapper;
        private readonly IOptionsMonitor<ApplicationSettingGlobal> _optionsMonitor;
        private readonly IApplicationSettingService _applicationSettingService;
        private readonly IValidator<DiamondRule> _validator;

        public DiamondRuleConfigurationController(ISender sender, IMapper mapper, IOptionsMonitor<ApplicationSettingGlobal> optionsMonitor, IApplicationSettingService applicationSettingService, IValidator<DiamondRule> validator)
        {
            _sender = sender;
            _mapper = mapper;
            _optionsMonitor = optionsMonitor;
            _applicationSettingService = applicationSettingService;
            _validator = validator;
        }

        [HttpGet()]
        public async Task<ActionResult> GetDiamondRule()
        {
            var diamondRule = _optionsMonitor.CurrentValue.DiamondRule;
            return Ok(diamondRule);
        }
        [HttpPost()]
        public async Task<ActionResult> UpdateDiamondRule([FromForm] DiamondRuleRequestDto diamondRuleRequestDto)
        {
            var diamondRule = _optionsMonitor.CurrentValue.DiamondRule;
            diamondRuleRequestDto.Adapt(diamondRule);
            var validationResult = _validator.Validate(diamondRule);
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
                _applicationSettingService.Set(DiamondRule.key,diamondRule);
            }
            return Ok(diamondRule);
        }
    }
}
