using DiamondShop.Domain.Common;
using DiamondShop.Domain.Models.Diamonds.Enums;
using DiamondShop.Domain.Models.DiamondShapes;
using DiamondShop.Domain.Models.DiamondShapes.ValueObjects;
using DiamondShop.Domain.Models.JewelryModels;
using DiamondShop.Domain.Models.JewelryModels.ValueObjects;
using DiamondShop.Domain.Models.Promotions.Enum;
using DiamondShop.Domain.Models.Promotions.ValueObjects;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Domain.Models.Promotions.Entities
{
    public class PromoReq : Entity<PromoReqId>
    {
        public PromotionId? PromotionId { get; set; }
        public Promotion? Promotion { get; set; }
        public DiscountId? DiscountId { get; set; }
        public Discount? Discount { get; set; }
        public string Name { get; set; }
        public TargetType TargetType { get; set; }
        public Operator Operator { get; set; }
        public decimal? Amount { get; set; }
        public int? Quantity { get; set; }
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
        public static PromoReq CreateJewelryRequirement(string Name, Operator @operator, bool isForAmount, decimal? amount, int? quantity, JewelryModelId jewelryModelId)
        {
            if (isForAmount)
            {
                if (amount <= 1000)
                    throw new Exception("yêu cầu trang sức có yêu cầu dưới 1000");
            }
            else
                if (quantity <= 0)
                throw new Exception("yêu cầu trang sức phải lớn hơn 0");
            return new PromoReq()
            {
                Id = PromoReqId.Create(),
                Name = Name,
                TargetType = TargetType.Jewelry_Model,
                Operator = @operator,
                Amount = isForAmount == true ? amount : null,
                Quantity = isForAmount == false ? quantity : null,
                ModelId = jewelryModelId
            };
        }
        public static PromoReq CreateDiamondRequirement(string Name, Operator @operator, bool isForAmount, decimal? amount, int? quantity,
            DiamondOrigin? diamondOrigin,
            float? caratFrom,
            float? caratTo,
            Clarity? clarityFrom,
            Clarity? clarityTo,
            Cut? cutFrom,
            Cut? cutTo,
            Color? colorFrom,
            Color? colorTo,
            List<DiamondShape> selectedDiamondShapes)
        {
            if (isForAmount)
            {
                if (amount <= 1000)
                    throw new Exception("yêu cầu kim cương có yêu cầu dưới 1000");
            }
            else
                if (quantity <= 0)
                    throw new Exception("yêu cầu kim cương phải lớn hơn 0");
            var Id = PromoReqId.Create();
            var newPromo = new PromoReq()
            {
                Id = Id,
                Name = Name,
                TargetType = TargetType.Diamond,
                Operator = @operator,
                Amount = isForAmount == true ? amount : null,
                Quantity = isForAmount == false ? quantity : null,
                ModelId = null,
                DiamondOrigin = diamondOrigin,
                CaratFrom = caratFrom,
                CaratTo = caratTo,
                ClarityFrom = clarityFrom,
                ClarityTo = clarityTo,
                ColorFrom = colorFrom,
                ColorTo = colorTo,
                CutFrom = cutFrom,
                CutTo = cutTo,
                PromoReqShapes = selectedDiamondShapes.Select(s => PromoReqShape.Create(Id, s.Id)).ToList(),
            };
            return newPromo;
        }
        public static PromoReq CreateOrderRequirement(string Name, Operator @operator, decimal amount)
        {
            if (amount <= 1000)
                throw new Exception("yêu cầu đơn hàng có yêu cầu dưới 1000");

            return new PromoReq()
            {
                Id = PromoReqId.Create(),
                Name = Name,
                TargetType = TargetType.Order,
                Operator = @operator,
                Amount = amount,
            };
        }
        public void SetPromotion(PromotionId promotionId)
        {
            PromotionId = promotionId;
        }
        public void SetDiscount(DiscountId discountId)
        {
            DiscountId = discountId;
        }

        public PromoReq() { }
    }
}
