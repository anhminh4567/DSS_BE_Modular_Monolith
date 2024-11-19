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
        public CreateBlogCommandValidator(IOptionsMonitor<ApplicationSettingGlobal> optionsMonitor)
        {
            var rule = optionsMonitor.CurrentValue.BlogRules;
            RuleFor(p => p.AccountId).NotEmpty();
            RuleFor(p => p.CreateBlogRequestDto).ChildRules(p =>
            {
                p.RuleFor(k => k.Title).NotEmpty();
                p.RuleForEach(k => k.BlogTags).NotEmpty();
                p.RuleFor(k => k.Thumbnail)
                .Must(k => k.Length <= rule.MaxContentSize)
                .WithMessage(rule.MaxContentSizeError)
                .Must(k => rule.AllowedThumbnailType.Contains(k.ContentType))
                .WithMessage(rule.ContentTypeError);
                p.RuleFor(k => k.Content)
                .NotEmpty();
            });
        }
    }
}
