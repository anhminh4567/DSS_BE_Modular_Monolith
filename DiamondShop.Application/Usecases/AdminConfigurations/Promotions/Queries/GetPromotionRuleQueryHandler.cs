﻿using DiamondShop.Domain.BusinessRules;
using DiamondShop.Domain.Common;
using FluentResults;
using MediatR;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Application.Usecases.AdminConfigurations.Promotions.Queries
{
    public record GetPromotionRuleQuery() : IRequest<Result<PromotionRule>>;
    internal class GetPromotionRuleQueryHandler : IRequestHandler<GetPromotionRuleQuery, Result<PromotionRule>>
    {
        private readonly IOptionsMonitor<ApplicationSettingGlobal> _optionsMonitor;

        public GetPromotionRuleQueryHandler(IOptionsMonitor<ApplicationSettingGlobal> optionsMonitor)
        {
            _optionsMonitor = optionsMonitor;
        }

        public async Task<Result<PromotionRule>> Handle(GetPromotionRuleQuery request, CancellationToken cancellationToken)
        {
            var get = _optionsMonitor.CurrentValue.PromotionRule;
            return Result.Ok(get);
            throw new NotImplementedException();
        }
    }
}
