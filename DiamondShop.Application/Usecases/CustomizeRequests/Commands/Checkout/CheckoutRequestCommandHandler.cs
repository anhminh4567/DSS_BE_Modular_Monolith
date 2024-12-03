using DiamondShop.Application.Dtos.Requests.Orders;
using DiamondShop.Application.Services.Interfaces;
using DiamondShop.Application.Services.Models;
using DiamondShop.Application.Usecases.Orders.Commands.Checkout;
using DiamondShop.Application.Usecases.Orders.Commands.Create;
using DiamondShop.Domain.BusinessRules;
using DiamondShop.Domain.Common;
using DiamondShop.Domain.Models.AccountAggregate.ValueObjects;
using DiamondShop.Domain.Models.CustomizeRequests.Enums;
using DiamondShop.Domain.Models.CustomizeRequests.ErrorMessages;
using DiamondShop.Domain.Models.CustomizeRequests.ValueObjects;
using DiamondShop.Domain.Models.Orders.Enum;
using DiamondShop.Domain.Models.Promotions.ValueObjects;
using DiamondShop.Domain.Models.Warranties.Enum;
using DiamondShop.Domain.Repositories;
using DiamondShop.Domain.Repositories.CustomizeRequestRepo;
using DiamondShop.Domain.Repositories.OrderRepo;
using DiamondShop.Domain.Services.interfaces;
using FluentResults;
using MediatR;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using System.Numerics;

namespace DiamondShop.Application.Usecases.CustomizeRequests.Commands.Checkout
{
    public record CheckoutCustomizeRequestDto(string customizeRequestId, BillingDetail BillingDetail, OrderRequestDto OrderRequestDto, string WarrantyCode, WarrantyType WarrantyType);
    public record CheckoutRequestCommand(string AccountId, CheckoutCustomizeRequestDto CheckoutRequestDto) : IRequest<Result<PaymentLinkResponse>>;
    internal class CheckoutRequestCommandHandler : IRequestHandler<CheckoutRequestCommand, Result<PaymentLinkResponse>>
    {
        private readonly ISender _sender;
        private readonly IOrderRepository _orderRepository;
        private readonly ICustomizeRequestRepository _customizeRequestRepository;
        private readonly IOrderTransactionService _orderTransactionService;
        private readonly IPaymentService _paymentService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAccountRepository _accountRepository;
        private readonly IOptionsMonitor<ApplicationSettingGlobal> _optionsMonitor;

        public CheckoutRequestCommandHandler(ISender sender, IOrderRepository orderRepository, ICustomizeRequestRepository customizeRequestRepository, IOrderTransactionService orderTransactionService, IPaymentService paymentService, IUnitOfWork unitOfWork, IAccountRepository accountRepository, IOptionsMonitor<ApplicationSettingGlobal> optionsMonitor)
        {
            _sender = sender;
            _orderRepository = orderRepository;
            _customizeRequestRepository = customizeRequestRepository;
            _orderTransactionService = orderTransactionService;
            _paymentService = paymentService;
            _unitOfWork = unitOfWork;
            _accountRepository = accountRepository;
            _optionsMonitor = optionsMonitor;
        }

        public async Task<Result<PaymentLinkResponse>> Handle(CheckoutRequestCommand request, CancellationToken token)
        {
            var orderRule = _optionsMonitor.CurrentValue.OrderRule;
            request.Deconstruct(out string accountId, out CheckoutCustomizeRequestDto checkoutRequestDto);
            checkoutRequestDto.Deconstruct(out string customizeRequestId, out BillingDetail billingDetail, out OrderRequestDto orderRequestDto, out string warrantyCode, out WarrantyType warrantyType);
            orderRequestDto.Deconstruct(out PaymentType paymentType, out string paymentId, out string paymentName, out string? promotionId, out bool isTransfer, out bool? isAtShop);
            billingDetail.Deconstruct(out string FirstName, out string LastName, out string Phone, out string Email, out string Providence, out string District, out string Ward, out string Address, out string? Note);
            var customizeRequest = await _customizeRequestRepository.GetById(CustomizeRequestId.Parse(customizeRequestId));
            if (customizeRequest == null)
                return Result.Fail(CustomizeRequestErrors.CustomizeRequestNotFoundError);
            if (customizeRequest.AccountId.Value != accountId)
                return Result.Fail(CustomizeRequestErrors.NoPermissionError);
            if (customizeRequest.Status != CustomizeRequestStatus.Accepted)
                return Result.Fail(CustomizeRequestErrors.UnacceptedCheckoutError);
            var existedOrderFlag = _orderRepository.IsRequestCreated(customizeRequest.Id);
            if (existedOrderFlag)
                return Result.Fail(CustomizeRequestErrors.ExistedOrderError);
            List<OrderItemRequestDto> items = new()
            {
                new(customizeRequest.JewelryId.Value,null,null,null,warrantyCode,warrantyType),
            };
            var parsedAccountId = AccountId.Parse(accountId);
            var getAccount = await _accountRepository.GetById(parsedAccountId);
            var getCurrentlyProcessOrder = await _orderRepository.GetUserProcessingOrders(getAccount);
            if (getCurrentlyProcessOrder.Count >= orderRule.MaxOrderAmountForCustomerToPlace)
            {
                return Result.Fail(new Error($"Số lượng tối đa đơn hàng bạn được đặt hàng là {orderRule.MaxOrderAmountForCustomerToPlace} đơn xử lý"));
            }
            if (String.IsNullOrEmpty(getAccount.PhoneNumber))
            {
                await _unitOfWork.BeginTransactionAsync();
                getAccount.PhoneNumber = Phone;
                await _accountRepository.Update(getAccount);
                await _unitOfWork.SaveChangesAsync();
                await _unitOfWork.CommitAsync();
            }
            var orderResult = await _sender.Send(new CreateOrderCommand(accountId, new CreateOrderInfo(paymentType, paymentId, paymentName, customizeRequestId, promotionId, billingDetail, items, isAtShop)));
            if (orderResult.IsFailed)
                return Result.Fail(orderResult.Errors);
            var order = orderResult.Value;
            if (isTransfer)
            {
                return new PaymentLinkResponse()
                {
                    PaymentUrl = ""
                };
            }
            else
            {
                var amount = paymentType == PaymentType.Payall ? _orderTransactionService.GetFullPaymentValueForOrder(order) : _orderTransactionService.GetCODValueForOrder(order);
                //Create Paymentlink if not transfer
                PaymentLinkRequest paymentLinkRequest = new PaymentLinkRequest()
                {
                    Account = order.Account,
                    Order = order,
                    Email = order.Account.Email,
                    Phone = billingDetail.Phone,
                    Address = order.ShippingAddress,
                    Title = $"Thanh toán cho đơn hàng {order.OrderCode}",
                    Description = $"Loại giao dịch - {paymentType.ToString()}",
                    Amount = amount,
                };
                var paymentLink = await _paymentService.CreatePaymentLink(paymentLinkRequest, token);
                return paymentLink;
            }
        }
    }
}
