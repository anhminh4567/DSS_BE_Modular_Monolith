﻿using DiamondShop.Application.Services.Interfaces;
using DiamondShop.Domain.Models.Orders;
using DiamondShop.Domain.Models.Orders.Entities;
using DiamondShop.Domain.Models.Orders.Enum;
using DiamondShop.Domain.Models.Transactions;
using DiamondShop.Domain.Models.Transactions.Events;
using DiamondShop.Domain.Repositories.OrderRepo;
using DiamondShop.Domain.Repositories.TransactionRepo;
using DiamondShop.Domain.Services.interfaces;
using MediatR;

namespace DiamondShop.Application.DomainEventConsumers
{
    internal class TransactionCreatedEventConsumer : INotificationHandler<TransactionCreatedEvent>
    {
        private readonly IOrderRepository _orderRepository;
        private readonly ITransactionRepository _transactionRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IOrderTransactionService _orderTransactionService;
        private readonly IOrderService _orderService;
        private readonly IOrderLogRepository _orderLogRepository;

        public TransactionCreatedEventConsumer(IOrderRepository orderRepository, ITransactionRepository transactionRepository, IUnitOfWork unitOfWork, IOrderTransactionService orderTransactionService, IOrderService orderService, IOrderLogRepository orderLogRepository)
        {
            _orderRepository = orderRepository;
            _transactionRepository = transactionRepository;
            _unitOfWork = unitOfWork;
            _orderTransactionService = orderTransactionService;
            _orderService = orderService;
            _orderLogRepository = orderLogRepository;
        }

        public async Task Handle(TransactionCreatedEvent notification, CancellationToken cancellationToken)
        {
            var getOrder = await _orderRepository.GetById(notification.Transaction.OrderId);
            if (getOrder is null)
            {
                throw new NullReferenceException("Order is null, impossible");
            }
            var orderTransactions = getOrder.Transactions;
            //if (orderTransactions.Any(x => x.Id == notification.Transaction.Id) == false)
            //{
            //    getOrder.AddTransaction(notification.Transaction);
            //}
            switch (getOrder.Status)
            {
                case OrderStatus.Pending:
                    if (getOrder.PaymentStatus == PaymentStatus.Pending)
                        HandlePendingState(getOrder, notification.Transaction);
                    break;
                case OrderStatus.Prepared:
                    if (getOrder.IsCollectAtShop)
                    {
                        if(getOrder.PaymentStatus == PaymentStatus.Deposited)
                            HandleDepositedState(getOrder, notification.Transaction);
                    }
                    break;
                case OrderStatus.Delivering:// means it is deposited
                    if (getOrder.PaymentStatus == PaymentStatus.Deposited)
                        HandleDepositedState(getOrder, notification.Transaction);
                    break;
                default:
                    throw new Exception("Invalid state for type payment to happen, there seems to be only 2 state possible for payment, PENDING & DELIVERYING");
            }
            await _orderRepository.Update(getOrder);
            await _unitOfWork.SaveChangesAsync();
            //throw new NotImplementedException();
        }
        private void HandlePendingState(Order order, Transaction transaction)
        {
            if (order.PaymentType == PaymentType.COD)// means it is Depositing transaction
            {
                order.Deposit(transaction);
                var log = OrderLog.CreateByChangeStatus(order, OrderStatus.Processing);
                _orderLogRepository.Create(log).Wait();
            }
            else if (order.PaymentType == PaymentType.Payall)// means it is Full payment transaction{
            {
                order.PayAll(transaction);
                var log = OrderLog.CreateByChangeStatus(order, OrderStatus.Processing);
                _orderLogRepository.Create(log).Wait();
            }
            else
                throw new Exception("Unknown payment type");
        }
        private void HandleDepositedState(Order order, Transaction transaction)
        {
            if (order.PaymentType == PaymentType.COD)// means it is Depositing transaction{
            {
                order.PayRemainingForDepositOrder(transaction);
                var log = OrderLog.CreateByChangeStatus(order, order.Status);
                _orderLogRepository.Create(log).Wait();
            }
            else
                throw new Exception("Unknown payment type");
        }

    }
}
