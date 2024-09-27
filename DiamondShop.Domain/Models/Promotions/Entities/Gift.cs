using DiamondShop.Domain.Common;
using DiamondShop.Domain.Models.Promotions.Enum;
using DiamondShop.Domain.Models.Promotions.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Domain.Models.Promotions.Entities
{
    public class Gift : Entity<GiftId>
    {
        public PromotionId? PromotionId { get; set; }
        public Promotion? Promotion { get; set; }
        public string Name { get; set; }
        public TargetType TargetType { get; set; }
        public string ItemId { get; set; }
        public UnitType UnitType { get; set; }
        public string UnitValue { get; set; }
        public decimal Amount { get; set; }
        public Gift() { }
    }
}
