using DiamondShop.Application.Services.Interfaces.AdminConfigurations.DiamondRuleConfig;
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

namespace DiamondShop.Application.Usecases.AdminConfigurations.Diamonds
{
    public record GetDiamondRuleQuery() : IRequest<Result<DiamondRule>>;
    internal class GetDiamondRuleQueryHandler : IRequestHandler<GetDiamondRuleQuery,Result<DiamondRule>>
    {
        private readonly IDiamondRuleConfigurationService _diamondRuleConfigurationService;
        private readonly IOptionsMonitor<ApplicationSettingGlobal> _optionsMonitor;

        public GetDiamondRuleQueryHandler(IDiamondRuleConfigurationService diamondRuleConfigurationService, IOptionsMonitor<ApplicationSettingGlobal> optionsMonitor)
        {
            _diamondRuleConfigurationService = diamondRuleConfigurationService;
            _optionsMonitor = optionsMonitor;
        }

        public async Task<Result<DiamondRule>> Handle(GetDiamondRuleQuery request, CancellationToken cancellationToken)
        {
            var get = await _diamondRuleConfigurationService.GetConfiguration();
            return Result.Ok(get);
            throw new NotImplementedException();
        }
    }
}
