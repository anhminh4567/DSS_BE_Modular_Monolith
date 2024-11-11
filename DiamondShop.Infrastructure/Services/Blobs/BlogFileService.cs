using Azure.Storage.Blobs;
using DiamondShop.Application.Commons.Models;
using DiamondShop.Application.Services.Interfaces.Blogs;
using DiamondShop.Domain.Common.ValueObjects;
using DiamondShop.Domain.Models.Blogs;
using DiamondShop.Domain.Models.Blogs.ValueObjects;
using DiamondShop.Domain.Models.JewelryModels.ValueObjects;
using DiamondShop.Infrastructure.Options;
using FluentEmail.Core;
using FluentResults;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Org.BouncyCastle.Utilities.IO;
using System.IO;

namespace DiamondShop.Infrastructure.Services.Blobs
{
    internal class BlogFileService : AzureBlobContainerService, IBlogFileService
    {
        internal const string PARENT_FOLDER = "Blogs";
        internal const string DELIMITER = "/";
        private readonly ILogger<BlogFileService> _logger;

        public BlogFileService(BlobServiceClient blobServiceClient, ILogger<BlogFileService> logger, IOptions<ExternalUrlsOptions> externalUrlsOptions) : base(blobServiceClient, logger, externalUrlsOptions)
        {
            _logger = logger;
        }

        public Task<Result> DeleteFiles(BlogId blogId, CancellationToken token = default)
        {
            string basePath = GetAzureFilePath(blogId);
            return base.DeleteFileAsync(basePath, token);
        }

        public Task<List<Media>> GetFolders(BlogId blogId, CancellationToken token = default)
        {
            string basePath = GetAzureFilePath(blogId);
            return base.GetFolders(basePath, token);
        }

        public async Task<Result<string[]>> UploadBlogContent(BlogId blogId, FileData[] streams, CancellationToken token = default)
        {
            string basePath = GetAzureFilePath(blogId);
            List<Task<Result<string>>> uploadTasks = new();
            foreach (var stream in streams)
            {
                uploadTasks.Add(Task.Run(async () =>
                {
                    var finalPath = $"{basePath}/{stream.FileName}";
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
        public async Task<Result<Media>> UploadThumbnail(BlogId blogId, FileData fileData, CancellationToken token = default)
        {
            string basePath = GetAzureFilePath(blogId);
            List<Task<Result<string>>> uploadTasks = new();
            var finalPath = $"{basePath}/{GetTimeStamp()}";
            if (fileData.FileExtension != null)
                finalPath = $"{finalPath}.{GetExtensionName(fileData.FileExtension)}";
            var result = await UploadFileAsync(finalPath, fileData.Stream, fileData.contentType, token);
            if (result.IsFailed || String.IsNullOrEmpty(result.Value))
            {

                _logger.LogError("Failed to upload file with name: {0}", fileData.FileName);
                return Result.Fail(result.Errors);
            }
            else
            {

                _logger.LogInformation("uploaded file with name: {0}", fileData.FileName);
                Media media = new Media() { MediaPath = result.Value, ContentType = fileData.contentType };
                return media;
            }
        }
        string GetExtensionName(string fileExtension) => fileExtension switch
        {
            "image/png" => "png",
            "image/jpeg" => "jpeg",
            "image/jpg" => "jpg",
            "image/gif" => "gif",
            "video/mp4" => "mp4",
            _ => fileExtension
        };


        string GetAzureFilePath(BlogId id) => $"{PARENT_FOLDER}/{id.Value}";
    }
}
