using DiamondShop.Application.Commons.Validators.ErrorMessages;
using DiamondShop.Domain.BusinessRules;
using DiamondShop.Domain.Models.Promotions;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Application.Dtos.Requests.ApplicationConfigurations.Promotions
{
    public class PromotionRuleRequestDto
    {
        public int? MaxDiscountPercent { get; set; } 
        public int? MinCode { get; set; } 
        public int? MaxCode { get; set; } 
        public int? BronzeUserDiscountPercent { get; set; } 
        public int? SilverUserDiscountPercent { get; set; } 
        public int? GoldUserDiscountPercent { get; set; } 
    }
    public class PromotionRuleValidator : AbstractValidator<PromotionRule>
    {
        
        public PromotionRuleValidator()
        {
            RuleFor(x => x.MaxDiscountPercent)
                .GreaterThanOrEqualTo(1)
                    .WithGreaterThanOrEqualMessage()
                .LessThanOrEqualTo(95)
                    .WithLessThanOrEqualMessage();
            RuleFor(x => x.BronzeUserDiscountPercent).GreaterThanOrEqualTo(1)
                    .WithGreaterThanOrEqualMessage()
                .LessThanOrEqualTo(95)
                    .WithLessThanOrEqualMessage();
            RuleFor(x => x.SilverUserDiscountPercent).GreaterThanOrEqualTo(1)
                    .WithGreaterThanOrEqualMessage()
                .LessThanOrEqualTo(95)
                    .WithLessThanOrEqualMessage()
                .Must((x, s) => x.BronzeUserDiscountPercent < s)
                    .WithMessage("người dùng bạc phảig giảm cao hơn đồng")
                    .When(x => x.BronzeUserDiscountPercent != null);
            RuleFor(x => x.GoldUserDiscountPercent).GreaterThanOrEqualTo(1)
                    .WithGreaterThanOrEqualMessage()
                .LessThanOrEqualTo(95)
                    .WithLessThanOrEqualMessage()
                .Must((x, s) => x.BronzeUserDiscountPercent < s && x.SilverUserDiscountPercent < s)
                    .WithMessage("người dùng vàng phảig giảm cao hơn đồng và bạc")
                    .When(x => x.BronzeUserDiscountPercent != null);
        }
    }
}
