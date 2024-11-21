using DiamondShop.Application.Commons.Validators;
using DiamondShop.Application.Commons.Validators.ErrorMessages;
using DiamondShop.Application.Dtos.Responses.Orders;
using DiamondShop.Domain.BusinessRules;
using DiamondShop.Domain.Repositories.OrderRepo;
using FluentResults;
using FluentValidation;
using MapsterMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Application.Usecases.Dashboard.Queries.GetOrderCompleted
{
    public class GetOrderCompletedByRangeQueryValidator : AbstractValidator<GetOrderCompletedByRangeQuery>
    {
        public GetOrderCompletedByRangeQueryValidator()
        {
            RuleFor(x => x.dateFrom).NotEmpty().WithNotEmptyMessage();
            RuleFor(x => x.dateTo).NotEmpty().WithNotEmptyMessage();
            When(x => x.dateFrom != null && x.dateTo != null, () =>
            {
                RuleFor(x => x.dateFrom).ValidDate();
                RuleFor(x => x.dateTo).ValidDate();
                RuleFor(x => x).ValidStartEndDate(x => x.dateFrom, x => x.dateTo);
            });
        }
    }
    public record GetOrderCompletedByRangeQuery(string? dateFrom, string? dateTo, bool? isCustomOrder) : IRequest<Result<GetOrderCompletedByRangeDto>>;
    internal class GetOrderCompletedByRangeQueryHandler : IRequestHandler<GetOrderCompletedByRangeQuery, Result<GetOrderCompletedByRangeDto>>
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IOrderLogRepository _orderLogRepository;
        private readonly IMapper _mapper;

        public GetOrderCompletedByRangeQueryHandler(IOrderRepository orderRepository, IOrderLogRepository orderLogRepository, IMapper mapper)
        {
            _orderRepository = orderRepository;
            _orderLogRepository = orderLogRepository;
            _mapper = mapper;
        }

        public async Task<Result<GetOrderCompletedByRangeDto>> Handle(GetOrderCompletedByRangeQuery request, CancellationToken cancellationToken)
        {
            var parsedDateFrom = DateTime.ParseExact(request.dateFrom,DateTimeFormatingRules.DateTimeFormat, null);
            var parsedDateTo = DateTime.ParseExact(request.dateTo, DateTimeFormatingRules.DateTimeFormat, null);
            var getCompleteOrderLogByDateRange = await _orderLogRepository.GetCompleteOrderLogByDateRange(parsedDateFrom, parsedDateTo, cancellationToken);
            var selectOrderId = getCompleteOrderLogByDateRange.Select(x => x.OrderId).ToList();
            var orderQuery = _orderRepository.GetQuery();
            orderQuery = _orderRepository.QueryFilter(orderQuery, x => selectOrderId.Contains(x.Id));
            if(request.isCustomOrder != null)
            {
                if(request.isCustomOrder.Value)
                {
                    orderQuery = _orderRepository.QueryFilter(orderQuery, x => x.CustomizeRequestId != null);
                }
                else
                {
                    orderQuery = _orderRepository.QueryFilter(orderQuery, x => x.CustomizeRequestId == null);
                }
            }
            var orders = orderQuery.ToList();
            List<OrderDto> completedOrder = new();
            foreach(var order in orders)
            {
                var orderLog = getCompleteOrderLogByDateRange.FirstOrDefault(x => x.OrderId == order.Id);
                var completeDate = orderLog.CreatedDate;
                var orderDto = _mapper.Map<OrderDto>(order);
                orderDto.CompleteDate = _mapper.Map<string>(completeDate);
                completedOrder.Add(orderDto);
            }

            var count = orders.Count();
            return new GetOrderCompletedByRangeDto
            {
                CompletedOrder = completedOrder,
                TotalOrder = count
            };
        }
    }
    public class GetOrderCompletedByRangeDto 
    {
        public int TotalOrder { get; set; }
        public List<OrderDto> CompletedOrder { get; set; } = new();
    }
}
