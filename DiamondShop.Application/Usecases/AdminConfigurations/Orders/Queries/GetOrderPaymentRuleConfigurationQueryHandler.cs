using DiamondShop.Domain.BusinessRules;
using DiamondShop.Domain.Common;
using FluentResults;
using MediatR;
using Microsoft.Extensions.Options;

namespace DiamondShop.Application.Usecases.AdminConfigurations.Orders.Queries
{
    public record GetOrderPaymentRuleConfigurationQuery() : IRequest<Result<OrderPaymentRules>>;
    internal class GetOrderPaymentRuleConfigurationQueryHandler : IRequestHandler<GetOrderPaymentRuleConfigurationQuery, Result<OrderPaymentRules>>
    {
        //private readonly IOrderRuleConfigurationServices _orderRuleConfigurationServices;
        private readonly IOptionsMonitor<ApplicationSettingGlobal> _optionsMonitor;
        public GetOrderPaymentRuleConfigurationQueryHandler(IOptionsMonitor<ApplicationSettingGlobal> optionsMonitor)
        {
            _optionsMonitor = optionsMonitor;
        }

        public async Task<Result<OrderPaymentRules>> Handle(GetOrderPaymentRuleConfigurationQuery request, CancellationToken cancellationToken)
        {
            var get = _optionsMonitor.CurrentValue.OrderPaymentRules;//_orderRuleConfigurationServices.GetConfiguration().Result;
            return get;
            throw new NotImplementedException();
        }
    }
}
