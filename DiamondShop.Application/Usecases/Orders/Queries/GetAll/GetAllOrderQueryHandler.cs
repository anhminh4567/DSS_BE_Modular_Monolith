using DiamondShop.Domain.Models.AccountAggregate.ValueObjects;
using DiamondShop.Domain.Models.Orders;
using DiamondShop.Domain.Repositories.OrderRepo;
using MediatR;

namespace DiamondShop.Application.Usecases.Orders.Queries.GetAll
{
    public record GetAllOrderQuery() : IRequest<List<Order>>;
    internal class GetAllOrderQueryHandler : IRequestHandler<GetAllOrderQuery, List<Order>>
    {
        private readonly IOrderRepository _orderRepository;

        public GetAllOrderQueryHandler(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }

        public async Task<List<Order>> Handle(GetAllOrderQuery request, CancellationToken cancellationToken)
        {
            var query = _orderRepository.GetQuery();
            query = _orderRepository.QueryInclude(query, p => p.Account);
            query = _orderRepository.QueryOrderBy(query, p => p.OrderBy(p => p.Status));
            var orders = query.ToList();
            return orders;
        }
    }
}
