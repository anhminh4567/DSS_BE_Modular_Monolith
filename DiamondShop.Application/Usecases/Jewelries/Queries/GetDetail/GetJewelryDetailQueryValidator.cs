using FluentValidation;

namespace DiamondShop.Application.Usecases.Jewelries.Queries.GetDetail
{
    public class GetJewelryDetailQueryValidator : AbstractValidator<GetJewelryDetailQuery>
    {
        public GetJewelryDetailQueryValidator()
        {
            RuleFor(p => p.jewelryId).NotEmpty();
        }
    }
}
