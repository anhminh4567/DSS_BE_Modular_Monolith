using Azure.Storage.Blobs;
using DiamondShop.Application.Commons.Models;
using DiamondShop.Application.Services.Interfaces;
using DiamondShop.Application.Services.Interfaces.JewelryModels;
using DiamondShop.Domain.Common.ValueObjects;
using DiamondShop.Domain.Models.Diamonds;
using DiamondShop.Domain.Models.Diamonds.ValueObjects;
using DiamondShop.Domain.Models.JewelryModels;
using DiamondShop.Domain.Models.JewelryModels.Entities;
using DiamondShop.Domain.Models.JewelryModels.ValueObjects;
using DiamondShop.Infrastructure.Options;
using FluentEmail.Core;
using FluentResults;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Syncfusion.XlsIO.Parser.Biff_Records.Formula;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DiamondShop.Infrastructure.Services.Blobs
{
    internal class JewelryModelFileService : AzureBlobContainerService, IJewelryModelFileService
    {
        internal const string PARENT_FOLDER = "Jewelry_Models";
        internal const string IMAGES_FOLDER = "Images";
        internal const string BASE_IMAGES_FOLDER = IMAGES_FOLDER + "/" + "BaseImages";
        internal const string BASE_METAL_IMAGES_FOLDER = BASE_IMAGES_FOLDER + "/" + "Metals";
        internal const string BASE_SIDE_DIAMOND_IMAGES_FOLDER = BASE_IMAGES_FOLDER + "/" + "SideDiamonds";
        internal const string BASE_MAIN_DIAMOND_IMAGES_FOLDER = BASE_IMAGES_FOLDER + "/" + "MainDiamonds";

        internal const string CATEGORIZED_IMAGES_FOLDER = IMAGES_FOLDER + "/" + "Categories";

        internal const string DELIMITER = "/";
        //https://....../blob/Jewelry_model/Jewelry_Model_Id/{metal}/{SD_1}/{SD_2}/{SD_3} / {SD_x}/{MD_x}/{MDs_x}/name_timestamp.jpeg

        private readonly ILogger<JewelryModelFileService> _logger;

        public JewelryModelFileService(ILogger<JewelryModelFileService> _loggerSelf, BlobServiceClient blobServiceClient, ILogger<AzureBlobContainerService> logger, IOptions<ExternalUrlsOptions> externalUrlsOptions) : base(blobServiceClient, logger, externalUrlsOptions)
        {
            _logger = _loggerSelf;
        }

        public Task<List<Media>> GetFolders(JewelryModel jewelryModel, CancellationToken cancellationToken = default)
        {
            var basePath = GetAzureFilePath(jewelryModel);
            return base.GetFolders(basePath, cancellationToken);
        }

        public GalleryTemplate MapPathsToCorrectGallery(JewelryModel jewelryModel, List<Media> paths, CancellationToken cancellationToken = default)
        {
            var gallery = new GalleryTemplate();
            gallery.GalleryFolder = GetAzureFilePath(jewelryModel);
            var basePath = GetAzureFilePath(jewelryModel);
            // init gallery dictionary and set keys 
            List<List<SideDiamondOpt>> listSideDiamonds = new();
            List<List<MainDiamondShape>> listMainDiamonds = new();
            //get all possible combinations of side diamonds and main diamonds
            //MapCorrectSideDiamondOptionCombinationBackTrack(jewelryModel.SideDiamonds, 0, new List<SideDiamondOpt>(), listSideDiamonds);
            MapCorrectMainDiamondCombinationBackTrack(jewelryModel.MainDiamonds, 0, new List<MainDiamondShape>(), listMainDiamonds);
            // init keys
            Dictionary<string, List<Media>> galleries = new();
            Action<string, Media> AddToCategory = (string tobeComparedPath, Media originalMedia) =>
            {
                int lastSlashIndex = tobeComparedPath.LastIndexOf('/');
                if (lastSlashIndex >= 0)
                {
                    var key = tobeComparedPath.Substring(0, lastSlashIndex);
                    if (!galleries.ContainsKey(key))
                        galleries[key] = new List<Media>();
                    galleries[key].Add(originalMedia);
                }
            };

            foreach (var path in paths)
            {
                var tobeComparedPath = path.MediaPath.Replace(GetAzureFilePath(jewelryModel), string.Empty);
                // handle images 
                if (tobeComparedPath.StartsWith("/" + CATEGORIZED_IMAGES_FOLDER))
                {
                    AddToCategory(tobeComparedPath, path);
                }
                else if (tobeComparedPath.StartsWith("/" + BASE_IMAGES_FOLDER))
                {
                    if (tobeComparedPath.StartsWith("/" + BASE_METAL_IMAGES_FOLDER))
                        AddToCategory(tobeComparedPath, path);

                    else if (tobeComparedPath.StartsWith("/" + BASE_MAIN_DIAMOND_IMAGES_FOLDER))
                        AddToCategory(tobeComparedPath, path);

                    else if (tobeComparedPath.StartsWith("/" + BASE_SIDE_DIAMOND_IMAGES_FOLDER))
                        AddToCategory(tobeComparedPath, path);

                    else
                        gallery.BaseImages.Add(path);
                }
                else
                {
                    gallery.Thumbnail = path;
                }
            }
            gallery.Gallery = galleries;
            return gallery;
        }

        public async Task<Result<string>> UploadThumbnail(JewelryModel jewelryModel, FileData thumb, CancellationToken cancellationToken = default)
        {
            string basePath = GetAzureFilePath(jewelryModel);
            string finalpath = $"{basePath}/{thumb.FileName}_{GetTimeStamp()}";
            var resutl = await base.UploadFileAsync(finalpath, thumb.Stream, thumb.contentType, cancellationToken);
            return resutl;
        }
        private string GetAzureCategoryFilePath(JewelryModel jewelryModel, string nameIdentifier)
        {
            var basePath = GetAzureFilePath(jewelryModel);
            basePath += $"/{CATEGORIZED_IMAGES_FOLDER}";
            basePath += $"/{nameIdentifier}";
            return basePath;
        }
        private string GetAzureMetalPathIdentifier(string fromBaseString, MetalId metalId)
        {
            //fromBaseString += $"/{BASE_METAL_IMAGES_FOLDER}";
            fromBaseString += $"/{metalId.Value}";
            return fromBaseString;
        }
        private string GetAzureMainDiamondPathIdentifier(string fromBaseString, List<MainDiamondShape>? combinedShapes)
        {
            throw new NotImplementedException();
            //if (combinedShapes != null)
            //    foreach (var mainDiamond in combinedShapes)
            //        fromBaseString += $"/{mainDiamond.MainDiamondReqId.Value}_{mainDiamond.ShapeId.Value}";
            //return fromBaseString;
        }
        private string GetAzureSideDiamondPathIdentifier(string fromBaseString, List<SideDiamondOpt>? combinedOptions)
        {
            throw new NotImplementedException();
            //if (combinedOptions != null)
            //    foreach (var sideDiamond in combinedOptions)
            //        fromBaseString += $"/{sideDiamond.SideDiamondReqId.Value}_{sideDiamond.Id.Value}";
            //return fromBaseString;
        }

        private string GetCategoryIdentifierName(MetalId metalId, List<SideDiamondOpt>? sideDiamondOpt, List<MainDiamondShape>? mainDiamondShape)
        {
            throw new NotImplementedException();
            //string basePath = $"{metalId.Value}";
            //if (mainDiamondShape != null)
            //    foreach (var mainDiamond in mainDiamondShape)
            //        basePath += $"/{mainDiamond.MainDiamondReqId.Value}_{mainDiamond.ShapeId.Value}";
            //if (sideDiamondOpt != null)
            //    foreach (var sideDiamond in sideDiamondOpt)
            //        basePath += $"/{sideDiamond.SideDiamondReqId.Value}_{sideDiamond.Id.Value}";
            //return basePath;
        }
        private string GetAzureFilePath(JewelryModel jewelryModel)
        {
            return $"{PARENT_FOLDER}/{jewelryModel.Id.Value}";
        }
        private string GetTimeStamp()
        {
            throw new NotImplementedException();
            //return DateTime.UtcNow.Ticks.ToString();
        }
        //private static void MapCorrectSideDiamondOptionCombinationBackTrack(List<SideDiamondReq> requirements, int reqPosition, List<SideDiamondOpt> currentCombination, List<List<SideDiamondOpt>> storedResult)
        //{
        //    if (reqPosition == requirements.Count)
        //    {
        //        storedResult.Add(currentCombination);
        //        return;
        //    }
        //    var currentReq = requirements[reqPosition];
        //    foreach (var opt in currentReq.SideDiamondOpts)
        //    {
        //        var newCombination = new List<SideDiamondOpt>(currentCombination);
        //        newCombination.Add(opt);
        //        MapCorrectSideDiamondOptionCombinationBackTrack(requirements, reqPosition + 1, newCombination, storedResult);
        //    }
        //}
        private static void MapCorrectMainDiamondCombinationBackTrack(List<MainDiamondReq> requirements, int reqPosition, List<MainDiamondShape> currentCombination, List<List<MainDiamondShape>> storedResult)
        {
            throw new NotImplementedException();
            //if (reqPosition == requirements.Count)
            //{
            //    storedResult.Add(currentCombination);
            //    return;
            //}
            //var currentReq = requirements[reqPosition];
            //foreach (var opt in currentReq.Shapes)
            //{
            //    var newCombination = new List<MainDiamondShape>(currentCombination);
            //    newCombination.Add(opt);
            //    MapCorrectMainDiamondCombinationBackTrack(requirements, reqPosition + 1, newCombination, storedResult);
            //}
        }

        public async Task<Result<string[]>> UploadCategory(JewelryModel jewelryModel, CategoryFileData[] fileStreams, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
            //get all base key for metals, side diamonds and main diamonds[BASE_... ]
            //List<(FileData fileData, string relativePath)> fileDataTobeUploaded = new();
            ////init keys
            //foreach (var file in fileStreams)
            //{
            //    string FinalPath = GetCategoryIdentifierName(file.MetalId, file.SideDiamondOpts, file.MainDiamonds);
            //    fileDataTobeUploaded.Add(new (new FileData(file.stream.FileName, file.stream.FileExtension, file.stream.contentType,file.stream.Stream),FinalPath));
            //}
            //string basePath = GetAzureFilePath(jewelryModel);
            //basePath = $"{basePath}/{CATEGORIZED_IMAGES_FOLDER}";
            //List<Task<Result<string[]>>> uploadTasks = new();
            //foreach (var file in fileDataTobeUploaded)
            //{
            //    var finalPath = $"{basePath}/{file.relativePath}";
            //    uploadTasks.Add(UploadFromBasePath(finalPath, new List<FileData> { file.fileData }.ToArray(), cancellationToken));
            //}
            //var results = await Task.WhenAll(uploadTasks);
            //var stringResult = results.Where(r => r.IsSuccess).SelectMany(r => r.Value).ToArray();
            //if (stringResult.Length == 0)
            //    return Result.Fail("Failed to upload any files at all");
            //return Result.Ok(stringResult);
        }

        public async Task<Result<string[]>> UploadBaseMetal(JewelryModel jewelryModel, BaseMetalFileData[] fileStreams, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
            //string basePath = GetAzureFilePath(jewelryModel);
            //basePath = $"{basePath}/{BASE_METAL_IMAGES_FOLDER}";
            //List<Task<Result<string[]>>> uploadTasks = new();
            //foreach (var file in fileStreams)
            //{
            //    var finalPath = $"{basePath}/{file.MetalId.Value}";
            //    uploadTasks.Add(UploadFromBasePath(finalPath, new List<FileData> { file.stream }.ToArray(), cancellationToken));
            //}
            //var results = await Task.WhenAll(uploadTasks);
            //var stringResult = results.Where(r => r.IsSuccess).SelectMany(r => r.Value).ToArray();
            //if (stringResult.Length == 0)
            //    return Result.Fail("Failed to upload any files at all");
            //return Result.Ok(stringResult);
        }

        public async Task<Result<string[]>> UploadBaseMainDiamond(JewelryModel jewelryModel, BaseMainDiamondFileData[] fileStreams, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
            //string basePath = GetAzureFilePath(jewelryModel);
            //basePath = $"{basePath}/{BASE_MAIN_DIAMOND_IMAGES_FOLDER}";
            //List<Task<Result<string[]>>> uploadTasks = new();
            //foreach (var file in fileStreams)
            //{
            //    //var path = GetAzureMainDiamondPathIdentifier("", file.MainDiamonds);
            //    //var finalPath = $"{basePath}/{file.MainDiamond.MainDiamondReqId.Value}_{file.MainDiamond.ShapeId.Value}";
            //    var pathOrderd = file.MainDiamondRequirements.Select(x => x.Id).OrderBy(x => x).ToArray();
            //    string path = "";
            //    pathOrderd.ForEach(x => path += $"/{x}");
            //    path.Remove(0, 1);// remove the / from the first position
            //    var finalPath = $"{basePath}/{path}";
            //    uploadTasks.Add(UploadFromBasePath(finalPath, new List<FileData> { file.stream }.ToArray(), cancellationToken));
            //}
            //var results = await Task.WhenAll(uploadTasks);
            //var stringResult = results.Where(r => r.IsSuccess).SelectMany(r => r.Value).ToArray();
            //if (stringResult.Length == 0)
            //    return Result.Fail("Failed to upload any files at all");
            //return Result.Ok(stringResult);
        }

        public async Task<Result<string[]>> UploadBaseSideDiamond(JewelryModel jewelryModel, BaseSideDiamondFileData[] fileStreams, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
            //string basePath = GetAzureFilePath(jewelryModel);
            //basePath = $"{basePath}/{BASE_SIDE_DIAMOND_IMAGES_FOLDER}";
            //List<Task<Result<string[]>>> uploadTasks = new();
            //foreach (var file in fileStreams)
            //{
            //    var getSideDiamondsPath = GetAzureSideDiamondPathIdentifier("", file.SideDiamondOpt);
            //    //var finalPath = $"{basePath}/{file.SideDiamondOpt.SideDiamondReqId.Value}_{file.SideDiamondOpt.Id.Value}";
            //    var finalPath = $"{basePath}/{getSideDiamondsPath}";
            //    uploadTasks.Add(UploadFromBasePath(finalPath, new List<FileData> { file.stream }.ToArray(), cancellationToken));
            //}
            //var results = await Task.WhenAll(uploadTasks);
            //var stringResult = results.Where(r => r.IsSuccess).SelectMany(r => r.Value).ToArray();
            //if (stringResult.Length == 0)
            //    return Result.Fail("Failed to upload any files at all");
            //return Result.Ok(stringResult);
        }

        public Task<Result<string[]>> UploadBase(JewelryModel jewelryModel, FileData[] fileStreams, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
            //string basePath = GetAzureFilePath(jewelryModel);
            //basePath = $"{basePath}/{BASE_IMAGES_FOLDER}";
            //return UploadFromBasePath(basePath, fileStreams, cancellationToken);
        }
        private async Task<Result<string[]>> UploadFromBasePath(string basePath, FileData[] fileStreams, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
            //List<Task<Result<string>>> uploadTasks = new();
            //foreach (var file in fileStreams)
            //{
            //    uploadTasks.Add(Task<Result<string>>.Run(async () =>
            //    {
            //        var stream = file.Stream;
            //        var finalPath = $"{basePath}/{file.FileName}_{GetTimeStamp()}";
            //        if (file.FileExtension != null)
            //            finalPath = $"{finalPath}.{file.FileExtension}";
            //        var result = await base.UploadFileAsync(finalPath, stream, file.contentType, cancellationToken);
            //        if (result.IsFailed)
            //            _logger.LogError("Failed to upload file with name: {0}", file.FileName);
            //        else
            //            _logger.LogInformation("uploaded file with name: {0}", file.FileName);
            //        return result;
            //    }));
            //}
            //var results = await Task.WhenAll(uploadTasks);
            //var stringResult = results.Where(r => r.IsSuccess).Select(r => r.Value).ToArray();
            //if (stringResult.Length == 0)
            //    return Result.Fail("Failed to upload any files at all");
            //return Result.Ok(stringResult);
        }
        //        foreach (var metal in jewelryModel.SizeMetals)
        //            {
        //                foreach (var mainDiamonds in listMainDiamonds)
        //                {
        //                    foreach (var sideDiamonds in listSideDiamonds)
        //                    {
        //                        var key = GetCategoryIdentifierName(metal.MetalId, sideDiamonds, mainDiamonds);
        //        var path = GetAzureCategoryFilePath(jewelryModel, key);
        //        allMappedPath.Append(path);
        //                        var newList = new List<Media>();
        //        galleries.Add(key, newList);
        //                    }
        //}
        //            }


        //string[] categoriesMapPaths = new string[] { };
        // get all base key for metals, side diamonds and main diamonds [ BASE_... ]
        //string[] metalPaths = new string[] { };
        //string[] sideDiamondPaths = new string[] { };
        //string[] mainDiamondPaths = new string[] { };
        // init keys
        //foreach (var metal in jewelryModel.SizeMetals)
        //{
        //    var metalPath = GetAzureMetalPathIdentifier("", metal.MetalId);
        //    metalPaths.Append(metalPath);
        //    galleries.Add($"/{BASE_METAL_IMAGES_FOLDER}/{metalPath}", new List<Media>());
        //}
        //foreach (var mainDiamonds in listMainDiamonds)
        //{
        //    var mainDiamondPath = GetAzureMainDiamondPathIdentifier("", mainDiamonds);
        //    mainDiamondPaths.Append(mainDiamondPath);
        //    galleries.Add($"/{BASE_MAIN_DIAMOND_IMAGES_FOLDER}/{mainDiamondPath}", new List<Media>());
        //}
        //foreach (var sideDiamonds in listSideDiamonds)
        //{
        //    var sideDiamondPath = GetAzureSideDiamondPathIdentifier("", sideDiamonds);
        //    sideDiamondPaths.Append(sideDiamondPath);
        //    galleries.Add($"/{BASE_SIDE_DIAMOND_IMAGES_FOLDER}/{sideDiamondPath}", new List<Media>());
        //}
        //foreach (var metal in metalPaths)
        //{
        //    foreach (var mainDiamond in mainDiamondPaths)
        //    {
        //        foreach (var sideDiamond in sideDiamondPaths)
        //        {
        //            var key = $"{metal}/{mainDiamond}/{sideDiamond}";
        //            categoriesMapPaths.Append(key);
        //            galleries.Add($"/{CATEGORIZED_IMAGES_FOLDER}/{key}", new List<Media>());
        //        }
        //    }
        //}
    }
}
