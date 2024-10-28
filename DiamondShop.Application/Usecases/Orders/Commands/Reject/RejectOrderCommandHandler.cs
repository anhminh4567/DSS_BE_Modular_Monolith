using DiamondShop.Application.Services.Interfaces;
using DiamondShop.Domain.Models.Diamonds;
using DiamondShop.Domain.Models.Jewelries;
using DiamondShop.Domain.Models.Orders;
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
using DiamondShop.Domain.Repositories.TransactionRepo;
using DiamondShop.Domain.Services.interfaces;
using DiamondShop.Domain.Models.AccountAggregate.ValueObjects;
using DiamondShop.Domain.Models.Orders.Enum;
using DiamondShop.Domain.Models.Orders.ValueObjects;

namespace DiamondShop.Application.Usecases.Orders.Commands.Reject
{
    public record RejectOrderCommand(string orderId, string userId, string reason) : IRequest<Result<Order>>;
    internal class RejectOrderCommandHandler : IRequestHandler<RejectOrderCommand, Result<Order>>
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IOrderItemRepository _orderItemRepository;
        private readonly ITransactionRepository _transactionRepository;
        private readonly IJewelryRepository _jewelryRepository;
        private readonly IDiamondRepository _diamondRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IOrderService _orderService;
        private readonly IOrderTransactionService _orderTransactionService;
        private readonly IPaymentService _paymentService;

        public RejectOrderCommandHandler(IOrderRepository orderRepository, IOrderItemRepository orderItemRepository, ITransactionRepository transactionRepository, IJewelryRepository jewelryRepository, IDiamondRepository diamondRepository, IUnitOfWork unitOfWork, IOrderService orderService, IPaymentService paymentService, IOrderTransactionService orderTransactionService)
        {
            _orderRepository = orderRepository;
            _orderItemRepository = orderItemRepository;
            _transactionRepository = transactionRepository;
            _jewelryRepository = jewelryRepository;
            _diamondRepository = diamondRepository;
            _unitOfWork = unitOfWork;
            _orderService = orderService;
            _paymentService = paymentService;
            _orderTransactionService = orderTransactionService;
        }

        public async Task<Result<Order>> Handle(RejectOrderCommand request, CancellationToken token)
        {
            request.Deconstruct(out string orderId, out string accountId, out string reason);
            await _unitOfWork.BeginTransactionAsync(token);
            var order = await _orderRepository.GetById(OrderId.Parse(orderId));
            if (order == null)
                return Result.Fail("No order found!");
            else if (!_orderService.IsCancellable(order.Status))
                return Result.Fail("This order can't be rejected anymore!");
            order.Status = OrderStatus.Rejected;
            order.PaymentStatus = PaymentStatus.Refunding;
            order.CancelledDate = DateTime.UtcNow;
            order.CancelledReason = reason;
            var transac = _orderTransactionService.AddRefundShopReject(order);
            await _orderRepository.Update(order);
            await _orderService.CancelItems(order, _orderRepository, _orderItemRepository, _jewelryRepository, _diamondRepository);
            await _unitOfWork.SaveChangesAsync(token);
            await _unitOfWork.CommitAsync(token);
            return order;
        }
    }
}
