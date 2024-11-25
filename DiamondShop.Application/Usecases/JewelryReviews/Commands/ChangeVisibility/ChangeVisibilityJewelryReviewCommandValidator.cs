using FluentValidation;

namespace DiamondShop.Application.Usecases.JewelryReviews.Commands.ChangeVisibility
{
    public class ChangeVisibilityJewelryReviewCommandValidator : AbstractValidator<ChangeVisibilityJewelryReviewCommand>
    {
        public ChangeVisibilityJewelryReviewCommandValidator()
        {
            RuleFor(c => c.JewelryId).NotEmpty();
        }
    }
}
