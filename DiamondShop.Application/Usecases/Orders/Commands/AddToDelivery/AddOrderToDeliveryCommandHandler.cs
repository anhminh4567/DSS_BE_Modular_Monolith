using DiamondShop.Application.Services.Interfaces;
using DiamondShop.Application.Usecases.Deliveries.Commands.Create;
using DiamondShop.Commons;
using DiamondShop.Domain.Models.Orders.Enum;
using DiamondShop.Domain.Models.Orders.ValueObjects;
using DiamondShop.Domain.Repositories.OrderRepo;
using DiamondShop.Domain.Services.interfaces;
using FluentResults;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Application.Usecases.Orders.Commands.AddToDelivery
{
    public record AddOrderToDeliveryCommand(string orderId, string deliveryId) : IRequest<Result>;
    internal class AddOrderToDeliveryCommandHandler : IRequestHandler<AddOrderToDeliveryCommand, Result>
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IOrderItemRepository _orderItemRepository;
        private readonly IDeliveryPackageRepository _deliveryPackageRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IOrderService _orderService;

        public AddOrderToDeliveryCommandHandler(IOrderRepository orderRepository, IDeliveryPackageRepository deliveryPackageRepository, IUnitOfWork unitOfWork, IOrderItemRepository orderItemRepository, IOrderService orderService)
        {
            _orderRepository = orderRepository;
            _deliveryPackageRepository = deliveryPackageRepository;
            _unitOfWork = unitOfWork;
            _orderItemRepository = orderItemRepository;
            _orderService = orderService;
        }

        public async Task<Result> Handle(AddOrderToDeliveryCommand request, CancellationToken token)
        {
            request.Deconstruct(out string orderId, out string deliveryId);
            await _unitOfWork.BeginTransactionAsync(token);

            var packageQuery = _deliveryPackageRepository.GetQuery();
            var package = packageQuery.FirstOrDefault(p => p.Id == DeliveryPackageId.Parse(deliveryId));
            if (package == null)
                return Result.Fail("This package delivery doesn't exist");
            if (package.Status == DeliveryPackageStatus.Preparing)
                return Result.Fail("This package is already being delivered or done");

            var orderQuery = _orderRepository.GetQuery();
            orderQuery = _orderRepository.QueryFilter(orderQuery, p => p.DeliveryPackageId == package.Id || p.Id == OrderId.Parse(orderId));
            var orders = orderQuery.ToList();

            //TODO: Add limit
            //if (orders.Count > DeliveryRules.MaxOrderInAPackage)
            //    return Result.Fail("This package is already full");

            _orderService.CheckForSameCity(orders);
            List<IError> errors = new List<IError>();
            var selected = orders.FirstOrDefault(p => p.Id == OrderId.Parse(orderId));
            if (selected == null)
                return Result.Fail("Can't get the selected order");
            if (selected.Status != OrderStatus.Prepared)
                return Result.Fail($"Order #{selected.Id} can only be deliverd when it's {OrderStatus.Prepared.ToString().ToLower()}!");
            
            selected.DeliveryPackageId = package.Id;
            _orderRepository.UpdateRange(orders);
            await _unitOfWork.SaveChangesAsync(token);
            await _unitOfWork.CommitAsync(token);
            //TODO: Add notification
            return Result.Ok();
        }
    }
}
