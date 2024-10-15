using FluentValidation;

namespace DiamondShop.Application.Usecases.Deliveries.Commands.Create
{
    public class CreateManyDeliveryFeeCommandValidator : AbstractValidator<CreateManyDeliveryFeeCommand>
    {
        public CreateManyDeliveryFeeCommandValidator()
        {
            RuleForEach(x => x.fees).SetValidator(new CreateDeliveryFeeCommandValidator());
        }
    }
}
