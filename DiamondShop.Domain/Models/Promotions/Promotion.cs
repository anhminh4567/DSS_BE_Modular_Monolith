using DiamondShop.Domain.Common;
using DiamondShop.Domain.Common.ValueObjects;
using DiamondShop.Domain.Models.Promotions.Entities;
using DiamondShop.Domain.Models.Promotions.Enum;
using DiamondShop.Domain.Models.Promotions.ValueObjects;
using FluentResults;
using Microsoft.Extensions.Primitives;
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
        public string PromoCode { get; set; }
        public string Description { get; set; } 
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        //public bool IsActive { get; set; }
        public int Priority { get; set; }
        public bool IsExcludeQualifierProduct { get; set; }
        public RedemptionMode RedemptionMode { get; set; }
        public List<PromoReq> PromoReqs { get; set; } = new ();
        public List<Gift> Gifts { get; set; } = new ();
        public Media? Thumbnail { get; set; }
        public Status Status { get; set; }
        public static Promotion Create(string name, string promoCode, string description, DateTime startDate, DateTime endDate, int priority , bool isExclude , RedemptionMode mode )
        {
            return new Promotion
            {
                Id = PromotionId.Create(),
                Name = name,
                PromoCode = promoCode,
                Description = description,
                StartDate = startDate.ToUniversalTime(),
                EndDate = endDate.ToUniversalTime(),
                Priority = priority,
                IsExcludeQualifierProduct = isExclude,
                //IsActive = false,
                Status = Status.Scheduled,
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
        public Result SetActive()
        {
            if (Status == Status.Cancelled || Status == Status.Expired)
                return Result.Fail("cannot set active for a promo that is already expired or cancelled");
            if (PromoReqs.Any() is false)
                return Result.Fail("cannot set active for a promo that have no requirement at all");
            if (Gifts.Any() is false)
                return Result.Fail("cannot set active for a promo that have no requirement at all");
            if (DateTime.UtcNow < StartDate)
                return Result.Fail("cannot set active since the start time is not up yet");
            Status = Status.Active;
            return Result.Ok();
        }
        public void Expired()
        {
            Status = Status.Expired;
        }
        public void Paused()
        {
            Status = Status.Paused;
        }
        public Result Cancel()
        {
            if (Status != Status.Scheduled && Status != Status.Paused)
                return Result.Fail("the promot must be paused first, or in schedule state to be cancelled");
            Status = Status.Expired;
            return Result.Ok();
        }
        public void ChangeActiveDate(DateTime startDate, DateTime endDate)
        {
            if(endDate <= startDate || startDate < DateTime.UtcNow)
                throw new InvalidOperationException("cannot change the start date to a date that is already passed");
            StartDate = startDate.ToUniversalTime();
            EndDate = endDate.ToUniversalTime();
        }
        public Promotion() { }
    }
}
