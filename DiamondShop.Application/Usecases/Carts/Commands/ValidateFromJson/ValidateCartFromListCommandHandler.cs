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
using DiamondShop.Application.Dtos.Requests.Carts;
using MapsterMapper;
using DiamondShop.Domain.Models.Promotions.ValueObjects;
using DiamondShop.Domain.Models.Promotions.Entities;
using DiamondShop.Domain.Models.Promotions;
using Azure.Core;
using DiamondShop.Domain.Repositories.DeliveryRepo;
using DiamondShop.Application.Dtos.Requests.Accounts;
using System.Numerics;

namespace DiamondShop.Application.Usecases.Carts.Commands.ValidateFromJson
{
    public record ValidateCartFromListCommand(CartRequestDto items) : IRequest<Result<CartModel>>;
    internal class ValidateCartFromListCommandHandler : IRequestHandler<ValidateCartFromListCommand, Result<CartModel>>
    {
        private readonly ICartModelService _cartModelService;
        private readonly ILocationService _locationService;
        private readonly IDeliveryService _deliveryService;
        private readonly IDeliveryFeeRepository _deliveryFeeRepository;
        private readonly IDiscountRepository _discountRepository;
        private readonly IPromotionRepository _promotionRepository;
        private readonly IMapper _mapper;

        public ValidateCartFromListCommandHandler(ICartModelService cartModelService, ILocationService locationService, IDeliveryService deliveryService, IDeliveryFeeRepository deliveryFeeRepository, IDiscountRepository discountRepository, IPromotionRepository promotionRepository, IMapper mapper)
        {
            _cartModelService = cartModelService;
            _locationService = locationService;
            _deliveryService = deliveryService;
            _deliveryFeeRepository = deliveryFeeRepository;
            _discountRepository = discountRepository;
            _promotionRepository = promotionRepository;
            _mapper = mapper;
        }

        public async Task<Result<CartModel>> Handle(ValidateCartFromListCommand request, CancellationToken cancellationToken)
        {
            var cartItem = _mapper.Map<List<CartItem>>(request.items.Items);
            PromotionId promotionId = null;
            ShippingPrice getShippingPrice = new();

            if (request.items.PromotionId != null)
                promotionId = PromotionId.Parse(request.items.PromotionId);
            var getPromotion = GetPromotion(promotionId).Result;
            var getDiscounts = await _discountRepository.GetActiveDiscount();
            List<CartProduct> getProducts = _cartModelService.GetCartProduct(cartItem).Result;//PrepareCartProduct(cartItem).Result;
            if (request.items.UserAddress != null)
            {
                 getShippingPrice = GetShippingPrice(request.items.UserAddress).Result;
            }
            Result<CartModel> result = await _cartModelService.ExecuteNormalOrder(getProducts, getDiscounts, getPromotion,getShippingPrice);
            if (result.IsSuccess)
                return result.Value;
            return Result.Fail(result.Errors);
        }
        private async Task<List<Promotion>> GetPromotion(PromotionId? promotionId)
        {
            if (promotionId != null)
            {
                var getPromotionById = await _promotionRepository.GetById(promotionId);
                if (getPromotionById == null)
                    return new List<Promotion>();
                return new List<Promotion> { getPromotionById };
            }
            else
            {
                return  await _promotionRepository.GetActivePromotion();
            }
        }
        private async Task<ShippingPrice> GetShippingPrice(AddressRequestDto addressRequestDto)
        {
            var shipPrice = new ShippingPrice();
            var shopLocation = _locationService.GetShopLocation();
            var createShopAddress = Address.Create(shopLocation.Province,shopLocation.District,shopLocation.Ward,shopLocation.Road,AccountId.Parse("0"),AddressId.Parse("0"));
            shipPrice.From = createShopAddress;
            var city = addressRequestDto.Province;
            var getCityIfFound = _locationService.GetProvinces().FirstOrDefault(x => x.Name == city);
            if(getCityIfFound == null)
            {
                shipPrice.To = null;
                return shipPrice;
            }    
            var createDestinationAddress = Address.Create(addressRequestDto.Province, addressRequestDto.District, addressRequestDto.Ward, addressRequestDto.Street, AccountId.Parse("1"), AddressId.Parse("1"));
            shipPrice.To = createDestinationAddress;
            var getDeliveryLocationFees = await _deliveryFeeRepository.GetLocationType();
            var deliveryFee = _deliveryService.GetDeliveryFeeForLocation(shipPrice.To,shipPrice.From,getDeliveryLocationFees);      
            shipPrice.DeliveryFeeFounded = deliveryFee;
            return shipPrice;
        }
    }

}
