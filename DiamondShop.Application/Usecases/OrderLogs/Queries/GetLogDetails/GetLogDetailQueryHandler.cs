using DiamondShop.Application.Dtos.Responses;
using DiamondShop.Application.Dtos.Responses.Orders;
using DiamondShop.Application.Services.Interfaces.Orders;
using DiamondShop.Commons;
using DiamondShop.Domain.Models.Orders.ValueObjects;
using DiamondShop.Domain.Repositories.OrderRepo;
using FluentResults;
using FluentValidation.Results;
using MapsterMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Application.Usecases.OrderLogs.Queries.GetLogDetails
{
    public record GetLogDetailQuery(string orderId, string logId) : IRequest<Result<OrderLogDto>>;
    internal class GetLogDetailQueryHandler : IRequestHandler<GetLogDetailQuery, Result<OrderLogDto>>
    {
        private readonly IOrderLogRepository _orderLogRepository;
        private readonly IOrderRepository _orderRepository;
        private readonly IOrderFileServices _orderFileServices;
        private readonly IMapper _mapper;

        public GetLogDetailQueryHandler(IOrderLogRepository orderLogRepository, IOrderRepository orderRepository, IOrderFileServices orderFileServices, IMapper mapper)
        {
            _orderLogRepository = orderLogRepository;
            _orderRepository = orderRepository;
            _orderFileServices = orderFileServices;
            _mapper = mapper;
        }

        public async Task<Result<OrderLogDto>> Handle(GetLogDetailQuery request, CancellationToken cancellationToken)
        {
            var parsedOrderId = OrderId.Parse(request.orderId);
            var parsedLogId = OrderLogId.Parse(request.logId);
            var getOrder = await _orderRepository.GetById(parsedOrderId);
            if (getOrder == null)
                return Result.Fail(new NotFoundError("Order not found"));
            var logs = await _orderLogRepository.GetOrderLogs(getOrder, cancellationToken);
            var getLog = logs.FirstOrDefault(x => x.Id == parsedLogId);
            if (getLog == null)
                return Result.Fail(new NotFoundError("Log not found"));

            var mappedLogs = _mapper.Map<List<OrderLogDto>>(logs);
            var getMappedLog = _mapper.Map<OrderLogDto>(getLog);
            getMappedLog.PreviousLog = mappedLogs.FirstOrDefault(x => x.Id == getMappedLog.PreviousLogId);
            var getImages = await _orderFileServices.GetOrderLogImages(getOrder, getLog, cancellationToken);
            getMappedLog.LogImages = _mapper.Map<List<MediaDto>>(getImages.Value);
            return getMappedLog;
        }
    }
}
