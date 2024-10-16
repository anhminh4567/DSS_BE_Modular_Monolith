using FluentValidation;

namespace DiamondShop.Application.Usecases.DeliveryFees.Commands.CreateMany
{
    public class CreateManyDeliveryFeeCommandValidator : AbstractValidator<CreateManyDeliveryFeeCommand>
    {
        public CreateManyDeliveryFeeCommandValidator()
        {
            RuleForEach(x => x.fees).SetValidator(new CreateDeliveryFeeCommandValidator());
        }
    }
}
