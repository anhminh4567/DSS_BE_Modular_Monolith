using Azure.Storage.Blobs;
using DiamondShop.Application.Commons.Models;
using DiamondShop.Application.Services.Interfaces.Transfers;
using DiamondShop.Domain.Common.ValueObjects;
using DiamondShop.Domain.Models.Jewelries;
using DiamondShop.Domain.Models.Jewelries.ValueObjects;
using DiamondShop.Domain.Models.Transactions;
using DiamondShop.Infrastructure.Options;
using FluentResults;
using HtmlAgilityPack.CssSelectors.NetCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using System.IO;

namespace DiamondShop.Infrastructure.Services.Blobs
{
    internal class TransferFileService : AzureBlobContainerService, ITransferFileService
    {
        private readonly ILogger<TransferFileService> _logger;

        public TransferFileService(BlobServiceClient blobServiceClient, ILogger<TransferFileService> logger, IOptions<ExternalUrlsOptions> externalUrlsOptions) : base(blobServiceClient, logger, externalUrlsOptions)
        {
            _logger = logger;
        }

        public Task<Result> DeleteTransferImage(Transaction transaction, CancellationToken token = default)
        {
            return base.DeleteFileAsync(transaction.Evidence.MediaPath, token);
        }

        public Task<List<Media>> GetTransferImage(Transaction transaction, CancellationToken token = default)
        {
            string basePath = GetAzureFilePath(transaction);
            return base.GetFolders(basePath, token);
        }
        public async Task<Result<string>> UploadTransferImage(Transaction transaction, FileData image, CancellationToken token = default)
        {
            var basePath = GetAzureFilePath(transaction);
            var finalPath = $"{basePath}/{GetTimeStamp()}";
            if (image.FileExtension != null)
                finalPath = $"{finalPath}.{image.FileName.Split(".")[0]}";
            var result = await base.UploadFileAsync(finalPath, image.Stream, image.contentType, token);
            return result;
        }

        private string GetAzureFilePath(Transaction transaction) => $"Transactions/{transaction.OrderId.Value}/{transaction.Id.Value}";
    }
}
