using DiamondShop.Application.Commons;
using DiamondShop.Domain.BusinessRules;
using FluentValidation;
using System.Globalization;

namespace DiamondShop.Application.Usecases.Discounts.Commands.Update
{
    public class UpdateDiscountCommandValidator : AbstractValidator<UpdateDiscountCommand>
    {
        public UpdateDiscountCommandValidator()
        {
            RuleFor(c => c.discountId).NotEmpty();
            RuleFor(c => c.name).NotEmpty().MinimumLength(2);
            RuleFor(c => c.percent).NotEmpty().GreaterThanOrEqualTo(1).LessThan(100);
            RuleFor(x => x.startDate).NotEmpty()
                .Must(DateTimeUtil.BeAValidDate).WithMessage("Invalid Start Date format.");

            RuleFor(x => x.endDate).NotEmpty()
                .Must(DateTimeUtil.BeAValidDate).WithMessage("Invalid End Date format.");

            RuleFor(x => x).Must((command) =>
            {
                var isValidStart = DateTime.TryParseExact(command.startDate, DateTimeFormatingRules.DateTimeFormat, null, DateTimeStyles.None, out DateTime startDate);
                var isValidEnd = DateTime.TryParseExact(command.endDate, DateTimeFormatingRules.DateTimeFormat, null, DateTimeStyles.None, out DateTime endDate);
                if (isValidStart && isValidEnd)
                {
                    if (startDate < DateTime.Now)
                    {
                        return false;
                    }
                    return startDate < endDate;//&& (endDate - startDate).TotalDays >= settings.MinimumPromotionDuration;
                }
                return false;
            });
        }
    }
}
