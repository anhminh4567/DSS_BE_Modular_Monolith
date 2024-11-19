using DiamondShop.Application.Services.Interfaces;
using DiamondShop.Application.Usecases.Deliveries.Commands.Create;
using DiamondShop.Domain.Models.Orders;
using DiamondShop.Domain.Models.Orders.Entities;
using DiamondShop.Domain.Models.Orders.Enum;
using DiamondShop.Domain.Models.Orders.ErrorMessages;
using DiamondShop.Domain.Models.Orders.ValueObjects;
using DiamondShop.Domain.Repositories;
using DiamondShop.Domain.Repositories.OrderRepo;
using DiamondShop.Domain.Services.interfaces;
using FluentResults;
using MediatR;

namespace DiamondShop.Application.Usecases.Orders.Commands.Redeliver
{
    public record  RedeliverOrderCommand(string orderId, string delivererId) : IRequest<Result<Order>>;
    internal class RedeliverOrderCommandHandler : IRequestHandler<RedeliverOrderCommand, Result<Order>>
    {
        private readonly IAccountRepository _accountRepository;
        private readonly IOrderRepository _orderRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IOrderService _orderService;
        private readonly IOrderLogRepository _orderLogRepository;

        public RedeliverOrderCommandHandler(IAccountRepository accountRepository, IOrderRepository orderRepository, IUnitOfWork unitOfWork, IOrderService orderService, IOrderLogRepository orderLogRepository)
        {
            _accountRepository = accountRepository;
            _orderRepository = orderRepository;
            _unitOfWork = unitOfWork;
            _orderService = orderService;
            _orderLogRepository = orderLogRepository;
        }

        public async Task<Result<Order>> Handle(RedeliverOrderCommand request, CancellationToken token)
        {
            request.Deconstruct(out string orderId, out string delivererId);
            await _unitOfWork.BeginTransactionAsync(token);
            var orderQuery = _orderRepository.GetQuery();
            var order = _orderRepository.QueryFilter(orderQuery, p => p.Id == OrderId.Parse(orderId)).FirstOrDefault();
            if (order == null)
                return Result.Fail(OrderErrors.OrderNotFoundError);
            if(order.ShipFailedCount > DeliveryRules.MaxRedelivery)
                return Result.Fail(OrderErrors.MaxRedeliveryError);
            await _orderService.AssignDeliverer(order, delivererId, _accountRepository, _orderRepository);
            order.ShipFailedDate = null;
            order.Status = OrderStatus.Prepared;
            var log = OrderLog.CreateByChangeStatus(order, OrderStatus.Delivery_Failed);
            await _orderLogRepository.Create(log);
            await _orderRepository.Update(order);
            await _unitOfWork.SaveChangesAsync(token);
            await _unitOfWork.CommitAsync(token);
            return order;
        }
    }
}
