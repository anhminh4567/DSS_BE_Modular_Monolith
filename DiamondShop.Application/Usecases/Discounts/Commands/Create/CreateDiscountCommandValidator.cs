using DiamondShop.Application.Commons.Utilities;
using DiamondShop.Domain.BusinessRules;
using FluentValidation;
using System.Globalization;

namespace DiamondShop.Application.Usecases.Discounts.Commands.Create
{
    public class CreateDiscountCommandValidator : AbstractValidator<CreateDiscountCommand>
    {
        public CreateDiscountCommandValidator()
        {
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
            }).WithName("Date Time Things");
            RuleFor(x => x.code).Must(code =>
            {
                if (code is null)
                    return true;
                if (code.Length < PromotionRules.MinCode || code.Length > PromotionRules.MaxCode)
                    return false;
                return true;
            });
        }
    }
}
