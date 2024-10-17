using DiamondShop.Domain.Common.Carts;
using DiamondShop.Domain.Models.Diamonds;
using DiamondShop.Domain.Models.JewelryModels.ValueObjects;
using DiamondShop.Domain.Models.Promotions.Entities;
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
            var requirements = discount.DiscountReq;
            bool isAnyProductHaveDiscount = false;
            for (int i = 0; i < requirements.Count; i++)
            {
                var requirement = requirements[i];
                for (int j = 0; j < cartModel.Products.Count; j++)
                {
                    var product = cartModel.Products[j];
                    if (product.IsValid is false)
                        continue;
                    if (product.IsHavingDiscount)
                    {
                        // this is when the product already have a discount and it is higher than the current discount
                        if (discount.DiscountPercent < product.DiscountPercent)
                        {
                            continue;
                        }
                    }
                    if (CheckIfProductMeetRequirement(product, requirement))
                    {
                        SetProductDiscountPrice(product, discount);
                        isAnyProductHaveDiscount = true;
                    }
                }
            }
            if (isAnyProductHaveDiscount)
            {
                cartModel.DiscountsApplied.Add(discount);
                //SetOrderPrice(cartModel);
                return Result.Ok();
            }
            else
            {
                return Result.Fail("No product meet the requirement, skip to the next discount");
            }
        }
        private void SetProductDiscountPrice(CartProduct product, Discount discount)
        {
            product.DiscountPercent = discount.DiscountPercent;
            product.DiscountId = discount.Id;
            product.ReviewPrice.DiscountAmountSaved = Math.Ceiling((product.ReviewPrice.DefaultPrice * discount.DiscountPercent) / 100);

        }
        private bool CheckIfProductMeetRequirement(CartProduct product, PromoReq requirement)
        {
            switch (requirement.TargetType)
            {
                case TargetType.Jewelry_Model:
                    if (product.Jewelry is not null)
                        return CheckIfJewelryModelMeetRequirement(product.Jewelry.ModelId, requirement);
                    return CheckIfJewelryModelMeetRequirement(product.JewelryModel.Id, requirement);
                case TargetType.Diamond:
                    if (product.Diamond is not null)
                    {
                        if (requirement.DiamondOrigin == DiamondOrigin.Both)
                            return _diamondServices.ValidateDiamond4C(product.Diamond, requirement.CaratFrom.Value, requirement.CaratTo.Value, requirement.ColorFrom.Value, requirement.ColorTo.Value, requirement.ClarityFrom.Value, requirement.ClarityTo.Value, requirement.CutFrom.Value, requirement.CutTo.Value);
                        else if (requirement.DiamondOrigin == DiamondOrigin.Lab)
                        {
                            if (product.Diamond.IsLabDiamond is false)
                                return false;
                            return _diamondServices.ValidateDiamond4C(product.Diamond, requirement.CaratFrom.Value, requirement.CaratTo.Value, requirement.ColorFrom.Value, requirement.ColorTo.Value, requirement.ClarityFrom.Value, requirement.ClarityTo.Value, requirement.CutFrom.Value, requirement.CutTo.Value);
                        }
                        else if (requirement.DiamondOrigin == DiamondOrigin.Natural)
                        {
                            if (product.Diamond.IsLabDiamond)
                                return false;
                            return _diamondServices.ValidateDiamond4C(product.Diamond, requirement.CaratFrom.Value, requirement.CaratTo.Value, requirement.ColorFrom.Value, requirement.ColorTo.Value, requirement.ClarityFrom.Value, requirement.ClarityTo.Value, requirement.CutFrom.Value, requirement.CutTo.Value);
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
                cartModel.OrderPrices.DiscountAmountSaved += item.ReviewPrice.DiscountAmountSaved;
            }
        }
    }
}
