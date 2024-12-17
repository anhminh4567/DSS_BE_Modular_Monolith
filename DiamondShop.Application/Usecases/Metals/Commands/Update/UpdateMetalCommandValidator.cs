using FluentValidation;

namespace DiamondShop.Application.Usecases.Metals.Commands.Update
{
    public class UpdateMetalCommandValidator : AbstractValidator<UpdateMetalCommand>
    {
        public UpdateMetalCommandValidator() 
        { 
            RuleFor(c => c.price)
                .NotNull().GreaterThanOrEqualTo(10000);
        }
    }
}
