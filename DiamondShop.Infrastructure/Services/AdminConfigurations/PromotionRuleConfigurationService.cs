using DiamondShop.Application.Services.Interfaces;
using DiamondShop.Application.Services.Interfaces.AdminConfigurations.PromotionRuleConfig;
using DiamondShop.Commons;
using DiamondShop.Domain.BusinessRules;
using DiamondShop.Domain.Common;
using FluentResults;
using FluentValidation;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Infrastructure.Services.AdminConfigurations
{
    internal class PromotionRuleConfigurationService : IPromotionRuleConfigurationService
    {
        private readonly IApplicationSettingService _applicationSettingService;
        private readonly IOptionsMonitor<ApplicationSettingGlobal> _optionsMonitor;
        private readonly IValidator<PromotionRule> _validator;

        public PromotionRuleConfigurationService(IApplicationSettingService applicationSettingService, IOptionsMonitor<ApplicationSettingGlobal> optionsMonitor, IValidator<PromotionRule> validator)
        {
            _applicationSettingService = applicationSettingService;
            _optionsMonitor = optionsMonitor;
            _validator = validator;
        }

        public Task<PromotionRule> GetConfiguration()
        {
            return Task.FromResult(_optionsMonitor.CurrentValue.PromotionRule);
        }

        public async Task<Result> SetConfiguration(PromotionRule newValidatedConfiguration)
        {
            var promotionRule = newValidatedConfiguration;
            var validationResult = _validator.Validate(promotionRule);
            if (validationResult.IsValid is false)
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
                return Result.Fail(validationError);
            }
            else
            {
                _applicationSettingService.Set(PromotionRule.key, promotionRule);
            }
            return Result.Ok();
            throw new NotImplementedException();
        }
    }
}
