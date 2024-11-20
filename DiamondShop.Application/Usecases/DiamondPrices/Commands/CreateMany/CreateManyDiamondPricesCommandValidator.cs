using DiamondShop.Application.Commons.Validators.ErrorMessages;
using DiamondShop.Application.Dtos.Requests.Diamonds;
using FluentValidation;

namespace DiamondShop.Application.Usecases.DiamondPrices.Commands.CreateMany
{
    public class CreateManyDiamondPricesCommandValidator : AbstractValidator<CreateManyDiamondPricesCommand>
    {
        public CreateManyDiamondPricesCommandValidator()
        {
            RuleFor(x => x.listPrices).NotNull().NotEmpty().WithNotEmptyMessage();
            //RuleForEach(x => x.listPrices).SetValidator(new DiamondPriceRequestDtoValidator());
        }
        //private class DiamondPriceRequestDtoValidator : AbstractValidator<DiamondPriceRequestDto>
        //{
        //    public DiamondPriceRequestDtoValidator()
        //    {
        //        RuleFor(c => c.price).NotEmpty().GreaterThan(0);
        //        RuleFor(c => c.DiamondCriteriaId.Value).NotEmpty();
        //        RuleFor(c => c.DiamondShapeId.Value).NotEmpty();

        //    }
        //}
    }
}
