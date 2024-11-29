using DiamondShop.Application.Services.Interfaces;
using DiamondShop.Commons;
using DiamondShop.Domain.BusinessRules;
using DiamondShop.Domain.Common;
using FluentResults;
using FluentValidation;
using Mapster;
using MapsterMapper;
using MediatR;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Application.Usecases.AdminConfigurations.Frontends.Commands
{
    public record UpdateFrontendDisplayConfigurationCommand(FrontendDisplayConfiguration requestDto) : IRequest<Result<FrontendDisplayConfiguration>>;
    internal class UpdateFrontendDisplayConfigurationCommandHandler : IRequestHandler<UpdateFrontendDisplayConfigurationCommand, Result<FrontendDisplayConfiguration>>
    {
        private readonly IMapper _mapper;
        private readonly IOptionsMonitor<ApplicationSettingGlobal> _optionsMonitor;
        private readonly IApplicationSettingService _applicationSettingService;
        private readonly IValidator<FrontendDisplayConfiguration> _validator;
        private readonly IBlobFileServices _blobFileServices;

        public UpdateFrontendDisplayConfigurationCommandHandler(IMapper mapper, IOptionsMonitor<ApplicationSettingGlobal> optionsMonitor, IApplicationSettingService applicationSettingService, IValidator<FrontendDisplayConfiguration> validator, IBlobFileServices blobFileServices)
        {
            _mapper = mapper;
            _optionsMonitor = optionsMonitor;
            _applicationSettingService = applicationSettingService;
            _validator = validator;
            _blobFileServices = blobFileServices;
        }

        public async Task<Result<FrontendDisplayConfiguration>> Handle(UpdateFrontendDisplayConfigurationCommand request, CancellationToken cancellationToken)
        {
            var displayRequest = request.requestDto;
            var displayRule = _optionsMonitor.CurrentValue.FrontendDisplayConfiguration;
            var clonedDisplayRule = JsonConvert.DeserializeObject<FrontendDisplayConfiguration>(JsonConvert.SerializeObject(displayRule));
            if (clonedDisplayRule is null)
                return Result.Fail("Không thể clone diamond rule cũ được");
            displayRequest.Adapt(clonedDisplayRule);
            //var updateResult = await _frontendDisplayRulesConfigurationService.SetConfiguration(clonedDisplayRule);
            //var displayRule = newValidatedConfiguration;
            var validationResult = _validator.Validate(clonedDisplayRule);
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
                _applicationSettingService.Set(FrontendDisplayConfiguration.Key, clonedDisplayRule);
            }
            return Result.Ok(_optionsMonitor.CurrentValue.FrontendDisplayConfiguration);

            //if (updateResult.IsFailed)
            //{
            //    return Result.Fail(updateResult.Errors);
            //}
            //var getPromotionRule = await _frontendDisplayRulesConfigurationService.GetConfiguration();
            //return Result.Ok(getPromotionRule);
            //throw new NotImplementedException();
        }
    }
}
