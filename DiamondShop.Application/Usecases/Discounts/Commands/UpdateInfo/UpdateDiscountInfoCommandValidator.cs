using DiamondShop.Application.Commons.Validators;
using FluentValidation;

namespace DiamondShop.Application.Usecases.Discounts.Commands.UpdateInfo
{
    public class UpdateDiscountInfoCommandValidator : AbstractValidator<UpdateDiscountInfoCommand>
    {
        public UpdateDiscountInfoCommandValidator()
        {
            RuleFor(x => x.discountId).NotEmpty();
            When(x => x.percent != null,() =>
            {
                RuleFor(x => x.percent).InclusiveBetween(1, 100);
            });
            When(x => x.UpdateStartEndDate != null, () =>
            {
                RuleFor(x => x.UpdateStartEndDate!.startDate).ValidDateGreaterThanUTCNow();
                RuleFor(x => x.UpdateStartEndDate!.endDate).ValidDate();
                RuleFor(x => x).ValidStartEndDate(x => x.UpdateStartEndDate!.startDate, x => x.UpdateStartEndDate!.endDate);
            });
        }
    }
}
