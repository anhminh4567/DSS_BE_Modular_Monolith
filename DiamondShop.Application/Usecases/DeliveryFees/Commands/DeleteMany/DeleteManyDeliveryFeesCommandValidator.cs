using FluentValidation;

namespace DiamondShop.Application.Usecases.DeliveryFees.Commands.DeleteMany
{
    public class DeleteManyDeliveryFeesCommandValidator : AbstractValidator<DeleteManyDeliveryFeesCommand>
    {
        public DeleteManyDeliveryFeesCommandValidator()
        {
            RuleFor(x => x.deliveryFeeIds).NotNull();
        }
    }
}
