using DiamondShop.Application.Commons.Validators;
using DiamondShop.Domain.BusinessRules;
using FluentValidation;

namespace DiamondShop.Application.Services.Interfaces.AdminConfigurations.DiamondRuleConfig.Models
{
    public class DiamondRuleValidator : AbstractValidator<DiamondRule>
    {
        public DiamondRuleValidator()
        {
            RuleFor(x => x.MinPriceOffset)
                .ValidNumberFraction()
                .WithMessage("MinPriceOffset only contain max 2 fractional number");

            RuleFor(x => x.MaxPriceOffset)
                .ValidNumberFraction()
                .WithMessage("MaxPriceOffset only contain max 2 fractional number");

            RuleFor(x => x.BiggestSideDiamondCarat)
                .GreaterThan(0)
                .WithMessage("BiggestSideDiamondCarat must be greater than or equal to 0");

            RuleFor(x => x.SmallestMainDiamondCarat)
                .GreaterThan(0)
                .WithMessage("SmallestMainDiamondCarat must be greater than or equal to 0");

            RuleFor(x => x.MainDiamondMaxFractionalNumber)
                .NotNull().LessThanOrEqualTo(3)
                .WithMessage("MainDiamondMaxFractionalNumber must be greater than or equal to 0");

            RuleFor(x => x.AverageOffsetVeryGoodCutFromIdealCut).Cascade(CascadeMode.Stop)
                .GreaterThan(-1m).ValidNumberFraction()
                .WithMessage("AverageOffsetVeryGoodCutFromIdealCut must be greater than or equal to -1");

            RuleFor(x => x.AverageOffsetGoodCutFromIdealCut).Cascade(CascadeMode.Stop)
                .GreaterThan(-1m).ValidNumberFraction()
                .WithMessage("AverageOffsetGoodCutFromIdealCut must be greater than or equal to -1");

            RuleFor(x => x.AverageOffsetVeryGoodCutFromIdealCut_FANCY_SHAPE).Cascade(CascadeMode.Stop)
                .GreaterThan(-1m).ValidNumberFraction()
                .WithMessage("AverageOffsetVeryGoodCutFromIdealCut_FANCY_SHAPE must be greater than or equal to -1");

            RuleFor(x => x.AverageOffsetGoodCutFromIdealCut_FANCY_SHAPE).Cascade(CascadeMode.Stop)
                .GreaterThan(-1m).ValidNumberFraction()
                .WithMessage("AverageOffsetGoodCutFromIdealCut_FANCY_SHAPE must be greater than or equal to -1");

            RuleFor(x => x.PearlOffsetFromFancyShape).Cascade(CascadeMode.Stop)
                .GreaterThan(-1m).ValidNumberFraction()
                .WithMessage("PearlOffsetFromFancyShape must be greater than or equal to -1");

            RuleFor(x => x.PrincessOffsetFromFancyShape).Cascade(CascadeMode.Stop)
                .GreaterThan(-1m).ValidNumberFraction()
                .WithMessage("PrincessOffsetFromFancyShape must be greater than or equal to -1");

            RuleFor(x => x.CushionOffsetFromFancyShape).Cascade(CascadeMode.Stop)
                .GreaterThan(-1m).ValidNumberFraction()
                .WithMessage("CushionOffsetFromFancyShape must be greater than or equal to -1");

            RuleFor(x => x.EmeraldOffsetFromFancyShape).Cascade(CascadeMode.Stop)
                .GreaterThan(-1m).ValidNumberFraction()
                .WithMessage("EmeraldOffsetFromFancyShape must be greater than or equal to -1");

            RuleFor(x => x.OvalOffsetFromFancyShape).Cascade(CascadeMode.Stop)
                .GreaterThan(-1m).ValidNumberFraction()
                .WithMessage("OvalOffsetFromFancyShape must be greater than or equal to -1");

            RuleFor(x => x.RadiantOffsetFromFancyShape).Cascade(CascadeMode.Stop)
                .GreaterThan(-1m).ValidNumberFraction()
                .WithMessage("RadiantOffsetFromFancyShape must be greater than or equal to -1");

            RuleFor(x => x.AsscherOffsetFromFancyShape).Cascade(CascadeMode.Stop)
                .GreaterThan(-1m).ValidNumberFraction()
                .WithMessage("AsscherOffsetFromFancyShape must be greater than or equal to -1");

            RuleFor(x => x.MarquiseOffsetFromFancyShape).Cascade(CascadeMode.Stop)
                .GreaterThan(-1m).ValidNumberFraction()
                .WithMessage("MarquiseOffsetFromFancyShape must be greater than or equal to -1");

            RuleFor(x => x.HeartOffsetFromFancyShape).Cascade(CascadeMode.Stop)
                .GreaterThan(-1m).ValidNumberFraction()
                .WithMessage("HeartOffsetFromFancyShape must be greater than or equal to -1");
        }
    }
}
