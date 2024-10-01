using DiamondShop.Domain.Common;
using DiamondShop.Domain.Common.ValueObjects;
using DiamondShop.Domain.Models.Promotions.Entities;
using DiamondShop.Domain.Models.Promotions.Enum;
using DiamondShop.Domain.Models.Promotions.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Domain.Models.Promotions
{
    public class Promotion : Entity<PromotionId>, IAggregateRoot
    {
        public string Name { get; set; }
        public string Description { get; set; } 
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool IsActive { get; set; }
        public int Priority { get; set; }
        public bool IsExcludeQualifierProduct { get; set; }
        public RedemptionMode RedemptionMode { get; set; }
        public List<PromoReq> PromoReqs { get; set; } = new ();
        public List<Gift> Gifts { get; set; } = new ();
        public MediaImage? Thumbnail { get; set; }
        public static Promotion Create(string name, string description, DateTime startDate, DateTime endDate, int priority , bool isExclude , RedemptionMode mode )
        {
            return new Promotion
            {
                Id = PromotionId.Create(),
                Name = name,
                Description = description,
                StartDate = startDate.ToUniversalTime(),
                EndDate = endDate.ToUniversalTime(),
                Priority = priority,
                IsExcludeQualifierProduct = isExclude,
                IsActive = false,
                RedemptionMode = mode,
            };
        }
        public void AddRequirement(PromoReq requirement)
        {
            PromoReqs.Add(requirement);
        }
        public void RemoveRequirement(PromoReq requirement)
        {
            PromoReqs.Remove(requirement);
        }
        public void AddGift(Gift gift)
        {
            Gifts.Add(gift);
        }
        public void RemoveGift(Gift gift)
        {
            Gifts.Remove(gift);
        }

        public Promotion() { }
    }
}
