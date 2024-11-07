using FluentValidation;

namespace DiamondShop.Application.Usecases.Warranties.Queries.GetDetail
{
    public class GetDetailWarrantyQueryValidator : AbstractValidator<GetDetailWarrantyQuery>
    {
        public GetDetailWarrantyQueryValidator()
        {
            RuleFor(p => p.WarrantyId).NotEmpty();
        }
    }
}
