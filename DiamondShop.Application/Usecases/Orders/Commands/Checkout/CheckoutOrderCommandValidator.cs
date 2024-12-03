using DiamondShop.Application.Usecases.Orders.Commands.Create;
using DiamondShop.Domain.Models.Orders.Enum;
using DiamondShop.Domain.Models.Warranties.Enum;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Application.Usecases.Orders.Commands.Checkout
{
    public class CheckoutOrderCommandValidator : AbstractValidator<CheckoutOrderCommand>
    {
        public CheckoutOrderCommandValidator()
        {
            RuleFor(p => p.AccountId).NotEmpty();
            RuleFor(p => p.CheckoutOrderInfo)
               .NotEmpty()
               .ChildRules(p =>
               {
                   p.RuleFor(k => k.OrderRequestDto)
                   .NotEmpty()
                   .ChildRules(k =>
                   {
                       k.RuleFor(m => m.PaymentType)
                       .Must(m => Enum.IsDefined(typeof(PaymentType), m));
                       k.RuleFor(m => m.PaymentName)
                       .NotEmpty();
                   });

                   p.RuleFor(k => k.OrderItemRequestDtos).NotEmpty();
                   p.RuleForEach(k => k.OrderItemRequestDtos)
                   .ChildRules(k =>
                   {
                       k.RuleFor(m => m.WarrantyCode)
                       .NotEmpty();
                       k.RuleFor(m => m.WarrantyType)
                       .Must(k => Enum.IsDefined(typeof(WarrantyType), k));
                   });
               });
            RuleFor(p => p.BillingDetail)
                .NotEmpty()
                .ChildRules(p =>
                {
                    p.RuleFor(k => k.FirstName);
                    p.RuleFor(k => k.LastName).NotEmpty();
                    p.RuleFor(k => k.Phone).Length(9, 11);
                    p.RuleFor(k => k.Email).EmailAddress();
                    p.RuleFor(k => k.Providence).NotEmpty();
                    p.RuleFor(k => k.Ward).NotEmpty();
                    p.RuleFor(k => k.Address).NotEmpty();
                });
           


        }
    }
}
