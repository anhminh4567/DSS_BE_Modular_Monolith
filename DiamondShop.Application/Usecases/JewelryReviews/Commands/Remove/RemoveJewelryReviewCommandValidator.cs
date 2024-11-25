using DiamondShop.Application.Usecases.JewelryReviews.Commands.ChangeVisibility;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Application.Usecases.JewelryReviews.Commands.Remove
{
    internal class RemoveJewelryReviewCommandValidator : AbstractValidator<ChangeVisibilityJewelryReviewCommand>
    {
        public RemoveJewelryReviewCommandValidator()
        {
            RuleFor(c => c.JewelryId).NotEmpty();
        }
    }
}
