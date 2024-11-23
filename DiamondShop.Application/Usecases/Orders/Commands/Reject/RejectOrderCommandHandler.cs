using DiamondShop.Application.Services.Interfaces;
using DiamondShop.Domain.Models.Orders;
using DiamondShop.Domain.Models.Orders.Entities;
using DiamondShop.Domain.Models.Orders.Enum;
using DiamondShop.Domain.Models.Orders.ErrorMessages;
using DiamondShop.Domain.Models.Orders.ValueObjects;
using DiamondShop.Domain.Repositories;
using DiamondShop.Domain.Repositories.JewelryRepo;
using DiamondShop.Domain.Repositories.OrderRepo;
using DiamondShop.Domain.Repositories.TransactionRepo;
using DiamondShop.Domain.Services.interfaces;
using FluentResults;
using MediatR;

namespace DiamondShop.Application.Usecases.Orders.Commands.Reject
{
    public record RejectOrderCommand(string OrderId, string UserId, string Reason) : IRequest<Result<Order>>;
    internal class RejectOrderCommandHandler : IRequestHandler<RejectOrderCommand, Result<Order>>
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IOrderService _orderService;
        private readonly IOrderTransactionService _orderTransactionService;
        private readonly IOrderLogRepository _orderLogRepository;

        public RejectOrderCommandHandler(IOrderRepository orderRepository, IUnitOfWork unitOfWork, IOrderService orderService, IOrderTransactionService orderTransactionService, IOrderLogRepository orderLogRepository)
        {
            _orderRepository = orderRepository;
            _unitOfWork = unitOfWork;
            _orderService = orderService;
            _orderTransactionService = orderTransactionService;
            _orderLogRepository = orderLogRepository;
        }

        public async Task<Result<Order>> Handle(RejectOrderCommand request, CancellationToken token)
        {
            request.Deconstruct(out string orderId, out string accountId, out string reason);
            await _unitOfWork.BeginTransactionAsync(token);
            var order = await _orderRepository.GetById(OrderId.Parse(orderId));
            if (order == null)
                return Result.Fail(OrderErrors.OrderNotFoundError);
            else if (!_orderService.IsCancellable(order.Status))
                return Result.Fail(OrderErrors.UncancellableError);
            _orderTransactionService.AddRefundShopReject(order);
            order.Status = OrderStatus.Rejected;
            order.PaymentStatus = PaymentStatus.Refunding;
            order.CancelledDate = DateTime.UtcNow;
            order.CancelledReason = reason;
            await _orderRepository.Update(order);
            await _orderService.CancelItems(order);
            var log = OrderLog.CreateByChangeStatus(order, OrderStatus.Rejected);
            await _orderLogRepository.Create(log);
            await _unitOfWork.SaveChangesAsync(token);
            await _unitOfWork.CommitAsync(token);
            return order;
        }
    }
}
