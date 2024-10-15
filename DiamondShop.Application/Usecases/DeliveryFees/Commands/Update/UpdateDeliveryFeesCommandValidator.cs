using DiamondShop.Application.Usecases.DeliveryFees.Commands.CreateMany;
using FluentValidation;

namespace DiamondShop.Application.Usecases.DeliveryFees.Commands.Update
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
