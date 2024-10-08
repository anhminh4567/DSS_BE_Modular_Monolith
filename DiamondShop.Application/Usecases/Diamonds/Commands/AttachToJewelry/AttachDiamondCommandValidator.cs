using FluentValidation;

namespace DiamondShop.Application.Usecases.Diamonds.Commands.AttachToJewelry
{
    public class AttachDiamondCommandValidator : AbstractValidator<AttachDiamondCommand>
    {
        public AttachDiamondCommandValidator()
        {
            RuleFor(p => p.JewelryId)
                .NotEmpty();

            RuleForEach(p => p.DiamondIds)
                .NotEmpty();
        }
    }
}
