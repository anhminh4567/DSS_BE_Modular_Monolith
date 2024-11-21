using DiamondShop.Application.Commons.Validators.ErrorMessages;
using FluentValidation;

namespace DiamondShop.Application.Usecases.Jewelries.Commands.Delete
{
    public class DeleteJewelryCommandValidator : AbstractValidator<DeleteJewelryCommand>
    {
        public DeleteJewelryCommandValidator() {
            RuleFor(p => p.JewelryId)
                .NotEmpty()
                    .WithNotEmptyMessage();
        }
    }
}
