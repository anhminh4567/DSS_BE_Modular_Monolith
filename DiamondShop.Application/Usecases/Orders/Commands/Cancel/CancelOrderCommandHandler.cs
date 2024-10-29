using DiamondShop.Application.Services.Interfaces;
using DiamondShop.Domain.Models.AccountAggregate.ValueObjects;
using DiamondShop.Domain.Models.Diamonds;
using DiamondShop.Domain.Models.Jewelries;
using DiamondShop.Domain.Models.Orders;
using DiamondShop.Domain.Models.Orders.Enum;
using DiamondShop.Domain.Models.Orders.ValueObjects;
using DiamondShop.Domain.Models.Transactions;
using DiamondShop.Domain.Repositories;
using DiamondShop.Domain.Repositories.JewelryRepo;
using DiamondShop.Domain.Repositories.OrderRepo;
using DiamondShop.Domain.Repositories.TransactionRepo;
using DiamondShop.Domain.Services.Implementations;
using DiamondShop.Domain.Services.interfaces;
using FluentResults;
using MediatR;

namespace DiamondShop.Api.Controllers.Orders.Cancel
{
    public record CancelOrderCommand(string OrderId, string AccountId, string reason) : IRequest<Result<Order>>;
    public record CancelOrderCommandHandler : IRequestHandler<CancelOrderCommand, Result<Order>>
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

        public CancelOrderCommandHandler(IOrderRepository orderRepository, IUnitOfWork unitOfWork, IOrderService orderService, IJewelryRepository jewelryRepository, IDiamondRepository diamondRepository, ITransactionRepository transactionRepository, IOrderTransactionService orderTransactionService)
        {
            _orderRepository = orderRepository;
            _unitOfWork = unitOfWork;
            _orderService = orderService;
            _jewelryRepository = jewelryRepository;
            _diamondRepository = diamondRepository;
            _transactionRepository = transactionRepository;
            _orderTransactionService = orderTransactionService;
        }

        public async Task<Result<Order>> Handle(CancelOrderCommand request, CancellationToken token)
        {
            request.Deconstruct(out string orderId, out string accountId, out string reason);
            await _unitOfWork.BeginTransactionAsync(token);
            var order = await _orderRepository.GetById(OrderId.Parse(orderId));
            if (order == null)
                return Result.Fail("No order found!");
            else if (!_orderService.IsCancellable(order.Status))
                return Result.Fail("This order can't be cancelled anymore!");
            if (order.AccountId != AccountId.Parse(accountId))
                return Result.Fail("You're not allowed to cancel this order");
            order.Status = OrderStatus.Cancelled;
            order.PaymentStatus = PaymentStatus.Refunding;
            order.CancelledDate = DateTime.UtcNow;
            order.CancelledReason = reason;
            _orderTransactionService.AddRefundUserCancel(order);
            await _orderRepository.Update(order);
            //Return to selling
            await _orderService.CancelItems(order,_orderRepository,_orderItemRepository,_jewelryRepository,_diamondRepository);
            await _unitOfWork.SaveChangesAsync(token);
            await _unitOfWork.CommitAsync(token);
            return Result.Ok(order);
        }
    }
}
