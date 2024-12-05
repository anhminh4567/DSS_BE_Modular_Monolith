using DiamondShop.Application.Services.Interfaces;
using DiamondShop.Application.Services.Models;
using DiamondShop.Domain.Models.Orders;
using DiamondShop.Domain.Models.Transactions;
using FluentResults;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Infrastructure.Services.Payments.Vnpays
{
    internal class VnpayPaymentService : IPaymentService
    {
        public Task<object> Callback()
        {
            throw new NotImplementedException();
        }

        public Task<Result<PaymentLinkResponse>> CreatePaymentLink(PaymentLinkRequest paymentLinkRequest, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<PaymentRefundDetail> GetRefundDetail(Transaction refundTransactionType)
        {
            throw new NotImplementedException();
        }

        public Task<PaymentDetail> GetTransactionDetail(Transaction payTrasactionType)
        {
            throw new NotImplementedException();
        }

        public Task<Result<PaymentRefundDetail>> Refund(Order order, Transaction forTransaction, decimal fineAmount, string description = null)
        {
            throw new NotImplementedException();
        }

        public Task RemoveAllPaymentCache(Order order)
        {
            throw new NotImplementedException();
        }
    }
}
