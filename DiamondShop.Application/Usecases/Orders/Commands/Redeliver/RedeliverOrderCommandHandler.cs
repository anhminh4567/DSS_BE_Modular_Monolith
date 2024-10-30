using DiamondShop.Application.Services.Interfaces;
using DiamondShop.Application.Usecases.Deliveries.Commands.Create;
using DiamondShop.Domain.Models.AccountAggregate.ValueObjects;
using DiamondShop.Domain.Models.Orders;
using DiamondShop.Domain.Models.Orders.Enum;
using DiamondShop.Domain.Models.Orders.ValueObjects;
using DiamondShop.Domain.Models.RoleAggregate;
using DiamondShop.Domain.Repositories;
using DiamondShop.Domain.Repositories.OrderRepo;
using DiamondShop.Domain.Services.interfaces;
using FluentResults;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Application.Usecases.Orders.Commands.Redeliver
{
    public record  RedeliverOrderCommand(string orderId, string delivererId) : IRequest<Result<Order>>;
    internal class RedeliverOrderCommandHandler : IRequestHandler<RedeliverOrderCommand, Result<Order>>
    {
        private readonly IAccountRepository _accountRepository;
        private readonly IOrderRepository _orderRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IOrderService _orderService;

        public RedeliverOrderCommandHandler(IOrderRepository orderRepository, IUnitOfWork unitOfWork, IAccountRepository accountRepository, IOrderService orderSerivce)
        {
            _orderRepository = orderRepository;
            _unitOfWork = unitOfWork;
            _accountRepository = accountRepository;
            _orderService = orderSerivce;
        }

        public async Task<Result<Order>> Handle(RedeliverOrderCommand request, CancellationToken token)
        {
            request.Deconstruct(out string orderId, out string delivererId);
            await _unitOfWork.BeginTransactionAsync(token);
            var orderQuery = _orderRepository.GetQuery();
            var order = _orderRepository.QueryFilter(orderQuery, p => p.Id == OrderId.Parse(orderId)).FirstOrDefault();
            if (order == null)
                return Result.Fail("This order doesn't exist");
            if(order.ShipFailedCount > DeliveryRules.MaxRedelivery)
                return Result.Fail("Maximum redelivery reached");
            await _orderService.AssignDeliverer(order, delivererId, _accountRepository, _orderRepository);
            order.ShipFailedDate = null;
            order.Status = OrderStatus.Prepared;
            await _orderRepository.Update(order);
            await _unitOfWork.SaveChangesAsync(token);
            await _unitOfWork.CommitAsync(token);
            return order;
        }
    }
}
