using DiamondShop.Domain.Common;
using FluentValidation;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Application.Usecases.JewelryReviews.Commands.Create
{
    public class CreateJewelryReviewCommandValidator : AbstractValidator<CreateJewelryReviewCommand>
    {
        public CreateJewelryReviewCommandValidator(IOptionsMonitor<ApplicationSettingGlobal> optionsMonitor)
        {
            var rule = optionsMonitor.CurrentValue.JewelryReviewRules;
            RuleFor(p => p.AccountId).NotEmpty();
            RuleFor(p => p.JewelryReviewRequestDto).ChildRules(p =>
            {
                p.RuleFor(k => k.JewelryId).NotEmpty();
                p.RuleFor(k => k.Content).NotEmpty();
                p.RuleFor(k => k.StarRating).InclusiveBetween(1, 5);
                p.RuleFor(k => k.Files).Must(k => k.Count() <= rule.MaxFileAllowed).WithMessage(rule.MaxFileAllowedError).When(k => k != null);
                p.RuleForEach(k => k.Files)
                .NotNull()
                .Must(p => p.Length <= rule.MaxContentSize).WithMessage(rule.MaxContentSizeError)
                .Must(p => rule.AllowedContentType.Contains(p.ContentType)).WithMessage(rule.ContentTypeError)
                .When(k => k != null);
            });
        }
    }
}
