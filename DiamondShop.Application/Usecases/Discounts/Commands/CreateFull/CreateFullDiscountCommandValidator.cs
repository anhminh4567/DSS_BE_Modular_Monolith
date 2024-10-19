using DiamondShop.Application.Usecases.Discounts.Commands.Create;
using DiamondShop.Application.Usecases.PromotionRequirements.Commands.CreateMany;
using FluentValidation;

namespace DiamondShop.Application.Usecases.Discounts.Commands.CreateFull
{
    public class CreateFullDiscountCommandValidator : AbstractValidator<CreateFullDiscountCommand>
    {
        public CreateFullDiscountCommandValidator()
        {
            RuleFor(x => x.CreateDiscount).NotNull();
            RuleFor(x => x.Requirements).NotNull();
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
