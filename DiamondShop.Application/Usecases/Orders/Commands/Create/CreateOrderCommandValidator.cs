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
            RuleFor(p => p.CreateOrderInfo)
                .NotEmpty()
                .ChildRules(p =>
                {
                    p.RuleFor(m => m.PaymentType)
                        .Must(m => Enum.IsDefined(typeof(PaymentType), m));
                    p.RuleFor(m => m.PaymentName)
                    .NotEmpty();
                    //p.RuleFor(k => k.Address).NotEmpty();
                    p.RuleFor(m => m.BillingDetail)
                        .NotNull();
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
