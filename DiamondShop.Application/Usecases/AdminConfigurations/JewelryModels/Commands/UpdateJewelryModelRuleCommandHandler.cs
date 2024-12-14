using DiamondShop.Application.Services.Interfaces;
using DiamondShop.Commons;
using DiamondShop.Domain.BusinessRules;
using DiamondShop.Domain.Common;
using FluentResults;
using FluentValidation;
using Mapster;
using MediatR;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Application.Usecases.AdminConfigurations.JewelryModels.Commands
{
    public record UpdateJewelryModelRuleRequestDto(int? MaximumSideDiamondOption,int? MaximumMainDiamond); 
    public record UpdateJewelryModelRuleCommand(UpdateJewelryModelRuleRequestDto requestDto) : IRequest<Result<JewelryModelRules>>;
    internal class UpdateJewelryModelRuleCommandHandler : IRequestHandler<UpdateJewelryModelRuleCommand, Result<JewelryModelRules>>
    {
        private readonly IOptionsMonitor<ApplicationSettingGlobal> _optionsMonitor;
        private readonly IApplicationSettingService _applicationSettingService;
        private readonly IValidator<JewelryModelRules> _validator;

        public UpdateJewelryModelRuleCommandHandler(IOptionsMonitor<ApplicationSettingGlobal> optionsMonitor, IApplicationSettingService applicationSettingService, IValidator<JewelryModelRules> validator)
        {
            _optionsMonitor = optionsMonitor;
            _applicationSettingService = applicationSettingService;
            _validator = validator;
        }

        public async Task<Result<JewelryModelRules>> Handle(UpdateJewelryModelRuleCommand request, CancellationToken cancellationToken)
        {
            var jewelryRequest = request.requestDto;
            var jewelryRule = _optionsMonitor.CurrentValue.JewelryModelRules;
            var clonedjewelryRule = JsonConvert.DeserializeObject<JewelryModelRules>(JsonConvert.SerializeObject(jewelryRule));
            if (clonedjewelryRule is null)
                return Result.Fail("Không thể clone diamond rule cũ được");
            jewelryRequest.Adapt(clonedjewelryRule);
            //var updateResult = await _frontendDisplayRulesConfigurationService.SetConfiguration(clonedDisplayRule);
            //var displayRule = newValidatedConfiguration;
            var validationResult = _validator.Validate(clonedjewelryRule);
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
                _applicationSettingService.Set(FrontendDisplayConfiguration.Key, clonedjewelryRule);
            }
            return Result.Ok(_optionsMonitor.CurrentValue.JewelryModelRules);
        }
    }
}
