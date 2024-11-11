using DiamondShop.Application.Commons.Models;
using DiamondShop.Application.Services.Interfaces;
using DiamondShop.Application.Services.Interfaces.Blogs;
using DiamondShop.Application.Services.Interfaces.JewelryReviews;
using DiamondShop.Domain.Common.ValueObjects;
using DiamondShop.Domain.Models.AccountAggregate.ValueObjects;
using DiamondShop.Domain.Models.Blogs;
using DiamondShop.Domain.Models.Blogs.Entities;
using DiamondShop.Domain.Models.Jewelries;
using DiamondShop.Domain.Repositories.BlogRepo;
using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ViewEngines;

namespace DiamondShop.Application.Usecases.Blogs.Commands.Create
{
    public record CreateBlogRequestDto(string Title, List<string> BlogTags, IFormFile? Thumbnail, IFormFile[] Contents);
    public record CreateBlogCommand(string AccountId, CreateBlogRequestDto CreateBlogRequestDto) : IRequest<Result<Blog>>;
    internal class CreateBlogCommandHandler : IRequestHandler<CreateBlogCommand, Result<Blog>>
    {
        private readonly IBlogRepository _blogRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IBlogFileService _blogFileService;
        public async Task<Result<Blog>> Handle(CreateBlogCommand request, CancellationToken token)
        {
            request.Deconstruct(out string accountId, out CreateBlogRequestDto createBlogRequestDto);
            createBlogRequestDto.Deconstruct(out string title, out List<string> blogTags, out IFormFile? thumbnail, out IFormFile[] contents);
            await _unitOfWork.BeginTransactionAsync(token);
            Blog blog = Blog.Create(blogTags.Select(p => new BlogTag(p)).ToList(), AccountId.Parse(accountId), title);
            await _blogRepository.Create(blog);
            await _unitOfWork.SaveChangesAsync(token);
            if (thumbnail != null)
            {
                var thumbnailResult = await _blogFileService.UploadThumbnail(blog.Id, new FileData("thumbnail", thumbnail.FileName, thumbnail.ContentType, thumbnail.OpenReadStream()), token);
                if(thumbnailResult.IsFailed)
                    return Result.Fail(thumbnailResult.Errors);
                blog.Thumbnail = thumbnailResult.Value;
                await _blogRepository.Update(blog);
                await _unitOfWork.SaveChangesAsync(token);
            }
            if (contents.Count() > 0)
            {
                FileData[] fileDatas = contents.Select(f => new FileData(f.Name, f.ContentType, f.ContentType, f.OpenReadStream())).ToArray();
                var result = await _blogFileService.UploadBlogContent(blog.Id, fileDatas);
                if (result.IsFailed)
                    return Result.Fail(result.Errors);
                blog.Medias.AddRange(result.Value.Select(p => Media.Create("", p, p.Split(".")[1])));
            }
            await _unitOfWork.CommitAsync(token);
            return blog;
        }
    }
}
