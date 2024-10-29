﻿using DiamondShop.Application.Services.Interfaces;
using DiamondShop.Domain.Models.Orders;
using DiamondShop.Domain.Models.Orders.Enum;
using DiamondShop.Domain.Models.Orders.ValueObjects;
using DiamondShop.Domain.Models.Transactions;
using DiamondShop.Domain.Models.Transactions.Enum;
using DiamondShop.Domain.Repositories;
using DiamondShop.Domain.Repositories.JewelryModelRepo;
using DiamondShop.Domain.Repositories.JewelryRepo;
using DiamondShop.Domain.Repositories.OrderRepo;
using DiamondShop.Domain.Repositories.TransactionRepo;
using DiamondShop.Domain.Services.interfaces;
using FluentResults;
using MediatR;

namespace DiamondShop.Application.Usecases.Orders.Commands.Proceed
{
    public record ProceedOrderCommand(string orderId, string? delivererId) : IRequest<Result<Order>>;
    internal class ProceedOrderCommandHandler : IRequestHandler<ProceedOrderCommand, Result<Order>>
    {
        private readonly IAccountRepository _accountRepository;
        private readonly IOrderRepository _orderRepository;
        private readonly IOrderItemRepository _orderItemRepository;
        private readonly ITransactionRepository _transactionRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IOrderService _orderService;
        private readonly IOrderTransactionService _orderTransactionService;
        private readonly ISender _sender;

        public ProceedOrderCommandHandler(IAccountRepository accountRepository, IOrderRepository orderRepository, IOrderItemRepository orderItemRepository, ITransactionRepository transactionRepository, IUnitOfWork unitOfWork, ISender sender, IOrderService orderService, IOrderTransactionService orderTransactionService)
        {
            _accountRepository = accountRepository;
            _orderRepository = orderRepository;
            _orderItemRepository = orderItemRepository;
            _transactionRepository = transactionRepository;
            _unitOfWork = unitOfWork;
            _sender = sender;
            _orderService = orderService;
            _orderTransactionService = orderTransactionService;
        }

        public async Task<Result<Order>> Handle(ProceedOrderCommand request, CancellationToken token)
        {
            request.Deconstruct(out string orderId, out string? delivererId);
            await _unitOfWork.BeginTransactionAsync(token);
            var order = await _orderRepository.GetById(OrderId.Parse(orderId));
            if (order == null)
                return Result.Fail("No order found!");
            if (!_orderService.IsProceedable(order.Status))
                return Result.Fail("This order can't proceed!");
            var orderItemQuery = _orderItemRepository.GetQuery();
            orderItemQuery = _orderItemRepository.QueryFilter(orderItemQuery, p => p.OrderId == order.Id);
            var orderItems = orderItemQuery.ToList();
            if (order.Status == OrderStatus.Pending)
            {
                Transaction trans = Transaction.CreateManualPayment(order.Id, $"Transfer from {order.Account?.FullName.FirstName} {order.Account?.FullName.LastName} for order #{order.Id.Value}", _orderTransactionService.GetFullPaymentValueForOrder(order), TransactionType.Pay);
                trans.AppTransactionCode = "";
                trans.PaygateTransactionCode = "";
                await _transactionRepository.Create(trans);
                order.Status = OrderStatus.Processing;
                order.PaymentStatus = order.PaymentType == PaymentType.Payall ? PaymentStatus.PaidAll : PaymentStatus.Deposited;
                await _orderRepository.Update(order);
                orderItems.ForEach(p => p.Status = OrderItemStatus.Prepared);
                _orderItemRepository.UpdateRange(orderItems);
            }
            else if (order.Status == OrderStatus.Processing)
            {
                order.Status = OrderStatus.Prepared;
            }
            else if (order.Status == OrderStatus.Prepared)
            {
                if (order.DelivererId == null)
                    return Result.Fail("No deliverer has been assigned to this order. Please assign immediately!");
                order.Status = OrderStatus.Delivering;
            }
            else if (order.Status == OrderStatus.Delivering)
            {
                if (delivererId == null)
                    return Result.Fail("No deliverer to assign.");
                if (order.DelivererId?.Value != delivererId)
                    return Result.Fail("Only the deliverer of this order can complete it.");
                if (order.PaymentType == PaymentType.COD)
                    //TODO: Add payment here
                    _orderTransactionService.AddCODPayment(order);
                order.Status = OrderStatus.Success;
            }
            else
            {
                return Result.Fail("Can't get order status.");
            }
            await _orderRepository.Update(order);
            await _unitOfWork.SaveChangesAsync(token);
            await _unitOfWork.CommitAsync(token);
            return order;
        }
    }
}
