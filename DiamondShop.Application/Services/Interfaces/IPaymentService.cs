using DiamondShop.Application.Services.Models;
using DiamondShop.Domain.Models.Orders;
using DiamondShop.Domain.Models.Transactions.ValueObjects;
using FluentResults;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace DiamondShop.Application.Services.Interfaces
{
    public interface IPaymentService
    {
        Task<Result<PaymentLinkResponse>> CreatePaymentLink(PaymentLinkRequest paymentLinkRequest, CancellationToken cancellationToken);
        Task<object> Callback();// the object is dependent on the provider, so return object, be anonymouse as possible
        Task<Result> Refund(Order order);
        Task GetTransactionDetail(Transaction payTrasactionType);
        Task GetRefundDetail(Transaction refundTransactionType);
    }
}
