using DiamondShop.Application.Commons.Validators.ErrorMessages;
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
                //RuleFor(c => c.Cut).IsInEnum();
                //RuleFor(c => c.Color).IsInEnum();
                //RuleFor(c => c.Clarity).IsInEnum();
                //When(x => x.Color != null,() =>
                //{
                //    RuleFor(c => c.Color).IsInEnum().WithIsInEnumMessage();
                //});
                //When(x => x.Clarity != null, () =>
                //{
                //    RuleFor(c => c.Clarity).IsInEnum().WithIsInEnumMessage();
                //});
                //When(x => x.Cut != null, () =>
                //{
                //    RuleFor(c => c.Cut).IsInEnum().WithIsInEnumMessage();
                //});
                RuleFor(c => c.CaratFrom).NotEmpty().
                        WithNotEmptyMessage()
                    .GreaterThan(0)
                        .WithGreaterThanMessage()
                    .Must((command, crf) => crf < command.CaratTo)
                        .WithMessage("carat from phải bé hơn carat to");
                RuleFor(c => c.CaratTo).NotEmpty()
                        .WithNotEmptyMessage()
                    .GreaterThan(0)
                        .WithGreaterThanMessage();
            }
        }
    }
   
}
