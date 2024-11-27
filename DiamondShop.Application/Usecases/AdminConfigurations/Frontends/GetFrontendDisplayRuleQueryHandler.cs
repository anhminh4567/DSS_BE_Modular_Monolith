using DiamondShop.Application.Services.Interfaces.AdminConfigurations.FrontendDisplays;
using DiamondShop.Domain.BusinessRules;
using DiamondShop.Domain.Common;
using FluentResults;
using MediatR;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Application.Usecases.AdminConfigurations.Frontends
{
    public record GetFrontendDisplayRuleQuery() : IRequest<Result<FrontendDisplayConfiguration>>;
    internal class GetFrontendDisplayRuleQueryHandler : IRequestHandler<GetFrontendDisplayRuleQuery, Result<FrontendDisplayConfiguration>>
    {
        private readonly IFrontendDisplayRulesConfigurationService _frontendDisplayRulesConfigurationService;
        private readonly IOptionsMonitor<ApplicationSettingGlobal> _optionsMonitor;

        public GetFrontendDisplayRuleQueryHandler(IFrontendDisplayRulesConfigurationService frontendDisplayRulesConfigurationService, IOptionsMonitor<ApplicationSettingGlobal> optionsMonitor)
        {
            _frontendDisplayRulesConfigurationService = frontendDisplayRulesConfigurationService;
            _optionsMonitor = optionsMonitor;
        }

        public async Task<Result<FrontendDisplayConfiguration>> Handle(GetFrontendDisplayRuleQuery request, CancellationToken cancellationToken)
        {
            var get = await _frontendDisplayRulesConfigurationService.GetConfiguration();
            return get;
            throw new NotImplementedException();
        }
    }
}
