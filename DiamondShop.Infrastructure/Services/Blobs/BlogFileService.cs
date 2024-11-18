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
using HtmlAgilityPack.CssSelectors.NetCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Org.BouncyCastle.Utilities.IO;
using System.IO;
using System.Text;

namespace DiamondShop.Infrastructure.Services.Blobs
{
    internal class BlogFileService : AzureBlobContainerService, IBlogFileService
    {
        internal const string PARENT_FOLDER = "Blogs";
        internal const string THUMBNAIL = "thumbnail";
        internal const string CONTENT = "content";
        internal const string DELIMITER = "/";
        private readonly ILogger<BlogFileService> _logger;

        public BlogFileService(BlobServiceClient blobServiceClient, ILogger<BlogFileService> logger, IOptions<ExternalUrlsOptions> externalUrlsOptions) : base(blobServiceClient, logger, externalUrlsOptions)
        {
            _logger = logger;
        }

        public async Task<Result> DeleteThumbnail(Blog blog, CancellationToken token = default)
        {
            if (string.IsNullOrEmpty(blog.Thumbnail.MediaPath))
                return Result.Fail("Can't get thumbnail path");
            return await base.DeleteFileAsync(blog.Thumbnail.MediaPath, token);
        }
        public Task<Result> DeleteContent(BlogId blogId, CancellationToken token = default)
        {
            string basePath = GetAzureFilePath(blogId) + $"/{CONTENT}.txt";
            return base.DeleteFileAsync(basePath, token);
        }

        public async Task<Result<string>> GetContent(BlogId blogId, CancellationToken token = default)
        {
            string basePath = GetAzureFilePath(blogId) + $"/{CONTENT}.txt";
            var result = await base.DownloadFileAsync(basePath, token);
            if (result.IsFailed)
                return Result.Fail(result.Errors);
            if (result.Value?.Stream == null)
                return Result.Fail("Can't get content");
            using(StreamReader sr = new StreamReader(result.Value.Stream))
            {
                return await sr.ReadToEndAsync(token);
            }
        }

        public async Task<Result<string>> UploadContent(BlogId blogId, string content, CancellationToken token = default)
        {
            string basePath = GetAzureFilePath(blogId);
            Task<Result<string>> uploadTask = Task.Run(async () =>
            {
                MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
                var finalPath = $"{basePath}/{CONTENT}.txt";
                var result = await UploadFileAsync(finalPath, stream, "text/plain", token);
                if (result.IsFailed)
                    _logger.LogError("Failed to upload file with name: {0}", "content.txt");
                else
                    _logger.LogInformation("uploaded file with name: {0}", "content.txt");
                return result;
            });
            var result = await uploadTask;
            if (result.IsSuccess)
                return result;
            else
            {
                return Result.Fail(result.Errors);
            }
        }
        public async Task<Result<Media>> UploadThumbnail(BlogId blogId, FileData fileData, CancellationToken token = default)
        {
            string basePath = GetAzureFilePath(blogId);
            List<Task<Result<string>>> uploadTasks = new();
            var finalPath = $"{basePath}/thumbnail";
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
