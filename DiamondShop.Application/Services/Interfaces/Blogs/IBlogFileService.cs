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
        Task<Result<string[]>> UploadBlogContent(BlogId blogId, FileData[] streams, CancellationToken token = default);
        Task<List<Media>> GetFolders(BlogId blogId, CancellationToken token = default);
        Task<Result> DeleteFiles(BlogId blogId, CancellationToken token = default);
    }
}
