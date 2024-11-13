using DiamondShop.Application.Commons.Validators;
using DiamondShop.Domain.Models.CustomizeRequests.Enums;
using DiamondShop.Domain.Models.Orders.Enum;
using FluentValidation;

namespace DiamondShop.Application.Usecases.CustomizeRequests.Queries.GetAll
{
    public class GetAllCustomizeRequestQueryValidator : AbstractValidator<GetAllCustomizeRequestQuery>
    {
        public GetAllCustomizeRequestQueryValidator()
        {
            RuleFor(p => p.Email)
                .EmailAddress().When(p => p.Email != null);

            RuleFor(p => p.Status)
                .Must(p => Enum.IsDefined(typeof(CustomizeRequestStatus), p)).When(p => p.Status != null);
        }
    }
}
