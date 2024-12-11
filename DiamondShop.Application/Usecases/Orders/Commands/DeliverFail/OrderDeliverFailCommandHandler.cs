using DiamondShop.Api.Controllers.Orders.Cancel;
using DiamondShop.Application.Services.Interfaces;
using DiamondShop.Application.Usecases.Deliveries.Commands.Create;
using DiamondShop.Domain.Models.AccountAggregate.ValueObjects;
using DiamondShop.Domain.Models.Orders;
using DiamondShop.Domain.Models.Orders.Entities;
using DiamondShop.Domain.Models.Orders.Enum;
using DiamondShop.Domain.Models.Orders.ErrorMessages;
using DiamondShop.Domain.Models.Orders.ValueObjects;
using DiamondShop.Domain.Repositories;
using DiamondShop.Domain.Repositories.JewelryRepo;
using DiamondShop.Domain.Repositories.OrderRepo;
using DiamondShop.Domain.Services.interfaces;
using FluentResults;
using MediatR;
using Newtonsoft.Json;

namespace DiamondShop.Application.Usecases.Orders.Commands.DeliverFail
{
    public record OrderDeliverFailRequestDto(string OrderId, bool IsShopFault = false);
    public record OrderDeliverFailCommand(string DelivererId, OrderDeliverFailRequestDto OrderDeliverFailRequestDto) : IRequest<Result<Order>>;
    internal class OrderDeliverFailCommandHandler : IRequestHandler<OrderDeliverFailCommand, Result<Order>>
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IOrderService _orderService;
        private readonly IOrderTransactionService _orderTransactionService;
        private readonly IOrderLogRepository _orderLogRepository;
        private readonly ISender _sender;

        public OrderDeliverFailCommandHandler(IOrderRepository orderRepository, IUnitOfWork unitOfWork, IOrderService orderService, IOrderTransactionService orderTransactionService, IOrderLogRepository orderLogRepository, ISender sender)
        {
            _orderRepository = orderRepository;
            _unitOfWork = unitOfWork;
            _orderService = orderService;
            _orderTransactionService = orderTransactionService;
            _orderLogRepository = orderLogRepository;
            _sender = sender;
        }

        public async Task<Result<Order>> Handle(OrderDeliverFailCommand request, CancellationToken token)
        {
            request.Deconstruct(out string delivererId, out OrderDeliverFailRequestDto orderDeliverFailRequestDto);
            orderDeliverFailRequestDto.Deconstruct(out string orderId, out bool isShopFault);
            await _unitOfWork.BeginTransactionAsync(token);
            var order = await _orderRepository.GetById(OrderId.Parse(orderId));
            if (order == null)
                return Result.Fail(OrderErrors.OrderNotFoundError);
            if (order.DelivererId != AccountId.Parse(delivererId))
                return Result.Fail(OrderErrors.OrderNotFoundError);
            if (!isShopFault)
                order.ShipFailedCount++;
            order.Status = OrderStatus.Delivery_Failed;
            var log = OrderLog.CreateByChangeStatus(order, OrderStatus.Delivery_Failed, ",lỗi bên " + (isShopFault ? "Shop" : "Khách"));
            await _orderLogRepository.Create(log);
            await _orderRepository.Update(order);
            await _unitOfWork.SaveChangesAsync(token);
            await _unitOfWork.CommitAsync(token);
            return order;
        }
    }
}
