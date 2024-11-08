using DiamondShop.Application.Commons.Models;
using DiamondShop.Domain.Common.ValueObjects;
using DiamondShop.Domain.Models.Jewelries;
using FluentResults;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Application.Services.Interfaces.JewelryReview
{
    public interface IJewelryReviewFileService : IBlobFileServices
    {
        Task<Result<string[]>> UploadReview(Jewelry jewelry, FileData[] streams, CancellationToken token = default);
        Task<List<Media>> GetFolder(Jewelry jewelry);

    }
}
