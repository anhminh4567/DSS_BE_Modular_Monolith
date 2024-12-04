using DiamondShop.Api.Controllers.Orders.Cancel;
using DiamondShop.Application.Usecases.Orders.Commands.DeliverFail;
using FluentValidation;

namespace DiamondShop.Application.Usecases.Orders.Commands.Cancel
{
    public class OrderDeliverFailCommandValidator : AbstractValidator<OrderDeliverFailCommand>
    {
        public OrderDeliverFailCommandValidator()
        {
            RuleFor(p => p.OrderDeliverFailRequestDto).ChildRules(p =>
            {
                p.RuleFor(k => k.OrderId).NotEmpty();
            });
            RuleFor(p => p.DelivererId).NotEmpty();
        }
    }
}
