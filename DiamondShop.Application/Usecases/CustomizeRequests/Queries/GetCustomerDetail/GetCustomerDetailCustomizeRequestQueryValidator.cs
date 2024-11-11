using FluentValidation;

namespace DiamondShop.Application.Usecases.CustomizeRequests.Queries.GetCustomerDetail
{
    public class GetCustomerDetailCustomizeRequestQueryValidator : AbstractValidator<GetCustomerDetailCustomizeRequestQuery>
    {
        public GetCustomerDetailCustomizeRequestQueryValidator()
        {
            RuleFor(p => p.RequestId).NotEmpty();
            RuleFor(p => p.AccountId).NotEmpty();
        }
    }
}
