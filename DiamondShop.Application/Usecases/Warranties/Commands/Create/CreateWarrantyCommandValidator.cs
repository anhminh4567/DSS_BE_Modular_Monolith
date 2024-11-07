using FluentValidation;

namespace DiamondShop.Application.Usecases.Warranties.Commands.Create
{
    public class CreateWarrantyCommandValidator : AbstractValidator<CreateWarrantyCommand>
    {
        public CreateWarrantyCommandValidator() {
            RuleFor(p => p.Type).IsInEnum();
            RuleFor(p => p.Name).NotEmpty();
            RuleFor(p => p.Code).NotEmpty();
            RuleFor(p => p.MonthDuration).GreaterThan(0);
            RuleFor(p => p.Price).GreaterThan(0);
        }
    }
}
