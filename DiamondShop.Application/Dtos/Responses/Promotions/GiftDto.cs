using DiamondShop.Application.Dtos.Responses.JewelryModels;
using DiamondShop.Domain.Models.Diamonds.Enums;
using DiamondShop.Domain.Models.Promotions.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Application.Dtos.Responses.Promotions
{
    public class GiftDto
    {
        public string Id { get; set; }
        public string PromotionId { get; set; }
        public string Name { get; set; }
        public TargetType TargetType { get; set; }
        public string? ItemCode { get; set; }
        public UnitType UnitType { get; set; }
        public decimal UnitValue { get; set; }
        public int Amount { get; set; }
        //diamond config
        //public List<string>? DiamondGiftShapes { get; set; } = new();
        //public DiamondOrigin? DiamondOrigin { get; set; }
        //public float? CaratFrom { get; set; }
        //public float? CaratTo { get; set; }
        //public Clarity? ClarityFrom { get; set; }
        //public Clarity? ClarityTo { get; set; }
        //public Cut? CutFrom { get; set; }
        //public Cut? CutTo { get; set; }
        //public Color? ColorFrom { get; set; }
        //public Color? ColorTo { get; set; }
        public decimal? MaxAmout { get; set; }
        public DiamondSpecDto? DiamondRequirementSpec { get; set; }
        public JewelryModelDto? GiftedModel { get; set; }
        public string Gift
        {
            get
            {
                string baseMessage = string.Empty;
                return string.Empty;
            }
        }
    }
}
