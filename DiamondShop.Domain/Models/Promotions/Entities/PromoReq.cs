using DiamondShop.Domain.Common;
using DiamondShop.Domain.Models.Diamonds.Enums;
using DiamondShop.Domain.Models.JewelryModels;
using DiamondShop.Domain.Models.JewelryModels.ValueObjects;
using DiamondShop.Domain.Models.Promotions.Enum;
using DiamondShop.Domain.Models.Promotions.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Domain.Models.Promotions.Entities
{
    public class PromoReq : Entity<PromoReqId>
    {
        public PromotionId PromotionId { get; set; }
        public Promotion Promotion { get; set; }
        public DiscountId DiscountId { get; set; }
        public Discount Discount { get; set; }
        public string Name { get; set; }
        public TargetType TargetType { get; set; }
        public Operator Operator { get; set; }
        public decimal Amount { get; set; }
        public int Quantity { get; set; }
        public JewelryModelId? ModelId { get; set; }
        public JewelryModel? Model { get; set; }
        public DiamondOrigin? DiamondOrigin { get; set; }
        public float? CaratFrom { get; set; }
        public float? CaratTo { get; set; }
        public Clarity? ClarityFrom { get; set; }
        public Clarity? ClarityTo { get; set; }
        public Cut? CutFrom { get; set; }
        public Cut? CutTo { get; set; }
        public Color? ColorFrom { get; set; }
        public Color? ColorTo { get; set; }
        public List<PromoReqShape> PromoReqShapes { get; set; } = new();
        public PromoReq() { }
    }
}
