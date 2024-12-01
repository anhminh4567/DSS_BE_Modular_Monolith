using DiamondShop.Application.Commons.Validators.ErrorMessages;
using DiamondShop.Domain.BusinessRules;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Application.Usecases.AdminConfigurations.Orders
{
    public class OrderPaymentRuleValidator : AbstractValidator<OrderPaymentRules>
    {
        public OrderPaymentRuleValidator()
        {
            RuleFor(x => x.DepositPercent).NotNull().WithNotEmptyMessage()
                .GreaterThan(0)
                    .WithGreaterThanMessage()
                .LessThan(90)
                    .WithLessThanMessage();
            RuleFor(x => x.MaxMoneyFine).NotNull().WithNotEmptyMessage()
                .GreaterThan(0)
                    .WithGreaterThanMessage()
                .LessThan(int.MaxValue)
                    .WithLessThanMessage();
            RuleFor(x => x.CODPercent).NotNull().WithNotEmptyMessage()
                .GreaterThan(0)
                    .WithGreaterThanMessage()
                .LessThan(90)
                    .WithLessThanMessage()
                .LessThan(x => x.DepositPercent)
                     .WithLessThanMessage();
            RuleFor(x => x.PayAllFine).NotNull().WithNotEmptyMessage()
                .GreaterThan(0)
                    .WithGreaterThanMessage()
                .LessThan(90)
                    .WithLessThanMessage();
            RuleFor(x => x.MinAmountForCOD).NotNull().WithNotEmptyMessage()
                .GreaterThan(100_000)
                    .WithGreaterThanMessage()
                .LessThan(int.MaxValue)
                    .WithLessThanMessage();
        }
    }
    
}
