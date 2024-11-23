using DiamondShop.Application.Dtos.Responses.Orders;
using DiamondShop.Application.Services.Interfaces.Orders;
using DiamondShop.Commons;
using DiamondShop.Domain.Models.Orders.Entities;
using DiamondShop.Domain.Models.Orders.ValueObjects;
using DiamondShop.Domain.Repositories.OrderRepo;
using FluentResults;
using MapsterMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Application.Usecases.OrderLogs.Queries.GetAllOrderLogs
{
    public record GetAllOrderLogQuery(string orderId) : IRequest<Result<List<OrderLogDto>>>;
    internal class GetAllOrderLogQueryHandler : IRequestHandler<GetAllOrderLogQuery, Result<List<OrderLogDto>>>
    {
        private readonly IOrderLogRepository _orderLogRepository;
        private readonly IOrderRepository _orderRepository;
        private readonly IOrderFileServices _orderFileServices;
        private readonly IMapper _mapper;

        public GetAllOrderLogQueryHandler(IOrderLogRepository orderLogRepository, IOrderRepository orderRepository, IOrderFileServices orderFileServices, IMapper mapper)
        {
            _orderLogRepository = orderLogRepository;
            _orderRepository = orderRepository;
            _orderFileServices = orderFileServices;
            _mapper = mapper;
        }

        public async Task<Result<List<OrderLogDto>>> Handle(GetAllOrderLogQuery request, CancellationToken cancellationToken)
        {
            var parsedOrderId = OrderId.Parse(request.orderId);
            var getOrder = await _orderRepository.GetById(parsedOrderId);
            if (getOrder == null)
                return Result.Fail(new NotFoundError("Order not found"));
            var logs = await _orderLogRepository.GetOrderLogs(getOrder, cancellationToken);
            var mappedLogs = _mapper.Map<List<OrderLogDto>>(logs);

            var getParentLog = mappedLogs.Where(x => x.IsParentLog).ToList();
            var getChildLog = mappedLogs.Where(x => !x.IsParentLog).ToList();

            //foreach (var child in getChildLog)
            //{
            //    var parent = getParentLog.FirstOrDefault(x => x.Id == child.PreviousLogId);
            //    if (parent != null)
            //    {
            //        parent.ChildLog.Add(child);
            //    }
            //}
            return getParentLog.OrderBy(x => x.CreatedDate).ToList();
        }
    }

}
