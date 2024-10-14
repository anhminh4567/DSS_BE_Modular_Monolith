using FluentValidation;

namespace DiamondShop.Application.Usecases.Orders.Commands.Create
{
    public class CreateOrderCommandValidator : AbstractValidator<CreateOrderCommand>
    {
        public CreateOrderCommandValidator()
        {

        }
    }
}
