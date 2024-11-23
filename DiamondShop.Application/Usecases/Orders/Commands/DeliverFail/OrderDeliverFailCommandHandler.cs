using DiamondShop.Application.Services.Interfaces;
using DiamondShop.Application.Usecases.Deliveries.Commands.Create;
using DiamondShop.Domain.Models.AccountAggregate.ValueObjects;
using DiamondShop.Domain.Models.Orders;
using DiamondShop.Domain.Models.Orders.Entities;
using DiamondShop.Domain.Models.Orders.Enum;
using DiamondShop.Domain.Models.Orders.ErrorMessages;
using DiamondShop.Domain.Models.Orders.ValueObjects;
using DiamondShop.Domain.Repositories;
using DiamondShop.Domain.Repositories.JewelryRepo;
using DiamondShop.Domain.Repositories.OrderRepo;
using DiamondShop.Domain.Services.interfaces;
using FluentResults;
using MediatR;
using Newtonsoft.Json;

namespace DiamondShop.Application.Usecases.Orders.Commands.DeliverFail
{
    public record OrderDeliverFailCommand(string OrderId, string DelivererId) : IRequest<Result<Order>>;
    internal class OrderDeliverFailCommandHandler : IRequestHandler<OrderDeliverFailCommand, Result<Order>>
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IOrderService _orderService;
        private readonly IOrderTransactionService _orderTransactionService;
        private readonly IOrderLogRepository _orderLogRepository;

        public OrderDeliverFailCommandHandler(IOrderRepository orderRepository, IUnitOfWork unitOfWork, IOrderService orderService, IOrderTransactionService orderTransactionService, IOrderLogRepository orderLogRepository)
        {
            _orderRepository = orderRepository;
            _unitOfWork = unitOfWork;
            _orderService = orderService;
            _orderTransactionService = orderTransactionService;
            _orderLogRepository = orderLogRepository;
        }

        public async Task<Result<Order>> Handle(OrderDeliverFailCommand request, CancellationToken token)
        {
            request.Deconstruct(out string orderId, out string delivererId);
            await _unitOfWork.BeginTransactionAsync(token);
            var order = await _orderRepository.GetById(OrderId.Parse(orderId));
            if (order == null)
                return Result.Fail(OrderErrors.OrderNotFoundError);
            if (order.DelivererId != AccountId.Parse(delivererId))
                return Result.Fail(OrderErrors.OrderNotFoundError);
            if (order.ShipFailedCount > DeliveryRules.MaxRedelivery)
            {
                var currentStatus = JsonConvert.DeserializeObject<OrderStatus>( JsonConvert.SerializeObject(order.Status));
                order.Status = OrderStatus.Cancelled;
                order.PaymentStatus = PaymentStatus.Refunding;
                order.CancelledDate = DateTime.UtcNow;
                order.CancelledReason = DeliveryRules.MaxRedeliveryError;
                _orderTransactionService.AddRefundUserCancel(order, currentStatus);
                //Return to selling
                await _orderService.CancelItems(order);
                var log = OrderLog.CreateByChangeStatus(order, OrderStatus.Cancelled);
                await _orderLogRepository.Create(log);
            }
            else
            {
                order.ShipFailedCount++;
                order.Status = OrderStatus.Delivery_Failed;
                var log = OrderLog.CreateByChangeStatus(order, OrderStatus.Delivery_Failed);
                await _orderLogRepository.Create(log);
            }
            await _orderRepository.Update(order);
            await _unitOfWork.SaveChangesAsync(token);
            await _unitOfWork.CommitAsync(token);
            return order;
        }
    }
}
