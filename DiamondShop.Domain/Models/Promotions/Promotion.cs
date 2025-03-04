﻿using DiamondShop.Domain.BusinessRules;
using DiamondShop.Domain.Common;
using DiamondShop.Domain.Common.ValueObjects;
using DiamondShop.Domain.Models.Orders;
using DiamondShop.Domain.Models.Orders.Enum;
using DiamondShop.Domain.Models.Promotions.Entities;
using DiamondShop.Domain.Models.Promotions.Enum;
using DiamondShop.Domain.Models.Promotions.ErrorMessages;
using DiamondShop.Domain.Models.Promotions.ValueObjects;
using FluentResults;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Net.NetworkInformation; 
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DiamondShop.Domain.Models.Promotions
{
    public class Promotion : Entity<PromotionId>, IAggregateRoot
    {
        //trạng thái của order mà được xem là chưa qua sử dụng
        public static OrderStatus[] StatusNOTQualifiedAsUsed = {   };//OrderStatus.Rejected
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
        public PromotionApplyLevel? ApplyLevel { get; set; }
        public int? Amount { get; set; }
        public decimal? MoneyLimit { get; set; }
        [NotMapped]
        public bool CanBePermanentlyDeleted => Status == Status.Cancelled || Status == Status.Expired;
        public static Promotion Create(string name, string promoCode, string description, DateTime startDate, DateTime endDate, int priority , bool isExclude , RedemptionMode mode )
        {
            return new Promotion
            {
                Id = PromotionId.Create(),
                Name = name,
                PromoCode = promoCode?? DateTime.UtcNow.ToString("yyyyMMddHHmmss"),
                Description = description,
                StartDate = startDate.ToUniversalTime(),
                EndDate = endDate.ToUniversalTime(),
                Priority = priority,
                IsExcludeQualifierProduct = isExclude,
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
            //if (DateTime.UtcNow < StartDate)
            //    return Result.Fail("cannot set active since the start time is not up yet");
            if (Status != Status.Scheduled && Status != Status.Paused && Status != Status.Active)
                return Result.Fail("cannot set active for a promo that is not in scheduled or paused state");
            Status = Status.Active;
            return Result.Ok();
        }
        public void Expired()
        {
            Status = Status.Expired;
        }
        public Result Paused()
        {
            if (Status == Status.Active)
                Status = Status.Paused;
            else if (Status == Status.Paused)
                return SetActive();
            else
                return Result.Fail("can only pause when active");
            return Result.Ok();
        }
        public Result Cancel()
        {
            if (Status != Status.Scheduled && Status != Status.Paused)
                return Result.Fail("the promot must be paused first, or in schedule state to be cancelled");
            Status = Status.Cancelled;
            return Result.Ok();
        }
        public void ChangeActiveDate(DateTime startDate, DateTime endDate)
        {
            //if(endDate <= startDate || startDate < DateTime.UtcNow)
            //    throw new InvalidOperationException("cannot change the start date to a date that is already passed");
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
        public List<Order> OrderThatTruelyUsedThisPromotion(List<Order> orders)
        {
            return orders.Where(o => o.PromotionId == Id && StatusNOTQualifiedAsUsed.Contains(o.Status) == false).ToList();
        }
        public void SetSoftDelete()
        {
            if (CanBePermanentlyDeleted == false)
                throw new Exception();
            Status = Status.Soft_deleted;
        }
        public Result ValidateRequirement(PromotionRule rule)
        {
            foreach (var req in PromoReqs)
            {
                var sameRequirement = PromoReqs.Where(x => x.TargetType == req.TargetType && x.Id != req.Id).ToList();
                if (sameRequirement.Count > 0)
                {
                    switch (req.TargetType)
                    {
                        case TargetType.Jewelry_Model:
                            var model = req.ModelId;
                            if (sameRequirement.Any(x => x.ModelId == model))
                            {
                                return Result.Fail(PromotionError.RequirementError.RequirementExist);
                            }
                            break;
                        default:
                            break;
                    }
                }
            }
            if (PromoReqs.Count > rule.MaxRequirement)
                return Result.Fail(PromotionError.RequirementError.ReachLimit(rule.MaxRequirement));
            return Result.Ok();
        }
        public Result ValidateGift(PromotionRule rule)
        {
            foreach (var gift in Gifts)
            {
                var sameTypeGif = Gifts.Where(x => x.TargetType == gift.TargetType && x.Id != gift.Id).ToList();
                if (sameTypeGif.Count > 0)
                {
                    var code = gift.ItemCode;
                    if (sameTypeGif.Any(x => x.ItemCode == code))
                    {
                        return Result.Fail(PromotionError.GiftError.DuplicateGift);
                    }
                }
            }
            if (Gifts.Count > rule.MaxGift)
                return Result.Fail(PromotionError.GiftError.ReachLimit(rule.MaxGift));
            if (Gifts.Where(x => x.TargetType == TargetType.Diamond).Count() > 1)
                return Result.Fail("Quà cho kim cương chỉ có 1 quà duy nhất để tránh lỗi giữa giá fix và %");
            return Result.Ok();
        }
        public Promotion() { }
    }
}
