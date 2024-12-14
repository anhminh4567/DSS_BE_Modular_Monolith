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

namespace DiamondShop.Application.Usecases.AdminConfigurations.JewelryModels.Queries
{
    public record GetJewelryModelQueryRule() : IRequest<Result<JewelryModelRules>>;
    internal class GetJewelryModelRuleQueryHandler : IRequestHandler<GetJewelryModelQueryRule, Result<JewelryModelRules>>
    {
        private readonly IOptionsMonitor<ApplicationSettingGlobal> _optionsMonitor;

        public GetJewelryModelRuleQueryHandler(IOptionsMonitor<ApplicationSettingGlobal> optionsMonitor)
        {
            _optionsMonitor = optionsMonitor;
        }

        public async Task<Result<JewelryModelRules>> Handle(GetJewelryModelQueryRule request, CancellationToken cancellationToken)
        {
            var get = _optionsMonitor.CurrentValue.JewelryModelRules;
            return get;
            throw new NotImplementedException(); ;
        }
    }
}
