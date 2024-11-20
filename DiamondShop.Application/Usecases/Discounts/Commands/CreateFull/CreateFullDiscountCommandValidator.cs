using DiamondShop.Application.Commons.Validators.ErrorMessages;
using DiamondShop.Application.Usecases.Discounts.Commands.Create;
using DiamondShop.Application.Usecases.PromotionRequirements.Commands.CreateMany;
using DiamondShop.Domain.Models.Promotions.Entities.ErrorMessages;
using FluentValidation;

namespace DiamondShop.Application.Usecases.Discounts.Commands.CreateFull
{
    public class CreateFullDiscountCommandValidator : AbstractValidator<CreateFullDiscountCommand>
    {
        public CreateFullDiscountCommandValidator()
        {
            ClassLevelCascadeMode = CascadeMode.Stop;
            RuleFor(x => x.CreateDiscount)
                .NotNull()
                    .WithNotEmptyMessage();
            RuleFor(x => x.Requirements)
                .NotNull()
                    .WithNotEmptyMessage();
            When(x => x.Requirements != null, () =>
            {
                RuleForEach(x => x.Requirements)
                    .Must(req => req.TargetType != Domain.Models.Promotions.Enum.TargetType.Order)
                    .WithMessage((command,req) => DiscountErrors.OrderTargetNotAllowed.Message +", requirent với tên là " + req.Name);
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
