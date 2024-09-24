using FluentValidation;

namespace DiamondShop.Application.Usecases.DiamondCriterias.Commands.Create
{
    public class CreateDiamondCriteriaCommandValidator : AbstractValidator<CreateDiamondCriteriaCommand>
    {
        public CreateDiamondCriteriaCommandValidator()
        {
            RuleFor(c => c.caratFrom).NotEmpty().GreaterThan(0)
                .Must( (command,crf) => crf <= command.caratTo );
            RuleFor(c => c.caratTo).NotEmpty().GreaterThan(0);
        }
    }
}
