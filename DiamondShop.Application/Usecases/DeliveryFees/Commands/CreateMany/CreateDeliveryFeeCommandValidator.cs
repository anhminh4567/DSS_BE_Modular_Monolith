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
            RuleFor(x => x.type).IsInEnum();
            RuleFor(x => x.ToLocationCity).NotNull()
                .When(x => x.type == DeliveryFeeType.LocationToCity)
                .ChildRules(ToLocationCity =>
                {
                    ToLocationCity.RuleFor(x => x.sourceCity).NotEmpty();
                    ToLocationCity.RuleFor(x => x.destinationCity).NotEmpty();
                });
            RuleFor(x => x.ToDistance).NotNull()
                .When(x => x.type == DeliveryFeeType.Distance)
                .ChildRules(ToDistance =>
                {
                    ToDistance.RuleFor(x => x.start).NotNull();
                    ToDistance.RuleFor(x => x.end).NotNull();
                    ToDistance.RuleFor(x => x.end).GreaterThan(x => x.start);
                });
        }
    }
}
