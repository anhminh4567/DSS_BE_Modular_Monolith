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
        private readonly IOptionsMonitor<ApplicationSettingGlobal> _optionsMonitor;

        public CreateJewelryReviewCommandValidator(IOptionsMonitor<ApplicationSettingGlobal> optionsMonitor)
        {
            var rule = optionsMonitor.CurrentValue.JewelryReviewRules;
            RuleFor(p => p.AccountId).NotEmpty();
            RuleFor(p => p.JewelryReviewRequestDto).ChildRules(p =>
            {
                p.RuleFor(k => k.JewelryId).NotEmpty();
                p.RuleFor(k => k.Content).NotEmpty();
                p.RuleFor(k => k.StarRating).InclusiveBetween(1, 5);
                p.RuleFor(k => k.Files).Must(k => k.Count() <= rule.MaxFileAllowed).WithMessage("You can only upload maximum of 5 files").When(k => k != null);
                p.RuleForEach(k => k.Files)
                .NotNull()
                .Must(p => p.Length <= rule.MaxContentSize).WithMessage($"File must be under {rule.MaxContentSizeInMb} Mb")
                .Must(p => rule.AllowedContentType.Contains(p.ContentType)).WithMessage($"App doens't support this type of file")
                .When(k => k != null);
            });
        }
    }
}
