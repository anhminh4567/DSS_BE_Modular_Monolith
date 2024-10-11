using DiamondShop.Application.Dtos.Requests.Orders;
using DiamondShop.Domain.Models.AccountAggregate.ValueObjects;
using DiamondShop.Domain.Models.Orders;
using DiamondShop.Domain.Models.Orders.Enum;
using DiamondShop.Domain.Repositories.OrderRepo;
using FluentResults;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Application.Usecases.Orders.Commands.Checkout
{
    public record OrderCheckoutCommand(OrderRequestDto OrderRequestDto, List<OrderItemRequestDto> OrderItemRequestDtos) : IRequest<Result<Order>>;
    internal class OrderCheckoutCommandHandler: IRequestHandler<OrderCheckoutCommand,Result<Order>>
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IOrderItemRepository _orderItemRepository;

        public OrderCheckoutCommandHandler(IOrderRepository orderRepository, IOrderItemRepository orderItemRepository)
        {
            _orderRepository = orderRepository;
            _orderItemRepository = orderItemRepository;
        }

        public async Task<Result<Order>> Handle(OrderCheckoutCommand request, CancellationToken token)
        {
            request.Deconstruct(out OrderRequestDto orderReq, out List<OrderItemRequestDto> orderItemReqs);

            
            OrderStatus status = orderReq.paymentType == PaymentType.Payall ? OrderStatus.Processing : OrderStatus.Pending; // if transfer
            var order = Order.Create(AccountId.Parse(orderReq.accountId), status, orderReq.paymentType, orderReq.totalPrice);
            await _orderRepository.Create(order, token);

            return order;
        }
    }
}
