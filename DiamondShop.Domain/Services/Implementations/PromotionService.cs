using DiamondShop.Domain.BusinessRules;
using DiamondShop.Domain.Common;
using DiamondShop.Domain.Common.Carts;
using DiamondShop.Domain.Models.AccountAggregate;
using DiamondShop.Domain.Models.Diamonds;
using DiamondShop.Domain.Models.Diamonds.Enums;
using DiamondShop.Domain.Models.Diamonds.ValueObjects;
using DiamondShop.Domain.Models.Jewelries;
using DiamondShop.Domain.Models.Jewelries.ValueObjects;
using DiamondShop.Domain.Models.JewelryModels;
using DiamondShop.Domain.Models.JewelryModels.ValueObjects;
using DiamondShop.Domain.Models.Orders;
using DiamondShop.Domain.Models.Promotions;
using DiamondShop.Domain.Models.Promotions.Entities;
using DiamondShop.Domain.Models.Promotions.Enum;
using DiamondShop.Domain.Models.Promotions.ErrorMessages;
using DiamondShop.Domain.Repositories.PromotionsRepo;
using DiamondShop.Domain.Services.interfaces;
using FluentResults;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Domain.Services.Implementations
{
    public partial class PromotionService : IPromotionServices
    {
        

        public Result ApplyPromotionOnCartModel(CartModel cartModel, Promotion promotion, PromotionRule promotionRule)
        {
            return ApplyPromotionOnCartModelGlobal(cartModel, promotion,promotionRule);
        }
        /// <summary>
        /// the thing is in error state, need to handle amuont or requirement promotion state, 
        /// </summary>
        /// <param name="cartModel"></param>
        /// <param name="promotion"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public static Result ApplyPromotionOnCartModelGlobal(CartModel cartModel, Promotion promotion,PromotionRule promotionRule)
        {
            //Init Data
            // the Data is that , the product req and gift is sorted, so that the ORDER TYPE is the last one
            var promotionRequirement = promotion.PromoReqs.OrderBy(r => r.TargetType).ThenByDescending(x => x.Amount).ThenByDescending(x => x.Quantity).ToList();
            var promotionGift = promotion.Gifts.OrderBy(r => r.TargetType).ThenByDescending(x => x.UnitValue).ToList();
            Dictionary<int, CartProduct> requirementProducts = new(); // int is index
            Dictionary<int, CartProduct> giftProducts = new();
            List<PromoReq> promoReqs = new();
            //List<Gift> promoGifts = new();
            bool IsRequirementMet = false;
            //clear previous promotion data applied on cart or product
            if (promotion.Status != Status.Active)
            {
                return Result.Fail(PromotionError.ApplyingError.NotActiveToUse);
            }
            //var orderReq = promotionRequirement.FirstOrDefault(r => r.TargetType == TargetType.Order);
            if (cartModel.Promotion.IsHavingPromotion is true)
                return Result.Fail(PromotionError.ApplyingError.AlreadyAppliedPromo);
            if (promotionRequirement.Count <= 0 || promotionGift.Count <= 0)
            {
                return Result.Fail(PromotionError.InvalidState);
                //throw new Exception("this promotion dont even have a requirement or gift, major error, it should not exist");
            }
            //account orders should not null here
            if (cartModel.Account != null)
            {
                if(cartModel.Account.CustomerOrders != null && cartModel.Account.CustomerOrders.Count > 0)
                {
                    var userUsedPromotion = promotion.OrderThatTruelyUsedThisPromotion(cartModel.Account.CustomerOrders);
                    var checkResult = CheckIfUserHasAlreadyUsedThisPromotionToLimit(userUsedPromotion, promotion,promotionRule); 
                    if(checkResult.IsFailed)
                        return checkResult;
                }
            }
            //Validate Requirements
            for (int i = 0; i < promotionRequirement.Count; i++)
            {
                var req = promotionRequirement[i];
               
                if (req.TargetType == TargetType.Order)
                {
                    var isValid = HandleOrderRequirement(cartModel, req);
                    if (isValid) // if the requirement is valid in the order, then add the requirement to the recognize list
                        promoReqs.Add(req);
                }
                else
                {
                    Dictionary<int, CartProduct> scopedRequirementProducts = new();// create a scopred requirement product
                    // so that if the handle products found requirement, BUT they are not valid to the promotion requirement
                    // like not haveing enought price or quantity met, then we can ignore this requirement entirely, and keep the
                    // original requirementProducts empty to add the real requirement that met the conditon
                    // NOTE: the requirement IS THE DECISION MAKER, to decide whether you add the product to the requirement list or not
                    var isAnyValid = HandleProductRequirement(cartModel, req, scopedRequirementProducts);
                    if (isAnyValid)
                    {
                        var totalItemCount = scopedRequirementProducts.Count;
                        var totalPrice = scopedRequirementProducts.Values.Sum(p => p.ReviewPrice.FinalPrice);
                        Action<Dictionary<int, CartProduct>> addRequirementProducts = (products) =>
                        {
                            foreach (var item in products)
                            {
                                // it should be try add, some other might have the same index
                                requirementProducts.TryAdd(item.Key, item.Value);
                            }
                            promoReqs.Add(req);
                        };
                        if (req.Amount != null)
                        {
                            if (totalPrice >= req.Amount)
                            {
                                addRequirementProducts(scopedRequirementProducts);
                                continue;
                            }
                        }
                        else if (req.Quantity != null)
                        {
                            if (totalItemCount >= req.Quantity)
                            {
                                addRequirementProducts(scopedRequirementProducts);
                                continue;
                            }
                        }
                        else
                        {
                            throw new Exception("Major error, requirement have no amount or quantity, at the requirement id: " + req.Id.Value + " with index: " + i);
                        }
                    }
                }
            }
            if (promoReqs.Count >= promotionRequirement.Count)
            {
                IsRequirementMet = true;
            }
            if (IsRequirementMet is false)
            {
                var metRequirementId = promoReqs.Select(r => r.Id).ToList();
                var missingRequirement = promotionRequirement.Where(r => !metRequirementId.Contains(r.Id)).ToList();
                cartModel.Promotion.MissingRequirement = missingRequirement;
                //importatnt to clean up , no clean up, next promotion applied WILL HAVE THE PREVIOUS PROMOTION DATA
                // which will result in WRONG AMOUNT APPLIED
                CleanupAfterAppliedFail(cartModel);
                return Result.Fail(PromotionError.ApplyingError.NotMeetRequirement);
            }
            else
            {
                // to this step, the requirement met the condition, so no need to further check
                cartModel.Promotion.Promotion = promotion;
                cartModel.Promotion.RequirementProductsIndex = requirementProducts.Keys.ToList();
            }
            //var giftReq = promotionGift.FirstOrDefault(r => r.TargetType == TargetType.Order);
            for (int i = 0; i < promotionGift.Count; i++)
            {
                var gift = promotionGift[i];
                if (gift.TargetType == TargetType.Order)
                {
                    HandleOrderGift(cartModel, gift);
                }
                else
                {
                    Dictionary<int, CartProduct> scopedGiftProducts = new();
                    HandleProductGift(cartModel,gift,promotion.IsExcludeQualifierProduct, scopedGiftProducts);
                    foreach (var prod in scopedGiftProducts)
                    {
                        giftProducts.TryAdd(prod.Key, prod.Value);
                    }
                }
            }
            cartModel.Promotion.GiftProductsIndex = giftProducts.Keys.ToList();
            //SetOrderPrice(cartModel);
            return Result.Ok();
        }



        private static bool HandleOrderRequirement(CartModel cartModel, PromoReq orderRequirement)
        {
            var isValid = orderRequirement.Operator switch
            {
                //Operator.Larger => cartModel.OrderPrices.FinalPrice > orderRequirement.Amount,
                //Operator.Equal_Or_Larger => cartModel.OrderPrices.FinalPrice >= orderRequirement.Amount,
                Operator.Larger => cartModel.OrderPrices.OrderPriceExcludeShipAndWarranty > orderRequirement.Amount,
                Operator.Equal_Or_Larger => cartModel.OrderPrices.OrderPriceExcludeShipAndWarranty >= orderRequirement.Amount,

                _ => throw new Exception("Major error, requirement for order have not operator")
            };
            return isValid;
        }
        private static bool HandleProductRequirement(CartModel cartModel, PromoReq requirement, Dictionary<int, CartProduct> scopedRequirementProducts)
        {
            //auto orderby smallest price to check requirement first to give customer best value
            var productList = cartModel.Products.OrderBy(x => x.ReviewPrice.DiscountPrice).ToList();
            //var promotionRequirement = promotion.PromoReqs;
            bool isAnyValid = false;
            for (int i = 0; i < productList.Count; i++)
            {
                //if(requirementProducts.Count >= )
                var product = productList[i];
                bool isApplied = false;
                if (product.IsValid is false)// skip the invalid product, are those products that dont have any model or jewelry assign to them
                    continue;
                if (product.IsHavingPromotion)
                    continue;
                if(scopedRequirementProducts.Count >= requirement.Quantity )// when the requirement quantity is met, then stop, so the other item can 
                {
                    break;
                }
                if (product.Jewelry is not null)
                    isApplied = CheckJewelryIsQualified(product.Jewelry, requirement);

                else if (product.JewelryModel is not null)
                    isApplied = CheckJewerlyModelIsQualified(product.JewelryModel.Id, requirement);

                else if (product.Diamond is not null)
                    isApplied = CheckDiamondIsQualified(product.Diamond, requirement);

                if (isApplied)
                {
                    product.PromotionId = requirement.PromotionId;
                    product.RequirementQualifedId = requirement.Id;
                    var indexOf = cartModel.Products.IndexOf(product);
                    scopedRequirementProducts.Add(indexOf, product);
                    isAnyValid = true;
                }
                else
                {
                    continue;
                }
            }
            return isAnyValid;
        }
        private static void CleanupAfterAppliedFail(CartModel cartModel)
        {
            cartModel.Promotion.ClearPreviousPromotionData();
            cartModel.Products.ForEach(p => p.ClearPreviousPromotion());
            cartModel.OrderPrices.PromotionAmountSaved = 0;
        }
        private static void HandleOrderGift(CartModel cartModel, Gift orderGift)
        {
            if (orderGift.TargetType != TargetType.Order)
                return;
            var orderPriceNow = cartModel.OrderPrices.OrderPriceExcludeShipAndWarranty;
            decimal promotionPriceSavedAmount = 0;
            switch (orderGift.UnitType)
            {
                case UnitType.Percent:
                    promotionPriceSavedAmount = Math.Ceiling((orderPriceNow * orderGift.UnitValue) / (decimal)100);
                    if(orderGift.MaxAmout != null)
                    {
                        if (promotionPriceSavedAmount > orderGift.MaxAmout.Value)
                        {
                            promotionPriceSavedAmount = orderGift.MaxAmout.Value;
                        }
                    }
                    break;
                case UnitType.Fix_Price:
                    promotionPriceSavedAmount = orderGift.UnitValue;
                    break;
                default:
                    throw new Exception("Major error, gift for order have not unit type ");
            }
            decimal correctSavedAmount = MoneyVndRoundUpRules.RoundAmountFromDecimal(promotionPriceSavedAmount);
            if(orderPriceNow - correctSavedAmount <= 0)
            {
                correctSavedAmount = orderPriceNow;
            }
            cartModel.OrderPrices.OrderAmountSaved += correctSavedAmount;//MoneyVndRoundUpRules.RoundAmountFromDecimal(correctSavedAmount);
            //cartModel.OrderPrices.PromotionAmountSaved += MoneyVndRoundUpRules.RoundAmountFromDecimal(promotionPriceSavedAmount);
        }
        /// <summary>
        /// the function handle the interation over product, to check which is gift, which is not
        /// if the product is gift, then set the price of the product to the gift price
        /// if the product is already a gift, and ALSO valid for another gift req, THEN IT WILL ONLYYYYYYYYYYY take the first gift as discount
        /// </summary>
        /// <param name="cartModel"></param>
        /// <param name="gift"></param>
        /// <param name="IsExcludeQualifierProduct"></param>
        /// <param name="giftProducts"></param>
        private static void HandleProductGift(CartModel cartModel, Gift gift,bool IsExcludeQualifierProduct, Dictionary<int, CartProduct> scopedGiftProducts)
        {
            var productList = cartModel.Products.OrderByDescending(x => x.ReviewPrice.DiscountPrice).ToList();
            //var promotionGift = promotion.Gifts;
            var amount = gift.Amount;
            for (int i = 0; i < productList.Count; i++)
            {
                var product = productList[i];
                if (product.IsValid is false)
                    continue;
                if (scopedGiftProducts.Count >= gift.Amount) // if the necessary amount is reach then stop
                    break;
                
                if (IsExcludeQualifierProduct)
                {
                    if (product.IsHavingPromotion && product.IsReqirement)
                        continue;
                }
                if(product.IsGift == true) // if the product you about to check is already a gift previously, then skip || THIS IS THE KEY COMPARISON
                                           // if you want to change the gift later to take on extra gift, then you need to change the logic here
                                           // also change the logic in the List<Gift> of product, so that the products knows which gift is asigned to it
                {
                    continue;
                }
                if (scopedGiftProducts.Count >= amount)
                    break;
                bool isThisGift = false;
                if (product.Jewelry is not null)
                    isThisGift = CheckIfJewelryIsGift(product.Jewelry, gift);
                else if (product.JewelryModel is not null)
                    isThisGift = CheckIfJewelryModelIsGift(product.JewelryModel.ModelCode, gift);
                else if (product.Diamond is not null)
                    isThisGift = CheckIfDiamondIsGift(product.Diamond, gift);
                if (isThisGift)
                {
                    product.PromotionId = gift.PromotionId;
                    product.GiftAssignedId = gift.Id;
                    SetProductPriceFromGift(cartModel,product, gift);
                    var correctIndex = cartModel.Products.IndexOf(product);
                    scopedGiftProducts.Add(correctIndex, product);
                }
                else
                {
                    continue;
                }
            }
            // after the loop if the count still small, mean you are missing some gift
            if (scopedGiftProducts.Count < amount)
            {
                cartModel.Promotion.MissingGifts.Add(new CartModelPromotion.MissingGift
                {
                    GiftId = gift.Id,
                    GiftType = gift.TargetType,
                    TotalQuantity = amount,
                    TakenQuantity = scopedGiftProducts.Count,
                    GiftTakenProductIndex = scopedGiftProducts.Keys.ToList()
                });
            }
            else // else then the loop above is break; from the comparison amount, means you have all the gift assigned right in the cart
            {
                //do nothing
            }
        }
        private static void SetProductPriceFromGift(CartModel cartModel,CartProduct product, Gift giftReq)
        {
            product.PromotionApplyGiftOnCartProduct(giftReq);
            cartModel.OrderPrices.PromotionAmountSaved += product.ReviewPrice.PromotionAmountSaved;  
        }
        public void SetOrderPrice(CartModel cartModel)
        {
            //the flow is, work on default -> discount -> promtoin -> final price
            //we += since we not sure if the previous amount exist
            var productList = cartModel.Products;
            foreach (var item in productList)
            {
                cartModel.OrderPrices.PromotionAmountSaved += item.ReviewPrice.PromotionAmountSaved;
            }
        }
        private static bool CheckJewerlyModelIsQualified(JewelryModelId jewelryModelId, PromoReq requirement)
        {
            if (requirement.TargetType != TargetType.Jewelry_Model)
                return false;
            if (requirement.ModelId == jewelryModelId)
            {
                return true;
            }
            return false;
        }
        private static bool CheckJewelryIsQualified(Jewelry jewelry, PromoReq requirements)
        {
            return CheckJewerlyModelIsQualified(jewelry.ModelId, requirements);
        }
        private static bool CheckDiamondIsQualified(Diamond diamond, PromoReq requirement)
        {

            if (requirement.TargetType != TargetType.Diamond)
                return false;
            bool Check4CResult = false;
            if (requirement.DiamondOrigin == DiamondOrigin.Both)
                Check4CResult =  CheckDiamond4C(diamond, requirement);

            else if (requirement.DiamondOrigin == DiamondOrigin.Natural && diamond.IsLabDiamond == false)
                Check4CResult =CheckDiamond4C(diamond, requirement);

            else if (requirement.DiamondOrigin == DiamondOrigin.Lab && diamond.IsLabDiamond == true)
                Check4CResult = CheckDiamond4C(diamond, requirement);

            else
                return false;
            if (Check4CResult == false)
                return false;
            else
            {
                var shapes = requirement.PromoReqShapes;
                return shapes.Any(s => s.ShapeId == diamond.DiamondShapeId);
            }
        }
        private static bool CheckDiamond4C(Diamond diamond, PromoReq requirement)
        {
            return ValidateDiamond4C(diamond, requirement.CaratFrom.Value, requirement.CaratTo.Value, requirement.ColorFrom.Value, requirement.ColorTo.Value, requirement.ClarityFrom.Value, requirement.ClarityTo.Value, requirement.CutFrom.Value, requirement.CutTo.Value);
        }
        private static bool CheckDiamond4CGift(Diamond diamond, Gift gift)
        {
            return ValidateDiamond4C(diamond, gift.CaratFrom.Value, gift.CaratTo.Value, gift.ColorFrom.Value, gift.ColorTo.Value, gift.ClarityFrom.Value, gift.ClarityTo.Value, gift.CutFrom.Value, gift.CutTo.Value);
        }
        private static bool ValidateDiamond4C(Diamond diamond, float caratFrom, float caratTo, Color colorFrom, Color colorTo, Clarity clarityFrom, Clarity clarityTo, Cut cutFrom, Cut cutTo)
        {
            //if (caratFrom <= diamond.Carat && caratTo >= diamond.Carat)
            //{
            //    if (colorFrom <= diamond.Color && colorTo >= diamond.Color)
            //    {
            //        if (clarityFrom <= diamond.Clarity && clarityTo >= diamond.Clarity)
            //        {
            //            if(diamond.Cut != null)
            //            {
            //                if (cutFrom <= diamond.Cut && cutTo >= diamond.Cut)
            //                {
            //                    return true;
            //                }
            //            }
            //        }
            //    }
            //}
            //return false;
            return DiamondServices.ValidateDiamond4CGlobal(diamond,caratFrom,caratTo,colorFrom,colorTo,clarityFrom, clarityTo, cutFrom, cutTo);
        }
        private static bool CheckIfDiamondIsGift(Diamond diamond, Gift gift)
        {
            bool isQualified = false;
            if (gift.TargetType != TargetType.Diamond)
                return false;
            if (gift.DiamondOrigin == DiamondOrigin.Both)
                isQualified = CheckDiamond4CGift(diamond, gift);

            else if (gift.DiamondOrigin == DiamondOrigin.Natural && diamond.IsLabDiamond == false)
                isQualified = CheckDiamond4CGift(diamond, gift);

            else if (gift.DiamondOrigin == DiamondOrigin.Lab && diamond.IsLabDiamond == true)
                isQualified = CheckDiamond4CGift(diamond, gift);
            else
                return false;
            if (isQualified == false)
                return false;
            else
            {
                var shapes = gift.DiamondGiftShapes;
                return shapes.Any(s => s == diamond.DiamondShapeId);
            }
        }
        private static bool CheckIfJewelryIsGift(Jewelry jewelry, Gift gift)
        {
            return CheckIfJewelryModelIsGift(jewelry.Model.ModelCode, gift);
        }
        private static bool CheckIfJewelryModelIsGift(string code, Gift gift)
        {
            if (gift.TargetType != TargetType.Jewelry_Model)
                return false;
            if (gift.ItemCode == code)           
                return true;

            return false;
        }

        public void ApplyPromotionOnDiamond(Diamond diamond, List<Promotion> activePromotion)
        {
            var getPromoHaveDiamondAsGift = activePromotion.Where(p => p.Gifts.Any(g => g.TargetType == TargetType.Diamond)).ToList();
            foreach(var promo in getPromoHaveDiamondAsGift)
            {
                var diamondGifts = promo.Gifts.Where(g => g.TargetType == TargetType.Diamond).ToList();
                decimal savedAmount = 0;
                foreach(var gift in diamondGifts)
                {
                    if(CheckIfDiamondIsGift(diamond, gift))
                    {
                        var currentSaveAmount= gift.UnitType switch
                        {
                            UnitType.Percent => Math.Ceiling((diamond.TruePrice * gift.UnitValue) / 100),
                            UnitType.Fix_Price => gift.UnitValue,
                            _ => throw new Exception("Major error, gift for product have not unit type ")
                        };
                        if(currentSaveAmount > savedAmount)
                            savedAmount = currentSaveAmount;
                    }
                }
                if(savedAmount > 0)
                {
                    if (diamond.PromotionReducedAmount == 0)
                        diamond.AssignPromotion(promo, savedAmount);
                    else
                        if(savedAmount > diamond.PromotionReducedAmount)
                            diamond.AssignPromotion(promo, savedAmount);
                }
            }
        }

        public void ApplyPromotionOnJewerly(Jewelry jewelry, List<Promotion> activePromotion)
        {
            var getPromoHaveJewelryAsGift = activePromotion.Where(p => p.Gifts.Any(g => g.TargetType == TargetType.Jewelry_Model)).ToList();
            foreach (var promo in getPromoHaveJewelryAsGift)
            {
                var jewelryGifts = promo.Gifts.Where(g => g.TargetType == TargetType.Jewelry_Model).ToList();
                decimal savedAmount = 0;
                foreach (var gift in jewelryGifts)
                {
                    if (CheckIfJewelryIsGift(jewelry, gift))
                    {
                        var currentSaveAmount = gift.UnitType switch
                        {
                            UnitType.Percent => Math.Ceiling((jewelry.TotalPrice * gift.UnitValue) / 100),
                            UnitType.Fix_Price => gift.UnitValue,
                            _ => throw new Exception("Major error, gift for product have not unit type ")
                        };
                        if (currentSaveAmount > savedAmount)
                            savedAmount = currentSaveAmount;
                    }
                }
                if (savedAmount > 0)
                {
                    if (jewelry.PromotionReducedAmount == 0)
                        jewelry.AssignJewelryPromotion(promo, savedAmount); 
                    else
                        if (savedAmount > jewelry.PromotionReducedAmount)
                            jewelry.AssignJewelryPromotion(promo, savedAmount);
                }
            }
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
    public partial class PromotionService : IPromotionServices
    {
        public static Result IsCartMeetPromotionRequirent(CartModel clonedCartModel,Promotion promotion,PromotionRule promotionRule)
        {
            var result = ApplyPromotionOnCartModelGlobal(clonedCartModel,promotion,promotionRule);
            if (result.IsSuccess)
                return result;
            return Result.Fail(result.Errors);
        }
        public static Result CheckIfUserHasAlreadyUsedThisPromotionToLimit(List<Order> userOrders , Promotion promotionToCheck, PromotionRule promotionRule)
        {
            var orderThatUsedThisPromotion = promotionToCheck.OrderThatTruelyUsedThisPromotion(userOrders);
            //TODO:  LOGIC HERE
            //if (orderThatUsedThisPromotion.Count >= promotionToCheck)
            //    return Result.Fail("this promotion is already used by the user and reach its limit");
            switch (promotionToCheck.RedemptionMode)
            {
                case RedemptionMode.Single:
                    if (orderThatUsedThisPromotion.Count >= 1)
                        return Result.Fail(PromotionError.RedemptionLimitReached(1));
                    break;
                case RedemptionMode.Multiple:
                    if (orderThatUsedThisPromotion.Count >= int.MaxValue)
                        return Result.Fail(PromotionError.RedemptionLimitReached(int.MaxValue));
                    break;
                default:
                    throw new Exception("Major error, redemption mode is not set");
            }
            return Result.Ok();
        }
    }
}
