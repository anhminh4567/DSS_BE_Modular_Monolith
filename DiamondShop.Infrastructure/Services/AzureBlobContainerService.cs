using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using DiamondShop.Application.Services.Interfaces;
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
        private readonly BlobServiceClient _blobServiceClient;
        private readonly ILogger<AzureBlobContainerService> _logger;
        private readonly IOptions<ExternalUrlsOptions> _externalUrlsOptions;

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
    }
}
