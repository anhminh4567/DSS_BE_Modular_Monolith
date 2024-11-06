using FluentValidation;

namespace DiamondShop.Application.Usecases.JewelryModels.Queries.GetDetail
{
    public class GetJewelryModelDetailQueryValidator : AbstractValidator<GetJewelryModelDetailQuery>
    {
        public GetJewelryModelDetailQueryValidator()
        {
            RuleFor(p => p.ModelId).NotEmpty();
        }
    }
}
