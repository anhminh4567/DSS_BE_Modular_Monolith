using FluentValidation;

namespace DiamondShop.Application.Usecases.CustomizeRequests.Queries.GetDetail
{
    public class GetDetailCustomizeRequestQueryValidator : AbstractValidator<GetDetailCustomizeRequestQuery>
    {
        public GetDetailCustomizeRequestQueryValidator()
        {
            RuleFor(p => p.RequestId).NotEmpty();
        }
    }
}
