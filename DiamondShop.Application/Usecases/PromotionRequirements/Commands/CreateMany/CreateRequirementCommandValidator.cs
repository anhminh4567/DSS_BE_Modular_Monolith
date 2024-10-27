using DiamondShop.Domain.Models.Promotions.Enum;
using FluentValidation;

namespace DiamondShop.Application.Usecases.PromotionRequirements.Commands.CreateMany
{
    public class CreateRequirementCommandValidator : AbstractValidator<CreateRequirementCommand>
    {
        public CreateRequirementCommandValidator()
        {
            RuleForEach(x => x.Requirements).SetValidator(new RequirementSpecValidator());
        }
    }
    public class RequirementSpecValidator : AbstractValidator<RequirementSpec>
    {
        public RequirementSpecValidator()
        {
            RuleFor(x => x.Name).NotEmpty().MinimumLength(2);
            RuleFor(x => x.TargetType).IsInEnum().WithMessage("TargetType is not valid.");
            RuleFor(x => x.Operator).IsInEnum().WithMessage("Operator is not valid.");
            RuleFor(x => x.MoneyAmount).GreaterThan(1000)
                .When(x => x.MoneyAmount.HasValue).WithMessage("MoneyAmount should be greater than 1000.");
            When(x => x.TargetType == TargetType.Diamond, () =>
            {
                RuleFor(x => x.DiamondRequirementSpec).NotNull();
                RuleFor(x => x.DiamondRequirementSpec.Origin).IsInEnum().WithMessage("not valid origin, can only be lab = 1, natural = 2, both = 3 ");

                RuleFor(x => x.DiamondRequirementSpec.caratFrom).GreaterThanOrEqualTo(0);
                RuleFor(x => x.DiamondRequirementSpec.caratTo).GreaterThanOrEqualTo(0);
                RuleFor(x => x.DiamondRequirementSpec.caratFrom).LessThanOrEqualTo(x => x.DiamondRequirementSpec.caratTo);

                RuleFor(x => (int)x.DiamondRequirementSpec.cutFrom).LessThanOrEqualTo(x => (int)x.DiamondRequirementSpec.cutTo);
                RuleFor(x => (int)x.DiamondRequirementSpec.clarityFrom).LessThanOrEqualTo(x => (int)x.DiamondRequirementSpec.clarityTo);
                RuleFor(x => (int)x.DiamondRequirementSpec.colorFrom).LessThanOrEqualTo(x => (int)x.DiamondRequirementSpec.colorTo);
            });
            When(x => x.TargetType == TargetType.Jewelry_Model, () =>
            {
                RuleFor(x => x.JewelryModelID).NotEmpty();
            });
        }
    }
}
