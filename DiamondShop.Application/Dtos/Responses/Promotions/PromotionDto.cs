using DiamondShop.Domain.Models.Promotions.Entities;
using DiamondShop.Domain.Models.Promotions.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Application.Dtos.Responses.Promotions
{
    public class PromotionDto
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool IsActive { get; set; }
        public int Priority { get; set; }
        public bool IsExcludeQualifierProduct { get; set; }
        public RedemptionMode RedemptionMode { get; set; }
        public List<RequirementDto> PromoReqs { get; set; } = new();
        public List<GiftDto> Gifts { get; set; } = new();
    }
}
