using DiamondShop.Application.Commons.Validators;
using DiamondShop.Domain.Models.CustomizeRequests.Enums;
using FluentValidation;

namespace DiamondShop.Application.Usecases.CustomizeRequests.Queries.GetCustomer
{
    public class GetCustomerCustomizeRequestQueryValidator : AbstractValidator<GetCustomerCustomizeRequestQuery>
    {
        public GetCustomerCustomizeRequestQueryValidator()
        {
            RuleFor(p => p.AccountId)
                .NotEmpty();
            RuleFor(p => p.GetCustomerRequestDto)
                .ChildRules(p =>
                {
                    p.RuleFor(k => k.CreatedDate)
                        .ValidDate().When(k => k.CreatedDate != null);

                    p.RuleFor(k => k.ExpiredDate)
                        .ValidDate().When(k => k.ExpiredDate != null);

                    p.RuleFor(k => k.Status)
                        .Must(k => Enum.IsDefined(typeof(CustomizeRequestStatus), k)).When(k => k.Status != null);
                });
        }
    }
}
