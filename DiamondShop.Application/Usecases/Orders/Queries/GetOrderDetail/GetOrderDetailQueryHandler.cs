using DiamondShop.Application.Dtos.Responses.Orders;
using DiamondShop.Domain.Models.AccountAggregate.ValueObjects;
using DiamondShop.Domain.Models.Orders;
using DiamondShop.Domain.Models.Orders.Entities;
using DiamondShop.Domain.Models.Orders.ErrorMessages;
using DiamondShop.Domain.Models.Orders.ValueObjects;
using DiamondShop.Domain.Models.RoleAggregate;
using DiamondShop.Domain.Repositories.JewelryReviewRepo;
using DiamondShop.Domain.Repositories.OrderRepo;
using DiamondShop.Domain.Services.interfaces;
using FluentResults;
using MapsterMapper;
using MediatR;
using Serilog;

namespace DiamondShop.Application.Usecases.Orders.Queries.GetUserOrderDetail
{
    public record GetOrderDetailQuery(string OrderId, string Role, string AccountId) : IRequest<Result<Order>>;
    internal class GetOrderDetailQueryHandler : IRequestHandler<GetOrderDetailQuery, Result<Order>>
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IOrderService _orderService;
        private readonly IOrderLogRepository _orderLogRepository;
        private readonly IJewelryReviewRepository _jewelryReviewRepository;
        private readonly IMapper _mapper;

        public GetOrderDetailQueryHandler(IOrderRepository orderRepository, IOrderService orderService, IOrderLogRepository orderLogRepository, IJewelryReviewRepository jewelryReviewRepository, IMapper mapper)
        {
            _orderRepository = orderRepository;
            _orderService = orderService;
            _orderLogRepository = orderLogRepository;
            _jewelryReviewRepository = jewelryReviewRepository;
            _mapper = mapper;
        }

        public async Task<Result<Order>> Handle(GetOrderDetailQuery request, CancellationToken cancellationToken)
        {
            request.Deconstruct(out string orderId, out string role, out string accountId);
            var orderQuery = _orderRepository.GetQuery();
            orderQuery = _orderRepository.GetDetailQuery(orderQuery);
            var order = _orderRepository.QueryFilter(orderQuery, p => p.Id == OrderId.Parse(orderId)).FirstOrDefault();
            if (order == null)
                return Result.Fail(OrderErrors.OrderNotFoundError);
            if ((role == AccountRole.CustomerId && order.AccountId != AccountId.Parse(accountId)) ||
                (role == AccountRole.DelivererId && order.DelivererId != AccountId.Parse(accountId)))
                return Result.Fail(OrderErrors.NoPermissionToViewError);
            var orderLog = await _orderLogRepository.GetOrderLogs(order, cancellationToken);
            List<OrderLog> returnLogDetail = new();
            ////var mappedLogs = _mapper.Map<List<OrderLogDto>>(orderLog);
            var getParentLog = orderLog.Where(x => x.IsParentLog).ToList();
            //var getChildLog = orderLog.Where(x => !x.IsParentLog).ToList();

            //foreach (var child in getChildLog)
            //{
            //    var parent = getParentLog.FirstOrDefault(x => x.Id == child.PreviousLogId);
            //    if (parent != null)
            //    {
            //        parent.ChildLogs.Add(child);
            //    }
            //}
            getParentLog.OrderBy(x => x.CreatedDate).ToList();
            order.Logs = getParentLog;
            return order;
        }
    }
}
