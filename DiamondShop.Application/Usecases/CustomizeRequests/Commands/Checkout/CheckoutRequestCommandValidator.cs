using DiamondShop.Domain.Models.Orders.Enum;
using DiamondShop.Domain.Models.Warranties.Enum;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Application.Usecases.CustomizeRequests.Commands.Checkout
{
    public class CheckoutRequestCommandValidator : AbstractValidator<CheckoutRequestCommand>
    {
        public CheckoutRequestCommandValidator()
        {
            RuleFor(p => p.CheckoutRequestDto).ChildRules(p =>
            {
                p.RuleFor(k => k.customizeRequestId).NotEmpty();
                p.RuleFor(k => k.BillingDetail)
                .NotEmpty()
                .ChildRules(k =>
                {
                    k.RuleFor(m => m.FirstName);
                    k.RuleFor(m => m.LastName).NotEmpty();
                    k.RuleFor(m => m.Phone).Length(9, 11);
                    k.RuleFor(m => m.Email).EmailAddress();
                    k.RuleFor(m => m.Providence).NotEmpty();
                    k.RuleFor(m => m.Ward).NotEmpty();
                    k.RuleFor(m => m.Address).NotEmpty();
                });
                p.RuleFor(k => k.OrderRequestDto)
                    .NotEmpty()
                    .ChildRules(k =>
                    {
                        k.RuleFor(m => m.PaymentType)
                        .Must(m => Enum.IsDefined(typeof(PaymentType), m));
                        k.RuleFor(m => m.PaymentName)
                        .NotEmpty();
                    });
                p.RuleFor(k => k.WarrantyCode)
                        .NotEmpty();
                p.RuleFor(k => k.WarrantyType)
                .Must(k => Enum.IsDefined(typeof(WarrantyType), k));

            });
        }
    }
}
