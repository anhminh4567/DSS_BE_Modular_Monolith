using DiamondShop.Application.Dtos.Requests.Diamonds;
using FluentValidation;

namespace DiamondShop.Application.Usecases.DiamondCriterias.Commands.CreateMany
{
    public class CreateManyDiamondCriteriasCommandValidator : AbstractValidator<CreateManyDiamondCriteriasCommand>
    {
        public CreateManyDiamondCriteriasCommandValidator()
        {
            RuleForEach(c => c.listCriteria).SetValidator(new CreateDiamondCriteriaCommandValidator());
        }
        public class CreateDiamondCriteriaCommandValidator : AbstractValidator<DiamondCriteriaRequestDto>
        {
            public CreateDiamondCriteriaCommandValidator()
            {
                RuleFor(c => c.Cut).IsInEnum();
                RuleFor(c => c.Color).IsInEnum();
                RuleFor(c => c.Clarity).IsInEnum();
                RuleFor(c => c.CaratFrom).NotEmpty().GreaterThan(0)
                    .Must((command, crf) => crf <= command.CaratTo);
                RuleFor(c => c.CaratTo).NotEmpty().GreaterThan(0);
            }
        }
    }
   
}
