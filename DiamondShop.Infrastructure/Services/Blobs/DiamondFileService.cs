using DiamondShop.Application.Services.Interfaces;
using DiamondShop.Application.Services.Interfaces.Diamonds;
using DiamondShop.Domain.BusinessRules;
using DiamondShop.Domain.Models.Diamonds;
using FluentResults;
using Microsoft.Extensions.Logging;
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
        private readonly ILogger<DiamondFileService> _logger;
        private readonly IBlobFileServices _blobFileServices;

        public DiamondFileService(ILogger<DiamondFileService> logger, IBlobFileServices blobFileServices)
        {
            _logger = logger;
            _blobFileServices = blobFileServices;
        }

        public async Task<Result<string>> UploadCertificatePdf(Diamond diamond, DiamondFileData pdfCertificate, CancellationToken cancellationToken = default)
        {
            string basePath = GetAzureFilePath(diamond);
            basePath = $"{basePath}/{GIA_FOLDER}";
            if(pdfCertificate.contentType != "application/pdf")
                return Result.Fail<string>("File is not a pdf");
            if(pdfCertificate.FileExtension != "pdf")
                return Result.Fail<string>("File extentsion is not a pdf");
            string finalpath = $"{basePath}/{pdfCertificate.FileName}_{GetTimeStamp()}";
            return await _blobFileServices.UploadFileAsync(finalpath, pdfCertificate.Stream, pdfCertificate.contentType, cancellationToken);
        }

        public async Task<Result<string[]>> UploadGallery(Diamond diamond, string galleryPath, DiamondFileData[] fileStreams, CancellationToken cancellationToken = default)
        {
            string basePath = GetAzureFilePath(diamond);
            basePath = $"{basePath}/{galleryPath}";

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

        public async Task<Result<string>> UploadThumbnail(Diamond diamond, DiamondFileData thumb, CancellationToken cancellationToken = default)
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
            return DateTime.UtcNow.ToString("yyMMddHHmmss");
        }
    }
}
