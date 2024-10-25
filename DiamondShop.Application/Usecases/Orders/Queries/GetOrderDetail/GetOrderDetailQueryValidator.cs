using DiamondShop.Application.Usecases.Orders.Queries.GetUserOrderDetail;
using FluentValidation;

namespace DiamondShop.Application.Usecases.Orders.Queries.GetAll
{
    public class GetOrderDetailQueryValidator : AbstractValidator<GetOrderDetailQuery>
    {
        public GetOrderDetailQueryValidator()
        {
            RuleFor(p => p.OrderId).NotEmpty();
            RuleFor(p => p.Role).NotEmpty();
            RuleFor(p => p.AccountId).NotEmpty();
        }
    }
}
