using DiamondShop.Domain.Models.AccountAggregate.ValueObjects;
using DiamondShop.Domain.Models.Orders;
using DiamondShop.Domain.Repositories.OrderRepo;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Application.Usecases.Orders.Queries.GetUserOrders
{
    public record GetUserOrdersQuery(string accountId) : IRequest<List<Order>>;
    internal class GetUserOrdersQueryHandler : IRequestHandler<GetUserOrdersQuery, List<Order>>
    {
        private readonly IOrderRepository _orderRepository;
        public GetUserOrdersQueryHandler(IOrderRepository jewelryModelRepository)
        {
            _orderRepository = jewelryModelRepository;
        }
        public async Task<List<Order>> Handle(GetUserOrdersQuery request, CancellationToken token)
        {
            var query = _orderRepository.GetQuery();
            query = _orderRepository.QueryInclude(query, p => p.Items);
            query = _orderRepository.QueryFilter(query, p => p.AccountId == AccountId.Parse(request.accountId));
            query = _orderRepository.QueryOrderBy(query, p => p.OrderBy(p => p.Status));
            var orders = query.ToList();
            return orders;
        }
    }
}
