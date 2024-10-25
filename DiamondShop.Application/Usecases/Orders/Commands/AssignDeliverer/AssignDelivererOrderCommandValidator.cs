using DiamondShop.Api.Controllers.Orders.AssignDeliverer;
using FluentValidation;

namespace DiamondShop.Application.Usecases.Orders.Commands.AssignDeliverer
{
    public class AssignDelivererOrderCommandValidator : AbstractValidator<AssignDelivererOrderCommand>
    {
        public AssignDelivererOrderCommandValidator()
        {
            RuleFor(p => p.orderId).NotEmpty();
            RuleFor(p => p.delivererId).NotEmpty();
        }
    }
}
