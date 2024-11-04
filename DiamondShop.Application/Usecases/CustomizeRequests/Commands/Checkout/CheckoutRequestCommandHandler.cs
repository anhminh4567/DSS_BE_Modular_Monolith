using DiamondShop.Application.Dtos.Requests.Orders;
using DiamondShop.Application.Services.Models;
using DiamondShop.Application.Usecases.Orders.Commands.Checkout;
using DiamondShop.Domain.Models.CustomizeRequests.Enums;
using DiamondShop.Domain.Models.Warranties.Enum;
using DiamondShop.Domain.Repositories.CustomizeRequestRepo;
using DiamondShop.Domain.Repositories.OrderRepo;
using FluentResults;
using MediatR;

namespace DiamondShop.Application.Usecases.CustomizeRequests.Commands.Checkout
{
    public record CheckoutCustomizeRequestDto(string customizeRequestId, BillingDetail BillingDetail, OrderRequestDto OrderRequestDto, string WarrantyCode, WarrantyType WarrantyType);
    public record CheckoutRequestCommand(string AccountId, CheckoutCustomizeRequestDto CheckoutRequestDto) : IRequest<Result<PaymentLinkResponse>>;
    internal class CheckoutRequestCommandHandler : IRequestHandler<CheckoutRequestCommand, Result<PaymentLinkResponse>>
    {
        private readonly ISender _sender;
        private readonly IOrderRepository _orderRepository;
        private readonly ICustomizeRequestRepository _customizeRequestRepository;

        public CheckoutRequestCommandHandler(ICustomizeRequestRepository customizeRequestRepository, ISender sender, IOrderRepository orderRepository)
        {
            _customizeRequestRepository = customizeRequestRepository;
            _sender = sender;
            _orderRepository = orderRepository;
        }

        public async Task<Result<PaymentLinkResponse>> Handle(CheckoutRequestCommand request, CancellationToken cancellationToken)
        {
            request.Deconstruct(out string accountId, out CheckoutCustomizeRequestDto checkoutRequestDto);
            checkoutRequestDto.Deconstruct(out string customizeRequestId, out BillingDetail billingDetail, out OrderRequestDto orderRequestDto, out string warrantyCode, out WarrantyType warrantyType);
            var customizeRequest = await _customizeRequestRepository.GetById(customizeRequestId);
            if (customizeRequest == null)
                return Result.Fail("This request doesn't exist");
            if (customizeRequest.AccountId.Value != accountId)
                return Result.Fail("Only the owner of the request can checkout");
            if (customizeRequest.Status != CustomizeRequestStatus.Accepted)
                return Result.Fail("This request needs to be accepted before checkout");
            var existedOrderFlag = _orderRepository.IsRequestCreated(customizeRequest.Id);
            if(existedOrderFlag)
                return Result.Fail("This request has already been created");
            List<OrderItemRequestDto> items = new()
            {
                new(customizeRequest.JewelryId.Value,null,null,null,warrantyCode,warrantyType),
            };
            CheckoutOrderInfo info = new(orderRequestDto,items);
            var result = await _sender.Send(new CheckoutOrderCommand(accountId, billingDetail, info));
            if (result.IsFailed)
                return Result.Fail(result.Errors);
            return result.Value;
        }
    }
}
