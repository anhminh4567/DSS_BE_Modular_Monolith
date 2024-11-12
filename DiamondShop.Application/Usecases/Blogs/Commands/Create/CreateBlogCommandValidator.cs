using DiamondShop.Domain.Common;
using FluentValidation;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Application.Usecases.Blogs.Commands.Create
{
    public class CreateBlogCommandValidator : AbstractValidator<CreateBlogCommand>
    {
        private readonly IOptionsMonitor<ApplicationSettingGlobal> _optionsMonitor;
        public CreateBlogCommandValidator(IOptionsMonitor<ApplicationSettingGlobal> optionsMonitor)
        {
            var rule = _optionsMonitor.CurrentValue.BlogRules;
            RuleFor(p => p.AccountId).NotEmpty();
            RuleFor(p => p.CreateBlogRequestDto).ChildRules(p =>
            {
                p.RuleFor(k => k.Title).NotEmpty();
                p.RuleForEach(k => k.BlogTags).NotEmpty();
                p.RuleFor(k => k.Thumbnail)
                .Must(k => k.Length <= rule.MaxContentSize)
                .WithMessage($"Thumbnail must be under {rule.MaxContentSizeInMb} Mb")
                .Must(k => rule.AllowedThumbnailType.Contains(k.ContentType))
                .WithMessage($"Thumbnail doens't support this type of file");
                p.RuleForEach(k => k.Contents)
                .Must(k => k.Length <= rule.MaxContentSize)
                .WithMessage($"File must be under {rule.MaxContentSizeInMb} Mb")
                .Must(k => rule.AllowedContentType.Contains(k.ContentType))
                .WithMessage($"Contents don't support this type of file");
            });
        }
    }
}
