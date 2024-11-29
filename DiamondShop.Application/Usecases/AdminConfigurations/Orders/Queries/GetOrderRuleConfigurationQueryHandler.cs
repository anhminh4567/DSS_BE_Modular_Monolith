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

namespace DiamondShop.Application.Usecases.AdminConfigurations.Orders.Queries
{
    public record GetOrderRuleConfigurationQuery() : IRequest<Result<OrderRule>>;
    internal class GetOrderRuleConfigurationQueryHandler : IRequestHandler<GetOrderRuleConfigurationQuery, Result<OrderRule>>
    {
        //private readonly IOrderRuleConfigurationServices _orderRuleConfigurationServices;
        private readonly IOptionsMonitor<ApplicationSettingGlobal> _optionsMonitor;
        public GetOrderRuleConfigurationQueryHandler(IOptionsMonitor<ApplicationSettingGlobal> optionsMonitor)
        {
            _optionsMonitor = optionsMonitor;
        }

        public async Task<Result<OrderRule>> Handle(GetOrderRuleConfigurationQuery request, CancellationToken cancellationToken)
        {
            var get = _optionsMonitor.CurrentValue.OrderRule;//_orderRuleConfigurationServices.GetConfiguration().Result;
            return get;
            throw new NotImplementedException();
        }
    }
}
