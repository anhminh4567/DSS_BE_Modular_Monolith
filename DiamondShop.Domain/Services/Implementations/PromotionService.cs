using DiamondShop.Domain.Common.Carts;
using DiamondShop.Domain.Models.Diamonds;
using DiamondShop.Domain.Models.Diamonds.Enums;
using DiamondShop.Domain.Models.Diamonds.ValueObjects;
using DiamondShop.Domain.Models.Jewelries;
using DiamondShop.Domain.Models.Jewelries.ValueObjects;
using DiamondShop.Domain.Models.JewelryModels;
using DiamondShop.Domain.Models.JewelryModels.ValueObjects;
using DiamondShop.Domain.Models.Promotions;
using DiamondShop.Domain.Models.Promotions.Entities;
using DiamondShop.Domain.Models.Promotions.Enum;
using DiamondShop.Domain.Services.interfaces;
using FluentResults;
using Microsoft.AspNetCore.Components.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Domain.Services.Implementations
{
    public class PromotionService : IPromotionServices
    {
        /// <summary>
        /// the thing is in error state, need to handle amuont or requirement promotion state, 
        /// </summary>
        /// <param name="cartModel"></param>
        /// <param name="promotion"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public Result ApplyPromotionOnCartModel(CartModel cartModel, Promotion promotion)
        {
            var promotionRequirement = promotion.PromoReqs;
            var promotionGift = promotion.Gifts;
            Dictionary<int, CartProduct> requirementProducts = new(); // int is index
            Dictionary<int, CartProduct> giftProducts = new();
            var orderReq = promotionRequirement.FirstOrDefault(r => r.TargetType == TargetType.Order);
            if(cartModel.Promotion.IsHavingPromotion is true)
                 throw new Exception("already have a promotoin, stop doing things");
            if (orderReq is not null)
                HandleOrderRequirement(cartModel, promotion, orderReq);
            else
                HandleProductRequirement(cartModel,promotion, requirementProducts);
            //now check if order have that promotion
            if (cartModel.Promotion.IsHavingPromotion)
            {
                bool isFinallyValid = true;
                // for a promotion to be used, ALL REQUIREMENT MUST BE MET
                foreach (var req in promotionRequirement)
                {
                    isFinallyValid = HandleFinalRequirementCheckAfterValidation(cartModel, req, requirementProducts);
                    if (isFinallyValid is false) 
                    {
                        cartModel.Promotion.MissingRequirement = req;
                        break;
                    }
                }
                if (isFinallyValid)
                {
                    var giftReq = promotionGift.FirstOrDefault(r => r.TargetType == TargetType.Order);
                    if (giftReq is not null)
                        HandleOrderGift(cartModel, promotion, giftReq);
                    else
                        HandleProductGift(cartModel, promotion, giftProducts);
                    foreach (var prod in cartModel.Products)
                    {
                        cartModel.OrderPrices.DefaultPrice += prod.ReviewPrice.DefaultPrice;
                        cartModel.OrderPrices.DiscountAmountSaved += prod.ReviewPrice.DiscountAmountSaved;
                        cartModel.OrderPrices.PromotionAmountSaved += prod.ReviewPrice.PromotionAmountSaved;
                    }
                    return Result.Ok();
                }
                else
                {
                    return Result.Fail("not all requirement are met with thise requirement id: " + cartModel.Promotion.MissingRequirement.Id + " || with name: "+ cartModel.Promotion.MissingRequirement.Name);
                }

            }
            else
                return Result.Fail("No promotion is applied");
        }
        private void HandleOrderRequirement(CartModel cartModel,Promotion promotion, PromoReq orderRequirement)
        {
            var isValid = orderRequirement.Operator switch
            {
                Operator.Larger => cartModel.OrderPrices.FinalPrice > orderRequirement.Amount,
                Operator.Equal_Or_Larger => cartModel.OrderPrices.FinalPrice >= orderRequirement.Amount,
                _ => throw new Exception("Major error, requirement for order have not operator")
            };
            if (isValid)
                cartModel.Promotion.Promotion = promotion;
        }
        private void HandleProductRequirement(CartModel cartModel, Promotion promotion, Dictionary<int, CartProduct> requirementProducts)
        {
            var productList = cartModel.Products;
            var promotionRequirement = promotion.PromoReqs;
            for (int i = 0; i < productList.Count; i++)
            {
                var product = productList[i];
                bool isApplied = false;
                if (product.IsHavingPromotion )
                    continue;
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
            SetOrderPrice(cartModel,promotion,promotionPriceSavedAmount);
        }
        private void HandleProductGift(CartModel cartModel, Promotion promotion, Dictionary<int, CartProduct> giftProducts)
        {
            var productList = cartModel.Products;
            var promotionGift = promotion.Gifts;
            for (int i = 0; i < productList.Count; i++)
            {
                var product = productList[i];
                Gift? isThisGift = null;
                if (promotion.IsExcludeQualifierProduct)
                {
                    if (product.IsHavingPromotion && product.IsReqirement)
                    {
                        continue;
                    }
                }
                //if (product.IsValid == false)
                //    continue;
                if (product.Jewelry is not null)
                    isThisGift = CheckIfJewelryIsGift(product.Jewelry, promotionGift);
                else if (product.JewelryModel is not null)
                    isThisGift = CheckIfJewelryModelIsGift(product.JewelryModel.Id, promotionGift);
                else if (product.Diamond is not null)
                    isThisGift = CheckIfDiamondIsGift(product.Diamond, promotionGift);
                if (isThisGift is not null)
                {
                    SetProductPrice(product,promotion,isThisGift);
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
            decimal savedAmount = giftReq.UnitType switch
            {
                UnitType.Percent => Math.Ceiling((product.ReviewPrice.DefaultPrice * giftReq.UnitValue) / 100),
                UnitType.Fix_Price => giftReq.UnitValue,
                UnitType.Free_Gift => product.ReviewPrice.DiscountPrice,
                _ => throw new Exception("Major error, gift for product have not unit type ")
            };
            product.ReviewPrice.PromotionAmountSaved += savedAmount;
        }
        private void SetOrderPrice(CartModel cartModel,Promotion promotion, decimal addedPromotionSavedAmount)
        {
            //the flow is, work on default -> discount -> promtoin -> final price
            //we += since we not sure if the previous amount exist
            cartModel.Promotion.Promotion = promotion;
            cartModel.OrderPrices.PromotionAmountSaved += addedPromotionSavedAmount;
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
        /// <summary>
        /// these 2 are the final step to validate the price or amount, to see the requirements list if they have met the condition
        /// </summary>
        /// <param name="promoReq"></param>
        /// <param name="requirementProducts"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        private bool HandleFinalRequirementCheckAfterValidation(CartModel cartModel, PromoReq promoReq, Dictionary<int, CartProduct> requirementProducts) 
        {
            var promoTarget = promoReq.TargetType;
            
            bool CheckIfValid(decimal? amount , int? quantity,PromoReq req)
            {
                if(amount is not null)
                {
                    return ( req.Operator) switch
                    {
                        (Operator.Equal_Or_Larger) => amount >= req.Amount,
                        (Operator.Larger) => amount > req.Amount,
                        _ => false
                    };
                }
                else if (quantity is not null)
                {
                    return (req.Operator) switch
                    {
                        (Operator.Equal_Or_Larger) => quantity >= req.Quantity,
                        (Operator.Larger) => quantity > req.Quantity,
                        _ => false
                    };
                }
                return false;
            }
            switch (promoTarget)
            {
                case TargetType.Diamond:
                    if(promoReq.Amount is not null)
                    {
                        var totalDiamondsPrice = TotalProductAmountFromRequirements(Diamond4CRange.ParseFromRequirement(promoReq),null, requirementProducts);
                        return CheckIfValid(totalDiamondsPrice,null,promoReq);
                    }
                    else if (promoReq.Quantity is not null)
                    {
                        var totalDiamondsQuantity = TotalProductQuantityFromRequirements(Diamond4CRange.ParseFromRequirement(promoReq), null, requirementProducts);
                        return CheckIfValid(null, totalDiamondsQuantity, promoReq);
                    }
                    break;
                case TargetType.Jewelry_Model:
                    if (promoReq.Amount is not null)
                    {
                        var totalJewelryModelsPrice = TotalProductAmountFromRequirements(null, promoReq.ModelId, requirementProducts);
                        return CheckIfValid(totalJewelryModelsPrice, null, promoReq);
                    }
                    else if (promoReq.Quantity is not null)
                    {
                        var totalJewelryModelsQuantity = TotalProductQuantityFromRequirements(null, promoReq.ModelId, requirementProducts);
                        return CheckIfValid(null, totalJewelryModelsQuantity, promoReq);
                    }
                    break;
                case TargetType.Order:
                    return CheckIfValid(cartModel.OrderPrices.FinalPrice,null,promoReq);
                default:
                    return false;
            }
            return false;
        }

        private decimal? TotalProductAmountFromRequirements(Diamond4CRange? diamond4CRange, JewelryModelId? jewelryModelId, Dictionary<int, CartProduct> requirementProducts)
        {
            decimal total = 0;
            if ( diamond4CRange is not null)
            {
                total = requirementProducts.Values.Where(prod =>
                {
                    if (prod.Diamond is not null)
                        return ValidateDiamond4C(prod.Diamond, diamond4CRange.CaratFrom, diamond4CRange.CaratTo, diamond4CRange.ColorFrom, diamond4CRange.ColorTo, diamond4CRange.ClarityFrom, diamond4CRange.ClarityTo, diamond4CRange.CutFrom, diamond4CRange.CutTo);
                    else
                        return false;

                }).Sum(p => p.ReviewPrice.FinalPrice);
            }
            else if (jewelryModelId is not null)
            {
                total = requirementProducts.Values.Where(p => p.JewelryModel?.Id == jewelryModelId).Sum(p => p.ReviewPrice.FinalPrice);
            }
            return total;
        }
        private int? TotalProductQuantityFromRequirements( Diamond4CRange? diamond4CRange, JewelryModelId? jewelryModelId, Dictionary<int, CartProduct> requirementProducts)
        {
            int total = 0;
            if (diamond4CRange is not null)
            {
                total = requirementProducts.Values.Where(prod =>
                {
                    if (prod.Diamond is not null)
                        return ValidateDiamond4C(prod.Diamond, diamond4CRange.CaratFrom, diamond4CRange.CaratTo, diamond4CRange.ColorFrom, diamond4CRange.ColorTo, diamond4CRange.ClarityFrom, diamond4CRange.ClarityTo, diamond4CRange.CutFrom, diamond4CRange.CutTo);
                    else
                        return false;

                }).Count();
            }
            else if (jewelryModelId is not null)
            {
                total = requirementProducts.Values.Where(p => p.JewelryModel?.Id == jewelryModelId).Count();
            }
            return total;
        }
         
    }
    /// <summary>
    /// this is for internal usage for promotion only
    /// </summary>
    internal record Diamond4C
    {
        public Cut Cut { get; set; }
        public Color Color { get; set; }
        public Clarity Clarity { get; set; }
        public float Carat { get; set; }
        public static Diamond4C ParseFromDiamond(Diamond diamond)
        {
            return new Diamond4C
            {
                Cut = diamond.Cut.Value,
                Color = diamond.Color,
                Clarity = diamond.Clarity,
                Carat = diamond.Carat
            };
        }
    }
    internal record Diamond4CRange
    {
        public float CaratFrom { get; set; }
        public float CaratTo { get; set; }
        public Clarity ClarityFrom { get; set; }
        public Clarity ClarityTo { get; set; }
        public Cut CutFrom { get; set; }
        public Cut CutTo { get; set; }
        public Color ColorFrom { get; set; }
        public Color ColorTo { get; set; }
        public static Diamond4CRange ParseFromRequirement(PromoReq promoReq) 
        {
            return new Diamond4CRange
            {
                CaratFrom = promoReq.CaratFrom.Value,
                CaratTo = promoReq.CaratTo.Value,
                ClarityFrom = promoReq.ClarityFrom.Value,
                ClarityTo = promoReq.ClarityTo.Value,
                CutFrom = promoReq.CutFrom.Value,
                CutTo = promoReq.CutTo.Value,
                ColorFrom = promoReq.ColorFrom.Value,
                ColorTo = promoReq.ColorTo.Value
            };
        }
    }
}
