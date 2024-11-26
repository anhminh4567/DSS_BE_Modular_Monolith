using DiamondShop.Application.Services.Interfaces;
using DiamondShop.Application.Services.Interfaces.AdminConfigurations.DiamondRuleConfig;
using DiamondShop.Application.Services.Interfaces.AdminConfigurations.DiamondRuleConfig.Models;
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
    internal class DiamondRuleConfigurationService : IDiamondRuleConfigurationService
    {
        private readonly IApplicationSettingService _applicationSettingService;
        private readonly IValidator<DiamondRule> _validator;
        private readonly IOptionsMonitor<ApplicationSettingGlobal> _optionsMonitor;

        public DiamondRuleConfigurationService(IApplicationSettingService applicationSettingService, IValidator<DiamondRule> validator, IOptionsMonitor<ApplicationSettingGlobal> optionsMonitor)
        {
            _applicationSettingService = applicationSettingService;
            _validator = validator;
            _optionsMonitor = optionsMonitor;
        }

        public Task<DiamondRule> GetConfiguration()
        {
            return Task.FromResult(_optionsMonitor.CurrentValue.DiamondRule);
        }

        public async Task<Result> SetConfiguration(DiamondRule newValidatedConfiguration)
        {
            //var diamondRule = newValidatedConfiguration;
            //diamondRuleRequestDto.Adapt(diamondRule);
            //var validationResult = _validator.Validate(diamondRule);
            //if (validationResult.IsValid is false)
            //{
            //    Dictionary<string, object> validationErrors = new();
            //    validationResult.Errors
            //        .ForEach(input =>
            //        {
            //            if (validationErrors.ContainsKey(input.PropertyName))
            //            {
            //                var errorList = (List<object>)validationErrors[input.PropertyName];
            //                errorList.Add(input.ErrorMessage);
            //            }
            //            else
            //                validationErrors.Add(input.PropertyName, new List<object> { input.ErrorMessage });
            //        });
            //    ValidationError validationError = new ValidationError($"validation error ", validationErrors);
            //    return MatchError(Result.Fail(validationError).Errors, ModelState);
            //}
            //else
            //{
            //    _applicationSettingService.Set(DiamondRule.key, diamondRule);
            //}
            throw new NotImplementedException();
        }
    }
}
