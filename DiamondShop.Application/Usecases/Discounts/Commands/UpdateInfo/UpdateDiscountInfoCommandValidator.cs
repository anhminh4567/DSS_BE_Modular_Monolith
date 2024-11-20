using DiamondShop.Application.Commons.Validators;
using DiamondShop.Application.Commons.Validators.ErrorMessages;
using FluentValidation;

namespace DiamondShop.Application.Usecases.Discounts.Commands.UpdateInfo
{
    public class UpdateDiscountInfoCommandValidator : AbstractValidator<UpdateDiscountInfoCommand>
    {
        public UpdateDiscountInfoCommandValidator()
        {
            RuleFor(x => x.discountId)
                .NotEmpty()
                    .WithNotEmptyMessage();
            When(x => x.discountPercent != null,() =>
            {
                RuleFor(x => x.discountPercent).InclusiveBetween(1, 100);
            });
            //When(x => x.UpdateStartEndDate != null, () =>
            //{
            //    RuleFor(x => x.UpdateStartEndDate!.startDate).ValidDateGreaterThanUTCNow();
            //    RuleFor(x => x.UpdateStartEndDate!.endDate).ValidDate();
            //    RuleFor(x => x).ValidStartEndDate(x => x.UpdateStartEndDate!.startDate, x => x.UpdateStartEndDate!.endDate);
            //});
            When(x => x.UpdateStartEndDate != null, () =>
            {
                //
                When(x => x.UpdateStartEndDate!.startDate != null, () =>
                {
                    RuleFor(x => x.UpdateStartEndDate!.startDate).ValidDateGreaterThanUTCNow();
                });
                When(x => x.UpdateStartEndDate!.endDate != null, () =>
                {
                    RuleFor(x => x.UpdateStartEndDate!.endDate).ValidDateGreaterThanUTCNow();
                });
                When(x => x.UpdateStartEndDate!.startDate != null && x.UpdateStartEndDate!.endDate != null, () =>
                {
                    RuleFor(x => x).ValidStartEndDate(x => x.UpdateStartEndDate!.startDate, x => x.UpdateStartEndDate!.endDate);
                });
            });
        }
    }
}
