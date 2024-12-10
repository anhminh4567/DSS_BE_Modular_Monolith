using DiamondShop.Application.Commons.Utilities;
using DiamondShop.Domain.BusinessRules;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Application.Commons.Validators
{
    public static class CustomValidatorExtension
    {
        public static IRuleBuilderOptions<T, decimal> ValidNumberFraction<T>(this IRuleBuilder<T, decimal> ruleBuilder)
        {
            return ruleBuilder.ValidNumberFractionBase(2);
        }
        public static IRuleBuilderOptions<T, decimal> ValidNumberFractionBase<T>(this IRuleBuilder<T, decimal> ruleBuilder, int maxFractionalNumber)
        {
            return ruleBuilder
                .NotNull()
                .Must((command, numberInput) =>
                {
                    var numToString = numberInput.ToString();
                    if (numToString == "0")
                    {
                        return true;
                    }

                    var getFractional = numToString.Split('.');
                    if (getFractional.Length == 1)
                    {
                        return true;
                    }
                    if (getFractional[1].Length > maxFractionalNumber)
                    {
                        return false;
                    }
                    return true;
                }).WithMessage("phần thập phân của số chỉ có thể là max " + maxFractionalNumber + "(s) số sau thập phân");
        }
        public static IRuleBuilderOptions<T, string> ValidDate<T>(this IRuleBuilder<T, string> ruleBuilder)
        {
            return ruleBuilder
                .NotEmpty()
                .Must(DateTimeUtil.BeAValidDate).WithMessage("format ngày tháng không hợp lệ, phải là " + DateTimeFormatingRules.DateTimeFormat);
        }
        public static IRuleBuilderOptions<T, string> ValidDateGreaterThanUTCNow<T>(this IRuleBuilder<T, string> ruleBuilder)
        {
            return ruleBuilder
                .NotEmpty()
                .Must(DateTimeUtil.BeAValidDate).WithMessage("format ngày tháng không hợp lệ, phải là " + DateTimeFormatingRules.DateTimeFormat)
                .Must(DateTimeUtil.BeGreaterThanUTCNow).WithMessage("Ngày tháng phải lớn hơn giờ UTC hiên tại, là: " + DateTime.UtcNow.ToLocalTime().ToString(DateTimeFormatingRules.DateTimeFormat));
        }
        public static IRuleBuilderOptions<T, T> ValidStartEndDate<T>(this IRuleBuilder<T, T> ruleBuilder, Func<T, string> startDateSelector, Func<T, string> endDateSelector)
        {
            return ruleBuilder
                .Must((command, _) =>
                {
                    var startDateStr = startDateSelector(command);
                    var endDateStr = endDateSelector(command);

                    var isValidStart = DateTime.TryParseExact(startDateStr, DateTimeFormatingRules.DateTimeFormat, null, DateTimeStyles.None, out DateTime startDate);
                    var isValidEnd = DateTime.TryParseExact(endDateStr, DateTimeFormatingRules.DateTimeFormat, null, DateTimeStyles.None, out DateTime endDate);

                    if (isValidStart && isValidEnd)
                    {
                        return startDate < endDate;
                    }
                    return false;
                })
                .WithMessage("Ngày bắt đầu phải bé hơn ngày kết thúc");
        }
    }
}
