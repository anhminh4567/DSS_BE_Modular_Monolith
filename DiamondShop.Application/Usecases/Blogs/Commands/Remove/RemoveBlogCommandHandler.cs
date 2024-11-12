using DiamondShop.Application.Dtos.Requests.Blogs;
using DiamondShop.Application.Services.Interfaces.Blogs;
using DiamondShop.Application.Services.Interfaces;
using DiamondShop.Application.Usecases.Blogs.Commands.Update;
using DiamondShop.Domain.Models.Blogs;
using DiamondShop.Domain.Repositories.BlogRepo;
using FluentResults;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DiamondShop.Domain.Models.AccountAggregate.ValueObjects;
using DiamondShop.Domain.Models.Blogs.ValueObjects;
using Newtonsoft.Json.Linq;

namespace DiamondShop.Application.Usecases.Blogs.Commands.Remove
{
    public record RemoveBlogCommand(string BlogId, string AccountId) : IRequest<Result>;
    internal class RemoveBlogCommandHandler : IRequestHandler<RemoveBlogCommand, Result>
    {
        private readonly IBlogRepository _blogRepository;
        private readonly IBlogFileService _blogFileService;
        private readonly IUnitOfWork _unitOfWork;

        public RemoveBlogCommandHandler(IBlogRepository blogRepository, IBlogFileService blogFileService, IUnitOfWork unitOfWork)
        {
            _blogRepository = blogRepository;
            _blogFileService = blogFileService;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result> Handle(RemoveBlogCommand request, CancellationToken token)
        {
            request.Deconstruct(out string blogId, out string accountId);
            await _unitOfWork.BeginTransactionAsync(token);
            var blog = await _blogRepository.GetById(BlogId.Parse(blogId));
            if (blog == null)
                return Result.Fail("This blog doesn't exist");
            if (blog.AccountId == AccountId.Parse(accountId))
                return Result.Fail("Only the author can remove the blog");
            var deleteFlag = await _blogFileService.DeleteFiles(blog.Id, token);
            if (deleteFlag.IsFailed)
                return Result.Fail(deleteFlag.Errors);
            await _blogRepository.Delete(blog);
            await _unitOfWork.SaveChangesAsync(token);
            await _unitOfWork.CommitAsync(token);
            return Result.Ok();
        }
    }
}
