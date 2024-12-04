using DiamondShop.Application.Services.Interfaces;
using DiamondShop.Application.Usecases.Deliveries.Commands.Create;
using DiamondShop.Domain.Common;
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
using Microsoft.Extensions.Options;

namespace DiamondShop.Application.Usecases.Orders.Commands.Redeliver
{
    public record RedeliverOrderCommand(string orderId, string delivererId) : IRequest<Result<Order>>;
    internal class RedeliverOrderCommandHandler : IRequestHandler<RedeliverOrderCommand, Result<Order>>
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IOrderService _orderService;
        private readonly IOrderLogRepository _orderLogRepository;
        private readonly IOptionsMonitor<ApplicationSettingGlobal> _optionsMonitor;

        public RedeliverOrderCommandHandler(IOrderRepository orderRepository, IUnitOfWork unitOfWork, IOrderService orderService, IOrderLogRepository orderLogRepository, IOptionsMonitor<ApplicationSettingGlobal> optionsMonitor)
        {
            _orderRepository = orderRepository;
            _unitOfWork = unitOfWork;
            _orderService = orderService;
            _orderLogRepository = orderLogRepository;
            _optionsMonitor = optionsMonitor;
        }

        public async Task<Result<Order>> Handle(RedeliverOrderCommand request, CancellationToken token)
        {
            request.Deconstruct(out string orderId, out string delivererId);
            var orderRule = _optionsMonitor.CurrentValue.OrderRule;
            await _unitOfWork.BeginTransactionAsync(token);
            var orderQuery = _orderRepository.GetQuery();
            var order = _orderRepository.QueryFilter(orderQuery, p => p.Id == OrderId.Parse(orderId)).FirstOrDefault();
            if (order == null)
                return Result.Fail(OrderErrors.OrderNotFoundError);
            if (order.ShipFailedCount > orderRule.MaxRedelivery)
                return Result.Fail(OrderErrors.MaxRedeliveryError);
            await _orderService.AssignDeliverer(order, delivererId);
            order.ShipFailedDate = null;
            order.Status = OrderStatus.Prepared;
            var log = OrderLog.CreateByChangeStatus(order, OrderStatus.Prepared);
            await _orderLogRepository.Create(log);
            await _orderRepository.Update(order);
            await _unitOfWork.SaveChangesAsync(token);
            await _unitOfWork.CommitAsync(token);
            var getNewOrder = await _orderRepository.GetById(order.Id);
            return getNewOrder;
        }
    }
}
