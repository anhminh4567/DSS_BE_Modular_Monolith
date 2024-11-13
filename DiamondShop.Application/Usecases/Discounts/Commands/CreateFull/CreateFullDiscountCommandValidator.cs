using DiamondShop.Application.Usecases.Discounts.Commands.Create;
using DiamondShop.Application.Usecases.PromotionRequirements.Commands.CreateMany;
using FluentValidation;

namespace DiamondShop.Application.Usecases.Discounts.Commands.CreateFull
{
    public class CreateFullDiscountCommandValidator : AbstractValidator<CreateFullDiscountCommand>
    {
        public CreateFullDiscountCommandValidator()
        {
            ClassLevelCascadeMode = CascadeMode.Stop;
            RuleFor(x => x.CreateDiscount).NotNull();
            RuleFor(x => x.Requirements).NotNull();
            When(x => x.Requirements != null, () =>
            {
                RuleForEach(x => x.Requirements)
                    .Must(req => req.TargetType != Domain.Models.Promotions.Enum.TargetType.Order)
                    .WithMessage((command,req) => "Requirement cannot be of targetType order, discount only accept diamond or jewelry as discount and only as percent, the error is at requirement named: " + req.Name);
            });
            //When(x => x.Requirements != null, () =>
            //{
            //    RuleForEach(x => x.Requirements).SetValidator(new RequirementSpecValidator());
            //});
            //When(x => x.CreateDiscount != null, () =>
            //{
            //    RuleFor(x => x.CreateDiscount).SetValidator(new CreateDiscountCommandValidator());
            //});
        }
    }
}
