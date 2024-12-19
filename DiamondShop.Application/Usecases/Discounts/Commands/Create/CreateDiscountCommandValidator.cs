using DiamondShop.Application.Commons.Utilities;
using DiamondShop.Application.Commons.Validators.ErrorMessages;
using DiamondShop.Domain.BusinessRules;
using FluentValidation;
using System.Globalization;

namespace DiamondShop.Application.Usecases.Discounts.Commands.Create
{
    public class CreateDiscountCommandValidator : AbstractValidator<CreateDiscountCommand>
    {
        public CreateDiscountCommandValidator()
        {
            RuleFor(c => c.name).NotEmpty()
                    .WithNotEmptyMessage()
                .MinimumLength(2)
                    .WithMinLenghtMessage();
            RuleFor(c => c.discountPercent)
                .NotEmpty()
                    .WithNotEmptyMessage()
                .GreaterThanOrEqualTo(1)
                    .WithGreaterThanOrEqualMessage()
                .LessThan(90)
                    .WithLessThanMessage();
            RuleFor(x => x.startDate).NotEmpty()
                .Must(DateTimeUtil.BeAValidDate);

            RuleFor(x => x.endDate).NotEmpty()
                .Must(DateTimeUtil.BeAValidDate);

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
            }).WithName("ngày bắt đầu bé hơn ngày kết thúc");
            //RuleFor(x => x.discountCode).Must(code =>
            //{
            //    if (code is null)
            //        return true;
            //    if (code.Length < PromotionRules.MinCode || code.Length > PromotionRules.MaxCode)
            //        return false;
            //    return true;
            //});\
            RuleFor(x => x.discountCode)
                .MinimumLength(PromotionRules.MinCode)
                    .WithMinLenghtMessage()
                .MaximumLength(PromotionRules.MaxCode)
                    .WithMaxLenghtMessage();
        }
    }
}
