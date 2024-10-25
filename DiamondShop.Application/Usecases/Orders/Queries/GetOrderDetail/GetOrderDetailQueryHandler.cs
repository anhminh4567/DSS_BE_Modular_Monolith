﻿using DiamondShop.Domain.Models.AccountAggregate.ValueObjects;
using DiamondShop.Domain.Models.Orders;
using DiamondShop.Domain.Models.Orders.ValueObjects;
using DiamondShop.Domain.Models.RoleAggregate;
using DiamondShop.Domain.Repositories.OrderRepo;
using FluentResults;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Application.Usecases.Orders.Queries.GetUserOrderDetail
{
    public record GetOrderDetailQuery(string OrderId, string Role, string AccountId) : IRequest<Result<Order>>;
    internal class GetOrderDetailQueryHandler : IRequestHandler<GetOrderDetailQuery, Result<Order>>
    {
        private readonly IOrderRepository _orderRepository;

        public GetOrderDetailQueryHandler(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }

        public async Task<Result<Order>> Handle(GetOrderDetailQuery request, CancellationToken cancellationToken)
        {
            request.Deconstruct(out string orderId, out string role, out string accountId);
            var orderQuery = _orderRepository.GetQuery();
            orderQuery = _orderRepository.GetDetailQuery(orderQuery);
            var order = _orderRepository.QueryFilter(orderQuery, p => p.Id == OrderId.Parse(orderId)).FirstOrDefault();
            if (order == null)
                return Result.Fail("This order doesn't exist.");
            if (
                (role == AccountRole.CustomerId && order.AccountId != AccountId.Parse(accountId)) ||
                (role == AccountRole.DelivererId && order.DelivererId != AccountId.Parse(accountId))
                )
                return Result.Fail("You don't have permission to access this order");
            return order;
        }
    }
}
