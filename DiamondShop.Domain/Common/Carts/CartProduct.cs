using DiamondShop.Domain.BusinessRules;
using DiamondShop.Domain.Models.Diamonds;
using DiamondShop.Domain.Models.Jewelries;
using DiamondShop.Domain.Models.JewelryModels;
using DiamondShop.Domain.Models.Promotions.Entities;
using DiamondShop.Domain.Models.Promotions.Enum;
using DiamondShop.Domain.Models.Promotions.ValueObjects;
using DiamondShop.Domain.Models.Warranties;
using Microsoft.Extensions.FileSystemGlobbing.Internal.PathSegments;
using System.Text.Json.Serialization;

namespace DiamondShop.Domain.Common.Carts
{
    public class CartProduct
    {
        public string CartProductId { get; set; } = DateTime.UtcNow.Ticks.ToString();
        public Jewelry? Jewelry { get; set; }
        public Diamond? Diamond { get; set; }
        public JewelryModel? JewelryModel { get; set; }
        public CheckoutPrice ReviewPrice { get; set; } = new();
        public Warranty CurrentWarrantyApplied { get; set; }
        public decimal CurrentWarrantyPrice { get; set; } 
        public string? EngravedText { get; set; }
        public string? EngravedFont { get; set; }
        public bool IsValid { get; set; } = true;//(Jewelry != null || Diamond != null || JewelryModel != null); set =>  }
        private string? _errorMessage = null;
        public string? ErrorMessage { get => GetErrorMessage(); set => _errorMessage = value; }
        public bool IsAvailable { get; set; } = true; // always true unless when check the product is sold or inactive
        public bool IsDuplicate { get; set; } = false; // always false, unless check the item is duplicate
        public bool IsProduct { get; set; } = false; // always false, unless check the item is a product
        public DiscountId? DiscountId { get; set; }
        public int? DiscountPercent { get; set; }
        public decimal? DiscountAmountSaved { get; set; }
        [JsonIgnore]
        public PromotionId? PromoDiscountId{ get; set; }
        [JsonIgnore]
        public decimal? PromoDiscountAmountSaved { get; set; }
        [JsonIgnore]
        public bool IsHavingPromoDiscount { get => PromoDiscountId is not null; }
        public bool IsHavingDiscount { get => DiscountId is not null; }
        
        public PromotionId? PromotionId { get; set; }
        public bool IsHavingPromotion { get => PromotionId is not null; }
        public PromoReqId? RequirementQualifedId{ get; set; }
        public bool IsReqirement { get => RequirementQualifedId is not null; }
        public GiftId? GiftAssignedId { get; set; }
        public bool IsGift { get => GiftAssignedId is not null; }
        private string? GetErrorMessage()
        {
            if(IsValid is false)
            {
                if(_errorMessage is null)
                {
                    if (IsProduct is false)
                        return "The parent item is invalid, so does the child item";
                    else if (IsDuplicate is true)
                        return "Product is duplicated, check cart to remove the same product";
                    else if (IsAvailable is false)
                        return "Product is not available";
                    else
                        return "Product is Invalid state";
                }
                else 
                    return _errorMessage;
            }
            else
            {
                return  null;
            }
        }
        public bool IsHavingBothDiamondAndJewelryModel()
        {
            return Diamond != null &&  JewelryModel != null;
        }
        public void ClearPreviousPromotion()
        {
            PromotionId = null;
            GiftAssignedId = null;
            RequirementQualifedId = null;
            ReviewPrice.PromotionAmountSaved = 0;
        }
        public bool IsDiamond()
        {
            if(Jewelry is null && JewelryModel is null && Diamond is not null)
                return true;
            return false;
        }
        public bool IsJewelry()
        {
            if (Jewelry is not null && Diamond is null)
                return true;
            return false;
        }
        public void PromotionApplyGiftOnCartProduct (Gift giftReq)
        {
            decimal savedAmount = GetAmountSavedFromGift( giftReq);
            if ((this.ReviewPrice.DiscountPrice - savedAmount) <= 0)
                savedAmount = this.ReviewPrice.DiscountPrice;
            this.ReviewPrice.PromotionAmountSaved = MoneyVndRoundUpRules.RoundAmountFromDecimal(savedAmount);
        }
        public void DiscountApplyGiftOnCartProduct(Gift giftReq)
        {
            decimal savedAmount = GetAmountSavedFromGift( giftReq);
            if ((this.ReviewPrice.DefaultPrice - savedAmount) <= 0)
                savedAmount = this.ReviewPrice.DefaultPrice;
            this.ReviewPrice.DiscountAmountSaved = MoneyVndRoundUpRules.RoundAmountFromDecimal(savedAmount);
        }
        public decimal GetAmountSavedFromGift(Gift giftReq)
        {
            decimal savedAmount = 0;
            switch (giftReq.UnitType)
            {
                case UnitType.Percent:
                    savedAmount = Math.Ceiling(this.ReviewPrice.DiscountPrice * (giftReq.UnitValue / 100m));
                    //savedAmount = Math.Ceiling(this.ReviewPrice.DefaultPrice * (giftReq.UnitValue / 100m));
                    if (giftReq.MaxAmout != null)
                    {
                        if (savedAmount > giftReq.MaxAmout.Value)
                        {
                            savedAmount = giftReq.MaxAmout.Value;
                        }
                    }
                    break;
                case UnitType.Fix_Price:
                    savedAmount = giftReq.UnitValue;
                    break;
                default:
                    throw new Exception("Major error, gift for product have not unit type ");
            }
            var trueSavedAmount = MoneyVndRoundUpRules.RoundAmountFromDecimal(savedAmount);//Math.Clamp(product.ReviewPrice.DiscountPrice - savedAmount,0,decimal.MaxValue);
            return trueSavedAmount;
        }
    }

}
