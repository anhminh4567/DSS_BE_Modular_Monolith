using DiamondShop.Application.Usecases.AdminConfigurations.CartModels.Queries;
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

namespace DiamondShop.Application.Usecases.AdminConfigurations.DiamondPrices
{
    public record GetDiamondPriceRuleQuery() : IRequest<Result<DiamondPriceRules>>;

    internal class GetDiamondPriceRulesQueryHandler : IRequestHandler<GetDiamondPriceRuleQuery, Result<DiamondPriceRules>>
    {

        private readonly IOptionsMonitor<ApplicationSettingGlobal> _optionsMonitor;

        public GetDiamondPriceRulesQueryHandler(IOptionsMonitor<ApplicationSettingGlobal> optionsMonitor)
        {
            _optionsMonitor = optionsMonitor;
        }

        public async Task<Result<DiamondPriceRules>> Handle(GetDiamondPriceRuleQuery request, CancellationToken cancellationToken)
        {
            var get = _optionsMonitor.CurrentValue.DiamondPriceRules;
            return get;
            throw new NotImplementedException();
        }
    }
}

