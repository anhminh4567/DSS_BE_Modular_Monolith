using DiamondShop.Domain.BusinessRules;
using DiamondShop.Domain.Models.AccountAggregate.ValueObjects;
using DiamondShop.Domain.Models.Orders;
using DiamondShop.Domain.Models.Orders.Entities;
using DiamondShop.Domain.Models.Orders.Enum;
using DiamondShop.Domain.Models.Transactions;
using FluentResults;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Domain.Services.interfaces
{
    public interface IOrderTransactionService
    {
        Result<(decimal allowAmount, decimal remainingAmount)> GetTransactionValueForOrder(Order order, decimal wantedAmount, TransactionRule transactionRule);
        decimal GetDepositValueForOrder(Order order);
        decimal GetFullPaymentValueForOrder(Order order);
        decimal GetRemaingValueForOrder(Order order);
        decimal GetCODValueForOrder(Order order);
        decimal GetCorrectAmountFromOrder(Order order);
        decimal GetRefundAmountFromOrder(Order order, decimal fineAmount);

        //refunding 
        decimal GetRefundUserCancelBeforeProcessing(Order order);
        void AddRefundShopReject(Order order, AccountId VerifierId, string TransactionCode, OrderStatus previousStatus);
        void AddRefundUserCancel(Order order, AccountId VerifierId, string TransactionCode, OrderStatus previousStatus);
        void AddCODPayment(Order order);

        decimal GetRefundUserCancelDuringProcessingAndPrepared(Order order);
        /// <summary>
        /// tính phí phạt và không trả phần ship 
        /// </summary>
        /// <param name="order"></param>
        /// <returns></returns>
        decimal GetRefundUserCancelAfterDelivery(Order order);
    }
}
