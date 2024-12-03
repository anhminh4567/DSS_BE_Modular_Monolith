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

namespace DiamondShop.Application.Usecases.AdminConfigurations.BankAccounts.Queries
{
    public class GetShopBankAccountRuleQuery(): IRequest<Result<ShopBankAccountRules>>;
    internal class GetShopBankAccountRuleQueryHandler : IRequestHandler<GetShopBankAccountRuleQuery, Result<ShopBankAccountRules>>
    {
        private readonly IOptionsMonitor<ApplicationSettingGlobal> _optionsMonitor;

        public GetShopBankAccountRuleQueryHandler(IOptionsMonitor<ApplicationSettingGlobal> optionsMonitor)
        {
            _optionsMonitor = optionsMonitor;
        }

        public async Task<Result<ShopBankAccountRules>> Handle(GetShopBankAccountRuleQuery request, CancellationToken cancellationToken)
        {
            return _optionsMonitor.CurrentValue.ShopBankAccountRules;
        }
    }
}
