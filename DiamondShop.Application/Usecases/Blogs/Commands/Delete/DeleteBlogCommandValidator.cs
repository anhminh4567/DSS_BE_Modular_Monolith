using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Application.Usecases.Blogs.Commands.Delete
{
    public class DeleteBlogCommandValidator : AbstractValidator<DeleteBlogCommand>
    {
        public DeleteBlogCommandValidator()
        {
            RuleFor(x => x.BlogId).NotEmpty();
        }
    }
}
