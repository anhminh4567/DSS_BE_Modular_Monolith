using DiamondShop.Application.Commons.Validators;
using FluentValidation;

namespace DiamondShop.Application.Usecases.Diamonds.Queries.DashBoard.GetBestSellingCaratRangeForShape
{
    public class GetBestSellingCaratRangeForShapeQueryValidator : AbstractValidator<GetBestSellingCaratRangeForShapeQuery>
    {
        public GetBestSellingCaratRangeForShapeQueryValidator()
        {
            RuleFor(x => x.shapeId).NotEmpty().WithMessage("ShapeId is required");
            RuleFor(x => x.caratFrom).Cascade(CascadeMode.Stop)
                .NotNull()
                .WithMessage("CaratFrom is required")
                .GreaterThanOrEqualTo(0)
                .WithMessage("must greater than 0 to copmare");
            RuleFor(x => x.caratTo).Cascade(CascadeMode.Stop)
                .NotNull()
                .WithMessage("Carat To is required")
                .GreaterThanOrEqualTo(0)
                .WithMessage("must greater than 0 to copmare");

            RuleFor(x => x.startDate).ValidDate()
                .When(x => x.startDate != null);
            RuleFor(x => x.endDate).ValidDate()
                .When(x => x.endDate != null);
            RuleFor(x => x).ValidStartEndDate(x => x.startDate,x => x.endDate)
                .When(x => x.startDate !=null && x.endDate != null);
        }
    }
}
