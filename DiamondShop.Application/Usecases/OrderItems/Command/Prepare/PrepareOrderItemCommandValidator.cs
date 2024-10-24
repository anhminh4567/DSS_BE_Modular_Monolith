using DiamondShop.Application.Usecases.OrderItems.Command.Prepare;
using FluentValidation;

namespace DiamondShop.Application.Usecases.Orders.Commands.AssignDeliverer
{
    public class PrepareOrderItemCommandValidator : AbstractValidator<PrepareOrderItemCommand>
    {
        public PrepareOrderItemCommandValidator()
        {
            RuleFor(p => p.orderItemIds).NotEmpty();
        }
    }
}
