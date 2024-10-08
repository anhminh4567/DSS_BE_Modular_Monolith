using DiamondShop.Application.Dtos.Responses.Promotions;
using DiamondShop.Domain.Models.Diamonds.Enums;
using DiamondShop.Domain.Models.Promotions.Enum;

namespace DiamondShop.Application.Dtos.Responses.Carts
{
    public class CartModelPromotionDto
    {
        public PromotionDto? Promotion { get; set; }
        public bool IsHavingPromotion { get; set; }
        public List<int> RequirementProductsIndex { get; set; }
        public List<int> GiftProductsIndex { get; set; }
        public List<MissingGiftDto> MissingGifts { get; set; }
        public RequirementDto? MissingRequirement { get; set; }
        public bool IsPromotionValidTobeUsed { get; set; }

        public class MissingGiftDto
        {
            public TargetType GiftType { get; set; }
            public int TotalQuantity { get; set; }
            public int TakenQuantity { get; set; }
            public int MissingQuantity { get; set; }
            public List<int> GiftTakenProductIndex { get; set; }
            public string? GiftId { get; set; }
            public MissingDiamondGiftDto? DiamondGifts { get; set; }
        }

        public class MissingDiamondGiftDto
        {
            public List<string>? DiamondGiftShapes { get; set; }
            public DiamondOrigin? DiamondOrigin { get; set; }
            public float? CaratFrom { get; set; }
            public float? CaratTo { get; set; }
            public Clarity? ClarityFrom { get; set; }
            public Clarity? ClarityTo { get; set; }
            public Cut? CutFrom { get; set; }
            public Cut? CutTo { get; set; }
            public Color? ColorFrom { get; set; }
            public Color? ColorTo { get; set; }
        }
    }
}
