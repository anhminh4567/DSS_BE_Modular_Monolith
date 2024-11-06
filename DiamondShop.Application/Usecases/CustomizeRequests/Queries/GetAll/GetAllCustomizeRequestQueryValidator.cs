using DiamondShop.Application.Commons.Validators;
using FluentValidation;

namespace DiamondShop.Application.Usecases.CustomizeRequests.Queries.GetAll
{
    public class GetAllCustomizeRequestQueryValidator : AbstractValidator<GetAllCustomizeRequestQuery>
    {
        public GetAllCustomizeRequestQueryValidator()
        {
            RuleFor(p => p.CreatedDate)
                .ValidDate().When(p => p.CreatedDate != null);

            RuleFor(p => p.ExpiredDate)
                .ValidDate().When(p => p.ExpiredDate != null);
        }
    }
}
