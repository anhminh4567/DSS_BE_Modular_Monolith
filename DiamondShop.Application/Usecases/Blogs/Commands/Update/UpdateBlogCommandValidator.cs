using DiamondShop.Domain.Common;
using FluentValidation;
using Microsoft.Extensions.Options;

namespace DiamondShop.Application.Usecases.Blogs.Commands.Update
{
    public class UpdateBlogCommandValidator : AbstractValidator<UpdateBlogCommand>
    {
        public UpdateBlogCommandValidator(IOptionsMonitor<ApplicationSettingGlobal> optionsMonitor)
        {
            var rule = optionsMonitor.CurrentValue.BlogRules;
            RuleFor(p => p.AccountId).NotEmpty();
            RuleFor(p => p.UpdateBlogRequestDto).ChildRules(p =>
            {
                p.RuleFor(k => k.Title).NotEmpty();
                p.RuleForEach(k => k.BlogTags).NotEmpty();
                p.RuleFor(k => k.Thumbnail)
                .Must(k => k.Length <= rule.MaxContentSize)
                .When(k => k.Thumbnail != null)
                .WithMessage(rule.MaxContentSizeError)
                .Must(k => rule.AllowedThumbnailType.Contains(k.ContentType))
                .When(k => k.Thumbnail != null)
                .WithMessage(rule.ContentTypeError);
            });
        }
    }
}
