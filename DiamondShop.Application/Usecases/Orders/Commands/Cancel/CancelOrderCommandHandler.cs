using DiamondShop.Application.Services.Interfaces;
using DiamondShop.Domain.Models.AccountAggregate.ValueObjects;
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

namespace DiamondShop.Api.Controllers.Orders.Cancel
{
    public record CancelOrderCommand(string OrderId, string AccountId, string Reason) : IRequest<Result<Order>>;
    public record CancelOrderCommandHandler : IRequestHandler<CancelOrderCommand, Result<Order>>
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IOrderService _orderService;
        private readonly IOrderTransactionService _orderTransactionService;
        private readonly IOrderLogRepository _orderLogRepository;

        public CancelOrderCommandHandler(IOrderRepository orderRepository, IUnitOfWork unitOfWork, IOrderService orderService, IOrderTransactionService orderTransactionService, IOrderLogRepository orderLogRepository)
        {
            _orderRepository = orderRepository;
            _unitOfWork = unitOfWork;
            _orderService = orderService;
            _orderTransactionService = orderTransactionService;
            _orderLogRepository = orderLogRepository;
        }

        public async Task<Result<Order>> Handle(CancelOrderCommand request, CancellationToken token)
        {
            request.Deconstruct(out string orderId, out string accountId, out string reason);
            await _unitOfWork.BeginTransactionAsync(token);
            var order = await _orderRepository.GetById(OrderId.Parse(orderId));
            if (order == null)
                return Result.Fail(OrderErrors.OrderNotFoundError);
            else if (!_orderService.IsCancellable(order.Status))
                return Result.Fail(OrderErrors.UncancellableError);
            if (order.AccountId != AccountId.Parse(accountId))
                return Result.Fail(OrderErrors.NoPermissionToCancelError);
            _orderTransactionService.AddRefundUserCancel(order);
            order.Status = OrderStatus.Cancelled;
            order.PaymentStatus = PaymentStatus.Refunding;
            order.CancelledDate = DateTime.UtcNow;
            order.CancelledReason = reason;
            await _orderRepository.Update(order);
            //Return to selling
            var res = await _orderService.CancelItems(order);
            if (res.IsFailed)
                return Result.Fail(res.Errors);
            var log = OrderLog.CreateByChangeStatus(order, order.Status);
            await _orderLogRepository.Create(log);
            await _unitOfWork.SaveChangesAsync(token);
            await _unitOfWork.CommitAsync(token);
            return Result.Ok(order);
        }
    }
}
