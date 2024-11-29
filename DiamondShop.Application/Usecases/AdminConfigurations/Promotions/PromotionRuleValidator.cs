using DiamondShop.Application.Commons.Validators.ErrorMessages;
using DiamondShop.Domain.BusinessRules;
using FluentValidation;

namespace DiamondShop.Application.Usecases.AdminConfigurations.Promotions
{
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
