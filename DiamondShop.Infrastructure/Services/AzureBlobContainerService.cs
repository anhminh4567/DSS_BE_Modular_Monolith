using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using DiamondShop.Application.Commons.Models;
using DiamondShop.Application.Services.Interfaces;
using DiamondShop.Domain.Common.ValueObjects;
using DiamondShop.Domain.Models.JewelryModels;
using DiamondShop.Infrastructure.Options;
using FluentResults;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Infrastructure.Services
{
    internal class AzureBlobContainerService : IBlobFileServices
    {
        protected readonly BlobServiceClient _blobServiceClient;
        private readonly ILogger<AzureBlobContainerService> _logger;
        protected readonly IOptions<ExternalUrlsOptions> _externalUrlsOptions;
        private const string PAGE_SIZE = "1000";
        public AzureBlobContainerService(BlobServiceClient blobServiceClient, ILogger<AzureBlobContainerService> logger, IOptions<ExternalUrlsOptions> externalUrlsOptions)
        {
            _blobServiceClient = blobServiceClient;
            _logger = logger;
            _externalUrlsOptions = externalUrlsOptions;
        }

        public async Task<Result> DeleteFileAsync(string filePath, CancellationToken cancellationToken = default)
        {

            BlobContainerClient blobContainerClient = GetCorrectBlobClient();
            BlobClient blobClient = blobContainerClient.GetBlobClient(filePath);
            var deleteResult = await blobClient.DeleteAsync(cancellationToken: cancellationToken);
            if (deleteResult.IsError)
                return Result.Fail("Fail to delete");
            return Result.Ok();
        }

        public async Task<Result<BlobFileResponseDto>> DownloadFileAsync(string filePath, CancellationToken cancellationToken = default)
        {
            BlobContainerClient blobContainerClient = GetCorrectBlobClient();
            BlobClient blobClient = blobContainerClient.GetBlobClient(filePath);
            var response = await blobClient.DownloadStreamingAsync(cancellationToken: cancellationToken);
            return Result.Ok(new BlobFileResponseDto()
            {
                Stream = response.Value.Content,
                ContentType = response.Value.Details.ContentType,
            });
        }

        public async Task<List<Media>> GetFolders(string folderPath, CancellationToken cancellationToken = default)
        {
            BlobContainerClient blobContainerClient = GetCorrectBlobClient();
            var blobItems =  blobContainerClient.GetBlobsByHierarchyAsync(prefix: folderPath, cancellationToken: cancellationToken);
            List<Media> relativePath = new();
            await foreach (var blobItem in blobItems)
            {
                var media = Media.Create(null, blobItem.Blob.Name,blobItem.Blob.Properties.ContentType);
                relativePath.Add(media);
            }
            return relativePath;
        }

        public async Task<Result<string>> UploadFileAsync(string filePath, Stream stream, string contentType, CancellationToken cancellationToken = default)
        {
            try
            {
                BlobContainerClient blobContainerClient = GetCorrectBlobClient();
                BlobClient blobClient = blobContainerClient.GetBlobClient(filePath);
                var uploadResult = await blobClient.UploadAsync(
                    stream,
                    new BlobHttpHeaders { ContentType = contentType, },
                    cancellationToken: cancellationToken);
                var tryGetBlobResult = uploadResult.Value;
                if (tryGetBlobResult is null)
                    throw new NullReferenceException();
                return Result.Ok(filePath);
            }
            catch (Exception ex)
            {
                _logger.LogError("fail to upload file at {filePath} ", filePath);
                return Result.Fail("fail to save file, try again");
            }
        }
        private BlobContainerClient GetCorrectBlobClient()
        {
            return _blobServiceClient.GetBlobContainerClient(_externalUrlsOptions.Value.Azure.ContainerName);
        }

        public string ToAbsolutePath(string relativePath)
        {
            return $"{_externalUrlsOptions.Value.Azure.BaseUrl}/{_externalUrlsOptions.Value.Azure.ContainerName}/{relativePath}";
        }

        public string ToRelativePath(string absolutePath)
        {
            var urls = $"{_externalUrlsOptions.Value.Azure.BaseUrl}/{_externalUrlsOptions.Value.Azure.ContainerName}/";
            return absolutePath.Replace(urls, string.Empty);
        }
        protected async Task<Result<string[]>> UploadFromBasePath(string basePath, FileData[] fileStreams, CancellationToken cancellationToken = default)
        {
            List<Task<Result<string>>> uploadTasks = new();
            foreach (var file in fileStreams)
            {
                uploadTasks.Add(Task<Result<string>>.Run(async () =>
                {
                    var stream = file.Stream;
                    var finalPath = $"{basePath}/{file.FileName}_{GetTimeStamp()}";
                    if (file.FileExtension != null)
                        finalPath = $"{finalPath}.{file.FileExtension}";
                    var result = await UploadFileAsync(finalPath, stream, file.contentType, cancellationToken);
                    if (result.IsFailed)
                        _logger.LogError("Failed to upload file with name: {0}", file.FileName);
                    else
                        _logger.LogInformation("uploaded file with name: {0}", file.FileName);
                    return result;
                }));
            }
            var results = await Task.WhenAll(uploadTasks);
            var stringResult = results.Where(r => r.IsSuccess).Select(r => r.Value).ToArray();
            if (stringResult.Length == 0)
                return Result.Fail("Failed to upload any files at all");
            return Result.Ok(stringResult);
        }
        protected string GetTimeStamp()
        {
            return DateTime.UtcNow.Ticks.ToString();
        }
    }
}
