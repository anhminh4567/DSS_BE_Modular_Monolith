using DiamondShop.Domain.Models.AccountAggregate.ValueObjects;
using DiamondShop.Domain.Models.Orders;
using DiamondShop.Domain.Models.Orders.ValueObjects;
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
    public record GetOrderDetailQuery(string orderId, string accountId) : IRequest<Result<Order>>;
    internal class GetOrderDetailQueryHandler : IRequestHandler<GetOrderDetailQuery, Result<Order>>
    {
        private readonly IOrderRepository _orderRepository;

        public GetOrderDetailQueryHandler(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }

        public async Task<Result<Order>> Handle(GetOrderDetailQuery request, CancellationToken cancellationToken)
        {
            request.Deconstruct(out string orderId, out string accountId);
            var orderQuery = _orderRepository.GetQuery();
            orderQuery = _orderRepository.QueryInclude(orderQuery, p => p.Items);
            orderQuery = _orderRepository.QueryFilter(orderQuery, p => p.Id == OrderId.Parse(orderId) && p.AccountId == AccountId.Parse(accountId));
            var order = orderQuery.FirstOrDefault();
            if (order == null)
                return Result.Fail("This order doesn't exist.");
            return order;
        }
    }
}
