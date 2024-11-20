using DiamondShop.Application.Commons.Validators.ErrorMessages;
using FluentValidation;

namespace DiamondShop.Application.Usecases.Diamonds.Commands.AttachToJewelry
{
    public class AttachDiamondCommandValidator : AbstractValidator<AttachDiamondCommand>
    {
        public AttachDiamondCommandValidator()
        {

            RuleFor(p => p.JewelryId)
                .NotEmpty().WithNotEmptyMessage();
            RuleForEach(p => p.DiamondIds)
                .NotEmpty().WithNotEmptyMessage();
        }
    }
}
