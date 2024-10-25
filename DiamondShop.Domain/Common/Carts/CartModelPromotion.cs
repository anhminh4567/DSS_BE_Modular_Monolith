using DiamondShop.Domain.Models.Promotions;
using DiamondShop.Domain.Models.Promotions.Enum;
using DiamondShop.Domain.Models.Diamonds.Enums;
using DiamondShop.Domain.Models.DiamondShapes.ValueObjects;
using DiamondShop.Domain.Models.Promotions.Entities;
using DiamondShop.Domain.Models.Promotions.ValueObjects;

namespace DiamondShop.Domain.Common.Carts
{
    public class CartModelPromotion 
    {
        public Promotion? Promotion { get; set; } 
        public bool IsHavingPromotion { get => Promotion is not null;  }
        public List<int> RequirementProductsIndex { get; set; } = new();
        public List<int> GiftProductsIndex { get; set; } = new();
        // the missing gift only happen if the promotion is for type product, and not order as gift
        public List<MissingGift> MissingGifts { get; set; } = new();
        public List<PromoReq>? MissingRequirement { get; set; } = new();
        public bool IsPromotionValidTobeUsed { get => MissingRequirement == null || MissingRequirement.Count == 0; } 
        public void ClearPreviousPromotionData()
        {
            Promotion = null;
            RequirementProductsIndex = new();
            GiftProductsIndex = new();
            MissingRequirement = new();
            MissingGifts = new();
        }
        public class MissingGift
        {
            public TargetType GiftType { get; set; } 
            public int TotalQuantity { get; set; }
            public int TakenQuantity { get; set; }
            public int MissingQuantity { get => TotalQuantity - TakenQuantity; }
            public List<int> GiftTakenProductIndex { get; set; } = new();
            public GiftId? GiftId { get; set; }
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
