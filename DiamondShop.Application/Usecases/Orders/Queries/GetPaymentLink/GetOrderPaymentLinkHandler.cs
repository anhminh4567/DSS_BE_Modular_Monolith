using DiamondShop.Application.Services.Interfaces;
using DiamondShop.Application.Services.Models;
using DiamondShop.Domain.Models.Orders.ValueObjects;
using DiamondShop.Domain.Repositories;
using DiamondShop.Domain.Repositories.OrderRepo;
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
    public record GetOrderPaymentLink(string orderId) : IRequest<Result<PaymentLinkResponse>>;
    internal class GetOrderPaymentLinkHandler : IRequestHandler<GetOrderPaymentLink, Result<PaymentLinkResponse>>
    {
        private readonly IPaymentService _paymentService;
        private readonly ILogger<GetOrderPaymentLinkHandler> _logger;
        private readonly IOrderRepository _orderRepository;
        private readonly IAccountRepository _accountRepository;
        private readonly IOrderTransactionService _orderTransactionService;

        public GetOrderPaymentLinkHandler(IPaymentService paymentService, ILogger<GetOrderPaymentLinkHandler> logger, IOrderRepository orderRepository, IAccountRepository accountRepository, IOrderTransactionService orderTransactionService)
        {
            _paymentService = paymentService;
            _logger = logger;
            _orderRepository = orderRepository;
            _accountRepository = accountRepository;
            _orderTransactionService = orderTransactionService;
        }

        public async Task<Result<PaymentLinkResponse>> Handle(GetOrderPaymentLink request, CancellationToken cancellationToken)
        {
            var parsedId = OrderId.Parse(request.orderId);
            var getOrder = await _orderRepository.GetById(parsedId);
            if(getOrder is null)
            {
                return Result.Fail("Order not found");
            }
            var getAccount = await _accountRepository.GetById(getOrder.AccountId);
            if(getAccount is null)
            {
                return Result.Fail("Account not found");
            }
            var correctAmount = _orderTransactionService.GetCorrectAmountFromOrder(getOrder);
            PaymentLinkRequest paymentLinkRequest = new() 
            {
                Account = getAccount,
                Address = getOrder.ShippingAddress,
                Amount = correctAmount,//getOrder.TotalPrice,
                Description = getOrder.Note,
                Email = getAccount.Email,
                Order = getOrder,
                Title = "Payment for order " + getOrder.Id.Value,
            };
            var result = await _paymentService.CreatePaymentLink(paymentLinkRequest,cancellationToken);
            return result;
        }
    }
}
