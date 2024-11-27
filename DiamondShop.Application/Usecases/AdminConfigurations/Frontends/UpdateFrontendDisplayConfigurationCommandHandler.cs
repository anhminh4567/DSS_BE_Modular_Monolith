using DiamondShop.Application.Services.Interfaces.AdminConfigurations.FrontendDisplays;
using DiamondShop.Domain.BusinessRules;
using DiamondShop.Domain.Common;
using FluentResults;
using Mapster;
using MediatR;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Application.Usecases.AdminConfigurations.Frontends
{
    public record UpdateFrontendDisplayConfigurationCommand(FrontendDisplayConfiguration requestDto) : IRequest<Result<FrontendDisplayConfiguration>>;
    internal class UpdateFrontendDisplayConfigurationCommandHandler : IRequestHandler<UpdateFrontendDisplayConfigurationCommand, Result<FrontendDisplayConfiguration>>
    {
        private readonly IFrontendDisplayRulesConfigurationService _frontendDisplayRulesConfigurationService;
        private readonly IOptionsMonitor<ApplicationSettingGlobal> _optionsMonitor;

        public UpdateFrontendDisplayConfigurationCommandHandler(IFrontendDisplayRulesConfigurationService frontendDisplayRulesConfigurationService, IOptionsMonitor<ApplicationSettingGlobal> optionsMonitor)
        {
            _frontendDisplayRulesConfigurationService = frontendDisplayRulesConfigurationService;
            _optionsMonitor = optionsMonitor;
        }

        public async Task<Result<FrontendDisplayConfiguration>> Handle(UpdateFrontendDisplayConfigurationCommand request, CancellationToken cancellationToken)
        {
            var displayRequest = request.requestDto;
            var displayRule  = _optionsMonitor.CurrentValue.FrontendDisplayConfiguration;
            var clonedDisplayRule = JsonConvert.DeserializeObject<FrontendDisplayConfiguration>(JsonConvert.SerializeObject(displayRule));
            if (clonedDisplayRule is null)
                return Result.Fail("Không thể clone diamond rule cũ được");
            displayRequest.Adapt(clonedDisplayRule);
            var updateResult = await _frontendDisplayRulesConfigurationService.SetConfiguration(clonedDisplayRule);
            if (updateResult.IsFailed)
            {
                return Result.Fail(updateResult.Errors);
            }
            var getPromotionRule = await _frontendDisplayRulesConfigurationService.GetConfiguration();
            return Result.Ok(getPromotionRule);
            throw new NotImplementedException();
        }
    }
}
