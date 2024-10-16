using DiamondShop.Application.Services.Interfaces;
using DiamondShop.Domain.Common.Carts;
using DiamondShop.Domain.Models.AccountAggregate.ValueObjects;
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
using DiamondShop.Domain.Models.AccountAggregate.Entities;

namespace DiamondShop.Application.Usecases.Carts.Commands.ValidateFromJson
{
    public record ValidateCartFromListCommand(List<CartItem> items) : IRequest<Result<CartModel>>;
    internal class ValidateCartFromListCommandHandler : IRequestHandler<ValidateCartFromListCommand, Result<CartModel>>
    {
        private readonly ICartModelService _cartModelService;
        private readonly ICartService _cartService;
        private readonly IJewelryRepository _jewelryRepository;
        private readonly IDiamondRepository _diamondRepository;
        private readonly IJewelryModelRepository _jewelryModelRepository;
        private readonly IDiamondPriceRepository _diamondPriceRepository;
        private readonly IDiscountRepository _discountRepository;
        private readonly IPromotionRepository _promotionRepository;
        private readonly ISizeMetalRepository _sizeMetalRepository;
        private readonly IMetalRepository _metalRepository;

        public ValidateCartFromListCommandHandler(ICartModelService cartModelService, ICartService cartService, IJewelryRepository jewelryRepository, IDiamondRepository diamondRepository, IJewelryModelRepository jewelryModelRepository, IDiamondPriceRepository diamondPriceRepository, IDiscountRepository discountRepository, IPromotionRepository promotionRepository, ISizeMetalRepository sizeMetalRepository, IMetalRepository metalRepository)
        {
            _cartModelService = cartModelService;
            _cartService = cartService;
            _jewelryRepository = jewelryRepository;
            _diamondRepository = diamondRepository;
            _jewelryModelRepository = jewelryModelRepository;
            _diamondPriceRepository = diamondPriceRepository;
            _discountRepository = discountRepository;
            _promotionRepository = promotionRepository;
            _sizeMetalRepository = sizeMetalRepository;
            _metalRepository = metalRepository;
        }

        public async Task<Result<CartModel>> Handle(ValidateCartFromListCommand request, CancellationToken cancellationToken)
        {
            List<CartProduct> getProducts = new();
            foreach (var item in request.items)
            {
                var product = await _cartModelService.FromCartItem(item, _jewelryRepository, _jewelryModelRepository, _diamondRepository);
                if (product is not null)
                    getProducts.Add(product);
            }
            Result<CartModel> result = await _cartModelService.Execute(getProducts, _discountRepository, _promotionRepository, _diamondPriceRepository, _sizeMetalRepository, _metalRepository);
            if (result.IsSuccess)
                return result.Value;
            return Result.Fail(result.Errors);
            //throw new NotImplementedException();
        }
    }

}
