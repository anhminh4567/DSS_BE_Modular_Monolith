using Azure.Storage.Blobs;
using DiamondShop.Application.Commons.Models;
using DiamondShop.Application.Services.Interfaces;
using DiamondShop.Application.Services.Interfaces.JewelryReview;
using DiamondShop.Domain.Common.ValueObjects;
using DiamondShop.Domain.Models.Diamonds;
using DiamondShop.Domain.Models.Diamonds.ValueObjects;
using DiamondShop.Domain.Models.Jewelries;
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

        public Task<List<Media>> GetFolder(Jewelry jewelry)
        {
            throw new NotImplementedException();
        }

        public async Task<Result<string[]>> UploadReview(Jewelry jewelry, FileData[] streams, CancellationToken token = default)
        {
            string basePath = GetAzureFilePath(jewelry);

            List<Task<Result<string>>> uploadTasks = new();
            foreach (var file in streams)
                for (int i = 0; i < streams.Count(); i++)
                {
                    var stream = streams[i];
                    uploadTasks.Add(Task.Run(async () =>
                    {
                        var stream = streams[i].Stream;
                        var finalPath = $"{basePath}/{ReviewFileName(jewelry.SerialCode,i)}_{GetTimeStamp()}";
                        if (streams[i].FileExtension != null)
                            finalPath = $"{finalPath}.{streams[i].FileExtension}";
                        var result = await UploadFileAsync(finalPath, stream, streams[i].contentType, token);
                        if (result.IsFailed)
                            _logger.LogError("Failed to upload file with name: {0}", streams[i].FileName);

                        else
                            _logger.LogInformation("uploaded file with name: {0}", streams[i].FileName);
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
        string ReviewFileName(string serialCode, int position) => $"{serialCode}_{position}";
        string GetAzureFilePath(Jewelry jewelry) => $"{PARENT_FOLDER}/{jewelry.ModelId.Value}/{jewelry.MetalId.Value}";
    }
}
