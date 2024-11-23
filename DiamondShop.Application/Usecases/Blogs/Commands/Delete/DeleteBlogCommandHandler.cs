using DiamondShop.Application.Services.Interfaces;
using DiamondShop.Application.Services.Interfaces.Blogs;
using DiamondShop.Domain.Models.AccountAggregate.ValueObjects;
using DiamondShop.Domain.Models.Blogs.ErrorMessages;
using DiamondShop.Domain.Models.Blogs.ValueObjects;
using DiamondShop.Domain.Models.CustomizeRequests.ErrorMessages;
using DiamondShop.Domain.Models.Orders.ErrorMessages;
using DiamondShop.Domain.Models.RoleAggregate;
using DiamondShop.Domain.Repositories.BlogRepo;
using FluentResults;
using MediatR;

namespace DiamondShop.Application.Usecases.Blogs.Commands.Delete
{
    public record DeleteBlogCommand(string BlogId, List<string> Roles, string AccountId) : IRequest<Result>;
    internal class DeleteBlogCommandHandler : IRequestHandler<DeleteBlogCommand, Result>
    {
        private readonly IBlogRepository _blogRepository;
        private readonly IBlogFileService _blogFileService;
        private readonly IUnitOfWork _unitOfWork;

        public DeleteBlogCommandHandler(IBlogRepository blogRepository, IBlogFileService blogFileService, IUnitOfWork unitOfWork)
        {
            _blogRepository = blogRepository;
            _blogFileService = blogFileService;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result> Handle(DeleteBlogCommand request, CancellationToken token)
        {
            request.Deconstruct(out string blogId, out List<string> roles, out string accountId);
            await _unitOfWork.BeginTransactionAsync(token);
            var blog = await _blogRepository.GetById(BlogId.Parse(blogId));
            if (blog == null)
                return Result.Fail(BlogErrors.BlogNotFoundError);
            if (!roles.Contains(AccountRole.ManagerId)&& blog.AccountId != AccountId.Parse(accountId))
                return Result.Fail(BlogErrors.NoPermissionError);
            List<Task<Result>> tasks = new()
            {
                _blogFileService.DeleteThumbnail(blog, token),
                _blogFileService.DeleteContent(blog.Id, token)
            };
            var results = await Task.WhenAll(tasks);
            if (results.Where(p => p.IsSuccess).Count() != results.Count())
            {
                var errors = new List<IError>();
                foreach (var result in results)
                {
                    errors.AddRange(result.Errors);
                }
                return Result.Fail(errors);
            }
            await _blogRepository.Delete(blog);
            await _unitOfWork.SaveChangesAsync(token);
            await _unitOfWork.CommitAsync(token);
            return Result.Ok();
        }
    }
}
