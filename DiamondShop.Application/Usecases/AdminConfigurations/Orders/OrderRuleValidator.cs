using DiamondShop.Application.Commons.Validators;
using DiamondShop.Application.Commons.Validators.ErrorMessages;
using DiamondShop.Domain.BusinessRules;
using FluentValidation;

namespace DiamondShop.Application.Usecases.AdminConfigurations.Orders
{
    public class OrderRuleValidator : AbstractValidator<OrderRule>
    {
        public OrderRuleValidator()
        {
            RuleFor(x => x.ExpiredOrderHour).GreaterThan(0).WithGreaterThanMessage();
            RuleFor(x => x.ExpectedDeliveryDate).GreaterThanOrEqualTo(1).WithGreaterThanOrEqualMessage();
            RuleFor(x => x.MaxOrderAmountForDelivery).GreaterThan(1000).WithGreaterThanMessage()
                .ValidNumberFraction();
            RuleFor(x => x.MaxOrderAmountForFullPayment).GreaterThan(1000).WithGreaterThanMessage()
                .ValidNumberFraction();
            RuleFor(x => x.DaysWaitForCustomerToPay).NotNull().WithNotEmptyMessage()
                .GreaterThan(0)
                    .WithGreaterThanMessage()
                .LessThan(60)
                    .WithLessThanMessage();
            RuleFor(x => x.MaxOrderAmountForCustomerToPlace).NotNull().WithNotEmptyMessage()
                .GreaterThan(0)
                    .WithGreaterThanMessage()
                .LessThan(50)
                    .WithLessThanMessage();
        }
    }
}
