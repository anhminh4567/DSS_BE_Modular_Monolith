using DiamondShop.Application.Services.Interfaces;
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
    public record ProceedOrderCommand(string orderId, string? delivererId = null) : IRequest<Result<Order>>;
    internal class ProceedOrderCommandHandler : IRequestHandler<ProceedOrderCommand, Result<Order>>
    {
        private readonly IAccountRepository _accountRepository;
        private readonly IPaymentMethodRepository _paymentMethodRepository;
        private readonly IOrderRepository _orderRepository;
        private readonly IOrderItemRepository _orderItemRepository;
        private readonly ITransactionRepository _transactionRepository;
        private readonly IDiamondRepository _diamondRepository;
        private readonly IJewelryRepository _jewelryRepository;
        private readonly IJewelryModelRepository _jewelryModelRepository;
        private readonly IMainDiamondRepository _mainDiamondRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMainDiamondService _mainDiamondService;
        private readonly IPaymentService _paymentService;
        private readonly ICartService _cartService;
        private readonly ICartModelService _cartModelService;
        private readonly ISender _sender;

        public ProceedOrderCommandHandler(IOrderRepository orderRepository, IOrderItemRepository orderItemRepository, IAccountRepository accountRepository, IUnitOfWork unitOfWork, IPaymentService paymentService, IJewelryRepository jewelryRepository, IMainDiamondRepository mainDiamondRepository, ISender sender, IDiamondRepository diamondRepository, IMainDiamondService mainDiamondService, IPaymentMethodRepository paymentMethodRepository, ICartService cartService, ITransactionRepository transactionRepository)
        {
            _orderRepository = orderRepository;
            _orderItemRepository = orderItemRepository;
            _accountRepository = accountRepository;
            _unitOfWork = unitOfWork;
            _paymentService = paymentService;
            _jewelryRepository = jewelryRepository;
            _mainDiamondRepository = mainDiamondRepository;
            _sender = sender;
            _diamondRepository = diamondRepository;
            _mainDiamondService = mainDiamondService;
            _paymentMethodRepository = paymentMethodRepository;
            _cartService = cartService;
            _transactionRepository = transactionRepository;
        }

        public async Task<Result<Order>> Handle(ProceedOrderCommand request, CancellationToken token)
        {
            request.Deconstruct(out string orderId, out string? delivererId);
            await _unitOfWork.BeginTransactionAsync(token);
            var orderQuery = _orderRepository.GetQuery();
            orderQuery = _orderRepository.QueryInclude(orderQuery, p => p.Account);
            var order = orderQuery.FirstOrDefault(p => p.Id == OrderId.Parse(orderId));
            if (order == null)
                return Result.Fail("No order found!");
            var orderItemQuery = _orderItemRepository.GetQuery();
            orderItemQuery = _orderItemRepository.QueryFilter(orderItemQuery, p => p.OrderId == order.Id);
            var orderItems = orderItemQuery.ToList();
            if (order.Status == OrderStatus.Pending)
            {
                Transaction trans = Transaction.CreateManualPayment(order.Id, $"Transfer from {order.Account?.FullName} for order #{order.Id}", order.TotalPrice, TransactionType.Pay);
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
