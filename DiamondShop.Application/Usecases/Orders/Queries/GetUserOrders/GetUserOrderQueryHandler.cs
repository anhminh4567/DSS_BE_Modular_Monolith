using DiamondShop.Application.Commons.Responses;
using DiamondShop.Application.Usecases.Orders.Queries.GetAll;
using DiamondShop.Application.Usecases.Orders.Queries.GetOrderFilter;
using DiamondShop.Domain.Models.AccountAggregate.ValueObjects;
using DiamondShop.Domain.Models.Orders;
using DiamondShop.Domain.Models.Orders.Enum;
using DiamondShop.Domain.Repositories.OrderRepo;
using MediatR;

namespace DiamondShop.Application.Usecases.Orders.Queries.GetUserOrders
{
    public record GetUserOrderQuery(string accountId, OrderPaging? OrderPaging = null) : IRequest<PagingResponseDto<Order>>;
    internal class GetUserOrderQueryHandler : IRequestHandler<GetUserOrderQuery, PagingResponseDto<Order>>
    {
        private readonly IOrderRepository _orderRepository;
        public GetUserOrderQueryHandler(IOrderRepository jewelryModelRepository)
        {
            _orderRepository = jewelryModelRepository;
        }
        public async Task<PagingResponseDto<Order>> Handle(GetUserOrderQuery request, CancellationToken token)
        {
            request.Deconstruct(out string? accountId, out OrderPaging? orderPaging);
            orderPaging.Deconstruct(out int pageSize, out int start, out OrderStatus? status, out DateTime? createdDate, out string? email);
            var query = _orderRepository.GetQuery();
            query = _orderRepository.QueryInclude(query, p => p.Account);
            query = _orderRepository.QueryFilter(query, p => p.AccountId == AccountId.Parse(accountId));
            if (status != null) query = _orderRepository.QueryFilter(query, p => p.Status == status);
            if (createdDate != null) query = _orderRepository.QueryFilter(query, p => p.CreatedDate >= createdDate);
            if (email != null) query = _orderRepository.QueryFilter(query, p => p.Account.Email.ToUpper().Contains(email.ToUpper()));
            query = _orderRepository.QueryOrderBy(query, p => p.OrderBy(p => p.Status));
            var count = query.Count();
            query.Skip(start * pageSize);
            query.Take(pageSize);
            var result = query.ToList();
            var totalPage = (int)Math.Ceiling((decimal)count / (decimal)pageSize);
            var orders = query.ToList();
            var response = new PagingResponseDto<Order>(
                totalPage: totalPage,
                currentPage: start + 1,
                Values: result
                );
            return response;
        }
        private PagingResponseDto<Order> BlankPaging() => new PagingResponseDto<Order>(
                  totalPage: 0,
                  currentPage: 0,
                  Values: []
                  );
    }
}
