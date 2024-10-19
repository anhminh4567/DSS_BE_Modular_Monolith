using DiamondShop.Application.Commons.Validators;
using FluentValidation;

namespace DiamondShop.Application.Usecases.Promotions.Commands.UpdateInfo
{
    public class UpdatePromotionInformatoinCommmandValidator : AbstractValidator<UpdatePromotionInformationCommand>
    {
        public UpdatePromotionInformatoinCommmandValidator()
        {
            RuleFor(x => x.promotionId)
                .NotEmpty()
                .WithMessage("Promotion Id is required");
            When(x => x.UpdateStartEndDate != null, () =>
            {
                RuleFor(x => x.UpdateStartEndDate!.startDate).ValidDateGreaterThanUTCNow();
                RuleFor(x => x.UpdateStartEndDate!.endDate).ValidDate();
                RuleFor(x => x).ValidStartEndDate(x => x.UpdateStartEndDate!.startDate, x => x.UpdateStartEndDate!.endDate);
            });
        }
    }
}
