using DiamondShop.Application.Services.Interfaces;
using DiamondShop.Domain.BusinessRules;
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

namespace DiamondShop.Application.Usecases.Orders.Commands.Reject
{
    public record RejectOrderCommand(string OrderId, string Reason, bool IsForUser = false) : IRequest<Result<Order>>;
    internal class RejectOrderCommandHandler : IRequestHandler<RejectOrderCommand, Result<Order>>
    {
        private readonly ITransactionRepository _transactionRepository;
        private readonly IOrderRepository _orderRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IOrderService _orderService;
        private readonly IOrderTransactionService _orderTransactionService;
        private readonly IOrderLogRepository _orderLogRepository;
        private readonly IPaymentService _paymentService;
        private readonly INotificationRepository _notificationRepository;

        public RejectOrderCommandHandler(IOrderRepository orderRepository, IUnitOfWork unitOfWork, IOrderService orderService, IOrderTransactionService orderTransactionService, IOrderLogRepository orderLogRepository, ITransactionRepository transactionRepository, IPaymentService paymentService, INotificationRepository notificationRepository)
        {
            _orderRepository = orderRepository;
            _unitOfWork = unitOfWork;
            _orderService = orderService;
            _orderTransactionService = orderTransactionService;
            _orderLogRepository = orderLogRepository;
            _transactionRepository = transactionRepository;
            _paymentService = paymentService;
            _notificationRepository = notificationRepository;
        }

        public async Task<Result<Order>> Handle(RejectOrderCommand request, CancellationToken token)
        {
            request.Deconstruct(out string orderId, out string reason, out bool isForUser);
            await _unitOfWork.BeginTransactionAsync(token);
            var order = await _orderRepository.GetById(OrderId.Parse(orderId));
            if (order == null)
                return Result.Fail(OrderErrors.OrderNotFoundError);
            else if (order.HasDelivererReturned is false)
                return Result.Fail(OrderErrors.DelivererHasNotReturnedError);
            else if (!_orderService.IsCancellable(order.Status))
                return Result.Fail(OrderErrors.UncancellableError);

            var transactions = await _transactionRepository.GetByOrderId(order.Id);
            //Can't reject because transaction hasn't been verified or is unvalid
            if (transactions != null && transactions.Any(p => p.Status == TransactionStatus.Verifying))
                return Result.Fail(OrderErrors.Transfer.ExistVerifyingTransferError);
            if (isForUser)
            {
                if (order.Status == OrderStatus.Pending || order.PaymentType == PaymentType.COD)
                {
                    order.PaymentStatus = PaymentStatus.No_Refund;
                }
                else
                {
                    order.PaymentStatus = PaymentStatus.Refunding;
                }
                order.Status = OrderStatus.Cancelled;
            }
            else
            {
                if (order.Status == OrderStatus.Pending)
                {
                    order.PaymentStatus = PaymentStatus.No_Refund;
                }
                else
                {
                    order.PaymentStatus = PaymentStatus.Refunding;
                }
                order.Status = OrderStatus.Rejected;
            }
            order.DelivererId = null;
            order.CancelledDate = DateTime.UtcNow;
            order.CancelledReason = reason;
            await _orderRepository.Update(order);
            await _orderService.CancelItems(order);
            var log = OrderLog.CreateByChangeStatus(order, OrderStatus.Rejected);
            await _orderLogRepository.Create(log);
            var notificationForShop = Notification.CreateShopMessage(order, $"khách hủy đơn hàng #{order.OrderCode} tại thời điểm {order.CancelledDate.Value.ToString(DateTimeFormatingRules.DateTimeFormat)}");
            await _notificationRepository.Create(notificationForShop);
            await _unitOfWork.SaveChangesAsync(token);
            await _unitOfWork.CommitAsync(token);
            await _paymentService.RemoveAllPaymentCache(order);
            return order;
        }
    }
}
