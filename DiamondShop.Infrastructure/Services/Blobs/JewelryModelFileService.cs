using Azure.Storage.Blobs;
using DiamondShop.Application.Commons.Models;
using DiamondShop.Application.Services.Interfaces;
using DiamondShop.Application.Services.Interfaces.JewelryModels;
using DiamondShop.Domain.Common.ValueObjects;
using DiamondShop.Domain.Models.Diamonds;
using DiamondShop.Domain.Models.JewelryModels;
using DiamondShop.Domain.Models.JewelryModels.Entities;
using DiamondShop.Infrastructure.Options;
using FluentResults;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Infrastructure.Services.Blobs
{
    internal class JewelryModelFileService : AzureBlobContainerService, IJewelryModelFileService
    {
        internal const string PARENT_FOLDER = "Jewelry_Models";
        internal const string IMAGES_FOLDER = "Images";
        internal const string DELIMITER = "/";
        //https://....../blob/Jewelry_model/Jewelry_Model_Id/{metal}/{SD_1}/{SD_2} / {SD_x}/{MD_x}/{MDs_x}/name_timestamp.jpeg

        private readonly ILogger<JewelryModelFileService> _logger;

        public JewelryModelFileService(ILogger<JewelryModelFileService> _loggerSelf , BlobServiceClient blobServiceClient, ILogger<AzureBlobContainerService> logger, IOptions<ExternalUrlsOptions> externalUrlsOptions) : base(blobServiceClient, logger, externalUrlsOptions)
        {
            _logger = _loggerSelf;
        }

        public Task<List<Media>> GetFolders(JewelryModel jewelryModel, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public GalleryTemplate MapPathsToCorrectGallery(JewelryModel jewelryModel, List<Media> paths, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<Result<string[]>> UploadGallery(JewelryModel jewelryModel, FileData[] fileStreams, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<Result<string>> UploadThumbnail(JewelryModel jewelryModel, FileData thumb, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }
        private string GetAzureFilePath(JewelryModel jewelry,Metal metal, List<SideDiamondOpt>? sideDiamondOpt, List<MainDiamondShape>? mainDiamondShape)
        {
            var basePath = GetAzureFilePath(jewelry);
            basePath += $"/{metal.Id.Value}";
            
            if(sideDiamondOpt != null)
            {
                foreach(var sideDiamond in sideDiamondOpt)
                {
                    basePath += $"/{sideDiamond.SideDiamondReqId.Value}_{sideDiamond.Id.Value}";
                }
            }
            if(mainDiamondShape != null)
            {
                foreach(var mainDiamond in mainDiamondShape)
                {
                    basePath += $"/{mainDiamond.MainDiamondReqId.Value}_{mainDiamond.ShapeId.Value}";
                }
            }
            return basePath;
        }
        private string GetAzureFilePath(JewelryModel jewelryModel)
        {
            return $"{PARENT_FOLDER}/{jewelryModel.Id.Value}";
        }
        private string GetTimeStamp()
        {
            return DateTime.UtcNow.Ticks.ToString();
        }
    }
}
