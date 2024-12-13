using DiamondShop.Application.Commons.Validators;
using DiamondShop.Application.Commons.Validators.ErrorMessages;
using DiamondShop.Domain.Common;
using FluentValidation;
using Microsoft.Extensions.Options;

namespace DiamondShop.Application.Usecases.Diamonds.Commands.ChangePriceOffset
{
    public class ChangeDiamondPriceOffsetCommanndValidator : AbstractValidator<ChangeDiamondPriceOffsetCommannd>
    {
        private readonly IOptionsMonitor<ApplicationSettingGlobal> _optionsMonitor;
        public ChangeDiamondPriceOffsetCommanndValidator(IOptionsMonitor<ApplicationSettingGlobal> optionsMonitor)
        {
            RuleLevelCascadeMode = CascadeMode.Stop;
            _optionsMonitor = optionsMonitor;
            var diamondRules = _optionsMonitor.CurrentValue.DiamondRule;
            RuleFor(x => x.diamondId)
                .NotNull()
                    .WithNotEmptyMessage();
            RuleFor(x => x.priceOffset.Value)
                .Cascade(CascadeMode.Stop)
                .GreaterThanOrEqualTo(diamondRules.MinPriceOffset)
                    .WithGreaterThanOrEqualMessage()
                .LessThanOrEqualTo(diamondRules.MaxPriceOffset)
                    .WithLessThanOrEqualMessage()
                .ValidNumberFractionBase(4)
                    .When(x => x.priceOffset != null);
            RuleFor(x => x.extraFee)
                .Cascade(CascadeMode.Stop)
                .GreaterThanOrEqualTo(0)
                    .WithGreaterThanOrEqualMessage()
                .LessThanOrEqualTo(10000000000)
                    .WithLessThanOrEqualMessage()
                    .When(x => x.extraFee != null);

        }
    }
}
