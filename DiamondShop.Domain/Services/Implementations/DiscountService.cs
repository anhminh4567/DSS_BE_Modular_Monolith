using DiamondShop.Domain.BusinessRules;
using DiamondShop.Domain.Common.Carts;
using DiamondShop.Domain.Models.Diamonds;
using DiamondShop.Domain.Models.JewelryModels.ValueObjects;
using DiamondShop.Domain.Models.Promotions.Entities;
using DiamondShop.Domain.Models.Promotions.Entities.ErrorMessages;
using DiamondShop.Domain.Models.Promotions.Enum;
using DiamondShop.Domain.Services.interfaces;
using FluentResults;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Domain.Services.Implementations
{
    public class DiscountService : IDiscountService
    {
        private readonly IDiamondServices _diamondServices;

        public DiscountService(IDiamondServices diamondServices)
        {
            _diamondServices = diamondServices;
        }

        /// <summary>
        /// REMEMBER, when the discount reach here, it is assumed to be correct, whether is active or out of date,
        /// the validatoin lies on the application layer
        /// </summary>
        /// <param name="cartModel"></param>
        /// <param name="discount"></param>
        /// <returns></returns>
        public Result ApplyDiscountOnCartModel(CartModel cartModel, Discount discount)
        {
            if (discount.Status != Status.Active)
            {
                return Result.Fail(DiscountErrors.ApplyingErrors.NotActiveToUse);
            }
            var requirements = discount.DiscountReq;
            bool isAnyProductHaveDiscount = false;
            for (int i = 0; i < requirements.Count; i++)
            {
                var requirement = requirements[i];
                for (int j = 0; j < cartModel.Products.Count; j++)
                {
                    var product = cartModel.Products[j];
                    var checkIfProductInDiscount = ApplyDiscountOnCartProduct(product, discount);
                    if (checkIfProductInDiscount.IsSuccess)
                        isAnyProductHaveDiscount = true;
                    if (product.IsValid is false)
                        continue;
                }
            }
            if (isAnyProductHaveDiscount)
            {
                if(cartModel.DiscountsApplied.Contains(discount) is false)// add if not in list yet
                    cartModel.DiscountsApplied.Add(discount);
                SetOrderPrice(cartModel);
                return Result.Ok();
            }
            else
            {
                return Result.Fail(DiscountErrors.ApplyingErrors.NotMeetRequirement);
            }
        }
        private void SetProductDiscountPrice(CartProduct product, Discount discount, Gift appliedGift =null)
        {
            product.DiscountPercent = discount.DiscountPercent;
            product.DiscountId = discount.Id;
            var savedAmount = Math.Ceiling((product.ReviewPrice.DefaultPrice * discount.DiscountPercent) / 100);
            product.ReviewPrice.DiscountAmountSaved = MoneyVndRoundUpRules.RoundAmountFromDecimal(savedAmount);
            //for gift 
            var gift = appliedGift;
            if (gift == null)
                return;
            product.DiscountApplyGiftOnCartProduct(gift);

        }
        private bool CheckIfProductMeetRequirementForGift(CartProduct product, Gift gift)
        {
            var fakeRequirement = new PromoReq()
            {
                TargetType = gift.TargetType,
                ModelId = gift.TargetType == TargetType.Jewelry_Model ? JewelryModelId.Parse(gift.ItemId) : null,
                CaratFrom = gift.CaratFrom,
                CaratTo = gift.CaratTo,
                ColorFrom = gift.ColorFrom,
                ColorTo = gift.ColorTo,
                ClarityFrom = gift.ClarityFrom,
                ClarityTo = gift.ClarityTo,
                CutFrom = gift.CutFrom,
                CutTo = gift.CutTo,
                DiamondOrigin = gift.DiamondOrigin
            };
            return CheckIfProductMeetRequirement(product, fakeRequirement);
        }
        private bool CheckIfProductMeetRequirement(CartProduct product, PromoReq requirement)
        {
            switch (requirement.TargetType)
            {
                case TargetType.Jewelry_Model:
                    if (product.Diamond is not null)
                        return false;
                    else if (product.Jewelry is not null)
                        return CheckIfJewelryModelMeetRequirement(product.Jewelry.ModelId, requirement);
                    else if (product.JewelryModel is not null)
                        return CheckIfJewelryModelMeetRequirement(product.JewelryModel.Id, requirement);
                    else
                        return false;
                case TargetType.Diamond:
                    if (product.Diamond is not null)
                    {
                        if (requirement.DiamondOrigin == DiamondOrigin.Both)
                            return DiamondServices.ValidateDiamond4CGlobal(product.Diamond, requirement.CaratFrom.Value, requirement.CaratTo.Value, requirement.ColorFrom.Value, requirement.ColorTo.Value, requirement.ClarityFrom.Value, requirement.ClarityTo.Value, requirement.CutFrom.Value, requirement.CutTo.Value);
                        else if (requirement.DiamondOrigin == DiamondOrigin.Lab)
                        {
                            if (product.Diamond.IsLabDiamond is false)
                                return false;
                            return DiamondServices.ValidateDiamond4CGlobal(product.Diamond, requirement.CaratFrom.Value, requirement.CaratTo.Value, requirement.ColorFrom.Value, requirement.ColorTo.Value, requirement.ClarityFrom.Value, requirement.ClarityTo.Value, requirement.CutFrom.Value, requirement.CutTo.Value);
                        }
                        else if (requirement.DiamondOrigin == DiamondOrigin.Natural)
                        {
                            if (product.Diamond.IsLabDiamond)
                                return false;
                            return DiamondServices.ValidateDiamond4CGlobal(product.Diamond, requirement.CaratFrom.Value, requirement.CaratTo.Value, requirement.ColorFrom.Value, requirement.ColorTo.Value, requirement.ClarityFrom.Value, requirement.ClarityTo.Value, requirement.CutFrom.Value, requirement.CutTo.Value);
                        }
                    }
                    return false;
                case TargetType.Order:
                    // right now we dont do discount on order, the discount on order is for the promotion partr, not discount
                    // discount is on product only 
                    return false;
                default:
                    return false;
            }
        }
        private bool CheckIfJewelryModelMeetRequirement(JewelryModelId jewelryModelId, PromoReq requirement)
        {
            if (requirement.TargetType != TargetType.Jewelry_Model)
                return false;
            if (requirement.ModelId == jewelryModelId)
            {
                return true;
            }
            return false;
        }

        public void SetOrderPrice(CartModel cartModel)
        {
            var productList = cartModel.Products;
            foreach (var item in productList)
            {
                if (item.IsValid)
                {
                    cartModel.OrderPrices.DiscountAmountSaved += item.ReviewPrice.DiscountAmountSaved;
                    // promotion amount saved is set by the prmotion service, since only 1 promotion is applied at a time
                    // and the promotion might include orderPromotion, which again, might affect the final price, 
                    // so the promotion amount saved is set here will be WRONG
                    //orderPrice.PromotionAmountSaved += product.ReviewPrice.PromotionAmountSaved;
                }
                //cartModel.OrderPrices.DiscountAmountSaved += item.ReviewPrice.DiscountAmountSaved;
            }
        }

        public Result ApplyDiscountOnCartProduct(CartProduct cartProduct, Discount discount)
        {
            if (discount.Status != Status.Active)
                return Result.Fail(DiscountErrors.ApplyingErrors.NotActiveToUse);
            
            var requirements = discount.DiscountReq;
            var gifts = discount.DiscountGift;
            bool isAnyProductHaveDiscount = false;
            for (int i = 0; i < requirements.Count; i++)
            {
                var requirement = requirements[i];
                if (cartProduct.IsValid is false)
                    continue;
                if (cartProduct.IsHavingDiscount)
                {
                    // this is when the product already have a discount and it is higher than the current discount
                    if (discount.DiscountPercent < cartProduct.DiscountPercent)
                    {
                        continue;
                    }
                }
                if (CheckIfProductMeetRequirement(cartProduct, requirement))
                {
                    SetProductDiscountPrice(cartProduct, discount);
                    isAnyProductHaveDiscount = true;
                    break;
                }
            }
            for (int i = 0; i < gifts.Count; i++)
            {
                var gift = gifts[i];
                if (cartProduct.IsValid is false)
                    continue;
                if (cartProduct.IsHavingDiscount)
                {
                    // this is when the product already have a discount and it is higher than the current discount
                    var amount = cartProduct.GetAmountSavedFromGift(gift);
                    if (amount <= cartProduct.DiscountAmountSaved) ;
                    {
                        continue;
                    }
                }
                if (CheckIfProductMeetRequirementForGift(cartProduct, gift))
                {
                    SetProductDiscountPrice(cartProduct, discount);
                    isAnyProductHaveDiscount = true;
                    break;
                }
            }
            if (isAnyProductHaveDiscount)
                return Result.Ok();
            
            else
                return Result.Fail(DiscountErrors.ApplyingErrors.NotMeetRequirement);
            
        }
    }
}
