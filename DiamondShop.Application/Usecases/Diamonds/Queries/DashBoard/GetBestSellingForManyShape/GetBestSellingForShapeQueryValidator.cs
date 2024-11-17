using DiamondShop.Application.Commons.Validators;
using FluentValidation;

namespace DiamondShop.Application.Usecases.Diamonds.Queries.DashBoard.GetBestSellingForManyShape
{
    public class GetBestSellingForShapeQueryValidator : AbstractValidator<GetBestSellingForShapeQuery>
    {
        public GetBestSellingForShapeQueryValidator()
        {
            RuleFor(x => x.startDate).ValidDate()
                .When(x => x.startDate != null);
            RuleFor(x => x.endDate).ValidDate()
                .When(x => x.endDate != null);
            RuleFor(x => x).ValidStartEndDate(x => x.startDate, x => x.endDate)
                .When(x => x.startDate != null && x.endDate != null);
        }
    }

}
