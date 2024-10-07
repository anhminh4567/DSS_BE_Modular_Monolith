using DiamondShop.Domain.Models.Promotions;
using DiamondShop.Domain.Models.Promotions.Enum;
using DiamondShop.Domain.Models.Diamonds.Enums;
using DiamondShop.Domain.Models.DiamondShapes.ValueObjects;
using DiamondShop.Domain.Models.Promotions.Entities;

namespace DiamondShop.Domain.Common.Carts
{
    public class CartModelPromotion 
    {
        public Promotion? Promotion { get; set; } 
        public bool IsHavingPromotion { get => Promotion is not null;  }
        public List<int> RequirementProductsIndex { get; set; } = new();
        public List<int> GiftProductsIndex { get; set; } = new();
        public List<MissingGift> MissingGifts { get; set; } = new();
        public PromoReq MissingRequirement { get; set; }
        public bool IsPromotionValidTobeUsed { get => MissingRequirement != null; } 
        public class MissingGift
        {
            public TargetType GiftType { get; set; }
            public int MissingQuantity { get; set; }
            public string? GiftId { get; set; }
            public MissingDiamondGift? DiamondGifts { get; set; }

        }
        public class MissingDiamondGift
        {
            public List<DiamondShapeId>? DiamondGiftShapes { get; set; } = new();
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
