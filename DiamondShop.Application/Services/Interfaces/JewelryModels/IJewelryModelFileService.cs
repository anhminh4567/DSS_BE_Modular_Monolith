using DiamondShop.Application.Commons.Models;
using DiamondShop.Domain.Common.ValueObjects;
using DiamondShop.Domain.Models.Diamonds;
using DiamondShop.Domain.Models.JewelryModels;
using FluentResults;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Application.Services.Interfaces.JewelryModels
{
    public interface IJewelryModelFileService : IBlobFileServices
    {
        Task<Result<string[]>> UploadGallery(JewelryModel jewelryModel, FileData[] fileStreams, CancellationToken cancellationToken = default);
        Task<Result<string>> UploadThumbnail(JewelryModel jewelryModel, FileData thumb, CancellationToken cancellationToken = default);
        Task<List<Media>> GetFolders(JewelryModel jewelryModel, CancellationToken cancellationToken = default);
        GalleryTemplate MapPathsToCorrectGallery(JewelryModel jewelryModel, List<Media> paths, CancellationToken cancellationToken = default);
    }
}
