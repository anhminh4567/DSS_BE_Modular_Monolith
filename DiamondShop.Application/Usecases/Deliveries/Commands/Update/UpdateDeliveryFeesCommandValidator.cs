using DiamondShop.Application.Usecases.Deliveries.Commands.Create;
using FluentValidation;

namespace DiamondShop.Application.Usecases.Deliveries.Commands.Update
{
    public class UpdateDeliveryFeesCommandValidator : AbstractValidator<UpdateDeliveryFeesCommand>
    {
        public UpdateDeliveryFeesCommandValidator()
        {
            RuleFor(x => x.feeId).NotEmpty();
            RuleFor(x => x.updatedObject).NotNull()
                .SetValidator(new CreateDeliveryFeeCommandValidator());
        }
    }
}
