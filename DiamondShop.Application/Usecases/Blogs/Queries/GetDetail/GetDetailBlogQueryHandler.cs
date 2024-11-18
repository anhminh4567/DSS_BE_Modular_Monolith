using DiamondShop.Application.Commons.Responses;
using DiamondShop.Application.Services.Interfaces.Blogs;
using DiamondShop.Domain.Models.Blogs;
using DiamondShop.Domain.Models.Blogs.ValueObjects;
using DiamondShop.Domain.Repositories.BlogRepo;
using FluentResults;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Application.Usecases.Blogs.Queries.GetDetail
{
    public record GetDetailBlogQuery(string BlogId) : IRequest<Result<Blog>>;
    internal class GetDetailBlogQueryHandler : IRequestHandler<GetDetailBlogQuery, Result<Blog>>
    {
        private readonly IBlogRepository _blogRepository;
        private readonly IBlogFileService _blogFileService;
        public GetDetailBlogQueryHandler(IBlogRepository blogRepository, IBlogFileService blogFileService)
        {
            _blogRepository = blogRepository;
            _blogFileService = blogFileService;
        }

        public async Task<Result<Blog>> Handle(GetDetailBlogQuery request, CancellationToken token)
        {
            request.Deconstruct(out string blogId);
            var blog = await _blogRepository.GetById(BlogId.Parse(blogId));
            if (blog == null)
                return Result.Fail("This blog doesn't exist");
            var contentFlag = await _blogFileService.GetContent(blog.Id, token);
            if (contentFlag.IsFailed)
                return Result.Fail(contentFlag.Errors);
            blog.Content = contentFlag.Value;
            return blog;
        }
    }
}
