using DiamondShop.Application.Services.Interfaces;
using DiamondShop.Application.Services.Models;
using DiamondShop.Domain.Models.AccountAggregate.ErrorMessages;
using DiamondShop.Domain.Models.Orders.ErrorMessages;
using DiamondShop.Domain.Models.Orders.ValueObjects;
using DiamondShop.Domain.Models.Transactions.Entities;
using DiamondShop.Domain.Models.Transactions.ErrorMessages;
using DiamondShop.Domain.Repositories;
using DiamondShop.Domain.Repositories.OrderRepo;
using DiamondShop.Domain.Repositories.TransactionRepo;
using DiamondShop.Domain.Services.interfaces;
using FluentResults;
using MediatR;
using Microsoft.AspNetCore.DataProtection.KeyManagement.Internal;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Application.Usecases.Orders.Queries.GetPaymentLink
{
    public record GetOrderPaymentLink(string OrderId) : IRequest<Result<PaymentLinkResponse>>;
    internal class GetOrderPaymentLinkHandler : IRequestHandler<GetOrderPaymentLink, Result<PaymentLinkResponse>>
    {
        private readonly IPaymentService _paymentService;
        private readonly ILogger<GetOrderPaymentLinkHandler> _logger;
        private readonly IOrderRepository _orderRepository;
        private readonly IAccountRepository _accountRepository;
        private readonly IOrderTransactionService _orderTransactionService;
        private readonly IPaymentMethodRepository _paymentMethodRepository;
        private readonly IExternalBankServices _externalBankServices;

        public GetOrderPaymentLinkHandler(IPaymentService paymentService, ILogger<GetOrderPaymentLinkHandler> logger, IOrderRepository orderRepository, IAccountRepository accountRepository, IOrderTransactionService orderTransactionService, IPaymentMethodRepository paymentMethodRepository, IExternalBankServices externalBankServices)
        {
            _paymentService = paymentService;
            _logger = logger;
            _orderRepository = orderRepository;
            _accountRepository = accountRepository;
            _orderTransactionService = orderTransactionService;
            _paymentMethodRepository = paymentMethodRepository;
            _externalBankServices = externalBankServices;
        }

        public async Task<Result<PaymentLinkResponse>> Handle(GetOrderPaymentLink request, CancellationToken cancellationToken)
        {
            //request.Deconstruct(out string orderId);
            var parsedId = OrderId.Parse(request.OrderId);
            var getOrder = await _orderRepository.GetById(parsedId);
            if(getOrder is null)
            {
                return Result.Fail(OrderErrors.OrderNotFoundError);
            }
            var paymentMethod = await _paymentMethodRepository.GetById(getOrder.PaymentMethodId);
            if(paymentMethod is null)
            {
                return Result.Fail(TransactionErrors.PaymentMethodErrors.NotFoundError);
            }
            var getAccount = await _accountRepository.GetById(getOrder.AccountId);
            if(getAccount is null)
            {
                return Result.Fail(AccountErrors.AccountNotFoundError);
            }
            var correctAmount = _orderTransactionService.GetCorrectAmountFromOrder(getOrder);
            if(paymentMethod.Id == PaymentMethod.ZALOPAY.Id)
            {
                PaymentLinkRequest paymentLinkRequest = new()
                {
                    Account = getAccount,
                    Address = getOrder.ShippingAddress,
                    Amount = correctAmount,//getOrder.TotalPrice,
                    Description = getOrder.Note,
                    Email = getAccount.Email,
                    Order = getOrder,
                    Title = "Thanh toán cho đơn hàng " + getOrder.OrderCode,
                };
                var result = await _paymentService.CreatePaymentLink(paymentLinkRequest, cancellationToken);
                return result;
            }
            else if(paymentMethod.Id == PaymentMethod.BANK_TRANSFER.Id)
            {
                var result = _externalBankServices.GenerateQrCodeFromOrder(getOrder,correctAmount);
                return new PaymentLinkResponse()
                {
                    PaymentUrl = null,
                    QrCode = result.qrImage
                };
            }else
            {
                return Result.Fail(TransactionErrors.PaymentMethodErrors.InCorrectForOnlinePayment);
            }
        }
    }
}
