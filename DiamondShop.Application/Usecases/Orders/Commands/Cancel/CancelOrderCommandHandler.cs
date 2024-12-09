using DiamondShop.Application.Services.Interfaces;
using DiamondShop.Domain.BusinessRules;
using DiamondShop.Domain.Models.AccountAggregate.ValueObjects;
using DiamondShop.Domain.Models.Notifications;
using DiamondShop.Domain.Models.Orders;
using DiamondShop.Domain.Models.Orders.Entities;
using DiamondShop.Domain.Models.Orders.Enum;
using DiamondShop.Domain.Models.Orders.ErrorMessages;
using DiamondShop.Domain.Models.Orders.ValueObjects;
using DiamondShop.Domain.Models.Transactions.Enum;
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
        private readonly ITransactionRepository _transactionRepository;
        private readonly IOrderRepository _orderRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IOrderService _orderService;
        private readonly IOrderTransactionService _orderTransactionService;
        private readonly IOrderLogRepository _orderLogRepository;
        private readonly INotificationRepository _notificationRepository;
        private readonly IPaymentService _paymentService;

        public CancelOrderCommandHandler(IOrderRepository orderRepository, IUnitOfWork unitOfWork, IOrderService orderService, IOrderTransactionService orderTransactionService, IOrderLogRepository orderLogRepository, INotificationRepository notificationRepository, IPaymentService paymentService, ITransactionRepository transactionRepository)
        {
            _orderRepository = orderRepository;
            _unitOfWork = unitOfWork;
            _orderService = orderService;
            _orderTransactionService = orderTransactionService;
            _orderLogRepository = orderLogRepository;
            _notificationRepository = notificationRepository;
            _paymentService = paymentService;
            _transactionRepository = transactionRepository;
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

            var transactions = await _transactionRepository.GetByOrderId(order.Id);
            //Can't cancel because transaction hasn't been verified
            if (transactions != null && transactions.Any(p => p.Status == TransactionStatus.Verifying))
                return Result.Fail(OrderErrors.Transfer.ExistVerifyingTransferError);
            //If deposit then no refund
            if(order.Status == OrderStatus.Pending)
            {
                order.PaymentStatus = PaymentStatus.No_Refund;
            }
            else
            {
                if (order.PaymentType == PaymentType.COD)
                    order.PaymentStatus = PaymentStatus.No_Refund;
                else
                    order.PaymentStatus = PaymentStatus.Refunding;
            }
            order.DelivererId = null;
            order.Status = OrderStatus.Cancelled;
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

            var notificationForShop = Notification.CreateShopMessage(order, $"khách hủy đơn hàng #{order.OrderCode} tại thời điểm {order.CancelledDate.Value.ToString(DateTimeFormatingRules.DateTimeFormat)}");
            await _notificationRepository.Create(notificationForShop);
            await _unitOfWork.SaveChangesAsync();
            await _paymentService.RemoveAllPaymentCache(order);
            await _unitOfWork.CommitAsync(token);
            return Result.Ok(order);
        }
    }
}
