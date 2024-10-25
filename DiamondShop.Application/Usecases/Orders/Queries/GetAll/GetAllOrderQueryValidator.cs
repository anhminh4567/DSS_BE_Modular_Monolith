using FluentValidation;

namespace DiamondShop.Application.Usecases.Orders.Queries.GetAll
{
    public class GetAllOrderQueryValidator : AbstractValidator<GetAllOrderQuery>
    {
        public GetAllOrderQueryValidator()
        {
            RuleFor(p => p.Role).NotEmpty();
            RuleFor(p => p.AccountId).NotEmpty();
        }
    }
}
