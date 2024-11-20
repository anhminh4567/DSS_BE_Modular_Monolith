using DiamondShop.Application.Commons.Validators.ErrorMessages;
using DiamondShop.Application.Usecases.PromotionGifts.Commands.CreateMany;
using DiamondShop.Application.Usecases.PromotionRequirements.Commands.CreateMany;
using DiamondShop.Application.Usecases.Promotions.Commands.Create;
using FluentValidation;

namespace DiamondShop.Application.Usecases.Promotions.Commands.CreateFull
{
    public class CreateFullPromotionCommandValidator : AbstractValidator<CreateFullPromotionCommand>
    {
        public CreateFullPromotionCommandValidator()
        {
            RuleFor(x => x.CreatePromotionCommand).NotNull().WithNotEmptyMessage();
            RuleFor(x => x.Requirements).NotNull().WithNotEmptyMessage();
            RuleFor(x => x.Gifts).NotNull().WithNotEmptyMessage();
            //When(x => x.Requirements != null, () =>
            //{
            //    var requirementSpecValidator = new RequirementSpecValidator();
            //    RuleForEach(x => x.Requirements).SetValidator(requirementSpecValidator);
            //});
            //When(x => x.Gifts != null, () =>
            //{
            //    var giftSpecValidator = new GiftSpecValidator();
            //    RuleForEach(x => x.Gifts).SetValidator(giftSpecValidator);
            //});
            //When(x => x.CreatePromotionCommand != null, () =>
            //{
            //    RuleFor(x => x.CreatePromotionCommand).SetValidator(new CreatePromotionCommandValidator());
            //});
        }
    }
}
