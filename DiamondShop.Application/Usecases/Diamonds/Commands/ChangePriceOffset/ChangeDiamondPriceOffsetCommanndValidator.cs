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
            _optionsMonitor = optionsMonitor;
            var diamondRules = _optionsMonitor.CurrentValue.DiamondRule;
            RuleFor(x => x.priceOffset)
                .Cascade(CascadeMode.Stop)
                .NotNull()
                    .WithNotEmptyMessage()
                .GreaterThanOrEqualTo(diamondRules.MinPriceOffset)
                    .WithGreaterThanOrEqualMessage()
                .LessThanOrEqualTo(diamondRules.MaxPriceOffset)
                    .WithLessThanOrEqualMessage();
            RuleFor(x => x.diamondId)
                .NotNull()
                    .WithNotEmptyMessage();
        }
    }
}
