using DiamondShop.Application.Dtos.Requests.Orders;
using DiamondShop.Application.Services.Interfaces;
using DiamondShop.Application.Services.Models;
using DiamondShop.Application.Usecases.Accounts.Commands.Update;
using DiamondShop.Application.Usecases.Orders.Commands.Create;
using DiamondShop.Domain.Common;
using DiamondShop.Domain.Models.AccountAggregate.ValueObjects;
using DiamondShop.Domain.Models.Orders.Enum;
using DiamondShop.Domain.Repositories;
using DiamondShop.Domain.Repositories.OrderRepo;
using DiamondShop.Domain.Services.interfaces;
using FluentResults;
using MediatR;
using Microsoft.Extensions.Options;

namespace DiamondShop.Application.Usecases.Orders.Commands.Checkout
{
    public record CheckoutOrderCommand(string AccountId, BillingDetail BillingDetail, CheckoutOrderInfo CheckoutOrderInfo) : IRequest<Result<PaymentLinkResponse>>;
    internal class CheckoutOrderCommandHandler : IRequestHandler<CheckoutOrderCommand, Result<PaymentLinkResponse>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ISender _sender;
        private readonly IPaymentService _paymentService;
        private readonly IOrderTransactionService _orderTransactionService;
        private readonly IOrderRepository _orderRepository;
        private readonly IAccountRepository _accountRepository;
        private readonly IOptionsMonitor<ApplicationSettingGlobal> _optionsMonitor ;

        public CheckoutOrderCommandHandler(IUnitOfWork unitOfWork, ISender sender, IPaymentService paymentService, IOrderTransactionService orderTransactionService, IOrderRepository orderRepository, IAccountRepository accountRepository, IOptionsMonitor<ApplicationSettingGlobal> optionsMonitor)
        {
            _unitOfWork = unitOfWork;
            _sender = sender;
            _paymentService = paymentService;
            _orderTransactionService = orderTransactionService;
            _orderRepository = orderRepository;
            _accountRepository = accountRepository;
            _optionsMonitor = optionsMonitor;
        }

        public async Task<Result<PaymentLinkResponse>> Handle(CheckoutOrderCommand request, CancellationToken token)
        {
            var orderRule = _optionsMonitor.CurrentValue.OrderRule;
            request.Deconstruct(out string accountId, out BillingDetail billingDetail, out CheckoutOrderInfo orderInfo);
            billingDetail.Deconstruct(out string FirstName, out string LastName, out string Phone, out string Email, out string Providence, out string District, out string Ward, out string Address, out string ? Note);
            orderInfo.Deconstruct(out OrderRequestDto orderRequestDto, out List<OrderItemRequestDto>  orderItemsRequestDto);
            orderRequestDto.Deconstruct(out PaymentType paymentType, out string paymentId, out string paymentName, out string promotionId, out bool isTransfer, out bool? IsAtShop);
            var parsedAccountId = AccountId.Parse(accountId);
            var getAccount = await _accountRepository.GetById(parsedAccountId);
            var getCurrentlyProcessOrder = await _orderRepository.GetUserProcessingOrders(getAccount);
            if(getCurrentlyProcessOrder.Count >= orderRule.MaxOrderAmountForCustomerToPlace)
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
            var orderResult = await _sender.Send(new CreateOrderCommand(accountId, new CreateOrderInfo(paymentType, paymentId, paymentName, null, promotionId, billingDetail, orderItemsRequestDto , IsAtShop)));
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
                PaymentLinkRequest paymentLinkRequest = new PaymentLinkRequest()
                {
                    Account = order.Account,
                    Order = order,
                    Email = order.Account.Email,
                    Phone = billingDetail.Phone,
                    Address = order.ShippingAddress,
                    Title = $"Thanh toán cho đơn hàng {order.OrderCode}",
                    Description = $"#{order.OrderCode}",
                    Amount = amount,
                };
                var paymentLink = await _paymentService.CreatePaymentLink(paymentLinkRequest, token);
                return paymentLink;
            }
        }
    }
}
