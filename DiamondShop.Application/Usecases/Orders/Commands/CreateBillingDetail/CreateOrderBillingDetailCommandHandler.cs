using DiamondShop.Domain.Repositories;
using DiamondShop.Domain.Repositories.JewelryRepo;
using DiamondShop.Domain.Repositories.OrderRepo;
using DiamondShop.Domain.Services.interfaces;
using FluentResults;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Application.Usecases.Orders.Commands.CreateBillingDetail
{
    public record CreateOrderBillingDetailCommand(string orderId) : IRequest<Result>;
    internal class CreateOrderBillingDetailCommandHandler : IRequestHandler<CreateOrderBillingDetailCommand, Result>
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IOrderItemRepository _orderItemRepository;
        private readonly IDiamondRepository _diamondRepository;
        private readonly IJewelryRepository _jewelryRepository;
        private readonly IDiamondServices _diamondServices;

        public CreateOrderBillingDetailCommandHandler(IOrderRepository orderRepository, IOrderItemRepository orderItemRepository, IDiamondRepository diamondRepository, IJewelryRepository jewelryRepository, IDiamondServices diamondServices)
        {
            _orderRepository = orderRepository;
            _orderItemRepository = orderItemRepository;
            _diamondRepository = diamondRepository;
            _jewelryRepository = jewelryRepository;
            _diamondServices = diamondServices;
        }

        public async Task<Result> Handle(CreateOrderBillingDetailCommand request, CancellationToken cancellationToken)
        {
            
            throw new NotImplementedException();
        }
    }
}
