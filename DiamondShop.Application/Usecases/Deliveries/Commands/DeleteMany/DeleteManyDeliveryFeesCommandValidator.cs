using FluentValidation;

namespace DiamondShop.Application.Usecases.Deliveries.Commands.DeleteMany
{
    public class DeleteManyDeliveryFeesCommandValidator : AbstractValidator<DeleteManyDeliveryFeesCommand>
    {
        public DeleteManyDeliveryFeesCommandValidator()
        {
            RuleFor(x => x.deliveryFeeIds).NotNull();
        }
    }
}
