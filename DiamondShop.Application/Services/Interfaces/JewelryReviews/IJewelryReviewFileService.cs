using DiamondShop.Application.Commons.Models;
using DiamondShop.Domain.Common.ValueObjects;
using DiamondShop.Domain.Models.Jewelries;
using DiamondShop.Domain.Models.JewelryModels.ValueObjects;
using FluentResults;

namespace DiamondShop.Application.Services.Interfaces.JewelryReviews
{
    public interface IJewelryReviewFileService : IBlobFileServices
    {
        Task<Result<string[]>> UploadReview(Jewelry jewelry, FileData[] streams, CancellationToken token = default);
        Task<List<Media>> GetFolders(JewelryModelId modelId, MetalId? metalId, CancellationToken token = default);
        Task<List<Media>> GetFolders(Jewelry jewelry, CancellationToken token = default);
        Task<Result> DeleteFiles(Jewelry jewelry, CancellationToken token = default);
    }
}
