using DiamondShop.Api.Controllers.Orders.Cancel;
using DiamondShop.Application.Usecases.Orders.Commands.DeliverFail;
using FluentValidation;

namespace DiamondShop.Application.Usecases.Orders.Commands.Cancel
{
    public class OrderDeliverFailCommandValidator : AbstractValidator<OrderDeliverFailCommand>
    {
        public OrderDeliverFailCommandValidator()
        {
            RuleFor(p => p.OrderId).NotEmpty();
            RuleFor(p => p.DelivererId).NotEmpty();
        }
    }
}
