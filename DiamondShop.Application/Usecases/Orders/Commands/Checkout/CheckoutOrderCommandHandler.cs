using DiamondShop.Application.Dtos.Requests.Orders;
using DiamondShop.Application.Services.Interfaces;
using DiamondShop.Application.Services.Models;
using DiamondShop.Application.Usecases.Orders.Commands.Create;
using DiamondShop.Domain.Models.Orders.Enum;
using DiamondShop.Domain.Repositories;
using DiamondShop.Domain.Repositories.JewelryModelRepo;
using DiamondShop.Domain.Repositories.JewelryRepo;
using DiamondShop.Domain.Repositories.OrderRepo;
using DiamondShop.Domain.Repositories.TransactionRepo;
using DiamondShop.Domain.Services.interfaces;
using FluentResults;
using MediatR;

namespace DiamondShop.Application.Usecases.Orders.Commands.Checkout
{
    public record CheckoutOrderCommand(string AccountId, BillingDetail BillingDetail, CheckoutOrderInfo CheckoutOrderInfo) : IRequest<Result<PaymentLinkResponse>>;
    internal class CheckoutOrderCommandHandler : IRequestHandler<CheckoutOrderCommand, Result<PaymentLinkResponse>>
    {
        private readonly IAccountRepository _accountRepository;
        private readonly IPaymentMethodRepository _paymentMethodRepository;
        private readonly IOrderRepository _orderRepository;
        private readonly IOrderItemRepository _orderItemRepository;
        private readonly IDiamondRepository _diamondRepository;
        private readonly IJewelryRepository _jewelryRepository;
        private readonly IJewelryModelRepository _jewelryModelRepository;
        private readonly IMainDiamondRepository _mainDiamondRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMainDiamondService _mainDiamondService;
        private readonly IPaymentService _paymentService;
        private readonly ICartService _cartService;
        private readonly ICartModelService _cartModelService;
        private readonly ISender _sender;
        private readonly IOrderService _orderService;
        private readonly IOrderTransactionService _orderTransactionService;

        public CheckoutOrderCommandHandler(IAccountRepository accountRepository, IPaymentMethodRepository paymentMethodRepository, IOrderRepository orderRepository, IOrderItemRepository orderItemRepository, IDiamondRepository diamondRepository, IJewelryRepository jewelryRepository, IJewelryModelRepository jewelryModelRepository, IMainDiamondRepository mainDiamondRepository, IUnitOfWork unitOfWork, IMainDiamondService mainDiamondService, IPaymentService paymentService, ICartService cartService, ICartModelService cartModelService, ISender sender, IOrderService orderService, IOrderTransactionService orderTransactionService)
        {
            _accountRepository = accountRepository;
            _paymentMethodRepository = paymentMethodRepository;
            _orderRepository = orderRepository;
            _orderItemRepository = orderItemRepository;
            _diamondRepository = diamondRepository;
            _jewelryRepository = jewelryRepository;
            _jewelryModelRepository = jewelryModelRepository;
            _mainDiamondRepository = mainDiamondRepository;
            _unitOfWork = unitOfWork;
            _mainDiamondService = mainDiamondService;
            _paymentService = paymentService;
            _cartService = cartService;
            _cartModelService = cartModelService;
            _sender = sender;
            _orderService = orderService;
            _orderTransactionService = orderTransactionService;
        }

        public async Task<Result<PaymentLinkResponse>> Handle(CheckoutOrderCommand request, CancellationToken token)
        {
            request.Deconstruct(out string accountId, out BillingDetail billingDetail, out CheckoutOrderInfo orderInfo);
            billingDetail.Deconstruct(out string FirstName, out string LastName, out string Phone, out string Email, out string Providence, out string District, out string Ward, out string Address, out string ? Note);
            orderInfo.Deconstruct(out OrderRequestDto orderRequestDto, out List<OrderItemRequestDto>  orderItemsRequestDto);
            orderRequestDto.Deconstruct(out PaymentType paymentType, out string paymentId, out string paymentName, out string promotionId, out bool isTransfer);
            string address = String.Join(" ", [billingDetail.Providence, billingDetail.District, billingDetail.Ward, billingDetail.Address]);
            var orderResult = await _sender.Send(new CreateOrderCommand(accountId, new CreateOrderInfo(paymentType, paymentId, paymentName, null, promotionId, address, orderItemsRequestDto)));
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
                    Title = $"Payment for Order#{order.Id}",
                    Description = $"{paymentType.ToString()} payment",
                    Amount = amount,
                };
                var paymentLink = await _paymentService.CreatePaymentLink(paymentLinkRequest, token);
                return paymentLink;
            }
        }
    }
}
