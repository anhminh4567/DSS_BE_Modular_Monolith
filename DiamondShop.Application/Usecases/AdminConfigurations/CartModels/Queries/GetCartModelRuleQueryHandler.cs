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

namespace DiamondShop.Application.Usecases.AdminConfigurations.CartModels.Queries
{
    public record GetCartModelRuleQuery() : IRequest<Result<CartModelRules>>;
    internal class GetCartModelRuleQueryHandler : IRequestHandler<GetCartModelRuleQuery, Result<CartModelRules>>
    {
        private readonly IOptionsMonitor<ApplicationSettingGlobal> _optionsMonitor;

        public GetCartModelRuleQueryHandler(IOptionsMonitor<ApplicationSettingGlobal> optionsMonitor)
        {
            _optionsMonitor = optionsMonitor;
        }

        public async Task<Result<CartModelRules>> Handle(GetCartModelRuleQuery request, CancellationToken cancellationToken)
        {
            var get = _optionsMonitor.CurrentValue.CartModelRules;
            return get;
            throw new NotImplementedException();
        }
    }

}
