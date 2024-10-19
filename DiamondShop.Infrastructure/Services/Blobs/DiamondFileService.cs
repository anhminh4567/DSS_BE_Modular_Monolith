using DiamondShop.Application.Commons.Models;
using DiamondShop.Application.Services.Interfaces;
using DiamondShop.Application.Services.Interfaces.Diamonds;
using DiamondShop.Domain.BusinessRules;
using DiamondShop.Domain.Common.ValueObjects;
using DiamondShop.Domain.Models.Diamonds;
using DiamondShop.Domain.Models.Diamonds.ValueObjects;
using DiamondShop.Infrastructure.Options;
using FluentResults;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Infrastructure.Services.Blobs
{
    internal class DiamondFileService : IDiamondFileService
    {

        internal const string PARENT_FOLDER = "Diamonds";
        internal const string GIA_FOLDER = "GIA";
        internal const string IMAGES_FOLDER = "Images";
        internal const string DELIMITER = "/";
        private readonly ILogger<DiamondFileService> _logger;
        private readonly IBlobFileServices _blobFileServices;
        private readonly IOptions<ExternalUrlsOptions> _externalUrlsOptions;

        public DiamondFileService(ILogger<DiamondFileService> logger, IBlobFileServices blobFileServices, IOptions<ExternalUrlsOptions> externalUrlsOptions)
        {
            _logger = logger;
            _blobFileServices = blobFileServices;
            _externalUrlsOptions = externalUrlsOptions;
        }

        public Task<Result> DeleteFileAsync(string filePath, CancellationToken cancellationToken = default)
        {
            return _blobFileServices.DeleteFileAsync(filePath,cancellationToken);
        }

        public Task<Result<BlobFileResponseDto>> DownloadFileAsync(string filePath, CancellationToken cancellationToken = default)
        {
            return _blobFileServices.DownloadFileAsync(filePath, cancellationToken);
        }

        public Task<List<Media>> GetFolders(string folderPath, CancellationToken cancellationToken = default)
        {
            //ignore folder path
            return _blobFileServices.GetFolders(folderPath, cancellationToken); 
        }

        public Task<List<Media>> GetFolders(Diamond diamond, CancellationToken cancellationToken = default)
        {
            var basePath = GetAzureFilePath(diamond);
            return _blobFileServices.GetFolders(basePath, cancellationToken);
        }
        // in diamond there is no category to worry about, so the base image will store all the image
        public GalleryTemplate MapPathsToCorrectGallery(Diamond diamond, List<Media> paths, CancellationToken cancellationToken = default)
        {
            var gallery = new GalleryTemplate();
            gallery.GalleryFolder = GetAzureFilePath(diamond);
            //var trimmedPaths = paths.Select(path => path.MediaPath.Replace(basePath, string.Empty)).ToList();
            foreach (var path in paths)
            {
                var tobeComparedPath = path.MediaPath.Replace(GetAzureFilePath(diamond), string.Empty);
                //tobeComparedPath = tobeComparedPath + DELIMITER;
                if (tobeComparedPath.StartsWith(DELIMITER + GIA_FOLDER))
                {
                    //ar giaMedia = Media.Create(null,path.MediaPath,path.ContentType);
                    gallery.Certificates.Add(path);
                }
                else if (tobeComparedPath.StartsWith(DELIMITER + IMAGES_FOLDER))
                {
                    gallery.BaseImages.Add(path);
                }
                else//else base image, 
                {
                    gallery.Thumbnail = path;
                }
            }
            return gallery;
        }

        public string ToAbsolutePath(string relativePath)
        {
           return _blobFileServices.ToAbsolutePath(relativePath);
        }

        public string ToRelativePath(string absolutePath)
        {
            return _blobFileServices.ToRelativePath(absolutePath);
        }

        public async Task<Result<string>> UploadCertificatePdf(Diamond diamond, FileData pdfCertificate, CancellationToken cancellationToken = default)
        {
            string basePath = GetAzureFilePath(diamond);
            basePath = $"{basePath}/{GIA_FOLDER}";
            if(pdfCertificate.contentType != "application/pdf")
                return Result.Fail<string>("File is not a pdf");
            if(pdfCertificate.FileExtension != ".pdf")
                return Result.Fail<string>("File extentsion is not a pdf");
            string finalpath = $"{basePath}/{pdfCertificate.FileName}_{GetTimeStamp()}";
            return await _blobFileServices.UploadFileAsync(finalpath, pdfCertificate.Stream, pdfCertificate.contentType, cancellationToken);
        }

        public Task<Result<string>> UploadFileAsync(string filePath, Stream stream, string contentType, CancellationToken cancellationToken = default)
        {
            return _blobFileServices.UploadFileAsync(filePath, stream, contentType, cancellationToken);
        }

        public async Task<Result<string[]>> UploadGallery(Diamond diamond, FileData[] fileStreams, CancellationToken cancellationToken = default)
        {
            string basePath = GetAzureFilePath(diamond);
            basePath = $"{basePath}/{IMAGES_FOLDER}";

            List<Task<Result<string>>> uploadTasks = new();
            foreach (var file in fileStreams)
            {
                uploadTasks.Add(Task<Result<string>>.Run(async () =>
                {
                    var stream = file.Stream;
                    var finalPath = $"{basePath}/{file.FileName}_{GetTimeStamp()}";
                    if (file.FileExtension != null)
                        finalPath = $"{finalPath}.{file.FileExtension}";
                    var result = await _blobFileServices.UploadFileAsync(finalPath, stream, file.contentType, cancellationToken);
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

        public async Task<Result<string>> UploadThumbnail(Diamond diamond, FileData thumb, CancellationToken cancellationToken = default)
        {
            string basePath = GetAzureFilePath(diamond);
            string finalpath = $"{basePath}/{thumb.FileName}_{GetTimeStamp()}";
            var resutl = await _blobFileServices.UploadFileAsync(finalpath, thumb.Stream, thumb.contentType, cancellationToken);
            return resutl;
        }

        private string GetAzureFilePath(Diamond diamond)
        {
            return $"{PARENT_FOLDER}/{diamond.Id.Value}";
        }
        private string GetTimeStamp()
        {
            return DateTime.UtcNow.Ticks.ToString();
        }
    }
}
