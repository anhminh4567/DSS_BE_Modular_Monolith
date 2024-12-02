using DiamondShop.Application.Commons.Validators.ErrorMessages;
using FluentValidation;

namespace DiamondShop.Application.Usecases.DiamondPrices.Commands.DeleteMany
{
    public class DeleteManyDiamondPriceCommandValidator : AbstractValidator<DeleteManyDiamondPriceCommand>
    {
        public DeleteManyDiamondPriceCommandValidator()
        {
            DeleteDiamondPriceParameterValidator validator = new DeleteDiamondPriceParameterValidator();
            RuleFor(x => x.isSideDiamond).NotNull().WithNotEmptyMessage();
            //RuleFor(x => x.isLab).NotNull();
            RuleFor(x => x.priceIds).Cascade(CascadeMode.Stop).NotNull().WithNotEmptyMessage();
            //RuleForEach(x => x.deleteList).SetValidator(validator);
        }
        public class DeleteDiamondPriceParameterValidator : AbstractValidator<DeleteDiamondPriceParameter>
        {
            public DeleteDiamondPriceParameterValidator()
            {
                RuleFor(x => x.criteriaId).NotEmpty().WithNotEmptyMessage();
            }
        }
    }
}
