using DiamondShop.Application.Services.Interfaces;
using DiamondShop.Domain.Common.Carts;
using DiamondShop.Domain.Models.AccountAggregate.Entities;
using DiamondShop.Domain.Repositories.JewelryModelRepo;
using DiamondShop.Domain.Repositories.JewelryRepo;
using DiamondShop.Domain.Repositories.PromotionsRepo;
using DiamondShop.Domain.Repositories;
using DiamondShop.Domain.Services.interfaces;
using FluentResults;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Application.Usecases.Orders.Commands.Validate
{
    public record ValidateOrderCommand(List<CartProduct> CartProducts):IRequest<Result<CartModel>>;
    internal class ValidateOrderCommandHandler : IRequestHandler<ValidateOrderCommand, Result<CartModel>>
    {
        private readonly ICartModelService _cartModelService;
        private readonly IDiamondPriceRepository _diamondPriceRepository;
        private readonly IDiscountRepository _discountRepository;
        private readonly IPromotionRepository _promotionRepository;
        private readonly ISizeMetalRepository _sizeMetalRepository;
        private readonly IMetalRepository _metalRepository;

        public ValidateOrderCommandHandler(ICartModelService cartModelService, IDiamondPriceRepository diamondPriceRepository, IDiscountRepository discountRepository, IPromotionRepository promotionRepository, ISizeMetalRepository sizeMetalRepository, IMetalRepository metalRepository)
        {
            _cartModelService = cartModelService;
            _diamondPriceRepository = diamondPriceRepository;
            _discountRepository = discountRepository;
            _promotionRepository = promotionRepository;
            _sizeMetalRepository = sizeMetalRepository;
            _metalRepository = metalRepository;
        }

        public async Task<Result<CartModel>> Handle(ValidateOrderCommand request, CancellationToken cancellationToken)
        {
            request.Deconstruct(out List<CartProduct> cartProducts);
            return await _cartModelService.Execute(cartProducts, _discountRepository, _promotionRepository, _diamondPriceRepository, _sizeMetalRepository, _metalRepository);
        }
    }
}
