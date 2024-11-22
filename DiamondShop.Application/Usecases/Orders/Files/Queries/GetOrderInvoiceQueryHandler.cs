using DiamondShop.Application.Dtos.Responses;
using DiamondShop.Application.Services.Interfaces.Orders;
using DiamondShop.Domain.Models.Orders.ErrorMessages;
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

namespace DiamondShop.Application.Usecases.Orders.Files.Queries
{
    public record GetOrderInvoiceQuery(string orderId) : IRequest<Result<MediaDto>>;
    internal class GetOrderInvoiceQueryHandler : IRequestHandler<GetOrderInvoiceQuery, Result<MediaDto>>
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IOrderFileServices _orderFileServices;
        private readonly IMapper _mapper;

        public GetOrderInvoiceQueryHandler(IOrderRepository orderRepository, IOrderFileServices orderFileServices, IMapper mapper)
        {
            _orderRepository = orderRepository;
            _orderFileServices = orderFileServices;
            _mapper = mapper;
        }

        public async Task<Result<MediaDto>> Handle(GetOrderInvoiceQuery request, CancellationToken cancellationToken)
        {
            var parsedId = OrderId.Parse(request.orderId);
            var order = await _orderRepository.GetById(parsedId);
            if (order == null)
                return Result.Fail(OrderErrors.OrderNotFoundError);
            if (order.Status != Domain.Models.Orders.Enum.OrderStatus.Success)
                return Result.Fail(OrderErrors.NotComplete());
            var getFiles = await _orderFileServices.GetOrCreateOrderInvoice(order);
            var mapped = _mapper.Map<MediaDto>(getFiles);
            return mapped;
        }
    }
    
}
