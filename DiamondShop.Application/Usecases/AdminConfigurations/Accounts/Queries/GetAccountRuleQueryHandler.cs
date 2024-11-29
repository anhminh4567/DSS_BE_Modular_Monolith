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

namespace DiamondShop.Application.Usecases.AdminConfigurations.Accounts.Queries
{
    public record GetAccountRuleQuery() : IRequest<Result<AccountRules>>;
    internal class GetAccountRuleQueryHandler : IRequestHandler<GetAccountRuleQuery, Result<AccountRules>>
    {
        private readonly IOptionsMonitor<ApplicationSettingGlobal> _optionsMonitor;

        public GetAccountRuleQueryHandler(IOptionsMonitor<ApplicationSettingGlobal> optionsMonitor)
        {
            _optionsMonitor = optionsMonitor;
        }

        public async Task<Result<AccountRules>> Handle(GetAccountRuleQuery request, CancellationToken cancellationToken)
        {
            var ger = _optionsMonitor.CurrentValue.AccountRules;
            return ger;
            throw new NotImplementedException();
        }
    }

}
