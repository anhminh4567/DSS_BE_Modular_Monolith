using DiamondShop.Application.Commons.Models;
using DiamondShop.Domain.Common.ValueObjects;
using DiamondShop.Domain.Models.Diamonds;
using FluentResults;
using Microsoft.AspNetCore.DataProtection.KeyManagement.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Application.Services.Interfaces.Diamonds
{
    public interface IDiamondFileService : IBlobFileServices
    {
        Task<Result<string[]>> UploadGallery(Diamond diamond,  FileData[] fileStreams , CancellationToken cancellationToken = default);
        Task<Result<string>> UploadThumbnail(Diamond diamond, FileData thumb, CancellationToken cancellationToken = default);
        Task<Result<string>> UploadCertificatePdf(Diamond diamond, FileData pdfCertificate, CancellationToken cancellationToken = default);
        Task<List<Media>> GetFolders(Diamond diamond, CancellationToken cancellationToken = default);
        GalleryTemplate MapPathsToCorrectGallery(Diamond diamond, List<Media> paths, CancellationToken cancellationToken = default);
    }
}
