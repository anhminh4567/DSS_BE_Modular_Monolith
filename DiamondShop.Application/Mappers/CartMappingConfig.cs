using DiamondShop.Application.Dtos.Responses.Carts;
using DiamondShop.Application.Dtos.Responses.Diamonds;
using DiamondShop.Application.Dtos.Responses.Jewelries;
using DiamondShop.Domain.Common.Carts;
using DiamondShop.Domain.Models.AccountAggregate.Entities;
using DiamondShop.Domain.Models.Diamonds.ValueObjects;
using DiamondShop.Domain.Models.JewelryModels.ValueObjects;
using Mapster;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Application.Mappers
{
    public class CartMappingConfig : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            config.NewConfig<CartItem, CartItemDto>()
                .Map(dest => dest.Id, src => src.Id.Value);

            config.NewConfig<CartModel, CartModelDto>()
                .Map(dest => dest.Promotion, src => src.Promotion)
                .Map(dest => dest.DiscountsApplied, src => src.DiscountsApplied)
                .Map(dest => dest.OrderPrices, src => src.OrderPrices)
                .Map(dest => dest.ShippingPrice, src => src.ShippingPrice)
                .Map(dest => dest.OrderCounter, src => src.OrderCounter)
                .Map(dest => dest.OrderValidation, src => src.OrderValidation)
                .Map(dest => dest.Products, src => src.Products);

            config.NewConfig<CartModelPromotion, CartModelPromotionDto>()
                .Map(dest => dest.Promotion, src => src.Promotion)
                .Map(dest => dest.IsHavingPromotion, src => src.IsHavingPromotion)
                .Map(dest => dest.RequirementProductsIndex, src => src.RequirementProductsIndex)
                .Map(dest => dest.GiftProductsIndex, src => src.GiftProductsIndex)
                .Map(dest => dest.MissingGifts, src => src.MissingGifts)
                .Map(dest => dest.MissingRequirement, src => src.MissingRequirement);


            config.NewConfig<CartModelValidationDto, CartModelValidationDto>()
                .Map(dest => dest.IsOrderValid, src => src.IsOrderValid)
                .Map(dest => dest.InvalidItemIndex, src => src.InvalidItemIndex);

            config.NewConfig<CheckoutPriceDto, CheckoutPriceDto>()
               .Map(dest => dest, src => src);

            config.NewConfig<ShippingPriceDto, ShippingPriceDto>()
                .Map(dest => dest, src => src);
            config.NewConfig<CartModelCounter, CartModelCounterDto>()
                .Map(dest => dest, src => src);

            config.NewConfig<CartProduct, CartProductDto>()
                .Map(dest => dest.Jewelry, src => src.Jewelry.Adapt<JewelryDto>())
                .Map(dest => dest.Diamond, src => src.Diamond.Adapt<DiamondDto>())
                .Map(dest => dest.JewelryModel, src => src.JewelryModel.Adapt<JewelryDto>())
                .Map(dest => dest.ReviewPrice, src => src.ReviewPrice.Adapt<CheckoutPriceDto>())
                .Map(dest => dest.DiscountId, src => src.DiscountId.Adapt<string>())
                .Map(dest => dest.PromotionId, src => src.PromotionId.Adapt<string>())
                .Map(dest => dest.RequirementQualifedId, src => src.RequirementQualifedId.Adapt<string>())
                .Map(dest => dest.GiftAssignedId, src => src.GiftAssignedId.Adapt<string>());
        }
    }
    
}
