using DiamondShop.Domain.Models.Orders.Enum;
using DiamondShop.Domain.Models.Warranties.Enum;
using FluentValidation;

namespace DiamondShop.Application.Usecases.Orders.Commands.Create
{
    public class CreateOrderCommandValidator : AbstractValidator<CreateOrderCommand>
    {
        public CreateOrderCommandValidator()
        {
            RuleFor(p => p.AccountId).NotEmpty();
            RuleFor(p => p.BillingDetail)
                .NotEmpty()
                .ChildRules(p =>
                {
                    p.RuleFor(k => k.FirstName).NotEmpty();
                    p.RuleFor(k => k.LastName).NotEmpty();
                    p.RuleFor(k => k.Phone).Length(9, 11);
                    p.RuleFor(k => k.Email).EmailAddress();
                    p.RuleFor(k => k.Providence).NotEmpty();
                    p.RuleFor(k => k.Ward).NotEmpty();
                    p.RuleFor(k => k.Address).NotEmpty();
                });
            RuleFor(p => p.CreateOrderInfo)
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


        }
    }
}
