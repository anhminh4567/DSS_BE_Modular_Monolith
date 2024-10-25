using DiamondShop.Application.Commons.Responses;
using DiamondShop.Application.Services.Interfaces;
using DiamondShop.Application.Usecases.Orders.Queries.GetOrderFilter;
using DiamondShop.Domain.Models.AccountAggregate.ValueObjects;
using DiamondShop.Domain.Models.Orders;
using DiamondShop.Domain.Models.Orders.Enum;
using DiamondShop.Domain.Models.RoleAggregate;
using DiamondShop.Domain.Repositories.OrderRepo;
using FluentResults;
using MediatR;

namespace DiamondShop.Application.Usecases.Orders.Queries.GetAll
{
    public record GetAllOrderQuery(string Role, string AccountId, OrderPaging? OrderPaging = null) : IRequest<Result<PagingResponseDto<Order>>>;
    internal class GetAllOrderQueryHandler : IRequestHandler<GetAllOrderQuery, Result<PagingResponseDto<Order>>>
    {
        private readonly IOrderRepository _orderRepository;

        public GetAllOrderQueryHandler(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }

        public async Task<Result<PagingResponseDto<Order>>> Handle(GetAllOrderQuery request, CancellationToken cancellationToken)
        {
            request.Deconstruct(out string role, out string accountId, out OrderPaging? orderPaging);
            orderPaging.Deconstruct(out int pageSize, out int start, out OrderStatus? status, out DateTime? createdDate, out DateTime? expectedDate, out string? email);
            var query = _orderRepository.GetQuery();
            //customer
            if (role == AccountRole.CustomerId)
            {
                query = _orderRepository.QueryFilter(query, p => p.AccountId == AccountId.Parse(accountId));
            }
            //deliverer
            else if (role == AccountRole.DelivererId)
            {
                query = _orderRepository.QueryFilter(query, p => p.DelivererId == AccountId.Parse(accountId));
            }
            //query = _orderRepository.QueryInclude(query, p => p.Account);
            if (status != null) query = _orderRepository.QueryFilter(query, p => p.Status == status);
            if (createdDate != null) query = _orderRepository.QueryFilter(query, p => p.CreatedDate >= createdDate);
            if (expectedDate != null) query = _orderRepository.QueryFilter(query, p => p.ExpectedDate >= expectedDate);
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
