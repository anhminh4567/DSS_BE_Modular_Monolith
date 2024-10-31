using Azure.Storage.Blobs;
using DiamondShop.Application.Commons.Models;
using DiamondShop.Application.Services.Interfaces.Orders;
using DiamondShop.Domain.Common.ValueObjects;
using DiamondShop.Domain.Models.Orders;
using DiamondShop.Domain.Models.Orders.Entities;
using DiamondShop.Domain.Models.Transactions;
using DiamondShop.Infrastructure.Options;
using FluentResults;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace DiamondShop.Infrastructure.Services.Blobs
{
    internal class OrderFileService : AzureBlobContainerService, IOrderFileServices
    {
        internal const string PARENT_FOLDER = "Order";
        internal const string IMAGES_FOLDER = "Images";
        internal const string TRANSACTION_FOLDER = IMAGES_FOLDER + "/" + "Transaction";
        internal const string ORDERLOG_FOLDER = IMAGES_FOLDER + "/" + "OrderLog";
        internal const string DELIMITER = "/";
        private GalleryTemplate _cachedGalleryTemplate;
        public OrderFileService(BlobServiceClient blobServiceClient, ILogger<AzureBlobContainerService> logger, IOptions<ExternalUrlsOptions> externalUrlsOptions) : base(blobServiceClient, logger, externalUrlsOptions)
        {
        }

        public async Task<GalleryTemplate> GetAllOrderImages(Order order, CancellationToken cancellationToken = default)
        {
            var paths = GetAzureFilePath(order);
            var getFolders = await GetFolders(paths, cancellationToken);
            return MapPathsToCorrectGallery(order, getFolders, cancellationToken);
        }

        public async Task<Result<string[]>> GetOrderLogImages(Order order, OrderLog orderLog, CancellationToken cancellationToken = default)
        {
            var gallery = GetCachedGalleryTemplate(order, cancellationToken);
            var key = $"{ORDERLOG_FOLDER}/{GetOrderLogNameIdentifier(orderLog)}";
            var getMedia = gallery.Gallery[key];
            if(getMedia != null)
                return Result.Ok(getMedia.Select(x => x.MediaPath).ToArray());
            return new string[] { };
        }

        public async Task<Result<string[]>> GetOrderTransactionImages(Order order, Transaction transaction, CancellationToken cancellationToken = default)
        {
            var gallery = GetCachedGalleryTemplate(order, cancellationToken);
            var key = $"{TRANSACTION_FOLDER}/{GetTransactionNameIdentifier(transaction)}";
            var getMedia = gallery.Gallery[key];
            if (getMedia != null)
                return Result.Ok(getMedia.Select(x => x.MediaPath).ToArray());
            return new string[] { };
        }

        public async Task<Result<string[]>> UploadOrderLogImage(Order order, OrderLog log, FileData[] images, CancellationToken cancellationToken = default)
        {
            string basePath = GetAzureFilePath(order);
            basePath = $"{basePath}/{ORDERLOG_FOLDER}";
            List<Task<Result<string[]>>> uploadTasks = new();
            foreach (var file in images)
            {
                var finalPath = $"{basePath}/{GetOrderLogNameIdentifier(log)}";
                uploadTasks.Add(UploadFromBasePath(finalPath, new List<FileData> { file }.ToArray(), cancellationToken));
            }
            var results = await Task.WhenAll(uploadTasks);
            var stringResult = results.Where(r => r.IsSuccess).SelectMany(r => r.Value).ToArray();
            if (stringResult.Length == 0)
                return Result.Fail("Failed to upload any files at all");
            return Result.Ok(stringResult);
        }

        public async Task<Result<string[]>> UploadOrderTransactionImage(Order order, Transaction transaction, FileData[] images, CancellationToken cancellationToken = default)
        {
            string basePath = GetAzureFilePath(order);
            basePath = $"{basePath}/{TRANSACTION_FOLDER}";
            List<Task<Result<string[]>>> uploadTasks = new();
            foreach (var file in images)
            {
                var finalPath = $"{basePath}/{GetTransactionNameIdentifier(transaction)}";
                uploadTasks.Add(UploadFromBasePath(finalPath, new List<FileData> { file }.ToArray(), cancellationToken));
            }
            var results = await Task.WhenAll(uploadTasks);
            var stringResult = results.Where(r => r.IsSuccess).SelectMany(r => r.Value).ToArray();
            if (stringResult.Length == 0)
                return Result.Fail("Failed to upload any files at all");
            return Result.Ok(stringResult);
        }
        public GalleryTemplate MapPathsToCorrectGallery(Order order, List<Media> paths, CancellationToken cancellationToken = default)
        {
            var basePath = GetAzureFilePath(order);
            var gallery = new GalleryTemplate();
            Action<string, Media> AddToCategory = (string tobeComparedPath, Media originalMedia) =>
            {
                int lastSlashIndex = tobeComparedPath.LastIndexOf('/');
                if (lastSlashIndex >= 0)
                {
                    var key = tobeComparedPath.Substring(0, lastSlashIndex);
                    if (gallery.Gallery.ContainsKey(key))
                        gallery.Gallery[key] = new List<Media>();
                    gallery.Gallery[key].Add(originalMedia);
                }
            };
            foreach (var path in paths)
            {
                var tobeComparedPath = path.MediaPath.Replace(basePath + DELIMITER, string.Empty);
                if (tobeComparedPath.StartsWith(TRANSACTION_FOLDER))
                    AddToCategory(tobeComparedPath, path);
                
                else if (tobeComparedPath.StartsWith(ORDERLOG_FOLDER))
                    AddToCategory(tobeComparedPath, path);

                else
                    gallery.BaseImages.Add(path);
                
            }
            return gallery;
        }
        private string GetOrderLogNameIdentifier(OrderLog log)
        {
            return $"{log.Id.Value}";
        }
        private string GetTransactionNameIdentifier(Transaction transaction)
        {
            return $"{transaction.Id.Value}";
        }
        private GalleryTemplate GetCachedGalleryTemplate(Order order, CancellationToken cancellationToken = default)
        {
            if (_cachedGalleryTemplate is null)
            {
                _cachedGalleryTemplate = GetAllOrderImages(order, cancellationToken).Result;
            }
            return _cachedGalleryTemplate;
        }

        private string GetAzureFilePath(Order order)
        {
            return $"{PARENT_FOLDER}/{order.Id.Value}";
        }
    }
}
