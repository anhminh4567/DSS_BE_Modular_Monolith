﻿using DiamondShop.Application.Dtos.Requests.Carts;
using DiamondShop.Application.Dtos.Responses.Carts;
using DiamondShop.Application.Dtos.Responses.Diamonds;
using DiamondShop.Application.Dtos.Responses.Jewelries;
using DiamondShop.Domain.Common.Carts;
using DiamondShop.Domain.Models.AccountAggregate.Entities;
using DiamondShop.Domain.Models.Diamonds.ValueObjects;
using DiamondShop.Domain.Models.Jewelries.ValueObjects;
using DiamondShop.Domain.Models.JewelryModels.ValueObjects;
using Mapster;
using Microsoft.AspNetCore.Routing.Constraints;
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
                .Map(dest => dest.Id, src => src.Id.Value)
                .Map(dest => dest.SideDiamondChoices, src => src.SideDiamondChoices.Select(x => x.Value).ToList());

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
                .Map(dest => dest.MissingRequirement, src => src.MissingRequirement)
                .Map(dest => dest.GiftProductsIndex , src => src.GiftProductsIndex)
                .Map(dest => dest.Promotion.PromoReqs , src => src.Promotion.PromoReqs)
                .Map(dest => dest.Promotion.Gifts , src => src.Promotion.Gifts);


            config.NewConfig<CartModelValidationDto, CartModelValidationDto>()
                .Map(dest => dest.IsOrderValid, src => src.IsOrderValid)
                .Map(dest => dest.InvalidItemIndex, src => src.InvalidItemIndex);

            config.NewConfig<CheckoutPrice, CheckoutPriceDto>()
               .Map(dest => dest, src => src).Compile();

            config.NewConfig<ShippingPrice, ShippingPriceDto>()
                .Map(dest => dest.DeliveryFeeFounded, src => src.DeliveryFeeFounded)
                .Map(dest => dest.To, src => src.To)
                .Map(dest => dest.From, src => src.From)
                .Compile();
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
                .Map(dest => dest.GiftAssignedId, src => src.GiftAssignedId.Adapt<string>())
                .Map(dest => dest.CurrentWarrantyApplied , src => src.CurrentWarrantyApplied );

            config.NewConfig<CartItemRequestDto, CartItem>()
                .Map(dest => dest.JewelryId, src => (src.JewelryId != null) ? JewelryId.Parse(src.JewelryId) : null)
                .Map(dest => dest.DiamondId, src => (src.DiamondId != null) ? DiamondId.Parse(src.DiamondId) : null)
                .Map(dest => dest.JewelryModelId, src => (src.JewelryModelId != null) ? JewelryModelId.Parse(src.JewelryModelId) : null)
                .Map(dest => dest.SizeId, src => (src.SizeId != null) ? SizeId.Parse(src.SizeId) : null)
                .Map(dest => dest.MetalId, src => (src.MetalId != null) ? MetalId.Parse(src.MetalId) : null)
                .Map(dest => dest.SideDiamondChoices, src => src.SideDiamondChoices.Select(x => SideDiamondOptId.Parse(x)).ToList())
                .Map(dest => dest.EngravedText, src => src.EngravedText)
                .Map(dest => dest.EngravedFont, src => src.EngravedFont);
        }
    }
    
}
