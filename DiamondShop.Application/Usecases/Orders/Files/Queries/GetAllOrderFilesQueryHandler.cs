using DiamondShop.Application.Commons.Models;
using DiamondShop.Application.Services.Interfaces.Orders;
using DiamondShop.Domain.Models.Orders.ErrorMessages;
using DiamondShop.Domain.Models.Orders.ValueObjects;
using DiamondShop.Domain.Repositories.OrderRepo;
using FluentResults;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Application.Usecases.Orders.Files.Queries
{
    public record GetAllOrderFilesQuery(string orderId): IRequest<Result<OrderGalleryTemplate>>;
    internal class GetAllOrderFilesQueryHandler : IRequestHandler<GetAllOrderFilesQuery, Result<OrderGalleryTemplate>>
    {
        private readonly IOrderFileServices _orderFileServices;
        private readonly IOrderRepository _orderRepository;

        public GetAllOrderFilesQueryHandler(IOrderFileServices orderFileServices, IOrderRepository orderRepository)
        {
            _orderFileServices = orderFileServices;
            _orderRepository = orderRepository;
        }

        public async Task<Result<OrderGalleryTemplate>> Handle(GetAllOrderFilesQuery request, CancellationToken cancellationToken)
        {
            var parsedId = OrderId.Parse(request.orderId);
            var getOrder = await _orderRepository.GetById(parsedId);
            if(getOrder == null)
                return Result.Fail(OrderErrors.OrderNotFoundError);
            var getGallery = await _orderFileServices.GetAllOrderImages(getOrder);
            return getGallery;
        }
    }
    
}
