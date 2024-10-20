using DiamondShop.Application.Dtos.Requests.Carts;
using DiamondShop.Application.Services.Interfaces;
using DiamondShop.Application.Usecases.Carts.Commands.ValidateFromJson;
using DiamondShop.Commons;
using DiamondShop.Domain.Common.Carts;
using DiamondShop.Domain.Models.AccountAggregate.ValueObjects;
using DiamondShop.Domain.Repositories;
using DiamondShop.Domain.Repositories.JewelryModelRepo;
using DiamondShop.Domain.Repositories.JewelryRepo;
using DiamondShop.Domain.Repositories.PromotionsRepo;
using DiamondShop.Domain.Services.interfaces;
using FluentResults;
using FluentValidation;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Application.Usecases.Carts.Commands.Validate
{
    public record ValidateCartCommand(string userId) : IRequest<Result<CartModel>>;
    internal class ValidateCartCommandHandler : IRequestHandler<ValidateCartCommand, Result<CartModel>>
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

        public ValidateCartCommandHandler(ICartModelService cartModelService, ICartService cartService, IJewelryRepository jewelryRepository, IDiamondRepository diamondRepository, IJewelryModelRepository jewelryModelRepository, IDiamondPriceRepository diamondPriceRepository, IDiscountRepository discountRepository, IPromotionRepository promotionRepository, ISizeMetalRepository sizeMetalRepository, IMetalRepository metalRepository)
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

        public async Task<Result<CartModel>> Handle(ValidateCartCommand request, CancellationToken cancellationToken)
        {
            var accId = AccountId.Parse(request.userId);
            var getUserCart = await _cartService.GetCartModel(accId);
            List<CartProduct> getProducts = new();
            foreach (var item in getUserCart)
            {
                var product = await _cartModelService.FromCartItem(item, _jewelryRepository, _jewelryModelRepository, _diamondRepository);
                if (product is not null)
                    getProducts.Add(product);
            }
            var getActiveDiscount = await _discountRepository.GetActiveDiscount();
            var getActivePromotion = await _promotionRepository.GetActivePromotion();
            Result<CartModel> result = await _cartModelService.Execute(getProducts,getActiveDiscount,getActivePromotion , _diamondPriceRepository, _sizeMetalRepository, _metalRepository);
            if (result.IsSuccess)
                return result.Value;
            return Result.Fail(result.Errors);
            //throw new NotImplementedException();
        }
    }
}
