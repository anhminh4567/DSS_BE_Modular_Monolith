using DiamondShop.Application.Services.Interfaces;
using DiamondShop.Domain.Models.Orders;
using DiamondShop.Domain.Models.Orders.Entities;
using DiamondShop.Domain.Models.Orders.Enum;
using DiamondShop.Domain.Models.Orders.ErrorMessages;
using DiamondShop.Domain.Models.Orders.ValueObjects;
using DiamondShop.Domain.Repositories.OrderRepo;
using DiamondShop.Domain.Services.interfaces;
using FluentResults;
using MediatR;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Application.Usecases.Orders.Commands.Refund
{
    public record RefundOrderCommand(string OrderId) : IRequest<Result<Order>>;
    internal class RefundOrderCommandHandler : IRequestHandler<RefundOrderCommand, Result<Order>>
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IOrderTransactionService _orderTransactionService;
        private readonly IOrderLogRepository _orderLogRepository;
        private readonly IOrderService _orderService;

        public RefundOrderCommandHandler(IOrderRepository orderRepository, IUnitOfWork unitOfWork, IOrderTransactionService orderTransactionService, IOrderLogRepository orderLogRepository, IOrderService orderService)
        {
            _orderRepository = orderRepository;
            _unitOfWork = unitOfWork;
            _orderTransactionService = orderTransactionService;
            _orderLogRepository = orderLogRepository;
            _orderService = orderService;
        }

        public async Task<Result<Order>> Handle(RefundOrderCommand request, CancellationToken token)
        {
            request.Deconstruct(out string orderId);
            await _unitOfWork.BeginTransactionAsync(token);
            var order = await _orderRepository.GetById(OrderId.Parse(orderId));
            if (order == null)
                return Result.Fail(OrderErrors.OrderNotFoundError);
            if (order.PaymentStatus != PaymentStatus.Refunding)
                return Result.Fail(OrderErrors.RefundedError);
            var getStateChangingLogs  = await _orderLogRepository.GetStateChangingLog(order);
            var currentStatus = order.Status;
            var orderCancelOrRejectLogs = getStateChangingLogs.Where(l => l.Status == currentStatus).FirstOrDefault();
            var indexOfLog = getStateChangingLogs.IndexOf(orderCancelOrRejectLogs);
            OrderLog previousLog = getStateChangingLogs[indexOfLog - 1];
            var previousStatus = previousLog.Status;
            if (order.Status == OrderStatus.Cancelled)
            {
                _orderTransactionService.AddRefundUserCancel(order, previousStatus);
            }
            else if(order.Status == OrderStatus.Rejected)
            {
                _orderTransactionService.AddRefundShopReject(order, previousStatus);
            }
            order.PaymentStatus = PaymentStatus.Refunded;

            await _orderRepository.Update(order);
            await _unitOfWork.SaveChangesAsync(token);
            await _unitOfWork.CommitAsync(token);
            return order;
        }
    }
}
