using DiamondShop.Application.Dtos.Requests.Deliveries;
using FluentValidation;

namespace DiamondShop.Application.Usecases.DeliveryFees.Commands.CreateMany
{
    public class CreateDeliveryFeeCommandValidator : AbstractValidator<CreateDeliveryFeeCommand>
    {
        public CreateDeliveryFeeCommandValidator()
        {
            RuleFor(x => x.name).NotEmpty();
            RuleFor(x => x.cost).GreaterThan(-1);
            RuleFor(x => x.provinceId).NotNull();
        }
    }
}
