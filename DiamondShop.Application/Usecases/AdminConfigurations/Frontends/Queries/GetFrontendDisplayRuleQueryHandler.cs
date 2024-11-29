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

namespace DiamondShop.Application.Usecases.AdminConfigurations.Frontends.Queries
{
    public record GetFrontendDisplayRuleQuery() : IRequest<Result<FrontendDisplayConfiguration>>;
    internal class GetFrontendDisplayRuleQueryHandler : IRequestHandler<GetFrontendDisplayRuleQuery, Result<FrontendDisplayConfiguration>>
    {
        private readonly IOptionsMonitor<ApplicationSettingGlobal> _optionsMonitor;

        public GetFrontendDisplayRuleQueryHandler(IOptionsMonitor<ApplicationSettingGlobal> optionsMonitor)
        {
            _optionsMonitor = optionsMonitor;
        }

        public async Task<Result<FrontendDisplayConfiguration>> Handle(GetFrontendDisplayRuleQuery request, CancellationToken cancellationToken)
        {
            var get = _optionsMonitor.CurrentValue.FrontendDisplayConfiguration;
            return get;
            throw new NotImplementedException();
        }
    }
}
