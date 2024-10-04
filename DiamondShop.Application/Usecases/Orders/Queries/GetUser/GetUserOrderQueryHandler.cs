using DiamondShop.Domain.Models.Orders;
using DiamondShop.Domain.Repositories.OrderRepo;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Application.Usecases.Orders.Queries.GetUser
{
    public record GetUserOrderQuery : IRequest<List<Order>>;
    internal class GetUserOrderQueryHandler : IRequestHandler<GetUserOrderQuery, List<Order>>
    {
        private readonly IOrderRepository _orderRepository;
        public GetUserOrderQueryHandler(IOrderRepository jewelryModelRepository)
        {
            _orderRepository = jewelryModelRepository;
        }
        public async Task<List<Order>> Handle(GetUserOrderQuery request, CancellationToken token)
        {
            var query = _orderRepository.GetQuery();
            return query.ToList();
        }
    }
}
