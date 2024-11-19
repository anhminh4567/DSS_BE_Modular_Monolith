using DiamondShop.Application.Commons.Models;
using DiamondShop.Application.Dtos.Requests.Blogs;
using DiamondShop.Application.Services.Interfaces;
using DiamondShop.Application.Services.Interfaces.Blogs;
using DiamondShop.Domain.Common.ValueObjects;
using DiamondShop.Domain.Models.AccountAggregate.ValueObjects;
using DiamondShop.Domain.Models.Blogs;
using DiamondShop.Domain.Models.Blogs.Entities;
using DiamondShop.Domain.Models.Blogs.ValueObjects;
using DiamondShop.Domain.Repositories.BlogRepo;
using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;

namespace DiamondShop.Application.Usecases.Blogs.Commands.Update
{
    public record UpdateBlogCommand(string AccountId, UpdateBlogRequestDto UpdateBlogRequestDto) : IRequest<Result<Blog>>;
    internal class UpdateBlogCommandHandler : IRequestHandler<UpdateBlogCommand, Result<Blog>>
    {
        private readonly IBlogRepository _blogRepository;
        private readonly IBlogFileService _blogFileService;
        private readonly IUnitOfWork _unitOfWork;

        public UpdateBlogCommandHandler(IBlogRepository blogRepository, IBlogFileService blogFileService, IUnitOfWork unitOfWork)
        {
            _blogRepository = blogRepository;
            _blogFileService = blogFileService;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<Blog>> Handle(UpdateBlogCommand request, CancellationToken token)
        {
            request.Deconstruct(out string accountId, out UpdateBlogRequestDto updateBlogRequestDto);
            updateBlogRequestDto.Deconstruct(out string blogId, out string title, out List<string> blogTags, out IFormFile? thumbnail, out string? content);
            await _unitOfWork.BeginTransactionAsync(token);
            var blog = await _blogRepository.GetById(BlogId.Parse(blogId));
            if (blog == null)
                return Result.Fail("This blog doesn't exist");
            if (blog.AccountId != AccountId.Parse(accountId))
                return Result.Fail("Only the author can edit the blog");
            blog.Title = title;
            blog.Tags = blogTags.Select(p => new BlogTag(p)).ToList();
            if (thumbnail != null)
            {
                var deleteThumbnailFlag = await _blogFileService.DeleteThumbnail(blog, token);
                if (deleteThumbnailFlag.IsFailed)
                    return Result.Fail(deleteThumbnailFlag.Errors);
                var thumbnailResult = await _blogFileService.UploadThumbnail(blog.Id, new FileData("thumbnail", thumbnail.FileName, thumbnail.ContentType, thumbnail.OpenReadStream()), token);
                if (thumbnailResult.IsFailed)
                    return Result.Fail(thumbnailResult.Errors);
                blog.Thumbnail = thumbnailResult.Value;
            }
            await _blogRepository.Update(blog);
            if (!String.IsNullOrEmpty(content))
            {
                var deleteContentFlag = await _blogFileService.DeleteContent(blog.Id, token);
                if (deleteContentFlag.IsFailed)
                    return Result.Fail(deleteContentFlag.Errors);
                var result = await _blogFileService.UploadContent(blog.Id, content);
                if (result.IsFailed)
                    return Result.Fail(result.Errors);
                blog.Content = content;
            }
            await _unitOfWork.SaveChangesAsync(token);
            await _unitOfWork.CommitAsync(token);
            return blog;
        }
    }
}
