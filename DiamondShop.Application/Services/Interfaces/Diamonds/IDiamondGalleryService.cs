using DiamondShop.Domain.Models.Diamonds;
using FluentResults;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Application.Services.Interfaces.Diamonds
{
    public interface IDiamondGalleryService
    {
        Task<Result<string[]>> UploadFiles(Stream[] fileStreams, CancellationToken cancellationToken = default);
        Task<Result<string[]>> UploadBaseImageGallery(Diamond diamond, Stream[] fileStreams, CancellationToken cancellationToken = default);
        Task<Result<string[]>> UploadGallery(Diamond diamond, string galleryPath, Stream[] fileStreams , CancellationToken cancellationToken = default);

    }
}
