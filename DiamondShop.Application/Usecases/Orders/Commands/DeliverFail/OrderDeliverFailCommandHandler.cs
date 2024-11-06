using DiamondShop.Application.Services.Interfaces;
using DiamondShop.Application.Usecases.Deliveries.Commands.Create;
using DiamondShop.Domain.Models.AccountAggregate.ValueObjects;
using DiamondShop.Domain.Models.Orders;
using DiamondShop.Domain.Models.Orders.Enum;
using DiamondShop.Domain.Models.Orders.ValueObjects;
using DiamondShop.Domain.Repositories.JewelryRepo;
using DiamondShop.Domain.Repositories;
using DiamondShop.Domain.Repositories.OrderRepo;
using FluentResults;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DiamondShop.Domain.Services.interfaces;
using DiamondShop.Domain.Services.Implementations;

namespace DiamondShop.Application.Usecases.Orders.Commands.DeliverFail
{
    public record OrderDeliverFailCommand(string OrderId, string DelivererId) : IRequest<Result<Order>>;
    internal class OrderDeliverFailCommandHandler : IRequestHandler<OrderDeliverFailCommand, Result<Order>>
    {
        private readonly IDiamondRepository _diamondRepository;
        private readonly IJewelryRepository _jewelryRepository;
        private readonly IOrderItemRepository _orderItemRepository;
        private readonly IOrderRepository _orderRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IOrderService _orderService;
        private readonly IOrderTransactionService _orderTransactionService;
        public OrderDeliverFailCommandHandler(IOrderRepository orderRepository, IUnitOfWork unitOfWork, IOrderService orderService, IDiamondRepository diamondRepository, IJewelryRepository jewelryRepository, IOrderItemRepository orderItemRepository)
        {
            _orderRepository = orderRepository;
            _unitOfWork = unitOfWork;
            _orderService = orderService;
            _diamondRepository = diamondRepository;
            _jewelryRepository = jewelryRepository;
            _orderItemRepository = orderItemRepository;
        }

        public async Task<Result<Order>> Handle(OrderDeliverFailCommand request, CancellationToken token)
        {
            request.Deconstruct(out string orderId, out string delivererId);
            await _unitOfWork.BeginTransactionAsync(token);
            var order = await _orderRepository.GetById(OrderId.Parse(orderId));
            if (order == null)
                return Result.Fail("This order doesn't exist");
            if (order.DelivererId != AccountId.Parse(delivererId))
                return Result.Fail("Only the deliverer of this order can change its status.");
            if (order.ShipFailedCount > DeliveryRules.MaxRedelivery)
            {
                order.Status = OrderStatus.Cancelled;
                order.PaymentStatus = PaymentStatus.Refunding;
                order.CancelledDate = DateTime.UtcNow;
                order.CancelledReason = "This order has reached maximum shipping attempt. By our policy, it is automatically cancelled.";
                _orderTransactionService.AddRefundUserCancel(order);
                //Return to selling
                await _orderService.CancelItems(order, _orderRepository, _orderItemRepository, _jewelryRepository, _diamondRepository);
            }
            else
            {
                order.ShipFailedCount++;
                order.Status = OrderStatus.Delivery_Failed;
            }
            await _orderRepository.Update(order);
            await _unitOfWork.SaveChangesAsync(token);
            await _unitOfWork.CommitAsync(token);
            return order;
        }
    }
}
