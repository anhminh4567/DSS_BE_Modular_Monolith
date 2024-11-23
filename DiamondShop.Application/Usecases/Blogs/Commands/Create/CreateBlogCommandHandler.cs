using DiamondShop.Application.Commons.Models;
using DiamondShop.Application.Dtos.Requests.Blogs;
using DiamondShop.Application.Services.Interfaces;
using DiamondShop.Application.Services.Interfaces.Blogs;
using DiamondShop.Domain.Common.ValueObjects;
using DiamondShop.Domain.Models.AccountAggregate.ValueObjects;
using DiamondShop.Domain.Models.Blogs;
using DiamondShop.Domain.Models.Blogs.Entities;
using DiamondShop.Domain.Repositories.BlogRepo;
using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace DiamondShop.Application.Usecases.Blogs.Commands.Create
{

    public record CreateBlogCommand(string AccountId, CreateBlogRequestDto CreateBlogRequestDto) : IRequest<Result<Blog>>;
    internal class CreateBlogCommandHandler : IRequestHandler<CreateBlogCommand, Result<Blog>>
    {
        private readonly IBlogRepository _blogRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IBlogFileService _blogFileService;

        public CreateBlogCommandHandler(IBlogRepository blogRepository, IUnitOfWork unitOfWork, IBlogFileService blogFileService)
        {
            _blogRepository = blogRepository;
            _unitOfWork = unitOfWork;
            _blogFileService = blogFileService;
        }

        public async Task<Result<Blog>> Handle(CreateBlogCommand request, CancellationToken token)
        {
            request.Deconstruct(out string accountId, out CreateBlogRequestDto createBlogRequestDto);
            createBlogRequestDto.Deconstruct(out string title, out List<string> blogTags, out IFormFile? thumbnail, out string content);
            await _unitOfWork.BeginTransactionAsync(token);
            Blog blog = Blog.Create(blogTags.Select(p => new BlogTag(p)).ToList(), AccountId.Parse(accountId), title);
            await _blogRepository.Create(blog);
            await _unitOfWork.SaveChangesAsync(token);
            var thumbnailResult = await _blogFileService.UploadThumbnail(blog.Id, new FileData("thumbnail", thumbnail.FileName, thumbnail.ContentType, thumbnail.OpenReadStream()), token);
            if (thumbnailResult.IsFailed)
                return Result.Fail(thumbnailResult.Errors);
            blog.Thumbnail = thumbnailResult.Value;
            await _blogRepository.Update(blog);
            await _unitOfWork.SaveChangesAsync(token);
            var result = await _blogFileService.UploadContent(blog.Id, content);
            if (result.IsFailed)
                return Result.Fail(result.Errors);
            blog.Content = content;
            await _unitOfWork.CommitAsync(token);
            return blog;
        }
    }
}
