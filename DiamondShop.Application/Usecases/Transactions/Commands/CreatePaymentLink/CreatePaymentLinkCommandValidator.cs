using DiamondShop.Application.Commons.Validators.ErrorMessages;
using FluentValidation;

namespace DiamondShop.Application.Usecases.Transactions.Commands.CreatePaymentLink
{
    public class CreatePaymentLinkCommandValidator : AbstractValidator<CreatePaymentLinkCommand>
    {
        public CreatePaymentLinkCommandValidator()
        {
            RuleFor(x => x.userId)
                .NotEmpty()
                    .WithNotEmptyMessage();
            RuleFor(x => x.orderId)
                .NotEmpty()
                    .WithNotEmptyMessage();
            RuleFor(x => x.paymentMethodName)
                .NotEmpty()
                    .WithNotEmptyMessage();
            RuleFor(x => x.PaymentType)
                .IsInEnum()
                    .WithIsInEnumMessage();
        }
    }
}
