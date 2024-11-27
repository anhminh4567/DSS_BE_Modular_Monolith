using DiamondShop.Application.Services.Interfaces.AdminConfigurations.DiamondRuleConfig;
using DiamondShop.Application.Services.Interfaces.AdminConfigurations.PromotionRuleConfig;
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

namespace DiamondShop.Application.Usecases.AdminConfigurations.Promotions
{
    public record GetPromotionRuleQuery() : IRequest<Result<PromotionRule>>;
    internal class GetPromotionRuleQueryHandler : IRequestHandler<GetPromotionRuleQuery, Result<PromotionRule>>
    {
        private readonly IPromotionRuleConfigurationService _promotionRuleConfigurationService;
        private readonly IOptionsMonitor<ApplicationSettingGlobal> _optionsMonitor;

        public GetPromotionRuleQueryHandler(IPromotionRuleConfigurationService promotionRuleConfigurationService, IOptionsMonitor<ApplicationSettingGlobal> optionsMonitor)
        {
            _promotionRuleConfigurationService = promotionRuleConfigurationService;
            _optionsMonitor = optionsMonitor;
        }

        public async Task<Result<PromotionRule>> Handle(GetPromotionRuleQuery request, CancellationToken cancellationToken)
        {
            var get = await _promotionRuleConfigurationService.GetConfiguration();
            return Result.Ok(get);
            throw new NotImplementedException();
        }
    }
}
