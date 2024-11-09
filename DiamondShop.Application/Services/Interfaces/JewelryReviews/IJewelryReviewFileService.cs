using DiamondShop.Application.Commons.Models;
using DiamondShop.Domain.Common.ValueObjects;
using DiamondShop.Domain.Models.Jewelries;
using DiamondShop.Domain.Models.Jewelries.Entities;
using DiamondShop.Domain.Models.JewelryModels.ValueObjects;
using FluentResults;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
