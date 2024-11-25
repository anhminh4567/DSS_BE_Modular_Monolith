using DiamondShop.Application.Services.Interfaces;
using DiamondShop.Application.Services.Interfaces.Orders;
using DiamondShop.Domain.Models.AccountAggregate.ErrorMessages;
using DiamondShop.Domain.Models.Orders;
using DiamondShop.Domain.Models.Orders.Entities;
using DiamondShop.Domain.Models.Orders.Enum;
using DiamondShop.Domain.Models.Orders.ErrorMessages;
using DiamondShop.Domain.Models.Orders.Events;
using DiamondShop.Domain.Models.Orders.ValueObjects;
using DiamondShop.Domain.Models.Transactions;
using DiamondShop.Domain.Models.Transactions.Enum;
using DiamondShop.Domain.Repositories;
using DiamondShop.Domain.Repositories.JewelryRepo;
using DiamondShop.Domain.Repositories.OrderRepo;
using DiamondShop.Domain.Repositories.TransactionRepo;
using DiamondShop.Domain.Services.interfaces;
using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Builder;

namespace DiamondShop.Application.Usecases.Orders.Commands.Proceed
{
    public record ProceedOrderCommand(string orderId, string accountId) : IRequest<Result<Order>>;
    internal class ProceedOrderCommandHandler : IRequestHandler<ProceedOrderCommand, Result<Order>>
    {
        private readonly IAccountRepository _accountRepository;
        private readonly IOrderRepository _orderRepository;
        private readonly IOrderItemRepository _orderItemRepository;
        private readonly ITransactionRepository _transactionRepository;
        private readonly IJewelryRepository _jewelryRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IOrderService _orderService;
        private readonly IOrderTransactionService _orderTransactionService;
        private readonly ISender _sender;
        private readonly IPublisher _publisher;
        private readonly IPaymentMethodRepository _paymentMethodRepository;
        private readonly IOrderLogRepository _orderLogRepository;
        private readonly IOrderFileServices _orderFileServices;
        private readonly IDiamondRepository _diamondRepository;

        public ProceedOrderCommandHandler(IAccountRepository accountRepository, IOrderRepository orderRepository, IOrderItemRepository orderItemRepository, ITransactionRepository transactionRepository, IJewelryRepository jewelryRepository, IUnitOfWork unitOfWork, IOrderService orderService, IOrderTransactionService orderTransactionService, ISender sender, IPublisher publisher, IPaymentMethodRepository paymentMethodRepository, IOrderLogRepository orderLogRepository, IOrderFileServices orderFileServices, IDiamondRepository diamondRepository)
        {
            _accountRepository = accountRepository;
            _orderRepository = orderRepository;
            _orderItemRepository = orderItemRepository;
            _transactionRepository = transactionRepository;
            _jewelryRepository = jewelryRepository;
            _unitOfWork = unitOfWork;
            _orderService = orderService;
            _orderTransactionService = orderTransactionService;
            _sender = sender;
            _publisher = publisher;
            _paymentMethodRepository = paymentMethodRepository;
            _orderLogRepository = orderLogRepository;
            _orderFileServices = orderFileServices;
            _diamondRepository = diamondRepository;
        }

