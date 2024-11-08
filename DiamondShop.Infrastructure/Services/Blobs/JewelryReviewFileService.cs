using Azure.Storage.Blobs;
using DiamondShop.Application.Commons.Models;
using DiamondShop.Application.Services.Interfaces;
using DiamondShop.Application.Services.Interfaces.JewelryReviews;
using DiamondShop.Domain.Common.ValueObjects;
using DiamondShop.Domain.Models.Diamonds;
using DiamondShop.Domain.Models.Diamonds.ValueObjects;
using DiamondShop.Domain.Models.Jewelries;
using DiamondShop.Domain.Models.Jewelries.Entities;
using DiamondShop.Domain.Models.JewelryModels.Entities;
using DiamondShop.Domain.Models.JewelryModels.ValueObjects;
using DiamondShop.Infrastructure.Options;
using FluentEmail.Core;
using FluentResults;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.IO;
using System.Threading;

namespace DiamondShop.Infrastructure.Services.Blobs
{
    internal class JewelryReviewFileService : AzureBlobContainerService, IJewelryReviewFileService
    {
        internal const string PARENT_FOLDER = "Reviews";
        internal const string DELIMITER = "/";

        private readonly ILogger<JewelryReviewFileService> _logger;

        public JewelryReviewFileService(BlobServiceClient blobServiceClient, ILogger<JewelryReviewFileService> logger, IOptions<ExternalUrlsOptions> externalUrlsOptions) : base(blobServiceClient, logger, externalUrlsOptions)
        {
            _logger = logger;
        }

        public Task<List<Media>> GetFolders(JewelryModelId modelId, MetalId? metalId, CancellationToken token = default)
        {
            //Get All review
            if (metalId == null)
            {
                string basePath = GetAzureFilePath(modelId);
                return base.GetFolders(basePath, token);
            }
            //Get metal related review
            else
            {
                string basePath = GetAzureFilePath(modelId, metalId);
                return base.GetFolders(basePath, token);
            }
        }

        public async Task<Result<string[]>> UploadReview(Jewelry jewelry, FileData[] streams, CancellationToken token = default)
        {
            string basePath = GetAzureFilePath(jewelry);

            List<Task<Result<string>>> uploadTasks = new();
            foreach (var stream in streams)
            {
                uploadTasks.Add(Task.Run(async () =>
                {
                    var finalPath = $"{basePath}/{ReviewFileName(jewelry.SerialCode)}_{GetTimeStamp()}";
                    if (stream.FileExtension != null)
                        finalPath = $"{finalPath}.{GetExtensionName(stream.FileExtension)}";
                    var result = await UploadFileAsync(finalPath, stream.Stream, stream.contentType, token);
                    if (result.IsFailed)
                        _logger.LogError("Failed to upload file with name: {0}", stream.FileName);

                    else
                        _logger.LogInformation("uploaded file with name: {0}", stream.FileName);
                    return result;
                }));
            }
            var results = await Task.WhenAll(uploadTasks);
            var stringResult = results.Where(r => r.IsSuccess).Select(r => r.Value).ToArray();
            if (stringResult.Length == streams.Length)
                return Result.Ok(stringResult);
            else
            {
                List<IError> errors = new List<IError>() { new Error("Failed to upload some file") };
                results.Where(r => r.IsFailed).ForEach(r => errors.AddRange(r.Errors));
                return Result.Fail(errors);
            }
        }
        string GetExtensionName(string fileExtension) => fileExtension switch
        {
            "image/png" => "png",
            "image/jpeg" => "jpeg",
            "image/jpg" => "jpg",
            "image/gif" => "gif",
            "video/mp4" => "mp4",
            "application/pdf" => "pdf",
            _ => fileExtension
        };
        string ReviewFileName(string serialCode) => $"{serialCode}";
        string GetAzureFilePath(Jewelry jewelry) => $"{PARENT_FOLDER}/{jewelry.ModelId.Value}/{jewelry.MetalId.Value}";
        string GetAzureFilePath(JewelryModelId modelId) => $"{PARENT_FOLDER}/{modelId.Value}";
        string GetAzureFilePath(JewelryModelId modelId, MetalId metalId) => $"{PARENT_FOLDER}/{modelId.Value}/{metalId.Value}";
    }
}
