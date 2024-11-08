using DiamondShop.Application.Commons.Models;
using DiamondShop.Domain.Common.ValueObjects;
using DiamondShop.Domain.Models.Diamonds;
using DiamondShop.Domain.Models.JewelryModels;
using DiamondShop.Domain.Models.JewelryModels.Entities;
using DiamondShop.Domain.Models.JewelryModels.ValueObjects;
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
        
        Task<Result<string[]>> UploadCategory(JewelryModel jewelryModel, CategoryFileData[] fileStreams, CancellationToken cancellationToken = default);
        Task<Result<string[]>> UploadBaseMetal(JewelryModel jewelryModel, BaseMetalFileData[] fileStreams, CancellationToken cancellationToken = default);
        Task<Result<string[]>> UploadBaseMainDiamond(JewelryModel jewelryModel, BaseMainDiamondFileData[] fileStreams, CancellationToken cancellationToken = default);
        Task<Result<string[]>> UploadBaseSideDiamond(JewelryModel jewelryModel, BaseSideDiamondFileData[] fileStreams, CancellationToken cancellationToken = default);
        Task<Result<string[]>> UploadBase(JewelryModel jewelryModel, FileData[] fileStreams, CancellationToken cancellationToken = default);

        Task<Result<string>> UploadThumbnail(JewelryModel jewelryModel, FileData thumb, CancellationToken cancellationToken = default);
        Task<List<Media>> GetFolders(JewelryModel jewelryModel, CancellationToken cancellationToken = default);
        JewelryModelGalleryTemplate MapPathsToCorrectGallery(JewelryModel jewelryModel, List<Media> paths, CancellationToken cancellationToken = default);
    }
    public record CategoryFileData(MetalId MetalId, SideDiamondOpt? SideDiamondOpts, FileData stream  );//, List<MainDiamondShape>? MainDiamonds
    public record BaseMetalFileData(MetalId MetalId, FileData stream);
    public record BaseMainDiamondFileData(List<MainDiamondReq> MainDiamondRequirements, FileData stream);
    public record BaseSideDiamondFileData(SideDiamondOpt SideDiamondOpt, FileData stream);



}