        public async Task<Result<Order>> Handle(ProceedOrderCommand request, CancellationToken token)
        {
            request.Deconstruct(out string orderId, out string? accountId);
            await _unitOfWork.BeginTransactionAsync(token);
            var order = await _orderRepository.GetById(OrderId.Parse(orderId));
            var account = await _accountRepository.GetById(order.AccountId);
            var paymentMethods = await _paymentMethodRepository.GetAll();
            if (order == null)
                return Result.Fail(OrderErrors.OrderNotFoundError);
            if (account == null)
                return Result.Fail(AccountErrors.AccountNotFoundError);
            if (!_orderService.IsProceedable(order.Status))
                return Result.Fail(OrderErrors.UnproceedableError);
            var orderItemQuery = _orderItemRepository.GetQuery();
            orderItemQuery = _orderItemRepository.QueryFilter(orderItemQuery, p => p.OrderId == order.Id);
            orderItemQuery = _orderItemRepository.QueryInclude(orderItemQuery, p => p.Jewelry);
            orderItemQuery = _orderItemRepository.QueryInclude(orderItemQuery, p => p.Diamond);
            var orderItems = orderItemQuery.ToList();
            if (order.Status == OrderStatus.Pending)
            {
                if(order.PaymentType == PaymentType.Payall)
                {
                    Transaction trans = Transaction.CreateManualPayment(order.Id, $"Chuyển khoản từ tài khoản {order.Account?.FullName.FirstName} {order.Account?.FullName.LastName} cho đơn hàng {order.OrderCode}", _orderTransactionService.GetFullPaymentValueForOrder(order), TransactionType.Pay);
                    trans.AppTransactionCode = "";
                    trans.PaygateTransactionCode = "";
                    await _transactionRepository.Create(trans);
                    order.Status = OrderStatus.Processing;
                    order.PaymentStatus = order.PaymentType == PaymentType.Payall ? PaymentStatus.PaidAll : PaymentStatus.Deposited;
                }
                else
                {
                    decimal depositAmount = 0;
                    if (order.IsCustomOrder)
                        depositAmount = _orderTransactionService.GetCorrectAmountFromOrder(order);
                    else
                        depositAmount = _orderTransactionService.GetCorrectAmountFromOrder(order);
                    Transaction trans = Transaction.CreateManualPayment(order.Id, $"Chuyển khoản từ tài khoản {order.Account?.FullName.FirstName} {order.Account?.FullName.LastName} cho đơn hàng {order.OrderCode}", depositAmount, TransactionType.Pay);
                    trans.AppTransactionCode = "";
                    trans.PaygateTransactionCode = "";
                    await _transactionRepository.Create(trans);
                    order.Status = OrderStatus.Processing;
                    order.PaymentStatus = order.PaymentType == PaymentType.Payall ? PaymentStatus.PaidAll : PaymentStatus.Deposited;
                }

                await _orderRepository.Update(order);
                var log = OrderLog.CreateByChangeStatus(order, OrderStatus.Processing);
                await _orderLogRepository.Create(log);
            }
            else if (order.Status == OrderStatus.Processing)
            {
                //Change jewelry status if customize
                var items = order.Items;
                foreach (var item in items)
                {
                    if (order.CustomizeRequestId != null)
                    {
                        item.Jewelry.SetSold();
                        var getJewelryDiamond = await _diamondRepository.GetDiamondsJewelry(item.Jewelry.Id);
                        foreach (var diamond in getJewelryDiamond)
                        {
                            diamond.SetSold(diamond.DefaultPrice.Value, diamond.SoldPrice.Value);
                            await _diamondRepository.Update(diamond);
                        }
                        await _jewelryRepository.Update(item.Jewelry);
                    }
                    item.Status = OrderItemStatus.Prepared;
                }
                _orderItemRepository.UpdateRange(items);
                order.Status = OrderStatus.Prepared;
                var log = OrderLog.CreateByChangeStatus(order, OrderStatus.Prepared);
                await _orderLogRepository.Create(log);
            }
            else if (order.Status == OrderStatus.Prepared)
            {
                if (order.DelivererId == null)
                    return Result.Fail(OrderErrors.NoDelivererAssignedError);
                order.Status = OrderStatus.Delivering;
                var log = OrderLog.CreateByChangeStatus(order, OrderStatus.Delivering);
                await _orderLogRepository.Create(log);
            }
            else if (order.Status == OrderStatus.Delivering)
            {
                if (accountId == null)
                    return Result.Fail(OrderErrors.NoDelivererToAssignError);
                if (order.DelivererId?.Value != accountId)
                    return Result.Fail(OrderErrors.OnlyDelivererAllowedError);
                if (order.PaymentType == PaymentType.COD)
                    //TODO: Add payment here
                    _orderTransactionService.AddCODPayment(order);
                var items = order.Items;
                foreach (var item in items)
                {
                    item.Status = OrderItemStatus.Done;
                }
                order.Status = OrderStatus.Success;
                var log = OrderLog.CreateByChangeStatus(order, OrderStatus.Success);
                await _orderLogRepository.Create(log);
                //order.Raise(new OrderCompleteEvent(account.Id,order.Id, DateTime.UtcNow));
                await _publisher.Publish(new OrderCompleteEvent(account.Id, order.Id, DateTime.UtcNow));
            }
            else
            {
                return Result.Fail(OrderErrors.OrderStatusNotFoundError);
            }
            await _orderRepository.Update(order);
            await _unitOfWork.SaveChangesAsync(token);
            await _unitOfWork.CommitAsync(token);
            return order;
        }
    }
}
