using DiamondShop.Application.Commons.Models;
using DiamondShop.Domain.Common.ValueObjects;
using DiamondShop.Domain.Models.Blogs;
using DiamondShop.Domain.Models.Blogs.ValueObjects;
using FluentResults;

namespace DiamondShop.Application.Services.Interfaces.Blogs
{
    public interface IBlogFileService : IBlobFileServices
    {
        Task<Result<Media>> UploadThumbnail(BlogId blogId, FileData fileData, CancellationToken token = default);
        Task<Result<string>> UploadContent(BlogId blogId, string content, CancellationToken token = default);
        Task<Result<string>> GetContent(BlogId blogId, CancellationToken token = default);
        Task<Result> DeleteThumbnail(Blog blog, CancellationToken token = default);
        Task<Result> DeleteContent(BlogId blogId, CancellationToken token = default);
    }
}
