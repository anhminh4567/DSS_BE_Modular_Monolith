using FluentValidation;

namespace DiamondShop.Application.Usecases.DeliveryFees.Commands.UpdateMany
{
    public class UpdateManyDeliveryFeeCommandValidator : AbstractValidator<UpdateManyDeliveryFeeCommand>
    {
        public UpdateManyDeliveryFeeCommandValidator()
        {
            this.ClassLevelCascadeMode = CascadeMode.Stop;
            var childValidator = new DeliveryFeeUpdateDtoValidator();
            RuleFor(x => x.deliveryFeeUpdateDtos).NotNull();
            RuleForEach(x => x.deliveryFeeUpdateDtos).SetValidator(childValidator);
        }
        public class DeliveryFeeUpdateDtoValidator : AbstractValidator<DeliveryFeeUpdateDto>
        {
            public DeliveryFeeUpdateDtoValidator()
            {
                RuleFor(x => x.deliveryFeeId).NotEmpty();
                RuleFor(x => x.newPrice).NotEmpty().GreaterThanOrEqualTo(0);
            }
        }
    }
}
