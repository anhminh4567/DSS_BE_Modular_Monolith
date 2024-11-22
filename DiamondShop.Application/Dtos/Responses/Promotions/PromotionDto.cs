using DiamondShop.Domain.Common.ValueObjects;
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
        public string PromoCode { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public bool IsActive { get => Status == Status.Active; }
        public int Priority { get; set; }
        public Status Status { get; set; }
        public bool IsExcludeQualifierProduct { get; set; }
        public RedemptionMode RedemptionMode { get; set; }
        public List<RequirementDto> PromoReqs { get; set; } = new();
        public List<GiftDto> Gifts { get; set; } = new();
        public MediaDto? Thumbnail { get; set; }
    }
}
