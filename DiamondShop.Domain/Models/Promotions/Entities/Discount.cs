using DiamondShop.Domain.BusinessRules;
using DiamondShop.Domain.Common;
using DiamondShop.Domain.Common.Ultilities;
using DiamondShop.Domain.Common.ValueObjects;
using DiamondShop.Domain.Models.Orders.Enum;
using DiamondShop.Domain.Models.Promotions.Enum;
using DiamondShop.Domain.Models.Promotions.ValueObjects;
using FluentResults;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Domain.Models.Promotions.Entities
{
    public class Discount : Entity<DiscountId>
    {
        public static List<OrderStatus> NotCountAsUsed = new() { OrderStatus.Rejected, OrderStatus.Cancelled  };
        public string Name { get; set; }

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        //public bool IsActive { get; set; }
        public string DiscountCode { get; set; }
        public int DiscountPercent { get; set; }
        public List<PromoReq> DiscountReq { get; set; } = new();
        public Media? Thumbnail { get; set; }
        public Status Status { get; set; }
        [NotMapped]
        public bool CanBePermanentlyDeleted => Status == Status.Cancelled || Status == Status.Expired;
        [NotMapped]
        public List<Gift> DiscountGift { get; set; } = new();
        public decimal? MoneyLimit { get; set; }
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
                Status = Status.Scheduled,
            };
        }
        public void SetPercentDiscount(int percent)
        {
            DiscountPercent = Math.Clamp(percent, 1, PromotionRules.MaxDiscountPercent);
        }
        public Result SetActive()
        {
            if (Status == Status.Cancelled || Status == Status.Expired)
                return Result.Fail("cannot set active for a discount that is already expired or cancelled");
            if(DiscountReq.Any() is false)
                return Result.Fail("cannot set active for a discount that have no requirement at all");
            //if(DateTime.UtcNow < StartDate)
            //    return Result.Fail( "cannot set active since the start time is not up yet");
            Status = Status.Active;
            return Result.Ok();
        }
        public Result Pause()
        {
            if (Status == Status.Active)
                Status = Status.Paused;
            else if (Status == Status.Paused)
                return SetActive();
            else
                return Result.Fail("can only pause when active");
            return Result.Ok();
        }
        public void Expired()
        {
            Status = Status.Expired;
        }
        public Result Paused()
        {
            if (Status == Status.Active || Status == Status.Paused)
                Status = Status.Paused;
            else
                return Result.Fail("can only pause when active");
            return Result.Ok();
        }
        public Result Cancel()
        {
            if (Status != Status.Scheduled && Status != Status.Paused)
                return Result.Fail("the discount must be paused first, or in schedule state to be cancelled");
            Status = Status.Cancelled;
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
        public void SetGift(Gift gift, bool isRemove = false)
        {
            if (isRemove is false)
                DiscountGift.Add(gift);
            else
                DiscountGift.Remove(gift);
        }
        public void ChangeActiveDate(DateTime startDate, DateTime endDate)
        {
            //if (endDate <= startDate || startDate < DateTime.UtcNow)
            //    throw new InvalidOperationException("cannot change the start date to a date that is already passed");
            //StartDate = startDate.ToUniversalTime();
            //EndDate = endDate.ToUniversalTime();
            ChangeStartDate(startDate);
            ChangeEndDate(endDate);
        }
        public void ChangeStartDate(DateTime startDate)
        {
            if (startDate < DateTime.UtcNow || startDate >= EndDate)
                throw new InvalidOperationException("cannot change the start date to a date that is already passed");
            StartDate = startDate.ToUniversalTime();
            Status = Status.Scheduled;
        }
        public void ChangeEndDate(DateTime enddate)
        {
            if (enddate <= StartDate)
                throw new InvalidOperationException("cannot change the end date to a date that is before start");
            EndDate = enddate.ToUniversalTime();
            //Status = Status.Scheduled;
        }
        private static string GetRandomCode()
        {
            return RandomGenerators.GetRandomString(PromotionRules.MinCode);
        }
        public void SetSoftDelete()
        {
            if (CanBePermanentlyDeleted == false)
                throw new Exception();
            Status = Status.Soft_deleted;
        }
        public Discount() { }
    }


}
