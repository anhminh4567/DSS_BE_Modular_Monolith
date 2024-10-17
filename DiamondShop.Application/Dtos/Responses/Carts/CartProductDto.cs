using DiamondShop.Application.Dtos.Responses.Diamonds;
using DiamondShop.Application.Dtos.Responses.Jewelries;
using DiamondShop.Application.Dtos.Responses.JewelryModels;

namespace DiamondShop.Application.Dtos.Responses.Carts
{
    public class CartProductDto
    {
        public string CartProductId { get; set; }
        public JewelryDto? Jewelry { get; set; }
        public DiamondDto? Diamond { get; set; }
        public JewelryModelDto? JewelryModel { get; set; }
        public CheckoutPriceDto ReviewPrice { get; set; } = new();
        public string? EngravedText { get; set; }
        public string? EngravedFont { get; set; }
        //public decimal? PurchasedPrice { get; set; }
        public bool IsValid { get; set; }
        public string? ErrorMessage { get; set; }
        public bool IsAvailable { get; set; } 
        public bool IsProduct { get; set; } 
        /////////////////////////////////
        /////////////////////////////////
        public string? DiscountId { get; set; }
        public int? DiscountPercent { get; set; }
        public bool IsHavingDiscount { get => DiscountId is not null; }
        public string? PromotionId { get; set; }
        public bool IsHavingPromotion { get => PromotionId is not null; }
        public string? RequirementQualifedId { get; set; }
        public bool IsReqirement { get => RequirementQualifedId is not null; }
        public string? GiftAssignedId { get; set; }
        public bool IsGift { get => GiftAssignedId is not null; }
    }
}
