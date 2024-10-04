using DiamondShop.Domain.Common.Carts;
using DiamondShop.Domain.Models.Diamonds;
using DiamondShop.Domain.Models.Diamonds.Enums;
using DiamondShop.Domain.Models.Jewelries;
using DiamondShop.Domain.Models.JewelryModels;
using DiamondShop.Domain.Models.JewelryModels.ValueObjects;
using DiamondShop.Domain.Models.Promotions;
using DiamondShop.Domain.Models.Promotions.Entities;
using DiamondShop.Domain.Models.Promotions.Enum;
using DiamondShop.Domain.Services.interfaces;
using FluentResults;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Domain.Services.Implementations
{
    internal class PromotionService : IPromotionServices
    {
        public Result ApplyPromotionOnCartModel(CartModel cartModel, Promotion promotion)
        {
            var promotionRequirement = promotion.PromoReqs;
            var promotionGift = promotion.Gifts;
            Dictionary<int, CartProduct> requirementProducts = new(); // int is index
            Dictionary<int, CartProduct> giftProducts = new();
            var orderReq = promotionRequirement.FirstOrDefault(r => r.TargetType == TargetType.Order);
            if(cartModel.Promotion.IsHavingPromotion is true)
            {
                 throw new Exception("already have a promotoin, stop doing things");
            }
            if (orderReq is not null)
            {
                HandleOrderRequirement(cartModel, promotion, orderReq);
            }
            else
            {
                HandleProductRequirement(cartModel,promotion, requirementProducts);
            }
            //now check if order have that promotion
            if (cartModel.Promotion.IsHavingPromotion)
            {
                return Result.Ok();
            }
            else
            {
                return Result.Fail("No promotion is applied");
            } 
        }
        private void HandleOrderRequirement(CartModel cartModel,Promotion promotion, PromoReq orderRequirement)
        {
            //var isValid = orderRequirement.Operator switch
            //{
            //    Operator.Larger => cartModel.OrderPrices.DiscountPrice > orderRequirement.Amount,
            //    Operator.Equal_Or_Larger => cartModel.OrderPrices.DiscountPrice >= orderRequirement.Amount,
            //    _ => throw new Exception("Major error, requirement for order have not operator")
            //};
            //if (isValid)
            //    cartModel.Promotion.Promotion = promotion;
        }
        private void HandleProductRequirement(CartModel cartModel, Promotion promotion, Dictionary<int, CartProduct> requirementProducts)
        {
            var productList = cartModel.Products;
            var promotionRequirement = promotion.PromoReqs;
            for (int i = 0; i < productList.Count; i++)
            {
                var product = productList[i];
                bool isApplied = false;
                if (product.IsValid == false)
                {
                    continue;
                }
                if (requirementProducts.Count >= promotionRequirement.Count)
                {
                    // to this step, the requirement met the condition, so no need to further check
                    cartModel.Promotion.Promotion = promotion;
                    cartModel.Promotion.RequirementProductsIndex = requirementProducts.Keys.ToList();
                    break;
                }
                if (product.Jewelry is not null)
                {
                    isApplied = CheckJewelryIsQualified(product.Jewelry, promotionRequirement);
                }
                else if (product.JewelryModel is not null)
                {
                    isApplied = CheckJewerlyModelIsQualified(product.JewelryModel.Id, promotionRequirement);
                }
                else if (product.Diamond is not null)
                {
                    isApplied = CheckDiamondIsQualified(product.Diamond, promotionRequirement);
                }
                if (isApplied)
                {
                    product.PromotionId = promotion.Id;
                    product.IsReqirement = true;
                    requirementProducts.Add(i, product);
                }
                else
                    continue;
            }
        }
        private void HandleOrderGift(CartModel cartModel, Promotion promotion, Gift orderGift)
        {
            if (orderGift.TargetType != TargetType.Order)
                return;
            // the price consist of default, discount saved, promtion saved and final
            // now we count the promotion saved -> depend on default - discount amount saved from previous calculation
            var orderPriceNow = cartModel.OrderPrices.DefaultPrice - cartModel.OrderPrices.DiscountAmountSaved;
            decimal promotionPriceSavedAmount = orderGift.UnitType switch
            {
                UnitType.Percent => Math.Ceiling( (orderPriceNow * orderGift.UnitValue) / 100), 
                UnitType.Fix_Price => orderGift.UnitValue,
                UnitType.Free_Gift => throw new Exception("Major error, gift for order have a type of freeGift ??? major error, check back flow "),
                _ => throw new Exception("Major error, gift for order have not unit type ")
            }  ;

            //the flow is, work on default -> discount -> promtoin -> final price
            //we += since we not sure if the previous amount exist
            cartModel.OrderPrices.PromotionAmountSaved += promotionPriceSavedAmount;
        }
        private void HandleProductGift(CartModel cartModel, Promotion promotion, Dictionary<int, CartProduct> giftProducts)
        {
            var productList = cartModel.Products;
            var promotionGift = promotion.Gifts;
            for (int i = 0; i < productList.Count; i++)
            {
                var product = productList[i];
                Gift? isThisGift = null;
                if (product.IsValid == false)
                {
                    continue;
                }
                if (product.Jewelry is not null)
                {
                    isThisGift = CheckIfJewelryIsGift(product.Jewelry, promotionGift);
                }
                else if (product.JewelryModel is not null)
                {
                    isThisGift = CheckIfJewelryModelIsGift(product.JewelryModel.Id, promotionGift);
                }
                else if (product.Diamond is not null)
                {
                    isThisGift = CheckIfDiamondIsGift(product.Diamond, promotionGift);
                }
                if (isThisGift is not null)
                {
                    product.PromotionId = promotion.Id;
                    product.IsGift = true;
                    giftProducts.Add(i, product);
                }
                else
                    continue;
            }
        }
        private void SetProductPrice(CartProduct product, Promotion promotion, Gift giftReq)
        {
            product.PromotionId = promotion.Id;
            product.IsGift = true;
            
        }
        private void SetOrderPrice(CartModel cartModel, Promotion promotion, decimal addedPromotionSavedAmount)
        {
            
        }
        private bool CheckJewerlyModelIsQualified(JewelryModelId jewelryModelId, List<PromoReq> requirements)
        {
            foreach (var requirement in requirements)
            {
                if (requirement.TargetType != TargetType.Jewelry_Model)
                    continue;
                if (requirement.ModelId == jewelryModelId)
                {
                    return true;
                }
            }
            return false;
        }
        private bool CheckJewelryIsQualified(Jewelry jewelry, List<PromoReq> requirements)
        {
            return CheckJewerlyModelIsQualified(jewelry.ModelId, requirements);
        }
        private bool CheckDiamondIsQualified(Diamond diamond, List<PromoReq> requirements)
        {
            foreach (var requirement in requirements)
            {
                if (requirement.TargetType != TargetType.Diamond)
                    continue;
                if (requirement.DiamondOrigin == DiamondOrigin.Both)
                {
                    return CheckDiamond4C(diamond, requirement);
                }
                else if (requirement.DiamondOrigin ==DiamondOrigin.Natural && diamond.IsLabDiamond == false)
                {
                     return CheckDiamond4C(diamond, requirement);
                }
                else if(requirement.DiamondOrigin == DiamondOrigin.Lab && diamond.IsLabDiamond == true)
                {
                    return CheckDiamond4C(diamond, requirement);
                }
                else
                {
                    return false;
                }
            }
            return false;
        }
        private bool CheckDiamond4C(Diamond diamond, PromoReq requirement)
        {
            return ValidateDiamond4C(diamond,requirement.CaratFrom.Value,requirement.CaratTo.Value,requirement.ColorFrom.Value,requirement.ColorTo.Value,requirement.ClarityFrom.Value,requirement.ClarityTo.Value,requirement.CutFrom.Value,requirement.CutTo.Value);
        }
        private bool CheckDiamond4CGift(Diamond diamond, Gift gift)
        {
            return ValidateDiamond4C(diamond, gift.CaratFrom.Value, gift.CaratTo.Value, gift.ColorFrom.Value, gift.ColorTo.Value, gift.ClarityFrom.Value, gift.ClarityTo.Value, gift.CutFrom.Value, gift.CutTo.Value);
        }
        private bool ValidateDiamond4C(Diamond diamond, float caratFrom, float caratTo, Color colorFrom, Color colorTo, Clarity clarityFrom, Clarity clarityTo, Cut cutFrom, Cut cutTo)
        {
            if (caratFrom <= diamond.Carat && caratTo >= diamond.Carat)
            {
                if (colorFrom <= diamond.Color && colorTo >= diamond.Color)
                {
                    if (clarityFrom <= diamond.Clarity && clarityTo >= diamond.Clarity)
                    {
                        if (cutFrom <= diamond.Cut && cutTo >= diamond.Cut)
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }
        private Gift? CheckIfDiamondIsGift(Diamond diamond, List<Gift> gifts)
        {
            foreach (var gift in gifts)
            {
                bool isQualified = false;
                if (gift.TargetType != TargetType.Diamond)
                    continue;
                if (gift.DiamondOrigin == DiamondOrigin.Both)
                {
                    isQualified= CheckDiamond4CGift(diamond, gift);
                }
                else if (gift.DiamondOrigin == DiamondOrigin.Natural && diamond.IsLabDiamond == false)
                {
                    isQualified = CheckDiamond4CGift(diamond, gift);
                }
                else if (gift.DiamondOrigin == DiamondOrigin.Lab && diamond.IsLabDiamond == true)
                {
                    isQualified =CheckDiamond4CGift(diamond, gift);
                }
                else
                {
                    return null;
                }
                //then check
                if (isQualified)
                {
                    return gift;
                }
            }
            return null;
        }
        private Gift? CheckIfJewelryIsGift(Jewelry jewelry, List<Gift> gifts)
        {
            return CheckIfJewelryModelIsGift(jewelry.ModelId, gifts);
        }
        private Gift? CheckIfJewelryModelIsGift(JewelryModelId jewelryModelId, List<Gift> gifts)
        {
            foreach (var gift in gifts)
            {
                if (gift.TargetType != TargetType.Jewelry_Model)
                    continue;
                if (gift.ItemId == jewelryModelId.Value)
                {
                    return gift;
                }
            }
            return null;
        }
    }
}
