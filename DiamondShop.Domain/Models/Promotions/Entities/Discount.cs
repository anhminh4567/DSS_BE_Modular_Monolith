using DiamondShop.Domain.BusinessRules;
using DiamondShop.Domain.Common;
using DiamondShop.Domain.Common.ValueObjects;
using DiamondShop.Domain.Models.Promotions.ValueObjects;
using FluentResults;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Domain.Models.Promotions.Entities
{
    public class Discount : Entity<DiscountId>
    {
        public string Name { get; set; }

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool IsActive { get; set; }
        public string DiscountCode { get; set; }
        public int DiscountPercent { get; set; }
        public List<PromoReq> DiscountReq { get; set; } = new();
        public Media? Thumbnail { get; set; }
        public static Discount Create(string name, DateTime startDate, DateTime endDate, int percent, string? code)
        {
            return new Discount()
            {
                Id = DiscountId.Create(),
                Name = name,
                StartDate = startDate.ToUniversalTime(),
                EndDate = endDate.ToUniversalTime(),
                DiscountPercent = Math.Clamp(percent, 1, PromotionRules.MaxDiscountPercent),
                DiscountCode = (code == null) ? GetRandomCode() : code,
            };
        }
        public Result SetActive()
        {
            if(DiscountReq.Any() is false)
            {
                return Result.Fail("cannot set active for a discount that have no requirement at all");
            }
            if(DateTime.UtcNow < StartDate)
            {
                return Result.Fail( "cannot set active since the start time is not up yet");
            }
            IsActive = true;
            return Result.Ok();
        }
        public void Update (string? name, DateTime? startDate, DateTime? endDate, int? percent)
        {
            Name = name == null ? Name : name ;
            StartDate = startDate == null ? StartDate : startDate.Value.ToUniversalTime();
            EndDate = endDate == null ? EndDate : endDate.Value.ToUniversalTime();
            DiscountPercent = percent == null ? DiscountPercent : percent.Value;
        }
        public void SetRequirement(PromoReq requirement, bool isRemove = false)
        {
            if(isRemove is false)
                DiscountReq.Add(requirement);
            else
                DiscountReq.Remove(requirement);
        }

        private static string GetRandomCode()
        {
            return DateTime.UtcNow.ToString("yyyyMMddHHmmss");
        }
        public Discount() { }
    }


}
