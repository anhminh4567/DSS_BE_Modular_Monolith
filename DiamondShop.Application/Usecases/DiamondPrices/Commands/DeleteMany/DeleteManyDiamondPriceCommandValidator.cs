using FluentValidation;

namespace DiamondShop.Application.Usecases.DiamondPrices.Commands.DeleteMany
{
    public class DeleteManyDiamondPriceCommandValidator : AbstractValidator<DeleteManyDiamondPriceCommand>
    {
        public DeleteManyDiamondPriceCommandValidator()
        {
            DeleteDiamondPriceParameterValidator validator = new DeleteDiamondPriceParameterValidator();
            RuleFor(x => x.isSideDiamond).NotNull();
            //RuleFor(x => x.isLab).NotNull();
            RuleFor(x => x.deleteList).Cascade(CascadeMode.Stop).NotNull();
            RuleForEach(x => x.deleteList).SetValidator(validator);
        }
        public class DeleteDiamondPriceParameterValidator : AbstractValidator<DeleteDiamondPriceParameter>
        {
            public DeleteDiamondPriceParameterValidator()
            {
                RuleFor(x => x.criteriaId).NotEmpty();
            }
        }
    }
}
