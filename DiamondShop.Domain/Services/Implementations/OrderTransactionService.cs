using DiamondShop.Domain.BusinessRules;
using DiamondShop.Domain.Models.Orders;
using DiamondShop.Domain.Services.interfaces;
using FluentResults;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Domain.Services.Implementations
{
    public class OrderTransactionService : IOrderTransactionService
    {
        private readonly ILogger<OrderTransactionService> _logger;

        public OrderTransactionService(ILogger<OrderTransactionService> logger)
        {
            _logger = logger;
        }

        public Result<(decimal allowAmount, decimal remainingAmount)> GetTransactionValueForOrder(Order order, decimal wantedAmount)
        {
            var orderTotal = order.TotalPrice;
            var remaining = orderTotal - wantedAmount;
            if (wantedAmount > TransactionRules.MaximumPerTransaction)
                return Result.Fail($"the wanted to pay amount is: {wantedAmount} , which is not in our rules is maximum = {TransactionRules.MaximumPerTransaction}");
            if (remaining < TransactionRules.MinimumPerTransaction)
                return Result.Fail($"the remaining money is: {remaining} , which is not in our rules is minimum = {TransactionRules.MinimumPerTransaction}");
            return Result.Ok(  (wantedAmount,remaining) );
        }
    }
}
